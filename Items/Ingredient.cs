using System.Diagnostics;

namespace ProjectPalladium.Items
{
    /* Items which can be used as ingredients for potions and other stations */
    public class Ingredient : Item
    {

        // optional items, for potion ingredients
        public static int NUM_EFFECTS = 4;
        public PotionEffects[] potionEffects = new PotionEffects[NUM_EFFECTS];
        public const int STACKSIZE = 99; // all ingredients stack to 99
        public enum PotionEffects
        {
            None,
            FortifyHealth,
            RestoreMana,
            FortifyEvocation,
            RestoreHealth,
        }

        public float potionStrength; // modifier for potion effects
        public float durationStrength; // modifier for potion duration
        public Ingredient(int id, string name, string textureName, int quantity, string description, bool potionIngredient = false, float potionStrength = 0f, float potionDuration=0f, PotionEffects[] effects = null)
            : base(id, name, textureName, quantity, description, STACKSIZE, potionIngredient)
        {
            this.durationStrength = potionDuration;
            this.potionStrength = potionStrength;
            this.potionEffects = effects;
        }


    }
}
