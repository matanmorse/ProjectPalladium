using ProjectPalladium.Items;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Metadata.Ecma335;

namespace ProjectPalladium.UI
{
    public class GhostItem
    {
        private TextRenderer itemCount;
        public Item item;
        private float alpha = 0.6f;
        private Vector2 pos;
        private Renderable sprite;
        private float scale;
        public int originalIndex;
        public GhostItem(Item item, float scale) {
            this.scale = scale;
            this.sprite = new Renderable(item.textureName);
            this.pos = Util.PointToVector2(Input.nativeMousePos);
            this.item = item;
            this.itemCount = new TextRenderer(pos);

            

        }

        public void Update()
        {
            pos = this.pos = Util.PointToVector2(Input.nativeMousePos);
            itemCount.pos = new Vector2(pos.X + sprite.Texture.Width * scale / 2, pos.Y + sprite.Texture.Height * scale / 2);
        }

        public void Draw(SpriteBatch b)
        {
            // handle potions and other items with multiple sprites
            if (item is Potion)
            {
                Potion potion = item as Potion;
                potion.contentSprite.Draw(b, pos, color: potion.contentColor, opacity: alpha, origin: Util.GetCenterOrigin(sprite), scale: scale);
            }

            sprite.Draw(b, pos, opacity: alpha, origin:Util.GetCenterOrigin(sprite), scale:scale);
            if (item.quantity > 1) { itemCount.Draw(b, item.quantity.ToString()); }
        }

    }
}
