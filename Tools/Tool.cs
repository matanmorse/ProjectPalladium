using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium.Items;

namespace ProjectPalladium.Tools
{
    public class Tool : Item
    {
        private const int TOOL_STACK_SIZE = 1;
        public Tool(int id, string name, string textureName, string description)
        : base(id, name, textureName, 1, description, TOOL_STACK_SIZE)
        {

        }

        public override void Update()
        {

        }

    }
}
