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
        public Button(OnEnter onEnter, OnClick onClick, OnLeave onLeave, Point pos, Point size)
        {
            this.bounds = new Rectangle(pos, size);
            this.onClick = onClick;
            this.onEnter = onEnter;
            this.onLeave = onLeave;
        }

        public void Update()
        {
            
            CheckEnter();
            CheckLeave();
            CheckClick();
        }

        public bool CheckEnter()
        {
            if (bounds.Contains(Input.mousePos) && !bounds.Contains(Input.previousMousePos))
            {
                Debug.WriteLine(Input.mousePos);
                Debug.WriteLine(bounds);
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
            if (mouseOver && Input.GetLeftMouseDown())
            {
                clickState = clickState ? false : true;
                return true;
            }
            return false;
        }
    }
}
