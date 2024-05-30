using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tutorial;
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

        public bool mouseOver = false;
        public bool clickState = false;

        private UIElement owner;
        public Button(OnEnter onEnter, OnClick onClick, OnLeave onLeave, Point pos, Point size, UIElement owner)
        {
            this.onClick = onClick;
            this.onEnter = onEnter;
            this.onLeave = onLeave;
            this.owner = owner;
            this.bounds = new Rectangle(pos, owner.ScalePoint(size));
            
        }

        public void Update()
        {
            CheckEnter();
            CheckLeave();
            if (CheckClick()) onClick();
            CheckHover();
        }

        public bool CheckHover()
        {
            if (bounds.Contains(Input.mousePos) && mouseOver == false)
            {
                mouseOver = true;
                return true;
            }
            return false;
        }

        public bool CheckEnter()
        {
            if (bounds.Contains(Input.mousePos) && !bounds.Contains(Input.previousMousePos))
            {
                mouseOver = true;
                return true;
            }
            return false;
        }
        
        public bool CheckLeave()
        {  
            if (!bounds.Contains(Input.mousePos) && bounds.Contains(Input.previousMousePos))
            {
                mouseOver = false;
                return true;
            }
            return false;
        }

        public bool CheckClick()
        {
            if (mouseOver && Input.GetLeftMouseClicked())
            {
                clickState = clickState ? false : true;
                return true;
            }
            return false;
        }
    }
}
