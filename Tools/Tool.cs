using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Items;
using ProjectPalladium.Utils;

namespace ProjectPalladium.Tools
{
    public class Tool : Item
    {
        private const int TOOL_STACK_SIZE = 1;
        public ToolSprite sprite;

        public Tool(int id, string name, string textureName, string description)
        : base(id, name, textureName, 1, description, TOOL_STACK_SIZE)
        {
            sprite = new ToolSprite(textureName, this);
        }

        public void DoToolAnimation()
        {
            sprite.DoToolAnimation();
        }
        
        public void Draw(SpriteBatch b)
        {
            if (!sprite.showing) return;
            sprite.Draw(b);
        }

    }
}
