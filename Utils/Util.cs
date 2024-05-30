using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectPalladium.Buildings;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;

namespace ProjectPalladium.Utils
{
    public class Util
    {
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
            float offsetX = (Game1.graphicsDevice.PresentationParameters.Bounds.Width - Game1.UINativeResolution.X * Game1.targetScale) / 2.0f;
            float offsetY = (Game1.graphicsDevice.PresentationParameters.Bounds.Height - Game1.UINativeResolution.Y * Game1.targetScale) / 2.0f;

            float nativeMouseX = (globalPos.X - offsetX) / Game1.targetScale;
            float nativeMouseY = (globalPos.Y - offsetY) / Game1.targetScale;

            nativeMouseX = MathHelper.Clamp(nativeMouseX, 0, Game1.UINativeResolution.X);
            nativeMouseY = MathHelper.Clamp(nativeMouseY, 0, Game1.UINativeResolution.Y);

            // subtract 1 b/c of rounding error
            return new Point((int)nativeMouseX - 1, (int)nativeMouseY - 1);
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

        public static Point GetTileFromPos(Vector2 pos)
        {
            int tileSize = SceneManager.CurScene.Map.tilemaps[0].TileSize;
            return new Point((int) pos.X / tileSize, (int) pos.Y / tileSize);
        }
    }
}
