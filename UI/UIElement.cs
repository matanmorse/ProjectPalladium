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

        protected float gameScale = Game1.scale;
        public float scale;
        public float opacity = 1f;
        private Renderable sprite;
        public Renderable Sprite { get { return sprite; } set { this.sprite = value; } }
        protected Point localPos;

        protected static Player player = Game1.player;
        public bool active = true; // are the buttons and any interactable objects on this element active?
        public Point LocalPos
        {
            get { return localPos; }
            set { localPos = value; UpdateGlobalPos(); }
        }

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
        public float rotation;
        private string subfolder = "ui/";

        public Button button;
        public UIElement(string name, string textureName, int localX, int localY, UIElement parent, OriginType originType = OriginType.def, float scale = 1f, bool isRoot = false, bool isBox = false, float rotation = 0f)
        {
            if (scale == 1f) scale = UIManager.defaultUIScale;
            this.scale = scale;
            this.isRoot = isRoot;
            this.name = name;
            
            if (isRoot) { this.localPos = Point.Zero; }

            this.isBox = isBox;
            this.parent = parent;
            this.sprite = new Renderable(subfolder, textureName, rotation:rotation);
            this.localPos = new Point(localX, localY);
            this.originType = originType;
            UpdateGlobalPos();
        }

    
        public virtual void Initialize() { }
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
            if (button != null && active) { button.Update(); }
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
                    sprite.Draw(b, Util.PointToVector2(globalPos), layer: Game1.layers.UI, scale: scale,
                        origin: new Vector2(Sprite.size.X / 2, sprite.size.Y / 2), opacity:opacity);
                }
                else
                {
                    sprite.Draw(b, new Vector2(drawPos.X, drawPos.Y), layer: Game1.layers.UI, scale: scale, opacity:opacity) ;
                }
            }

            if (children.Count != 0) foreach (UIElement child in children) { child.Draw(b); }

        }

        // make this element a child of the root child
        protected void AddToRoot()
        {
            if (isRoot) return;
            UIManager.rootElement.AddChild(this);
        }

        public virtual void AddChild(string name, string textureName, int localX, int localY, OriginType originType = OriginType.def, float scale = 1f)
        {
            if (scale == 1f) scale = UIManager.defaultUIScale;
            children.Add(new UIElement(name, textureName, localX, localY, this, originType, scale));
        }
        public virtual void AddChild(UIElement child) { children.Add(child); }
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
            // position of the button will depend on the origin type
            if (originType == OriginType.center)
            {
                if (this is ItemSlot) // ItemSlots have constant size but sprites are sometimes null, so include a special case
                {
                    button = new Button(onEnter, onClick, onLeave, globalPos - new Point(1, 1) - ScalePoint(new Point(8, 8)), size, this);
                    return;
                }
                button = new Button(onEnter, onClick, onLeave, globalPos - new Point(1, 1) - ScalePoint(sprite.size / new Point(2)), size, this);
                return;
            }
            button = new Button(onEnter, onClick, onLeave, globalPos - new Point(1, 1), size, this);
        }

        public virtual void ToggleShowing ()
        {
            showing = !showing;
        }
     
    }
}
