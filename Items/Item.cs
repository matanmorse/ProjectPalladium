using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Items
{
    public class Item
    {
        public int id;
        public string name;
        public string textureName;
        public int quantity;
        public string description;
        public int stackSize;

        public static Item none = new Item(-1, "", "", -1, "", -1);



        public Item(int id, string name, string textureName, int quantity, string description, int stacksize)
        {
            this.id = id;
            this.name = name;
            this.textureName = textureName;
            this.quantity = quantity;
            this.description = description;
            this.stackSize = stacksize;
        }
        public static Dictionary<String, Item> Items = new Dictionary<String, Item>()
        {
            { "wand", new Item(0, "wand", "wand", 1, "a wand", 1) },
            { "ectoplasmic gem", new Item(1, "Ectoplasmic Gem", "ectoplasmicgem", 1, "a gem", 99)}
        };

        public Item Clone()
        {
            return (Item)(this.MemberwiseClone());
        }

        public override int GetHashCode() { return 0; }

        public override bool Equals(object obj)
        {
            if (obj is Item)
            {
                return ((Item)obj).id == this.id;
            }
            return false;
        }
    };
 

}
  

