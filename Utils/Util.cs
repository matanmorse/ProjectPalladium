using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectPalladium.Buildings;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;
using System.Reflection.Metadata.Ecma335;
using ProjectPalladium;
using System.Transactions;
using System;
using static ProjectPalladium.Utils.Util;
namespace ProjectPalladium.Utils
{
    public class Util
    {
        public static float scale = Game1.scale;
        public static int tileSize = 16;
        public static Vector2 OneTileDown = new Vector2(0, 1);
        public static Vector2 OneTileDownGameWorld = LocalPosToGlobalPos(OneTileDown);
        static Texture2D pixel = new Texture2D(Game1.instance.GraphicsDevice, 1, 1);

        public struct Circle
        {

            public Circle(Vector2 pos, int radius)
            {
                this.pos = pos.ToPoint();
                x = (int)pos.X;
                y = (int)pos.Y;
                this.radius = radius;
            }
            public Circle(Point pos, int radius)
            {
                this.pos = pos;
                x = pos.X;
                y = pos.Y;
                this.radius = radius;
            }
            public Circle(int x, int y, int radius)
            {
                this.x = x;
                this.y = y;
                pos = new Point(x, y);
                this.radius = radius;
            }

            public int radius;
            public int x;
            public int y;
            private Point pos;
            public Point Pos { get { return pos; } set { x = value.X; y = value.Y; this.pos = value; } }
            private Vector2[] CreateCirclePoints(float radius, int sides)
            {
                Vector2[] points = new Vector2[sides];
                float angleStep = MathHelper.TwoPi / sides;

                for (int i = 0; i < sides; i++)
                {
                    float angle = i * angleStep;
                    points[i] = new Vector2(radius * (float)Math.Cos(angle), radius * (float)Math.Sin(angle));
                }

                return points;
            }
            public void DrawCircle(SpriteBatch spriteBatch)
            {
                int sides = 20;
                Vector2 center = new Vector2(x, y);
                Vector2[] points = CreateCirclePoints(radius, sides);

                for (int i = 0; i < points.Length; i++)
                {
                    Vector2 start = points[i] + center;
                    Vector2 end = points[(i + 1) % points.Length] + center;
                    DrawLine(spriteBatch, start, end, Color.Red);
                }
            }

            public bool Intersects(Rectangle r, bool includeTotalIntersection = true)
            {
                // find the closest point on the rectangle to the center of the circle
                Vector2 closestPoint = new Vector2(Math.Clamp(x, r.X, r.X + r.Width), Math.Clamp(y, r.Y, r.Y + r.Height)); 

                float distFromClosestPointSquared = (pos.ToVector2() - closestPoint).LengthSquared();

                // using euclidian distance formula, if distance to closest point is larger than radius-squared of the circle, the two do not intersect
                if (distFromClosestPointSquared <= (radius * radius)) return true;
                if ((!includeTotalIntersection)) return distFromClosestPointSquared <= (radius * radius);

                // if not direct intersection, check if rectangle is completely within circle
                // Check if the circle is within the left and right bounds of the rectangle
                bool withinLeftAndRight = (pos.X - radius >= r.Location.X) &&
                (pos.X + radius <= r.Location.X + r.Width);

                // Check if the circle is within the top and bottom bounds of the rectangle
                bool withinTopAndBottom = (pos.Y - radius >= r.Location.Y) &&
                (r.Center.Y + radius <= r.Location.Y + r.Height);

                return withinLeftAndRight && withinTopAndBottom;

            }
        }


        public static void DrawLine(SpriteBatch b, Vector2 start, Vector2 end, Color color)
        {
            int thickness = 5;

            // Create a 1x1 white texture
            pixel.SetData(new[] { Color.White });

            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            b.Draw(pixel,
                new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), thickness),
                null,
                color,
                angle,
                new Vector2(0f, 0.5f), // origin at center of line
                SpriteEffects.None,
                1f);    
        }

        // cross product of 2d vectors
        public static float Cross(Vector2 v1, Vector2 v2)
        {
            return (v1.X * v2.Y) - (v1.Y * v2.X);
        }
        public static void DrawRectangle(Rectangle rect, SpriteBatch b)
        {
            Color color = Color.White;
            pixel.SetData(new[] { Color.White });

            int thickness = 5;

            b.Draw(pixel, new Vector2(rect.Left, rect.Top ), new Rectangle(0, 0, thickness, rect.Size.Y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            b.Draw(pixel, new Vector2(rect.Left , rect.Top ), new Rectangle(0, 0, rect.Size.X, thickness), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f); ;
            b.Draw(pixel, new Vector2(rect.Left + rect.Size.X, rect.Top ), new Rectangle(0, 0, thickness, rect.Size.Y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f); ;
            b.Draw(pixel, new Vector2(rect.Left , rect.Top + rect.Size.Y -1), new Rectangle(0, 0, rect.Size.X + thickness, thickness), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f); ;

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

        /* Calculates the difference (in minutes) between two game times */
        public static int CalculateMinutesDifference(GameManager.GameWorldTime EndTime, GameManager.GameWorldTime StartTime)
        {
            int endAdjustedHours = EndTime.isAM ? EndTime.Hour : EndTime.Hour + 12; // convert to 24-hour time
            int startAdjustedHours = StartTime.isAM ? StartTime.Hour : StartTime.Hour + 12;

            return (((endAdjustedHours * 60) + EndTime.Minute) - ((startAdjustedHours * 60) + StartTime.Minute));
        }

        public static int FindTileIDFromRect(Rectangle sourceRectangle, Texture2D tileMap)
        {
            int tilesPerRow = tileMap.Width / tileSize;

            // the tile's x and y index is the texture coordinate divide by tile size, convert this to 1 dimensional index
            int index = (sourceRectangle.Y / tileSize) * tilesPerRow + (sourceRectangle.X / tileSize);
            
            // because of the fact that tile ID 0 is actually represented by 1 in the tmx file (since 0 is an empty tile), we have to distinguish between two kinds of index 0's
            // one kind is the empty rectangle representing no tile, and one is the first tile in the list, which has an id of 0 but is given as one in the tmx file.
            // therefore to correctly determine if a rectangle is truly index 0 or just empty we need to check the size of the rectangle.
            if (index == 0)
            {
                if (sourceRectangle.Size == Point.Zero) { return index; } // empty rectangle, return index 0
            }

            return index + 1; // otherwise the true index is one more than the position because of tmx fuckery
        }

        // return a rectangle of a certain size centered on the point
        public static Rectangle MakeRangeRect(Point pos, int range)
        {
            Point size = new Point(range) * new Point((int)Game1.scale);
            return new Rectangle((int)(pos.X - size.X / 2), (int)(pos.Y - size.Y / 2), size.X, size.Y);
        }
    }
}
