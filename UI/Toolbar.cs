using ProjectPalladium.Items;
using ProjectPalladium.Utils;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using ProjectPalladium.Tools;
namespace ProjectPalladium.UI
{
    public class Toolbar : UIElement
    {
        public const int toolBarSize = 10;
        public Inventory inv;
        public Toolbar(string name, string textureName, int localX, int localY, UIElement parent,
            OriginType originType = OriginType.def, bool isRoot = false, bool isBox = false, float scale = 1f) 
            : base(name, textureName, localX, localY, parent, originType, isRoot:isRoot, isBox: isBox, scale:scale)
        {
            UIManager uiManager = Game1.UIManager;
            for (int i = 0; i < toolBarSize; i++)
            {
                Item item = Item.none;
                Point pos = Util.OneToTwoDimensionalIndex(i, toolBarSize);
                ItemSlot slot = new ItemSlot("", item, 0, 0, this, pos, scale: scale);
                slot.OnSlotClicked += SlotClicked;
                children.Add(slot);
            }

            AddToRoot();
        }

        public void ResetItemSlot(int index)
        {
            if (index == -1) return;
            (children[index] as ItemSlot).Reset();
        }
        private void SlotClicked(ItemSlot slot)
        {
            if (Game1.player.DialogueBoxOpen) return;
            if (Game1.player.ActiveItem != Item.none && Game1.player.ActiveItem != null) 
            { children[Game1.player.ActiveItemIndex].button.clickState = false ; }

            if (slot.button.clickState) { 
                SetActiveItem(slot);
                
                Game1.player.holdingTool = slot.Item is Tool;

            }
            else { 

                Game1.player.ActiveItem = Item.none;
                Game1.player.holdingTool = false;
            }
        }

        private void SetActiveItem(ItemSlot slot)
        {
            Game1.player.ActiveItem = slot.Item;
            Game1.player.ActiveItemIndex = slot.index;
        }

        public bool ReplaceItemSlot(Item item, int index)
        {
            Point pos = Util.OneToTwoDimensionalIndex(index, toolBarSize);
            children[index] = new ItemSlot(item.name, item, 0, 0, this, Util.OneToTwoDimensionalIndex(index, toolBarSize), scale:scale);
            return true;
        }

        public void UpdateToolbar()
        {
            for (int i = 0; i < toolBarSize; i++)
            {
                // if the item in inventory is empty, get the none item
                Item itemInInv = inv.GetAtIndex(i) == null ? Item.none : inv.GetAtIndex(i);
                ItemSlot curSlot = (ItemSlot)children[i];

                if (!(curSlot.Item.IsSameItemStack(itemInInv) && curSlot.index == i)) { ((ItemSlot)children[i]).Item = itemInInv; }
            }
            
        }

        public void Reset()
        {
            Game1.player.ActiveItem = null;
            foreach (ItemSlot slot in children)
            {
                slot.Reset();
            }
        }
    }
}
