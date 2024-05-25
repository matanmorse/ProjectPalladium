using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ProjectPalladium.Utils;
using Microsoft.Xna.Framework;
using ProjectPalladium.Items;
using Tutorial;
using System.Runtime.CompilerServices;
using System.Diagnostics;
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

        private int swap1 = -1;
        private bool updateItems;

        private List<UIElement> tmpItems;
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

            if (swap1 == -1 && slot.Item == Item.none) return;
            if (swap1 == -1) { swap1 = slot.index; return; }
            if (swap1 == slot.index) { swap1 = -1; return; }
            else { SwapItems(slot.index); }
        }

        private void SwapItems(int index2)
        {
            tmpItems = new List<UIElement>(children); // make copy of list of children
            updateItems = true;
            
            ((ItemSlot)children[swap1]).Reset();
            ((ItemSlot)children[index2]).Reset();
            inv.SwapItems(swap1, index2);

            swap1 = -1;
        }

        public override void ToggleShowing()
        {
            Input.Update();
            base.ToggleShowing();
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
            if (updateItems) children = tmpItems; updateItems = false;
        }
       
        public bool ReplaceItemSlot(Item item, int index, List<UIElement> tmpList)
        {
            updateItems = true;
            Point pos = Util.OneToTwoDimensionalIndex(index, NUM_COLUMNS);
            ItemSlot newItem = new ItemSlot(item.name, item, 0, 0, this, Util.OneToTwoDimensionalIndex(index, NUM_COLUMNS), scale:scale);
            newItem.OnSlotClicked += SlotClicked;
            tmpList[index] = newItem;
            return true;
        }

        public void UpdateInventory()
        {
            tmpItems = new List<UIElement>(children); // make copy of list of children
            toolbar.UpdateToolbar();
           
            for (int i = 0; i < INV_SIZE; i++)
            {
                // if the item in inventory is empty, get the none item
                Item itemInInv = inv.GetAtIndex(i) == null ? Item.none : inv.GetAtIndex(i);
                if (((ItemSlot)children[i]).Item != itemInInv) { ReplaceItemSlot(itemInInv, i, tmpItems); }
            }

        }
        
    }
}
