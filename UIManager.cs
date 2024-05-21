using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium
{
    public class UIManager
    {
        public UIElement rootElement;
        public UIManager(UIElement root) { 
           rootElement = root;
           Initialize();
        }

        public void Initialize()
        {
            rootElement.AddChild("toolbar1", "toolbar", Game1.NativeResolution.X / 2, Game1.NativeResolution.Y - 15, UIElement.OriginType.center);

        }
        public void Update()
        {
            rootElement.Update();
        }

        public void Draw(SpriteBatch b)
        {
            rootElement.Draw(b);
        }
    }
}
