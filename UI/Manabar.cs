using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using ProjectPalladium.Utils;

namespace ProjectPalladium.UI
{
    public class Manabar : UIElement
    {

        private Renderable bottleSprite = new Renderable(subfolder + "manabarbottle");
        private Texture2D contentsTexture = Game1.contentManager.Load<Texture2D>(subfolder + "manabarcontents");
        public Point topLeft;
        public Point maxSize;


        private Color MANA_COLOR = Color.BlueViolet;
        private Rectangle manaRectangle;
        private Texture2D ManaRect;

        public Manabar()
        :base("manabar", "", 0,0, UIManager.rootElement)
        {
            scale = UIManager.defaultUIScale + 1f;
            Sprite = bottleSprite;
            LocalPos = new Point((int)(2 * scale), Game1.UINativeResolution.Y - (int)((Sprite.size.Y + 2)* scale));
            topLeft = new Point(4 * (int)scale, 9 * (int)scale);
            maxSize = new Point((int) (10 * scale), (int) (55 * scale));
            AddToRoot();
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
            float interpolatedMana = (float) player.Mana / Player.MAX_MANA;
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
            if (!showing) return;
            base.Draw(b);
            b.Draw(contentsTexture, manaRectangle, Color.White);
        }

    }
}
