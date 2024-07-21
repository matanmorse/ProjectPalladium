using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
using System.Diagnostics;

namespace ProjectPalladium.UI
{
    public class SpeechBox : DialogBoxItem
    {
        private string originalText;
        private Renderable portrait;
        private string name;

        private static Point portraitSize = new Point(64, 64);

        private Point pos;
        private Point portraitPos;

        private float portraitScale = Game1.scale * 0.25f;

        private float textScrollSpeed = 75f; // millis per character
        private float timeSinceCharAdded = 0f;
        int charIndex = -1;
        Vector2 textSize;

        private bool askedThisTick = false;

        private bool scrollingComplete = false;
        private string text = "";
        private TextRenderer textRenderer;
        public SpeechBox(string text, string name)
        {
            MainDialogBox dialogBox = UIManager.dialogBox;
            textSize = new Vector2(UIManager.dialogBox.unpaddedSize.X * 0.75f, 1000);

            onClickIn = OnClick;
            onClickOut = OnClick;
            this.portrait = new Renderable("portraits/placeholderportrait");

            this.originalText = UIManager.dialogBox.FormatText(text, textSize);
            this.pos = UIManager.dialogBox.textPos.ToPoint();
            textRenderer = new TextRenderer(pos.ToVector2(), TextRenderer.Origin.topLeft);

            portraitPos = new Point(pos.X + dialogBox.unpaddedSize.X - (int)(portraitSize.X * portraitScale), 
                (pos.Y));
            Debug.WriteLine(textSize);
            Debug.WriteLine(textRenderer.font.MeasureString(originalText));
        }

        public void OnClick()
        {
            // for unexplainable reasons, the speech box button is getting clicked twice, so this function only works every other click :P
            if (askedThisTick)
            {
                askedThisTick = false;
                return;
            }
            if (scrollingComplete) { UIManager.dialogBox.CloseDialogBox(); }
            InstaCompleteText();
            askedThisTick = true;

        }
        public void InstaCompleteText()
        {
            text = originalText;
            scrollingComplete = true;
        }

        public override void Draw(SpriteBatch b)
        {
            // Util.DrawLine(b, pos.ToVector2(), pos.ToVector2() + new Vector2((int)UIManager.dialogBox.unpaddedSize.X, 0), Color.White);
            textRenderer.Draw(b, text, layer:0.96f);
            portrait.Draw(b, portraitPos.ToVector2(), scale: portraitScale);
        }

        public override void Update(GameTime gameTime)
        {
            if (scrollingComplete) return;
            if (charIndex + 1 == originalText.Length) { scrollingComplete = true; return; } // finished scrolling text

            if (!UIManager.dialogBox.showing) return;
            timeSinceCharAdded += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeSinceCharAdded > textScrollSpeed)
            {
                timeSinceCharAdded = 0f;
                charIndex++;
                text += originalText[charIndex];
            }
        }
    }
}
