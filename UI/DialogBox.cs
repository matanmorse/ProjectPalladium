using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ProjectPalladium.Utils;
using System.Diagnostics;

namespace ProjectPalladium.UI
{
    public class DialogBox : UIElement
    {
        private static Texture2D backgroundTexture = new Texture2D(Game1.graphicsDevice, 1, 1);
        private TextRenderer textRenderer;

        private const float defaultScale = 2f;
        private const float maxWidth = 100 * defaultScale;

        private Point itemSlotOffset = (new Point(10, 10) * new Point((int)UIManager.inventoryScale)); // use the inventory scale since we want to be at edge of itemslot
        private Rectangle bounds;
        private Point size;
        private Point padding = new Point(10, 5);
        Point paddingOffset;

        private string text;

        public DialogBox(string name, Point pos, string text) 
            : base(name, "", pos.X, pos.Y, UIManager.rootElement, OriginType.def, defaultScale, false, false, 0)
        {
            this.text = text;
            paddingOffset = ScalePoint(new Point(5 , 2));
            Vector2 textPos = (pos + itemSlotOffset + paddingOffset).ToVector2();
            this.textRenderer = new TextRenderer(textPos, TextRenderer.Origin.topLeft);

            this.bounds = CalculateBounds();

            backgroundTexture.SetData(new[] { Color.White });
        }

        public void SetPosition(Point pos)
        {
            this.bounds.Location = pos + itemSlotOffset;
            this.textRenderer.pos = (bounds.Location + paddingOffset).ToVector2 ();
        }
        private Rectangle CalculateBounds()
        {
            List<string> words = text.Split(" ").ToList();
            string resultString = "";
            float lineSize = 0f;
            string line = "";

            if (textRenderer.font.MeasureString(text).X < maxWidth)
            {
                resultString = text;
            }
            else
            {
                foreach (string word in words)
                {

                    float wordSize = textRenderer.font.MeasureString(word).X;
                    lineSize += wordSize;

                    if (lineSize > maxWidth)
                    {
                        // this line is done, add a newline
                        resultString += line + "\n";

                        // start new line with this word
                        lineSize = wordSize;
                        line = word + " ";
                    }
                    else
                    {
                        line += word + " ";
                    }

                }
                resultString += line.TrimEnd(); // Trim any trailing space

            }


            this.text = resultString;
            this.size = textRenderer.font.MeasureString(resultString).ToPoint() + ScalePoint(padding);
            return new Rectangle(localPos + itemSlotOffset, size);
        }
        public override void Draw(SpriteBatch b)
        {
            base.Draw(b);
            b.Draw(backgroundTexture, bounds, new Rectangle(0,0,1,1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth:0.97f);
            textRenderer.Draw(b, text, layer:0.98f);
        }
    }
}
