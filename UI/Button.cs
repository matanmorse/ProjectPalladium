using System.Diagnostics;
using Microsoft.Xna.Framework;
using ProjectPalladium;
using ProjectPalladium.Utils;
namespace ProjectPalladium.UI
{
    public class Button
    {
        public Rectangle bounds;
        public delegate void OnEnter();
        public OnEnter onEnter;

        public delegate void OnClick();
        public OnClick onClick;

        public delegate void OnLeave();
        public OnLeave onLeave;

        public delegate void OnRightClick();
        public OnRightClick onRightClick; 

        public bool mouseOver = false;
        public bool clickState = false;

        public bool UI; // is this a UI button or GameWorld button?

        private UIElement owner;
        public Button(OnEnter onEnter, OnClick onClick, OnLeave onLeave, Point pos, Point size, UIElement owner, OnRightClick onRightClick=null)
        {
            this.onClick = onClick;
            this.onEnter = onEnter;
            this.onLeave = onLeave;
            this.onRightClick = onRightClick;
            this.owner = owner;
            this.bounds = new Rectangle(pos, owner.ScalePoint(size));
            this.UI = true;

        }

        public void SetBounds(Point size)
        {
            this.bounds.Size = size;
        }

        public void SetBounds(Rectangle rect)
        {
            this.bounds = rect;
        }

        public Button(OnEnter onEnter, OnClick onClick, OnLeave onLeave, Point pos, Point size, OnRightClick onRightClick = null)
        {
            this.onClick = onClick;
            this.onEnter = onEnter;
            this.onLeave = onLeave;
            this.onRightClick = onRightClick;
            this.bounds = new Rectangle(pos, size);
            this.UI = false;
        }


        public void Update(GameTime gameTime)
        {
            // if (owner is MainDialogBox) { Debug.WriteLine("hello"); }
            // list of conditions when we want the button to be inactive
            CheckEnter();
            CheckLeave();
            if (CheckLeftClick()) if (onClick != null) { onClick(); }
            if (CheckRightClick() ) if (onRightClick != null) onRightClick();
            CheckHover();
        }

        // the following ternary gore is so that we don't have to store an instance variable for every single element
        public bool CheckHover()
        {
            if (bounds.Contains(UI ? Input.nativeMousePos : Input.gameWorldMousePos) && mouseOver == false)
            {
                mouseOver = true;
                return true;
            }
            return false;
        }

        public bool CheckEnter()
        {
            if (bounds.Contains(UI ? Input.nativeMousePos : Input.gameWorldMousePos) && !bounds.Contains(UI ? Input.previousNativeMousePos : Input.prevGameWorldMousePos))
            {
                mouseOver = true;
                return true;
            }
            return false;
        }
        
        public bool CheckLeave()
        {  
            if (!bounds.Contains(UI ? Input.nativeMousePos : Input.gameWorldMousePos) && bounds.Contains(UI ? Input.previousNativeMousePos : Input.prevGameWorldMousePos))
            {
                mouseOver = false;
                return true;
            }
            return false;
        }

        public bool CheckLeftClick()
        {

            if (mouseOver && Input.GetLeftMouseClicked())
            {
                clickState = clickState ? false : true;
                return true;
            }
            return false;
        }

        public bool CheckRightClick()
        {
            if (mouseOver && Input.GetRightMouseClicked())
            {
                return true;
            }
            return false;
        }
    }
}
