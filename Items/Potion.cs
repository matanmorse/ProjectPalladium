using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Items
{
    public class Potion : Item
    {
        public Renderable bottleSprite = new Renderable("potionbottle");
        public Renderable contentSprite = new Renderable("potioncontents");
        public static int POTION_ID = 6;
        public static int POTION_STACKSIZE = 5;
        public Color contentColor = Color.Blue;
        public Potion(string name, string description) 
            : base(POTION_ID, name, "potionbottle", 1, "", POTION_STACKSIZE)
        {
            sprite = bottleSprite; // for logic purposes
        }


        public override void Draw(SpriteBatch b, Vector2 pos, float scale, Vector2 origin)
        {
            
            bottleSprite.Draw(b, pos, layer: Game1.layers.UI, scale:scale, origin:origin);
            contentSprite.Draw(b, pos, color: contentColor, layer: Game1.layers.UI, scale: scale, origin: origin);
        }
    }
}
