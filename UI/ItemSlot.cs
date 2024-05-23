using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
namespace ProjectPalladium.UI
{
    public class ItemSlot : UIElement
    {
        private readonly Point ITEM_SLOT_OFFSET = new Point(4, 1);
        private readonly Point OFFSET_PER_POSITION = new Point(17, 0);
        private readonly Point TOOLBAR_TOP_LEFT;
        // the position of the item in the toolbar, from left to right
        public int position;
        public ItemSlot(string name, string textureName, int localX, int localY, UIElement parent, int position,
            OriginType originType = OriginType.def, bool isRoot = false, bool isBox = false) : base(name, textureName, localX, localY, parent, originType, isRoot:isRoot, isBox:isBox)
        {
            TOOLBAR_TOP_LEFT = (parent.Sprite.size / new Point(-2, -2));
            localPos = TOOLBAR_TOP_LEFT + ITEM_SLOT_OFFSET + OFFSET_PER_POSITION * new Point(position, position);
            UpdateGlobalPos();
            AddButton(null, null, null, new Point(16, 16));
        }

        public override void Draw(SpriteBatch b) 
        {
            base.Draw(b);
            if (button != null) if (button.mouseOver || button.clickState) { Util.DrawRectangle(button.bounds, b); }
        }



    }
}
