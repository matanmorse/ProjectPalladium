using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectPalladium.Buildings;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;
using System.Reflection.Metadata.Ecma335;

namespace ProjectPalladium.Utils
{
    public class Util
    {
        public static float scale = Game1.scale;
        public static int tileSize = 16;
        public static void DrawRectangle(Rectangle rect, SpriteBatch b)
        {
            Color color = Color.Red;
            Texture2D _texture = new Texture2D(b.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] { Color.DeepPink });

            int thickness = 2;

            b.Draw(_texture, new Vector2(rect.Left, rect.Top ), new Rectangle(0, 0, thickness, rect.Size.Y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            b.Draw(_texture, new Vector2(rect.Left , rect.Top ), new Rectangle(0, 0, rect.Size.X, thickness), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f); ;
            b.Draw(_texture, new Vector2(rect.Left + rect.Size.X, rect.Top ), new Rectangle(0, 0, thickness, rect.Size.Y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f); ;
            b.Draw(_texture, new Vector2(rect.Left , rect.Top + rect.Size.Y -1), new Rectangle(0, 0, rect.Size.X + thickness, thickness), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f); ;

        }
        public static Point ScalePoint(Point pt)
        {
            return new Point((int)(pt.X * scale), (int)(pt.Y * scale));
        }

        public static Vector2 GlobalPosToLocalPos(Vector2 global)
        {
            return new Vector2(global.X / (tileSize * Game1.scale), global.Y / (tileSize * Game1.scale));
        }
        public static Vector2 LocalPosToGlobalPos(Vector2 local)
        {
            return new Vector2(local.X * tileSize * Game1.scale, local.Y * tileSize * Game1.scale);
        }

        // convert a serialized rectange to a rectangle at a specific point, with offsets
        public static Rectangle makeRectFromPoints(ColliderDetails col, Vector2 pos)
        {
            return new Rectangle((int)(pos.X + col.location[0] * Game1.scale), (int)(pos.Y + col.location[1] * Game1.scale),
                            (int)(col.size[0] * Game1.scale), (int)(col.size[1] * Game1.scale));
        }

        // gets the mouse position in terms of native resolution
        public static Point GetNativeMousePos(Point globalPos)
        {
            float offsetX = (Game1.graphicsDevice.PresentationParameters.Bounds.Width - Game1.UINativeResolution.X * Game1.UITargetScale) / 2.0f;
            float offsetY = (Game1.graphicsDevice.PresentationParameters.Bounds.Height - Game1.UINativeResolution.Y * Game1.UITargetScale) / 2.0f;

            float nativeMouseX = (globalPos.X - offsetX) / Game1.UITargetScale;
            float nativeMouseY = (globalPos.Y - offsetY) / Game1.UITargetScale;

            nativeMouseX = MathHelper.Clamp(nativeMouseX, 0, Game1.UINativeResolution.X);
            nativeMouseY = MathHelper.Clamp(nativeMouseY, 0, Game1.UINativeResolution.Y);

            // subtract 1 b/c of rounding error
            return new Point((int)nativeMouseX - 1, (int)nativeMouseY - 1);
        }

        public static Point GetGameworldMousePos(Point globalPos)
        {
            float offsetX = (Game1.graphicsDevice.PresentationParameters.Bounds.Width - Game1.NativeResolution.X * Game1.gameWorldTargetScale) / 2.0f;
            float offsetY = (Game1.graphicsDevice.PresentationParameters.Bounds.Height - Game1.NativeResolution.Y * Game1.gameWorldTargetScale) / 2.0f;

            float nativeMouseX = (globalPos.X - offsetX) / Game1.gameWorldTargetScale;
            float nativeMouseY = (globalPos.Y - offsetY) / Game1.gameWorldTargetScale;

            // add 1 due to rounding errors
            nativeMouseX = MathHelper.Clamp(nativeMouseX, 0, Game1.NativeResolution.X);
            nativeMouseY = MathHelper.Clamp(nativeMouseY + 1, 0, Game1.NativeResolution.Y);

            // have to account for the translation effect to get in game world coordinates
            return new Point((int) (nativeMouseX - Game1.translation.Translation.X), (int) (nativeMouseY - Game1.translation.Translation.Y));
        }

        public static Point GetNearestTile(Point pos)
        {
            return new Point((int)(pos.X / (Map.scaledTileSize)), (int)(pos.Y / (Map.scaledTileSize)));
        }
        public static Point GetNearestTile(Vector2 pos)
        {
            return new Point((int) (pos.X / (Map.scaledTileSize)), (int) (pos.Y / (Map.scaledTileSize)));
        }


        public static Point OneToTwoDimensionalIndex(int index, int columns)
        {
            return new Point(index % columns, index / columns);
        }

        public static Vector2 PointToVector2(Point pt)
        {
            return new Vector2(pt.X, pt.Y);
        }

        public static Vector2 GetCenterOrigin(Renderable text)
        {
            return new Vector2(text.size.X / 2, text.size.Y / 2);
        }

        public static bool IsTileWithinOneTileOfPlayer(Point tile)
        {
            Point playerTile = Game1.player.feet;

            return (
                playerTile == tile || (
                (tile.X >= playerTile.X - 1 && tile.X <= playerTile.X + 1) 
                && 
                (tile.Y >= playerTile.Y - 1 && tile.Y <= playerTile.Y + 1)
                ));
        }
        public static Point GetTileFromPos(Vector2 pos)
        {
            int tileSize = Map.scaledTileSize;
            return new Point((int) pos.X / tileSize, (int) pos.Y / tileSize);
        }


    }
}
