using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.UI
{
    public class TimeDisplay : UIElement
    {
        public TextRenderer timeText;
        public Vector2 TIME_TEXT_OFFSET;

        
        public TimeDisplay(string name, string texturename, UIElement parent) : base(name, texturename, 0,0, parent, scale: 4f)
        {
            this.LocalPos = new Point(Game1.UINativeResolution.X - (int)(70 * scale), (int)(10 * scale));
            
            TIME_TEXT_OFFSET = (Sprite.size.ToVector2() * scale) / 2;
                
            timeText = new TextRenderer(globalPos.ToVector2() + TIME_TEXT_OFFSET, TextRenderer.Origin.center, "timetext");

            AddToRoot();
        }

        public override void Draw(SpriteBatch b)
        {
            base.Draw(b);
            timeText.Draw(b, GameManager.time.ToString());
        }
    }
}
