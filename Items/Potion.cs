using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Items
{
    public class Potion : Item
    {
        public Renderable bottleSprite = new Renderable("potionbottle");
        public Renderable contentSprite = new Renderable("potioncontents");
        public static int POTION_ID = 6;
        public static int POTION_STACKSIZE = 5;
        public Color contentColor;
        Item[] ingredients;
        public Potion(Item[] ingredients) 
            : base(POTION_ID, "", "potionbottle", 1, "", POTION_STACKSIZE)
        {
            this.name = "Potion";
            this.ingredients = ingredients;
            contentColor = CalculateColor();
            sprite = bottleSprite; // for logic purposes
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
        public bool Equals(Potion other)
        {
            return (this.contentColor == other.contentColor); 
        }
        public override void Draw(SpriteBatch b, Vector2 pos, float scale, Vector2 origin)
        {
            
            bottleSprite.Draw(b, pos, layer: Game1.layers.UI, scale:scale, origin:origin);
            contentSprite.Draw(b, pos, color: contentColor, layer: Game1.layers.UI, scale: scale, origin: origin);
        }
    }
}
