using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Utils
{
    public class TextRenderer
    {
        private SpriteFont font;
        public Vector2 pos;
        public TextRenderer(Vector2 pos)
        {
            font = Game1.contentManager.Load<SpriteFont>("text");
            this.pos = pos;
        }

        public void Draw(SpriteBatch b, string text)
        {
            
            Vector2 size = font.MeasureString(text);
            b.DrawString(font, text, pos, Color.Black, 0f, size, 1f, SpriteEffects.None, 1f);
        }
        
    }
}
