using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
namespace ProjectPalladium.UI
{
    public class UIElement
    {
        public enum OriginType
        {
            def,
            center,
        }

        public float scale;
        private Renderable sprite;
        public Renderable Sprite { get { return sprite; } set { } }
        protected Point localPos;
        // the global position centered at top-right
        public Point globalPos;
        // this is the origin position of the element where it is drawn. May or may not be the same as the global pos depending on origin type.
        public Point drawPos;

        private OriginType originType;
        public string name;

        public UIElement parent;
        public List<UIElement> children = new List<UIElement>();


        public bool showing = true;

        // has this element been updated?
        public bool needsUpdating = false;
        private bool isRoot;
        private bool isBox;

        public Button button;
        public UIElement(string name, string textureName, int localX, int localY, UIElement parent, OriginType originType=OriginType.def, float scale=1f, bool isRoot = false, bool isBox=false)
        {
            if (scale == 1f) scale = UIManager.defaultUIScale;
            this.scale = scale;
            this.isRoot = isRoot;
            this.name = name;
            
            if (isRoot) { this.localPos = Point.Zero; }

            this.isBox = isBox;
            this.parent = parent;
            this.sprite = new Renderable(textureName);
            this.localPos = new Point(localX, localY);
            this.originType = originType;
            UpdateGlobalPos();

        }

    
        public void UpdateGlobalPos()
        {
            if (isRoot)
            {
                globalPos = localPos;
                return;
            }
            globalPos = parent.globalPos + localPos;
            drawPos = globalPos;
            
        }

        public virtual void Update()
        {
            if (!showing) return;
            if (button != null) { button.Update(); }
            if (needsUpdating)
            {
                UpdateGlobalPos();
                needsUpdating = false;
            }
            foreach (UIElement child in children) { child.Update(); }
        }

        public Vector2 ScaleVector(Vector2 vtr)
        {
            return Vector2.Transform(vtr, Matrix.CreateScale(scale));
        }

        public Point ScalePoint(Point pt)
        {
            return new Point((int) (pt.X * scale), (int) (pt.Y * scale));
        }

        public Rectangle ScaleRect(Rectangle r)
        {
            return new Rectangle((int)(r.X * scale), (int)(r.Y * scale), (int)(r.Width * scale), (int)(r.Height * scale));
        }
        public virtual void Draw(SpriteBatch b) {
            if (!showing) return; // if not shoaeing, don't draw this or the children
            if (!(isRoot || isBox))
            {
                if (originType == OriginType.center)
                {
                    sprite.Draw(b, new Vector2(drawPos.X, drawPos.Y) - ScaleVector(new Vector2(Sprite.size.X / 2, Sprite.size.Y / 2)), layer: Game1.layers.UI, scale:scale);
                }
                else
                {
                    sprite.Draw(b, new Vector2(drawPos.X, drawPos.Y), layer: Game1.layers.UI, scale:scale);
                }
            }

            if (children.Count != 0) foreach (UIElement child in children) { child.Draw(b); }

        }


        public void AddChild(string name, string textureName, int localX, int localY, OriginType originType = OriginType.def, float scale = 1f)
        {
            if (scale == 1f) scale = UIManager.defaultUIScale;
            children.Add(new UIElement(name, textureName, localX, localY, this, originType, scale));
        }
        public void AddChild(UIElement child) { children.Add(child); }
        public UIElement GetChild(string name)
        {
            return children.First(x => x.name == name);
        }
        public IEnumerable<UIElement> GetAllChildren(string name)
        {
            return children.Where(x => x.name == name);
        }

        public void AddButton(Button.OnEnter onEnter, Button.OnLeave onLeave, Button.OnClick onClick, Point size)
        {
            button = new Button(onEnter, onClick, onLeave, globalPos - new Point(1, 1), size, this) ;
        }

        public virtual void ToggleShowing ()
        {
            showing = !showing;
        }
     
    }
}
