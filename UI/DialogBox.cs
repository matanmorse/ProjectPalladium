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
using System.ComponentModel;

namespace ProjectPalladium.UI
{
    public class DialogBox : UIElement
    {
        private static Texture2D backgroundTexture = Game1.contentManager.Load<Texture2D>("dialogbox");
         Texture2D fillerTexture = new Texture2D(Game1.graphicsDevice, 1, 1);
        private TextRenderer textRenderer;

        private const float defaultScale = 4f;
        private const float maxWidth = 75 * defaultScale;

        private Object owner;

        private Point itemSlotOffset = (new Point(10, 10) * new Point((int)UIManager.inventoryScale)); // use the inventory scale since we want to be at edge of itemslot
        private Rectangle bounds;
        private Point size;
        public static Point padding = new Point(15,12);
        Point paddingOffset;

        private string text;
        private string originalText; 

        private bool needsToDrawBox = true; 

        private static int cornerSize = 3;
        private static int edgeSize = 3;
        private static int centerSize = 3;
        // rectangles for the dialog box slices
        Rectangle topLeft = new Rectangle(0,0,cornerSize, cornerSize);
        Rectangle topEdge = new Rectangle(cornerSize,0, edgeSize, edgeSize);
        Rectangle topRight = new Rectangle(cornerSize + edgeSize, 0, cornerSize, cornerSize);
        Rectangle rightEdge = new Rectangle(cornerSize + edgeSize, cornerSize, edgeSize, edgeSize);
        Rectangle bottomRight = new Rectangle(cornerSize + edgeSize, cornerSize + edgeSize, cornerSize, cornerSize);
        Rectangle bottomEdge = new Rectangle(cornerSize, cornerSize + edgeSize, edgeSize, edgeSize);
        Rectangle bottomLeft = new Rectangle(0, cornerSize + edgeSize, cornerSize, cornerSize);
        Rectangle leftEdge = new Rectangle(0, cornerSize, edgeSize, edgeSize);
        Rectangle center = new Rectangle(cornerSize, cornerSize, centerSize, centerSize);

        Vector2 bottomRightOrigin = new Vector2(cornerSize, cornerSize);
        Vector2 bottomLeftOrigin = new Vector2(0, cornerSize);
        Vector2 topRightOrigin = new Vector2(cornerSize, 0);
        public DialogBox(string name, Point pos, string text, Object owner, OriginType originType =OriginType.def) 
            : base(name, "", pos.X, pos.Y, UIManager.rootElement, originType, defaultScale, false, false, 0)
        {
            this.owner = owner;

            this.originalText = text;
            this.text = text;
            paddingOffset = ScalePoint(new Point(7, 5));
            Vector2 textPos = (pos + itemSlotOffset + paddingOffset).ToVector2();
            this.textRenderer = new TextRenderer(textPos, TextRenderer.Origin.topLeft);

            this.bounds = CalculateBounds();

            fillerTexture.SetData(new[] { Color.White });
        }

        public void SetPosition(Point pos)
        {
            this.bounds.Location = pos + itemSlotOffset;
            this.textRenderer.pos = (bounds.Location + paddingOffset).ToVector2 ();
        }

        // manually set a size for this dialog box (default is dynamic sizing)
        public void SetSize(Point size)
        {
            this.size = size + ScalePoint(padding);
            this.bounds.Size = this.size;

            // recalculate text
            this.text = FormatText(originalText, (size - ScalePoint(padding)).ToVector2());
            this.textRenderer.pos = (this.bounds.Location + paddingOffset).ToVector2();
        }
        private string FormatText(string text, Vector2 maxSize)
        {
            List<string> words = text.Split(" ").ToList();
            string resultString = "";
            float lineSize = padding.X * defaultScale;
            string line = "";

            if (textRenderer.font.MeasureString(text).X < maxSize.X)
            {
                resultString = text;
            }
            else
            {
                foreach (string word in words)
                {

                    float wordSize = textRenderer.font.MeasureString(word).X;
                    lineSize += wordSize;

                    if (lineSize > maxSize.X)
                    {
                        // this line is done, add a newline
                        resultString += line + "\n";

                        // start new line with this word
                        lineSize = wordSize + (padding.X * defaultScale);
                        line = word + " ";
                    }
                    else
                    {
                        line += word + " ";
                    }

                }
                resultString += line.TrimEnd(); // Trim any trailing space

            }
            return resultString;
        }

        private Rectangle CalculateBounds()
        {
            this.text = FormatText(originalText, new Vector2(maxWidth, 0));
            this.size = textRenderer.font.MeasureString(text).ToPoint() + ScalePoint(padding);

            Rectangle newBounds;
            if (owner is ItemSlot)
            {
                newBounds = new Rectangle(localPos + itemSlotOffset, size);
            }
            else
            {
                newBounds = new Rectangle(localPos, size);
            }
            return newBounds;
        }


        public void DrawDialogBox(SpriteBatch b)
        {


            // first, draw corners
            b.Draw(backgroundTexture, bounds.Location.ToVector2(), topLeft, Color.White, 0f, Vector2.Zero, defaultScale, SpriteEffects.None, 0.955f);
            b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(0, bounds.Height), bottomLeft, Color.White, 0f, bottomLeftOrigin, defaultScale, SpriteEffects.None, 0.955f);
            b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(bounds.Width, bounds.Height), bottomRight, Color.White, 0f, bottomRightOrigin, defaultScale, SpriteEffects.None, 0.955f);
            b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(bounds.Width, 0), topRight, Color.White, 0f, topRightOrigin, defaultScale, SpriteEffects.None, 0.955f);

            // draw left and right edge
            int numSides = (int)(bounds.Height / (edgeSize * defaultScale)) - 1; // subtract number required for corners
            for (int i = 1; i < numSides + 1; i++)
            {
                b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(0, i * edgeSize * defaultScale), leftEdge, Color.White, 0f
                    ,Vector2.Zero, defaultScale, SpriteEffects.None, 0.95f);
            }
            for (int i = 1; i < numSides + 1; i++)
            {
                b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(bounds.Width - edgeSize * defaultScale, i * edgeSize * defaultScale), leftEdge, Color.White, 0f
                    , Vector2.Zero, defaultScale, SpriteEffects.None, 0.95f);
            }

            // draw top and bottom edge
            int numTops = (int)(bounds.Width / (edgeSize * defaultScale)) - 1; // subtract number required for corners
            for (int i = 1; i < numTops + 1; i++)
            {
                b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(i * edgeSize * defaultScale,0), topEdge, Color.White, 0f
                    , Vector2.Zero, defaultScale, SpriteEffects.None, 0.95f);
            }
            for (int i = 1; i < numTops + 1; i++)
            {
                b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(i * edgeSize * defaultScale, bounds.Height - edgeSize * defaultScale), bottomEdge, Color.White, 0f
                    , Vector2.Zero, defaultScale, SpriteEffects.None, 0.95f);
            }

            // last, draw center
            b.Draw(backgroundTexture, bounds, center, Color.White, 0f, Vector2.Zero, SpriteEffects.None, .949f);
            b.Draw(fillerTexture, bounds, new Rectangle(0, 0, 1, 1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth: 0.94f);

        }
        public override void Draw(SpriteBatch b)
        {
           
            base.Draw(b);
            DrawDialogBox(b);
            textRenderer.Draw(b, text, layer:0.98f);
        }

        
    }
}
