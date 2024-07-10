using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Items
{
    /* Items which can be used as ingredients for potions and other stations */
    public class Ingredient : Item
    {
        private const int NUM_EFFECTS = 4;
        public PotionEffects[] effects = new PotionEffects[NUM_EFFECTS];
        public int strength; // modifier for potion effects
        public Ingredient(int id, string name, string textureName, int quantity, string description, int stacksize, bool potionIngredient = false, PotionEffects[] effects = null)
            : base(id, name, textureName, quantity, description, stacksize, potionIngredient)
        {
            this.effects = effects;
        }


    }
}
