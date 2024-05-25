using ProjectPalladium.Items;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Reflection.Metadata.Ecma335;
namespace ProjectPalladium.UI
{
    public class Toolbar : UIElement
    {
        private const int toolBarSize = 10;
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
                children.Add(new ItemSlot("", item, 0, 0, this, pos, scale:scale));
            }
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
                if (((ItemSlot)children[i]).Item != itemInInv) { ReplaceItemSlot(itemInInv, i); }
            }
            
        }
    }
}
