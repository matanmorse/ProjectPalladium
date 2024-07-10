﻿using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium.Tools;
using ProjectPalladium.Stations;
using Microsoft.Xna.Framework;

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
        public bool potionIngredient;

        public Renderable sprite;
        public static Item none = new Item(-1, "", "", -1, "", -1);



        public Item(int id, string name, string textureName, int quantity, string description, int stacksize, bool potionIngredient=false)
        {
            this.id = id;
            this.name = name;
            this.textureName = textureName;
            this.quantity = quantity;
            this.description = description;
            this.stackSize = stacksize;
            this.potionIngredient = potionIngredient;
            this.sprite = new Renderable(textureName);
        }
        private static Dictionary<String, Item> Items = new Dictionary<String, Item>()
        {
            { "wand", new Wand(0, "wand", "wand", "a wand") },
            { "ectoplasmic gem", new Item(1, "Ectoplasmic Gem", "ectoplasmicgem", 1, "a gem", 99, potionIngredient:true)},
            { "mana melon seed", new Seed(2, "Mana Melon Seed", "manamelonseed", 1, "some mana melon seeds", 99, "manamelonplant") },
            { "scrying orb", new Placeable(3, "Scrying Orb", "scryingorb", 1, "An orb for scrying.")},
            { "cauldron", new Placeable(4, "Cauldron", "cauldron", 1, "A cauldron for brewing potions.", worldObjectType:typeof(Cauldron)) }
        
        };

        public static Item GetItemFromRegistry(string name)
        {
            return Items[name].Clone();
        }

        public Item Clone()
        {
            return (Item)(this.MemberwiseClone());
        }

        public override string ToString()
        {
            return name + " " + quantity;
        }
        public override int GetHashCode() { return 0; }

        public bool IsSameItemStack (Item other)
        {
            if (other == null) return false;
            return (other.id == id && other.quantity == quantity);
        }

        public override bool Equals(object obj)
        {
            if (obj is Item)
            {
                return ((Item)obj).id == this.id;
            }
            return false;
        }
        public static bool operator ==(Item i1, Item i2)
        {
            if ((object)i1 == null && (object)i2 == null) return true;

            if ((object) i1 == null || (object) i2 == null) return false;
            return (i1.id == i2.id);
        }

        public static bool operator != (Item i1, Item i2)
        {
            
            if ((object)i1 == null && (object)i2 == null) return false;
            if ((object)i1 == null ^ (object)i2 == null) return true;
           
            return !(i1.Equals(i2));
        }

        public virtual void Update() {}

        public virtual void Use() {}

        public virtual void Draw(SpriteBatch b, Vector2 pos, float scale, Vector2 origin)
        {
            sprite.Draw(b, pos, layer: Game1.layers.UI, scale: scale, origin:origin);
        }
    };
 

}
  

