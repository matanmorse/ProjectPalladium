using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.UI
{
    public class Toolbar : UIElement
    {
        private const int toolBarSize = 10;
        public Toolbar(string name, string textureName, int localX, int localY, UIElement parent, 
            OriginType originType = OriginType.def, bool isRoot = false, bool isBox = false) : base(name, textureName, localX, localY, parent, originType, isRoot:isRoot, isBox: isBox)
        {
            for (int i = 0; i < toolBarSize - 3; i++)
            {
                AddChild(new ItemSlot("slot " + i, "wand", 0, 0, this, i));
            }
            AddChild(new ItemSlot("slot " , "wand", 0, 0, this, 9));

        }
    }
}
