using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace ProjectPalladium.Content
{
    public class Util
    {
        public static void DrawRectangle(Rectangle rect, SpriteBatch b)
        {
            Color color = Color.Red;
            Texture2D _texture = new Texture2D(b.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] {Color.DeepPink});

            int thickness = 5;
            Point rectUpperLeft = new Point(rect.Left, rect.Top);
            Point rectLowerLeft = new Point(rect.Right, rect.Bottom);

            Vector2 origin = new Vector2(rect.X / 2, rect.Y / 2);

            Rectangle top = new Rectangle(rectUpperLeft, new Point(rect.Size.X, thickness));
            Rectangle bottom = new Rectangle(new Point(rect.Left, rect.Bottom - thickness), new Point(rect.Size.X, thickness));
            Rectangle left = new Rectangle(rectUpperLeft, new Point(thickness, rect.Size.Y));
            Rectangle right = new Rectangle(new Point(rect.Right - thickness, rect.Top), new Point(thickness, rect.Size.Y));

            b.Draw(_texture, right, color);
            b.Draw(_texture, top, color);
            b.Draw(_texture, bottom, color);
            b.Draw(_texture, left, color);


        }
    }
}
