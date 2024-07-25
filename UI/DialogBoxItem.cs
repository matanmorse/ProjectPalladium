using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace ProjectPalladium.UI
{
    public abstract class DialogBoxItem
    {
        protected DialogBoxItem(Button.OnClick onClickIn = null, Delegate onClickOut = null)
        {
            this.onClickIn = onClickIn;
            this.onClickOut = onClickOut;

            
            // workaround since functions not compile-time constants, but default click-out behavior is to close the dialog box.
            // this.onClickOut ??= UIManager.dialogBox.CloseDialogBox;
        }

        public delegate void Delegate();
        public Button.OnClick onClickIn; // to be done when user clicks inside dialog box when this element is showing
        public Delegate onClickOut; // to be done when user clicks outside dialog box when this element is showing
        public abstract void Draw(SpriteBatch b);
        public abstract void Update(GameTime gameTime);
    }
}
