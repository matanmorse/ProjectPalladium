using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ProjectPalladium.Utils;
using Microsoft.Xna.Framework;
using ProjectPalladium.Items;
using ProjectPalladium;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
namespace ProjectPalladium.UI
{
    public class InventoryUI : UIElement
    {
        private const int NUM_ROWS = 3;
        private const int NUM_COLUMNS = 10;
        private const int INV_SIZE = 30;
        private Inventory inv;

        public Toolbar toolbar;
        public Inventory Inventory { get { return inv; } set { this.inv = value; } }

        private bool updateItems;

        private List<UIElement> tmpItems;

        public GhostItem ghostItem;

        public InventoryUI(string name, string textureName, int localX, int localY, UIElement parent,
            OriginType originType = OriginType.def, float scale = 1f, bool isRoot = false, bool isBox = false)
            : base(name, textureName, localX, localY, parent, originType, scale, isRoot, isBox)
        {
            Initialize();
        }

        public void Initialize()
        {
            inv = new Inventory(this);
            // populate the inventory with ItemSlot(s)
            for (int i = 0; i < INV_SIZE; i++)
            {
                Item item = inv.GetAtIndex(i);
                if (item == null) item = Item.none;
                ItemSlot slot = new ItemSlot(item.name, item, 0, 0, this, Util.OneToTwoDimensionalIndex(i, NUM_COLUMNS), scale:scale);
                slot.OnSlotClicked += SlotClicked;
                AddChild(slot);
            }
        }

        private void SlotClicked(ItemSlot slot)
        {
            Item itemInSlot = inv.GetAtIndex(slot.index);
            if (ghostItem == null && slot.Item == Item.none) return;

        
            // in this case, create a ghost item that follows the player's mouse
            if (ghostItem == null) { 

                CreateGhostItem(slot.Item.Clone());
                ghostItem.originalIndex = slot.index;

                inv.RemoveItemAtIndex(slot.index, ghostItem.item);
                return;
            }
            
            else {  // we need to do some kind of swapping or combining
                if (itemInSlot == ghostItem.item && itemInSlot.quantity < itemInSlot.stackSize && ghostItem.item.quantity < ghostItem.item.stackSize)
                {
                    inv.AddItem(ghostItem.item, ghostItem.item.quantity);
                    DestroyGhostItem();
                }
                else if (inv.GetAtIndex(slot.index) == Item.none)
                {
                    inv.AddItemAtIndex(slot.index, ghostItem.item);
                    DestroyGhostItem();
                }
                else
                {
                    SwapItems(slot.index);
                    
                }
               
            }
        }

        public void CreateGhostItem(Item item)
        {
            this.ghostItem = new GhostItem(item, scale);
        }

        public void DestroyGhostItem()
        {
            this.ghostItem = null;
        }

        private void SwapItems(int index2)
        {
            tmpItems = new List<UIElement>(children); // make copy of list of children
            updateItems = true;

          
            inv.SwapItems(index2);
            // voodoo magic 
            for (int i = 0; i < INV_SIZE; i++)
            {
                ((ItemSlot)children[i]).Reset();
            }

        }

        public override void ToggleShowing()
        {
            Input.Update();
            base.ToggleShowing();

            Game1.player.usingItemLocked = showing;

            foreach(UIElement child in children)
            {
                if (child.GetType() != typeof(ItemSlot)) continue;
                child.button.mouseOver = false;
                child.button.clickState = false;
                child.button.Update();
            }

        }

        public override void Update()
        {
            base.Update();
            if (ghostItem != null) { ghostItem.Update(); }
            if (updateItems) children = tmpItems; updateItems = false;
        }
       


        public void UpdateInventory()
        {
            tmpItems = new List<UIElement>(children); // make copy of list of children
            toolbar.UpdateToolbar();

            for (int i = 0; i < INV_SIZE; i++)
            {
                // if the item in inventory is empty, get the none item
                Item itemInInv = inv.GetAtIndex(i) == null ? Item.none : inv.GetAtIndex(i);
                Item itemInItemSlot = ((ItemSlot)(children[i])).Item;

                // if the item in the inventory does not EXACTLY match the item in the UI, replace it
                if ( !(itemInInv == itemInItemSlot && itemInInv.quantity == itemInItemSlot.quantity && i == ((ItemSlot)children[i]).index))
                {
                    ((ItemSlot)(tmpItems[i])).Item = itemInInv; updateItems = true;
                }

            }

            
        }

        public override void Draw(SpriteBatch b)
        {
            base.Draw(b);
            if (ghostItem != null) { ghostItem.Draw(b); }
        }

    }
}
