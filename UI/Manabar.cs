using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace ProjectPalladium.UI
{
    public class Manabar : UIElement
    {


        public Point topLeft;
        public Point maxSize;


        private Color MANA_COLOR = Color.BlueViolet;
        private Rectangle manaRectangle;
        private Texture2D ManaRect;

        public Manabar(string name, string textureName, UIElement parent)
        :base(name, textureName, 0,0, parent)
        {
            LocalPos = new Point((int)(10 * scale), Game1.UINativeResolution.Y - (int)((Sprite.size.Y + 7)* scale));
            topLeft = new Point(0, (int) (12 * scale));
            maxSize = new Point((int) (10 * scale), (int) (53 * scale));
        }

        public override void Initialize()
        {
            UpdateManaRectangle();
        }
        public void UpdateManaRectangle()
        {
            if (Game1.player == null && player == null) return;
            else if (player == null) { player = Game1.player; }

            // proportion of the bar that should be full
            float interpolatedMana = (float) player.mana / Player.MAX_MANA;
            // calculate correct area based on current mana
            manaRectangle = new Rectangle(globalPos + topLeft + new Point(0, (int) Math.Abs(maxSize.Y * interpolatedMana - maxSize.Y)), new Point(maxSize.X, (int)(maxSize.Y * interpolatedMana)));
            
        } 
        public override void Update()
        {
            UpdateManaRectangle();
            base.Update();
        }

        public override void Draw(SpriteBatch b)
        {
            if (ManaRect == null)
            {
                ManaRect = new Texture2D(b.GraphicsDevice, 1, 1);
                ManaRect.SetData(new[] { MANA_COLOR });
            }
            b.Draw(ManaRect, manaRectangle, Color.White);
            base.Draw(b);
        }

    }
}
