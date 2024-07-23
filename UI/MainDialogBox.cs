using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Items;
using ProjectPalladium.Utils;
using System;
using System.Diagnostics;
using System.Linq;


namespace ProjectPalladium.UI
{
    public class MainDialogBox : DialogBox
    {
        // Nightmare code to center the dialog box on the toolbar
        private static Point dialogueBoxSize = new Point((int)(Game1.UINativeResolution.X * 0.5f), (int)(100 * defaultScale));
        private static Point dialogBoxPos = new Point(UIManager.toolbar.globalPos.X - ((dialogueBoxSize.X + (int)((DialogBox.padding.X - 1) * defaultScale)) / 2), (int)(325 * defaultScale) - dialogueBoxSize.Y);

        private float animationTime = 0f;
        private static float animationLength = 0.5f; // number of seconds the transition animation lasts
        private static float animationSpeed = 1 / (animationLength * 60);
        // things that can potentially go in the dialog box
        DialogBoxItem currentMenu;
        public MainDialogBox(string name, string text) : base(name, dialogBoxPos, text, UIManager.rootElement, OriginType.def, useCenterOrigin:true)
        {
            animationTime = 0f;
            showing = false;
            SetSize(dialogueBoxSize);
            finalSize = dialogueBoxSize;
            AddToRoot();

            
            pos = (bounds.Location + (bounds.Size / new Point(2))).ToVector2();
            AddButton(null, null, null, (dialogueBoxSize + padding * new Point((int)scale)) / new Point((int)scale));
        }

        public void OpenDialogBox()
        {
            SetSize(Point.Zero);
            animationTime = 0f;
            Game1.player.DialogueBoxOpen = true;
            UIManager.toolbar.active = false;
            this.showing = true;

            this.button.onClick = currentMenu.onClickIn;
        }

        public void CloseDialogBox()
        {
            Game1.player.DialogueBoxOpen = false;
            UIManager.toolbar.active = true;
            this.showing = false;
            this.currentMenu = null;
        }

        public void AskChoice(string prompt, string choice1, string choice2, Button.OnClick onChoice1=null, Button.OnClick onChoice2=null)
        {
            this.currentMenu = new ChoiceMenu(prompt, choice1, choice2, onChoice1, onChoice2);
            OpenDialogBox();
        }

        public void ShowDialog(string[] text)
        {
            this.currentMenu = new SpeechBox(text, "mage");
            OpenDialogBox();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Debug.WriteLine(gameTime.TotalGameTime.Ticks);
            base.Update(gameTime);
            if (!showing) return;

            if (!(animationTime + animationSpeed > 1.1f))
            {
                animationTime = Math.Clamp(animationTime + animationSpeed, 0f, 1f);
                SetSize(new Point((int)(finalSize.X * animationTime), (int)(finalSize.Y * animationTime)));
            }


            if (currentMenu != null && animationTime == 1f)
            {
                currentMenu.Update(gameTime);
            }

            // close if we click out of dialog box when open
            if (showing && !button.mouseOver && Input.GetLeftMouseClicked())
            {
                if (currentMenu.onClickOut != null) currentMenu.onClickOut.Invoke();
            }
        }
        public override void Draw(SpriteBatch b)
        {
            
            base.Draw(b);
            if (!showing) return;
            if (currentMenu != null && animationTime == 1f)
            {
                currentMenu.Draw(b);
            }
        }
    }
}
