using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace ProjectPalladium.UI
{
    public class UIElement
    {
        public enum OriginType
        {
            def,
            center,
        }
        
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

        private float scale;

        public bool showing = true;

        // has this element been updated?
        public bool needsUpdating = false;
        private bool isRoot;
        private bool isBox;

        protected Button button;
        public UIElement(string name, string textureName, int localX, int localY, UIElement parent, OriginType originType=OriginType.def, float scale=1f, bool isRoot = false, bool isBox=false)
        {
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

        public void Update()
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

        public virtual void Draw(SpriteBatch b) {

            if (!(isRoot || !showing || isBox))
            {
                if (originType == OriginType.center)
                {
                    sprite.Draw(b, new Vector2(drawPos.X, drawPos.Y) - new Vector2(Sprite.size.X / 2, Sprite.size.Y / 2), layer: Game1.layers.UI);
                }
                else
                {
                    sprite.Draw(b, new Vector2(drawPos.X, drawPos.Y), layer: Game1.layers.UI);
                }
            }

            if (children.Count != 0) foreach (UIElement child in children) { child.Draw(b); }

        }

       
        public void AddChild(string name, string textureName, int localX, int localY, OriginType originType = OriginType.def)
        {
            children.Add(new UIElement(name, textureName, localX, localY, this, originType));
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
            button = new Button(onEnter, onClick, onLeave, globalPos - new Point(1, 1), size) ;
        }
     
    }
}
