using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Effects;
using ProjectPalladium.Utils;
using System.Collections.Generic;
using PotionEffects = ProjectPalladium.Items.Ingredient.PotionEffects;
namespace ProjectPalladium.Items
{
    public class Potion : Item
    {
        
        public struct ApplicableEffect
        {
            
            public static readonly ApplicableEffect None = new ApplicableEffect();
            public PotionEffects effect;
            public IPotionEffect effectStrategy;

            public float strength; // multiplier to base effect
            public int duration; // seconds the potion is active for

            public ApplicableEffect(PotionEffects effect, float strength, int duration)
            {
                this.effect = effect;
                this.strength = strength;
                this.duration = duration;

                if (effectStrategies.ContainsKey(effect))
                {
                    this.effectStrategy = effectStrategies[effect];
                }
                else
                {
                    this.effectStrategy = null;
                }
            }

            public void Apply()
            {
                if (effectStrategy == null) return;
                effectStrategy.ApplyEffect(strength, duration);
            }

            public bool Equals(ApplicableEffect other)
            {
                return (other.effect == this.effect && other.duration == this.duration && other.strength == this.strength);
            }
            public override string ToString()
            {
                if (this.Equals(ApplicableEffect.None)) return "None";
                string result = "Effect: " + effect + "\n Strength: " + strength;
                if (duration > 0) result += "\n Duration: " + duration;
                return result;
            }
        }

        public static Dictionary<PotionEffects, int> baseValues = new Dictionary<PotionEffects, int>()
            {
                {PotionEffects.RestoreMana, 10 },
                {PotionEffects.RestoreHealth, 10 },
                {PotionEffects.FortifyHealth, 5 },
                {PotionEffects.FortifyEvocation, 5 },
            };

        public static Dictionary<PotionEffects, int> baseDurations = new Dictionary<PotionEffects, int>()
            {
                {PotionEffects.RestoreMana, 0 },
                {PotionEffects.RestoreHealth, 0 },
                {PotionEffects.FortifyHealth, 60 },
                {PotionEffects.FortifyEvocation, 60 },
            };

       
        public Renderable bottleSprite = new Renderable("potionbottle");
        public Renderable contentSprite = new Renderable("potioncontents");

        public static int POTION_ID = 6;
        public static int POTION_STACKSIZE = 5;
        const int NUM_EFFECTS = 4; // max number of effects

        public Color contentColor;
        private Item[] ingredients;
        private ApplicableEffect[] effects;


        private static Dictionary<PotionEffects, IPotionEffect> effectStrategies = new Dictionary<PotionEffects, IPotionEffect>()
        {
            { PotionEffects.RestoreMana, new RestoreManaEffect() },
        };

        public Potion(Item[] ingredients) 
        : base(POTION_ID, "", "potionbottle", 1, "", POTION_STACKSIZE)
        {
            this.name = "Potion";
            this.ingredients = ingredients;
            this.effects = CalculateEffects();
            contentColor = CalculateColor();
                
            sprite = bottleSprite; // for logic purposes

            this.description = this.ToString();
        }

        private ApplicableEffect[] CalculateEffects()
        {
            ApplicableEffect[] result = new ApplicableEffect[NUM_EFFECTS];
            Dictionary<PotionEffects, int> counts = new Dictionary<PotionEffects, int>();
            Dictionary<PotionEffects, float> strengths = new Dictionary<PotionEffects, float>(); // associated strengths for each effect
            Dictionary<PotionEffects, float> durations = new Dictionary<PotionEffects, float>(); // associated durations for each effect

            // first, count number of occurances of each potion effect
            for (int i = 0; i < ingredients.Length; i++)
            {
                Ingredient item = ingredients[i] as Ingredient;
                if (item.potionEffects == null) continue; // shouldn't happen, but just in case

                for (int j = 0; j < item.potionEffects.Length; j++)
                {
                    PotionEffects effect = item.potionEffects[j];
                    if (!(counts.ContainsKey(effect)))
                    {
                        counts.Add(effect, 1);
                        strengths.Add(effect, item.potionStrength);
                        durations.Add(effect, item.durationStrength);
                    }
                    else
                    {
                        counts[effect]++;
                        strengths[effect] += item.potionStrength;
                        durations[effect] += item.durationStrength;
                    }
                }
            }

            int index = 0;
            foreach (PotionEffects key in counts.Keys)
            {
    
                if (key == PotionEffects.None) continue;
                if (counts[key] >= 2) // if more than two occurances, this is a potion effect
                {
                    result[index] = new ApplicableEffect(key,  baseValues[key] * strengths[key], (int)(baseDurations[key] * durations[key]));
                    index++;
                }
            }
            // ensure no null items in effects array
            while (index < NUM_EFFECTS)
            {
                result[index] = ApplicableEffect.None;
                index++;
            }
            return result;
        }
        private Color CalculateColor()
        {
            Color[] averageColors = new Color[ingredients.Length];

            int i = 0;
            foreach (Item item in ingredients)
            {
                Color[] colorData = new Color[item.sprite.Texture.Width * item.sprite.Texture.Height];
                item.sprite.Texture.GetData(colorData);

                int totalRed = 0;
                int totalGreen = 0;
                int totalBlue = 0;

                int numColors = 0;

                foreach (Color color in colorData)
                {
                    if (color == Color.Transparent) continue;
                    totalRed += color.R;
                    totalGreen += color.G;
                    totalBlue += color.B;
                    numColors++;
                }

                int avgRed = totalRed / numColors;
                int avgGreen = totalGreen / numColors;
                int avgBlue = totalBlue / numColors;

                Color avgColor = new Color(avgRed, avgGreen, avgBlue);
                averageColors[i] = avgColor;
                i++;
            }

            int tRed = 0;
            int tGreen = 0;
            int tBlue = 0;
            int nColors = 0;

            for (int j = 0; j < averageColors.Length; j++)
            {


                if (averageColors[j] == Color.Transparent) continue;
                tRed += averageColors[j].R;
                tGreen += averageColors[j].G;
                tBlue += averageColors[j].B;
                nColors++;
            }
            Color totalAverageColor = new Color(tRed/nColors, tGreen/nColors, tBlue/nColors);
            return totalAverageColor;
        }

        public override void Use()
        {
            foreach(ApplicableEffect effect in effects)
            {
                effect.Apply();
            }
            Game1.player.inventory.RemoveCurrentItem(1);
        }


        public bool Equals(Potion other)
        {
            return (this.contentColor == other.contentColor && this.quantity == other.quantity); 
        }
        public override void Draw(SpriteBatch b, Vector2 pos, float scale, Vector2 origin, float layer = 0.91f)
        {
            
            bottleSprite.Draw(b, pos, layer: 0.95f, scale:scale, origin:origin);
            contentSprite.Draw(b, pos, color: contentColor, layer: Game1.layers.UI +0.01f, scale: scale, origin: origin);
        }

        public override string ToString()
        {
            string result = "Potion";
            int i = 1;
            foreach (ApplicableEffect eff in effects)
            {
                if (eff.Equals(ApplicableEffect.None)) continue;
                result += "\n" + i + "\n" + eff.ToString();
                i++;
            }
            return result;
        }
    }
}
