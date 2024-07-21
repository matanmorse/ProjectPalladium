using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectPalladium.Utils;
using System.Diagnostics;

namespace ProjectPalladium.UI
{
    /* Meant to go inside the dialog box, present a yes/no choice */

    public class ChoiceMenu : DialogBoxItem
    {
        private Point textPadding = new Point(6,6) * new Point((int)(UIManager.defaultUIScale));
        private string prompt;
        private string choice1 = "Yes";
        private string choice2 = "No";

        Button.OnClick onChoice1;
        Button.OnClick onChoice2;

        private Button choice1Button;
        private Button choice2Button;

        TextRenderer promptText;
        private TextRenderer choice1Text;
        private TextRenderer choice2Text;

        private Point pos;

        private Vector2 choice1Size;
        private Vector2 choice2Size;
        private Vector2 promptSize;

        private MainDialogBox dialogBox;
        public ChoiceMenu(string prompt, string choice1, string choice2, Button.OnClick onChoice1 =null, Button.OnClick onChoice2 = null)
        {
            this.dialogBox = UIManager.dialogBox;
            this.pos = dialogBox.textPos.ToPoint(); // the menu starts at the text position
            this.prompt = dialogBox.FormatText(prompt);
            this.choice1 = dialogBox.FormatText(choice1);
            this.choice2 = dialogBox.FormatText(choice2);
            this.onChoice1 = onChoice1;
            this.onChoice2 = onChoice2;


            promptText = new TextRenderer(pos.ToVector2(), TextRenderer.Origin.topLeft);
            promptSize = promptText.font.MeasureString(this.prompt);

            choice1Text = new TextRenderer(new Vector2(pos.X, WithPadding(promptText.pos + promptSize).Y), TextRenderer.Origin.topLeft);
            choice1Size = choice1Text.font.MeasureString(choice1);

            choice2Text = new TextRenderer(new Vector2(pos.X, WithPadding(choice1Text.pos + choice1Size).Y), TextRenderer.Origin.topLeft);
            choice2Size = choice2Text.font.MeasureString(choice2);

            choice1Button = new Button(null, OnChoice1Clicked, null, WithoutPadding(choice1Text.pos).ToPoint(), new Point(dialogBox.unpaddedSize.X , (int)choice1Size.Y), owner:dialogBox);
            choice2Button = new Button(null, OnChoice2Clicked, null, WithoutPadding(choice2Text.pos).ToPoint(), new Point(dialogBox.unpaddedSize.X, (int)choice2Size.Y), owner: dialogBox);
            choice1Button.SetBounds(new Point(dialogBox.unpaddedSize.X + textPadding.X, (int)choice1Size.Y + textPadding.Y));
            choice2Button.SetBounds(new Point(dialogBox.unpaddedSize.X + textPadding.X, (int)choice2Size.Y + textPadding.Y));

        }

        private Vector2 WithPadding(Vector2 size)
        {
            return (size + textPadding.ToVector2());
        }
        private Vector2 WithoutPadding(Vector2 size)
        {
            return (size - textPadding.ToVector2() / 2);
        }
        public override void Update(GameTime gameTime)
        {
            choice1Button.Update(gameTime);
            choice2Button.Update(gameTime);
        }
        private void OnChoice1Clicked()
        {
            dialogBox.CloseDialogBox();
            if (onChoice1 == null) { return; }
            onChoice1.Invoke();
        }

        private void OnChoice2Clicked()
        {
            dialogBox.CloseDialogBox();
            if (onChoice2 == null) { return; }
            onChoice2.Invoke();
        }


        public override void Draw(SpriteBatch b)
        {
            if (choice1Button.mouseOver)
            {
                Util.DrawRectangle(choice1Button.bounds, b);
            }
            if (choice2Button.mouseOver)
            {
                Util.DrawRectangle(choice2Button.bounds, b);
            }

            promptText.Draw(b, prompt, layer: 0.99f);
            choice1Text.Draw(b, choice1, layer:0.99f);
            choice2Text.Draw(b, choice2, layer: 0.99f);
        }

       
    }
}
