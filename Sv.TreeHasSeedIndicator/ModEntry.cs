using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.TerrainFeatures;

namespace Sv.TreeHasSeedIndicator
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.eventUp)
                return;

            DrawSeedBubbleForTrees(GetValidTrees());
        }

        private static List<Tree> GetValidTrees()
        {
            return Game1.currentLocation.terrainFeatures.Values
                .OfType<Tree>()
                .Where(t => t.growthStage.Value >= 5 && !t.stump.Value)
                .ToList();
        }

        private static void DrawSeedBubbleForTrees(List<Tree> treeList)
        {
            foreach (Tree tree in treeList)
            {
                if (!tree.hasSeed.Value) continue;

                DrawSeedBubble(Game1.spriteBatch, tree);
            }
        }

        public static void DrawSeedBubble(SpriteBatch spriteBatch, Tree tree)
        {
            var tile = tree.currentTileLocation;
            var position = new Vector2(tile.X * 64, tile.Y * 64);
            float y_offset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);

            Vector2 bubble_draw_position = position + new Vector2(-.14f, -3.5f) * 64f + new Vector2(0f, y_offset);
            Vector2 item_relative_to_bubble = new(40f, 43f);

            spriteBatch.Draw(
                texture: Game1.mouseCursors,
                position: Game1.GlobalToLocal(Game1.viewport, bubble_draw_position),
                sourceRectangle: new Rectangle(141, 465, 20, 24),
                color: Color.White * 0.75f,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: 1);

            spriteBatch.Draw(
                texture: Game1.objectSpriteSheet,
                position: Game1.GlobalToLocal(Game1.viewport, bubble_draw_position + item_relative_to_bubble),
                sourceRectangle: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, GetTreeSeedIndex(tree), 16, 16),
                color: Color.White * 0.75f,
                rotation: 0f,
                origin: new Vector2(8f, 8f),
                scale: 3.5f,
                effects: SpriteEffects.None,
                layerDepth: 2);
        }

        private static int GetTreeSeedIndex(Tree tree) =>
            tree.treeType.Value switch
            {
                3 => 311,
                1 => 309,
                8 => 292,
                2 => 310,
                6 or 9 => 88,
                _ => -1,
            };
    }
}
