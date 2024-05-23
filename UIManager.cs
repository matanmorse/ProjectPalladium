using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectPalladium.UI;
using static ProjectPalladium.UI.UIElement.OriginType;
using Tutorial;
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
            rootElement.AddChild(new Toolbar("toolbar", "toolbar", Game1.NativeResolution.X / 2, Game1.NativeResolution.Y - 15, rootElement, originType:center));
            rootElement.AddChild("mana bar", "manabar", 10, Game1.NativeResolution.Y - 70);
            rootElement.AddChild(new UIElement("spellbook", "spellbook", Game1.NativeResolution.X - 40, Game1.NativeResolution.Y - 30, rootElement));
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
