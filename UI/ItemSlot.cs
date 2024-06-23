using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
using ProjectPalladium.Items;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
namespace ProjectPalladium.UI
{

    public class ItemSlot : UIElement
    {
        private readonly Point ITEM_SLOT_OFFSET = new Point(4, 1);
        private readonly Point OFFSET_PER_INDEX = new Point(17, 17);
        public int index;
        private readonly Point TOOLBAR_TOP_LEFT;
        private Item item;
        public Item Item { get { return this.item; } set {

                this.item = value; this.name = this.item.name;
                this.Sprite = new Renderable(item.textureName); 
            } }
        private Rectangle bounds = new Rectangle();
        private TextRenderer itemCount;

        public event Action<ItemSlot> OnSlotClicked;
       
        public void onClick()
        {
            if (OnSlotClicked == null) return;
            OnSlotClicked.Invoke(this);
        }

        // the position of the item in the toolbar, from left to right
        public ItemSlot(string name, Item item, int localX, int localY, UIElement parent, Point position,
            OriginType originType = OriginType.def, bool isRoot = false, bool isBox = false, float scale = 1f) 
            : base(name, item.textureName, localX, localY, parent, OriginType.center, isRoot:isRoot, isBox:isBox, scale:scale)
        {
            // position offsets
            TOOLBAR_TOP_LEFT = (parent.Sprite.size / new Point(-2, -2));
            Point CENTER_OFFSET = ((new Point(16,16)) / new Point(2));

            localPos = ScalePoint(TOOLBAR_TOP_LEFT + ITEM_SLOT_OFFSET + CENTER_OFFSET + OFFSET_PER_INDEX * position);

            this.index = position.X + (position.Y * 10);
            this.item = item;
            
            UpdateGlobalPos();

            AddButton(null, null, onClick, new Point(16, 16));

            // bounds of this item slot
            bounds = ScaleRect(new Rectangle(globalPos.X, globalPos.Y, 16, 16));

            // initialize the item count text
            Vector2 ItemCountTextPos = new Vector2(globalPos.X + (bounds.Width / 2), globalPos.Y + (bounds.Height / 2));
            this.itemCount = new TextRenderer(ItemCountTextPos);
            
        }

        private void ApplyEffects(SpriteBatch b)
        {
            if (button.mouseOver && !button.clickState)
            {
                scale = parent.scale + 0.5f;
            }
            else
            {
                scale = parent.scale;
                if (button != null && item != Item.none) if (item.IsSameItemStack(Game1.player.ActiveItem) && parent is Toolbar) { Util.DrawRectangle(button.bounds, b); }
            }
        }

        public void Reset()
        {
            this.button.mouseOver = false;
            this.button.clickState = false;

        }
        public override void Draw(SpriteBatch b) 
        {
            ApplyEffects(b);

            base.Draw(b);
            
            if (item.quantity > 1) { itemCount.Draw(b, item.quantity.ToString()); }

        }
    }
}
