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
        Vector2 origin; // origin of the current item
        public Item Item { get { return this.item; } set {

                this.item = value; this.name = this.item.name;
                this.Sprite = new Renderable(item.textureName);

                GenerateDialogBox();
            }
        }
        private Rectangle bounds = new Rectangle();
        private TextRenderer itemCount;
        private DialogBox itemInfo; 

        public event Action<ItemSlot> OnSlotClicked;

        // generating a dialog box for an item is an expensive operation, so we can cache
        public static Dictionary<Item, DialogBox> cachedDialogBoxes = new Dictionary<Item, DialogBox>(); 
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

            GenerateDialogBox();
        }

       
        private void ApplyEffects(SpriteBatch b)
        {
            if (!parent.active) return;
            if (button.mouseOver && (!button.clickState || parent is InventoryUI) && !(index == Game1.player.ActiveItemIndex))
            {
                scale = parent.scale + 0.5f;
            }
            else
            {
                scale = parent.scale;
                if (button != null && item != Item.none) if (parent is Toolbar && this.index == Game1.player.ActiveItemIndex) { Util.DrawRectangle(button.bounds, b); }
            }
        }

        public void Reset()
        {
            this.button.mouseOver = false;
            this.button.clickState = false;

        }

        private void GenerateDialogBox()
        {
            if (parent is Toolbar) return;
            if (item == Item.none) { this.itemInfo = null; return; }

            // first, check if this is in the cache
            if (cachedDialogBoxes.ContainsKey(item)) 
            {
                DialogBox cacheRetrievedBox = cachedDialogBoxes[item];
                cacheRetrievedBox.SetPosition(globalPos);
                this.itemInfo = cacheRetrievedBox;  return; 
            }
           
            DialogBox newDialogBox = new DialogBox("Item Info", globalPos, item.description, this);
            cachedDialogBoxes.Add(item, newDialogBox); // cache the dialog box
            this.itemInfo = newDialogBox;
            
        }

        public override void Draw(SpriteBatch b) 
        {
            ApplyEffects(b);

            // draw item centered
            origin = ((item.sprite.size.ToVector2()) / 2);
            
            item.Draw(b, drawPos.ToVector2(), scale, origin);
            
            if (item.quantity > 1) { itemCount.Draw(b, item.quantity.ToString()); }
            if (parent is InventoryUI && button.mouseOver && itemInfo != null && UIManager.inventoryUI.ghostItem == null)
            {
                itemInfo.Draw(b);
            }
        }
    }
}
