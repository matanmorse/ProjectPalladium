using ProjectPalladium.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace ProjectPalladium.Tools
{
    public class ToolSprite : Renderable
    {
        public bool showing = false;
        private Tool tool;
        private Vector2 pos;
        private const int FRAMES = 4;
        private Point bottomLeft;
        private static Point scale = new Point((int)Game1.scale);
        // points from top-left of sprite to appear at certain frames
        private Point[] ANIMATION_OFFSETS = new Point
        [FRAMES]
        {
            new Point(13, 20) * scale,
            new Point(14, 9) * scale,
            new Point(13, 20) * scale,
            new Point(13, 20) * scale
        };

        private float[] ROTATIONS = new float
       [FRAMES]
       {
            MathHelper.ToRadians(0),
            MathHelper.ToRadians(-45),
            MathHelper.ToRadians(135),
            MathHelper.ToRadians(135),
       };

        private int frame = 0;
        public ToolSprite(string textureName, Tool tool) : base(textureName)
        {
            this.tool = tool;
            bottomLeft = new Point(Texture.Bounds.Top, Texture.Bounds.Right);
        }

        public void NextFrame()
        {
            frame++;
        }
        public void DoToolAnimation()
        {
            showing = true;
            // get position from top-left of player
            this.pos = (Game1.player.pos - Game1.player.sprite.origin * new Vector2(Game1.scale));
        }

        public void EndAnim()
        {
            frame = 0;
            showing = false;
        }
       
        public void Draw(SpriteBatch b)
        {
            base.Draw(b, pos + ANIMATION_OFFSETS[frame].ToVector2() , origin:bottomLeft.ToVector2(), layer:1f, scale:Game1.scale / 1.5f, rotation: ROTATIONS[frame]);;
        }
    }
}
