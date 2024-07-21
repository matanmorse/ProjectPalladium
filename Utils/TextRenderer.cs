using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace ProjectPalladium.Utils
{
    public class TextRenderer
    {
        public bool showing = true;
        public SpriteFont font;
        public Vector2 pos;
        private Origin originType;
        public enum Origin
        {
            bottomRight,
            topLeft,
            center,
        }

        public TextRenderer(Vector2 pos, Origin originType=Origin.bottomRight, string textType = "text")
        {
            this.originType = originType;
            font = Game1.contentManager.Load<SpriteFont>(textType);
            this.pos = pos;
        }

        public virtual void Draw(SpriteBatch b, string text, float layer = 0.948f)
        {

            if (text == null) return;
            if (!showing) return;

            Vector2 size = font.MeasureString(text);

            Vector2 origin = Vector2.Zero;
            switch (originType) 
            {
                case Origin.bottomRight: origin = size; break;
                case Origin.topLeft: origin = Vector2.Zero; break;
                case Origin.center: origin = size / 2; break;
            }

            b.DrawString(font, text, pos, Color.Black, 0f, origin, 1f, SpriteEffects.None, layerDepth:layer);
        }
        
    }
}
