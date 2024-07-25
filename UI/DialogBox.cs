using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using ProjectPalladium.Utils;
using System.Diagnostics;

namespace ProjectPalladium.UI
{
    public class DialogBox : UIElement
    {
        private static string subfolder = "ui/";
        private static Texture2D backgroundTexture = Game1.contentManager.Load<Texture2D>(subfolder + "dialogbox");
         Texture2D fillerTexture = new Texture2D(Game1.graphicsDevice, 1, 1);
        private TextRenderer textRenderer;

        protected const float defaultScale = 4f;
        private const float maxWidth = 100 * defaultScale;

        private Object owner;

        private Point itemSlotOffset = (new Point(10, 10) * new Point((int)UIManager.inventoryScale)); // use the inventory scale since we want to be at edge of itemslot
        protected Rectangle bounds;
        public Point unpaddedSize;
        private Point size;
        protected Point maxSize;
        protected Point finalSize;
        public Point Size
        {
            get { return size; }
            set 
            {
                size = value;
                unpaddedSize = size - padding * new Point((int)scale);
            }
        }
        public static Point padding = new Point(10,10);
        public Point paddingOffset;
        protected Vector2 centerOrigin;
        public Vector2 pos; // position, center origin
        private string text;
        private string originalText;
        public Vector2 textPos; // where all text should be drawn starting from
        private bool needsToDrawBox = true;

        protected bool useCenterOrigin;
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
        public DialogBox(string name, Point pos, string text, Object owner, OriginType originType =OriginType.def, bool useCenterOrigin=false) 
            : base(name, "", pos.X, pos.Y, UIManager.rootElement, originType, defaultScale, false, false, 0)
        {
            this.useCenterOrigin = useCenterOrigin;
            this.owner = owner;

            this.originalText = text;
            this.text = text;
            paddingOffset = new Point(5,4) * new Point((int)scale) ;
            textPos = (pos + itemSlotOffset + paddingOffset).ToVector2();
            this.textRenderer = new TextRenderer(textPos, TextRenderer.Origin.topLeft);

            this.bounds = CalculateBounds();

            fillerTexture.SetData(new[] { Color.White });

            centerOrigin = (bounds.Location + ((bounds.Size * new Point((int)defaultScale)) / new Point(2))).ToVector2();
        }

        public void SetPosition(Point pos)
        {
            this.bounds.Location = pos + itemSlotOffset;
            this.textPos = (bounds.Location + paddingOffset).ToVector2();
            this.textRenderer.pos = textPos;
        }

        // manually set a size for this dialog box (default is dynamic sizing)
        public void SetSize(Point size)
        {
            
            this.Size = size + ScalePoint(padding);
            this.bounds.Size = this.Size;
            this.maxSize = this.Size;

            // recalculate text
            this.text = FormatText(originalText);
            this.textPos = (bounds.Location + paddingOffset).ToVector2();
            this.textRenderer.pos = textPos;
        }

