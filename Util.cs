﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace ProjectPalladium
{
    public class Util
    {
        public static int tileSize = 16;
        public static void DrawRectangle(Rectangle rect, SpriteBatch b)
        {
            Color color = Color.Red;
            Texture2D _texture = new Texture2D(b.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] { Color.DeepPink });

            int thickness = 5;
            Point rectUpperLeft = new Point(rect.Left, rect.Top);
            Point rectLowerLeft = new Point(rect.Right, rect.Bottom);

            Vector2 origin = new Vector2(rect.X / 2, rect.Y / 2);

            Rectangle top = new Rectangle(rectUpperLeft, new Point(rect.Size.X, thickness));
            Rectangle bottom = new Rectangle(new Point(rect.Left, rect.Bottom - thickness), new Point(rect.Size.X, thickness));
            Rectangle left = new Rectangle(rectUpperLeft, new Point(thickness, rect.Size.Y));
            Rectangle right = new Rectangle(new Point(rect.Right - thickness, rect.Top), new Point(thickness, rect.Size.Y));

            b.Draw(_texture, right, null, color);
            b.Draw(_texture, top, color);
            b.Draw(_texture, bottom, color);
            b.Draw(_texture, left, color);
        }

        public static Vector2 GlobalPosToLocalPos(Vector2 global)
        {
            return new Vector2(global.X / tileSize * Game1.scale, global.Y / tileSize * Game1.scale);
        }
        public static Vector2 LocalPosToGlobalPos(Vector2 local)
        {
            return new Vector2(local.X * tileSize * Game1.scale, local.Y * tileSize * Game1.scale);
        }

        // convert a serialized rectange to a rectangle at a specific point, with offsets
        public static Rectangle makeRectFromPoints(ColliderDetails col, Vector2 pos)
        {
            return new Rectangle((int)(pos.X + (col.location[0] * Game1.scale)), (int)(pos.Y + (col.location[1] * Game1.scale)),
                            (int)(col.size[0] * Game1.scale), (int)(col.size[1] * Game1.scale));
        }
    }
}
