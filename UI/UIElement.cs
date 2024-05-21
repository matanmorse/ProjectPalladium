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
        private Vector2 localPos;
        // the global position centered at top-right
        public Vector2 globalPos;
        // this is the origin position of the element where it is drawn. May or may not be the same as the global pos depending on origin type.
        public Vector2 drawPos;

        private OriginType originType;
        public string name;

        public UIElement parent;
        public List<UIElement> children = new List<UIElement>();

        public bool showing = true;

        // has this element been updated?
        public bool needsUpdating = false;
        private bool isRoot;
        private bool isBox;

        public UIElement(string name, string textureName, int localX, int localY, UIElement parent, OriginType originType=OriginType.def, bool isRoot = false, bool isBox=false)
        {
            this.isRoot = isRoot;
            this.name = name;

            if (isRoot) { this.localPos = Vector2.Zero; }

            this.isBox = isBox;
            this.parent = parent;
            this.sprite = new Renderable(textureName);
            this.localPos = new Vector2(localX, localY);
            this.originType = originType;
            UpdateGlobalPos();

        }

        public void UpdateGlobalPos()
        {
            Debug.WriteLine("name " + name);
            if (isRoot)
            {
                globalPos = localPos;
                return;
            }
            Debug.WriteLine(parent.globalPos);
            globalPos = parent.globalPos + localPos;
            if (originType == OriginType.center)
            {
                drawPos = globalPos - new Vector2(sprite.Texture.Width / 2, sprite.Texture.Height / 2);
            }
            else
            {
                drawPos = globalPos;
            }
        }

        public void Update()
        {
            if (!showing) return;

            if (needsUpdating)
            {
                UpdateGlobalPos();
                needsUpdating = false;
            }
            foreach (UIElement child in children) { child.Update(); }
        }

        public void Draw(SpriteBatch b) {
            if (!(isRoot || !showing || isBox))
            {
                sprite.Draw(b, drawPos, layer: Game1.layers.UI);
            }

            if (children.Count != 0) foreach (UIElement child in children) { child.Draw(b); }

        }

        public void AddChild(string name, string textureName, int localX, int localY, OriginType originType = OriginType.def)
        {
            children.Add(new UIElement(name, textureName, localX, localY, this, originType));
        }

        public UIElement GetChild(string name)
        {
            return children.First(x => x.name == name);
        }
        public IEnumerable<UIElement> GetAllChildren(string name)
        {
            return children.Where(x => x.name == name);
        }

     
    }
}