        public string FormatText(string text)
        {
            return FormatText(text, (size - ScalePoint((padding * new Point(2)) + new Point(16))).ToVector2());
        }
        public string FormatText(string text, Vector2 maxSize)
        {
            List<string> words = text.Split(" ").ToList();
            string resultString = "";
            float lineSize = padding.X * defaultScale;
            string line = "";
            float spaceSize = textRenderer.font.MeasureString(" ").X;

            if (textRenderer.font.MeasureString(text).X < maxSize.X)
            {
                resultString = text;
            }
            else
            {
                foreach (string word in words)
                {
                    float wordSize = textRenderer.font.MeasureString(word).X;
                    lineSize += wordSize + spaceSize;

                    if (lineSize > maxSize.X)
                    {
                        // this line is done, add a newline
                        resultString += line + "\n";

                        // start new line with this word
                        lineSize = wordSize + (padding.X * scale);
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
            this.Size = textRenderer.font.MeasureString(text).ToPoint() + ScalePoint(padding);

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


        public void DrawDialogBoxWithCenterOrigin(SpriteBatch b)
        {
            //// first, draw corners
            Vector2 topLeftPos = pos - (bounds.Size.ToVector2() / 2);
            Vector2 bottomLeftPos = pos - new Vector2(bounds.Size.X / 2, -bounds.Size.Y / 2);
            Vector2 bottomRightPos = pos - new Vector2(-bounds.Size.X / 2, -bounds.Size.Y / 2);
            Vector2 topRightPos = pos - new Vector2(-bounds.Size.X / 2, bounds.Size.Y / 2);

            b.Draw(backgroundTexture, topLeftPos, topLeft, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.955f); ;
            b.Draw(backgroundTexture, bottomLeftPos, bottomLeft, Color.White, 0f, bottomLeftOrigin, scale, SpriteEffects.None, 0.955f);
            b.Draw(backgroundTexture, bottomRightPos, bottomRight, Color.White, 0f, bottomRightOrigin, scale, SpriteEffects.None, 0.955f);
            b.Draw(backgroundTexture, topRightPos, topRight, Color.White, 0f, topRightOrigin, scale, SpriteEffects.None, 0.955f);

            //// draw left and right edge
            int numSides = (int)(bounds.Height / (edgeSize * defaultScale)) - 1; // subtract number required for corners
            for (int i = 1; i < numSides + 1; i++)
            {
                b.Draw(backgroundTexture, topLeftPos + new Vector2(0, i * edgeSize * defaultScale), leftEdge, Color.White, 0f
                    , Vector2.Zero, scale, SpriteEffects.None, 0.95f);
            }
            for (int i = 1; i < numSides + 1; i++)
            {
                b.Draw(backgroundTexture, topLeftPos + new Vector2(bounds.Width - edgeSize * defaultScale, i * edgeSize * defaultScale), leftEdge, Color.White, 0f
                    , Vector2.Zero, scale, SpriteEffects.None, 0.95f);
            }

            // draw top and bottom edge
            int numTops = (int)(bounds.Width / (edgeSize * defaultScale)) - 1; // subtract number required for corners
            for (int i = 1; i < numTops + 1; i++)
            {
                b.Draw(backgroundTexture, topLeftPos + new Vector2(i * edgeSize * defaultScale, 0), topEdge, Color.White, 0f
                    , Vector2.Zero, scale, SpriteEffects.None, 0.95f);
            }
            for (int i = 1; i < numTops + 1; i++)
            {
                b.Draw(backgroundTexture, topLeftPos + new Vector2(i * edgeSize * defaultScale, bounds.Height - edgeSize * defaultScale), bottomEdge, Color.White, 0f
                    , Vector2.Zero, scale, SpriteEffects.None, 0.95f);
            }

            //// last, draw center
            b.Draw(backgroundTexture, new Rectangle(pos.ToPoint() - bounds.Size / new Point(2), bounds.Size), center, Color.White, 0f, Vector2.Zero, SpriteEffects.None, .949f);
            b.Draw(fillerTexture, new Rectangle(pos.ToPoint() - bounds.Size / new Point(2), bounds.Size), new Rectangle(0, 0, 1, 1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth: 0.94f);

        }
        public void DrawDialogBox(SpriteBatch b)
        {
            if (useCenterOrigin)
            {
                DrawDialogBoxWithCenterOrigin(b);
                return;
            }

            //// first, draw corners
            Vector2 topLeftPos = centerOrigin - (bounds.Size.ToVector2() / 2);
            b.Draw(backgroundTexture, bounds.Location.ToVector2(), topLeft, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.955f);
            b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(0, bounds.Height), bottomLeft, Color.White, 0f, bottomLeftOrigin, scale, SpriteEffects.None, 0.955f);
            b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(bounds.Width, bounds.Height), bottomRight, Color.White, 0f, bottomRightOrigin, scale, SpriteEffects.None, 0.955f);
            b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(bounds.Width, 0), topRight, Color.White, 0f, topRightOrigin, scale, SpriteEffects.None, 0.955f);

            // draw left and right edge
            int numSides = (int)(bounds.Height / (edgeSize * defaultScale)) - 1; // subtract number required for corners
            for (int i = 1; i < numSides + 1; i++)
            {
                b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(0, i * edgeSize * defaultScale), leftEdge, Color.White, 0f
                    , Vector2.Zero, scale, SpriteEffects.None, 0.95f);
            }
            for (int i = 1; i < numSides + 1; i++)
            {
                b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(bounds.Width - edgeSize * defaultScale, i * edgeSize * defaultScale), leftEdge, Color.White, 0f
                    , Vector2.Zero, scale, SpriteEffects.None, 0.95f);
            }

            // draw top and bottom edge
            int numTops = (int)(bounds.Width / (edgeSize * defaultScale)) - 1; // subtract number required for corners
            for (int i = 1; i < numTops + 1; i++)
            {
                b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(i * edgeSize * defaultScale, 0), topEdge, Color.White, 0f
                    , Vector2.Zero, scale, SpriteEffects.None, 0.95f);
            }
            for (int i = 1; i < numTops + 1; i++)
            {
                b.Draw(backgroundTexture, bounds.Location.ToVector2() + new Vector2(i * edgeSize * defaultScale, bounds.Height - edgeSize * defaultScale), bottomEdge, Color.White, 0f
                    , Vector2.Zero, scale, SpriteEffects.None, 0.95f);
            }

            // last, draw center
            b.Draw(backgroundTexture, bounds, center, Color.White, 0f, Vector2.Zero, SpriteEffects.None, .949f);
            b.Draw(fillerTexture, bounds, new Rectangle(0, 0, 1, 1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth: 0.94f);

        }
        public override void Draw(SpriteBatch b)
        {
            if (!showing) return;
            base.Draw(b);
            DrawDialogBox(b);
            textRenderer.Draw(b, text, layer:0.98f);
        }

        
    }
}
