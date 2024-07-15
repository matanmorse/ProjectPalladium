using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.UI
{
    public class MainDialogBox : DialogBox
    {
        // Nightmare code to center the dialog box on the toolbar
        private static Point dialogueBoxSize = new Point((int)(Game1.UINativeResolution.X * 0.5f), (int)(100 * defaultScale));
        private static Point dialogBoxPos = new Point(UIManager.toolbar.globalPos.X - ((dialogueBoxSize.X + (int)((DialogBox.padding.X - 1) * defaultScale)) / 2), (int)(340 * defaultScale) - dialogueBoxSize.Y);

        private float animationTime = 0f;
        private static float animationLength = 0.5f; // number of seconds the transition animation lasts
        private static float animationSpeed = 1 / (animationLength * 60);
        // things that can potentially go in the dialog box
        ChoiceMenu choiceMenu;
        public MainDialogBox(string name, string text) : base(name, dialogBoxPos, text, UIManager.rootElement, OriginType.def, useCenterOrigin:true)
        {
            animationTime = 0f;
            showing = false;
            SetSize(dialogueBoxSize);
            finalSize = dialogueBoxSize;
            AddToRoot();

            
            pos = (bounds.Location + (bounds.Size / new Point(2))).ToVector2();

        }

        public void OpenDialogBox()
        {
            SetSize(Point.Zero);
            animationTime = 0f;
            Game1.player.DialogueBoxOpen = true;
            UIManager.toolbar.active = false;
            this.showing = true;
        }

        public void CloseDialogBox()
        {
            Game1.player.DialogueBoxOpen = false;
            UIManager.toolbar.active = true;
            this.showing = false;
            this.choiceMenu = null;
        }

        public void AskChoice(string prompt, string choice1, string choice2, Button.OnClick onChoice1=null, Button.OnClick onChoice2=null)
        {
            this.choiceMenu = new ChoiceMenu(prompt, choice1, choice2, onChoice1, onChoice2);
            OpenDialogBox();
        }
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            base.Update();
            if (!showing) return;

            animationTime = Math.Clamp(animationTime + animationSpeed, 0f, 1f);                
            SetSize(new Point((int)(finalSize.X * animationTime), (int)(finalSize.Y * animationTime)));

            if (choiceMenu != null && animationTime == 1f)
            {
                choiceMenu.Update();
            }
        }
        public override void Draw(SpriteBatch b)
        {
            
            base.Draw(b);
            if (!showing) return;
            if (choiceMenu != null && animationTime == 1f)
            {
                choiceMenu.Draw(b);
            }
        }
    }
}
