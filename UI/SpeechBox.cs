using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace ProjectPalladium.UI
{
    public class SpeechBox : DialogBoxItem
    {
        private string[] originalText;
        private Renderable portrait;
        private string name;

        private static Point portraitSize = new Point(64, 64);

        private Point pos;
        private Point portraitPos;
        private Point namePos;

        private static float portraitScale = Game1.scale * 0.25f;

        private static Point scaledPortraitSize = portraitSize * new Point((int)portraitScale);

        private float textScrollSpeed = 75f; // millis per character
        private float timeSinceCharAdded = 0f;
        int charIndex = -1;
        Vector2 textSize;

        private bool askedThisTick = false;

        private bool scrollingComplete = false;
        private string text = "";
        private TextRenderer textRenderer;
        private TextRenderer nameRenderer;

        int textSlideIndex = 0;
        public SpeechBox(string[] text, string name)
        {
            // get text sizes based on dialog box
            MainDialogBox dialogBox = UIManager.dialogBox;
            textSize = new Vector2(UIManager.dialogBox.unpaddedSize.X * 0.75f, 1000);

            // asign behavior for click out and click in
            onClickIn = OnClick;
            onClickOut = OnClick;

            originalText = new string[text.Length];
            // format all the text
            for (int i = 0; i < text.Length; i++)
            {
                originalText[i] = UIManager.dialogBox.FormatText(text[i], textSize);
            }

            this.pos = UIManager.dialogBox.textPos.ToPoint();
            textRenderer = new TextRenderer(pos.ToVector2(), TextRenderer.Origin.topLeft);


            // init portrait
            this.portrait = new Renderable("portraits/placeholderportrait");
            portraitPos = new Point(pos.X + dialogBox.unpaddedSize.X - (int)(portraitSize.X * portraitScale), 
                (pos.Y));

            // make sure name is capitalized
            this.name = name[0].ToString().ToUpper() + name.Substring(1);
            
            namePos = new Point(portraitPos.X + (int) (scaledPortraitSize.X / 2), portraitPos.Y + scaledPortraitSize.Y + (int)(8 * dialogBox.scale));
            nameRenderer = new TextRenderer(namePos.ToVector2(), originType: TextRenderer.Origin.center);
        }

        public void OnClick()
        {
            // for unexplainable reasons, the speech box button is getting clicked twice, so this function only works every other click :P
            if (askedThisTick)
            {
                askedThisTick = false;
                return;
            }

            if (scrollingComplete) 
            { 
                if (textSlideIndex == originalText.Length - 1)
                {
                    UIManager.dialogBox.CloseDialogBox();
                }
                else
                {
                    GoToNextSlide();
                }
            }
            else
            {
                InstaCompleteText();
            }

            askedThisTick = true;

        }

        private void GoToNextSlide()
        {
            text = "";
            scrollingComplete = false;
            textSlideIndex++;
            timeSinceCharAdded = 0f;
            charIndex = -1;
        }
        public void InstaCompleteText()
        {
            text = originalText[textSlideIndex];
            scrollingComplete = true;
        }

        public override void Draw(SpriteBatch b)
        {
            // Util.DrawLine(b, pos.ToVector2(), pos.ToVector2() + new Vector2((int)UIManager.dialogBox.unpaddedSize.X, 0), Color.White);
            textRenderer.Draw(b, text, layer:0.96f);
            portrait.Draw(b, portraitPos.ToVector2(), scale: portraitScale);
            nameRenderer.Draw(b, name, layer: 0.96f);
        }

        public override void Update(GameTime gameTime)
        {
            if (scrollingComplete) return;
            if (charIndex + 1 == originalText[textSlideIndex].Length) { scrollingComplete = true; return; } // finished scrolling text

            if (!UIManager.dialogBox.showing) return;
            timeSinceCharAdded += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeSinceCharAdded > textScrollSpeed)
            {
                timeSinceCharAdded = 0f;
                charIndex++;
                text += originalText[textSlideIndex][charIndex];
            }
        }
    }
}
