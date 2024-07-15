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


        // things that can potentially go in the dialog box
        ChoiceMenu choiceMenu;
        public MainDialogBox(string name, string text) : base(name, dialogBoxPos, text, UIManager.rootElement, OriginType.def)
        {
            scale = 0f;
            showing = false;
            SetSize(dialogueBoxSize);
            AddToRoot();

        }

        public void OpenDialogBox()
        {
            Game1.player.dialogueBoxOpen = true;
            UIManager.toolbar.active = false;
            this.showing = true;
        }

        public void CloseDialogBox()
        {
            this.scale = 0f;
            Game1.player.dialogueBoxOpen = false;
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
            
            if (showing) {
                scale = Math.Clamp(scale + 0.1f, 0f, defaultScale);
                bounds.Size = new Point(Math.Clamp(bounds.Size.X + 1, 0, maxSize.X), Math.Clamp(bounds.Size.Y + 1, 0, maxSize.Y));
            }
            if (!showing) return;
            base.Update();
            if (choiceMenu != null)
            {
                choiceMenu.Update();
            }
        }
        public override void Draw(SpriteBatch b)
        {
            
            base.Draw(b);
            if (!showing) return;
            if (choiceMenu != null)
            {
                choiceMenu.Draw(b);
            }
        }
    }
}
