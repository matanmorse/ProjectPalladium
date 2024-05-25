using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectPalladium.UI;
using static ProjectPalladium.UI.UIElement.OriginType;
using Tutorial;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
namespace ProjectPalladium
{
    public class UIManager
    {
        public UIElement rootElement;

        public static float defaultUIScale = 2f;

        public InventoryUI inventoryUI;
        public Toolbar toolbar;
        public UIManager(UIElement root) { 
           rootElement = root;
           Initialize();
        }


        public void Initialize()
        {
            
            inventoryUI = new InventoryUI("inventory", "inventory", Game1.UINativeResolution.X / 2, Game1.UINativeResolution.Y / 2, rootElement, originType: center, scale:3f);

            toolbar = new Toolbar("toolbar", "toolbar", Game1.UINativeResolution.X / 2, Game1.UINativeResolution.Y - 30 , rootElement, originType: center, scale:2.75f);
            toolbar.inv = inventoryUI.Inventory;
            Debug.WriteLine(toolbar.globalPos);

            inventoryUI.toolbar = toolbar;
            inventoryUI.showing = false;
            rootElement.AddChild(inventoryUI);
            rootElement.AddChild(toolbar);
            rootElement.AddChild("mana bar", "manabar", 10, Game1.UINativeResolution.Y - 150);
            rootElement.AddChild(new UIElement("spellbook", "spellbook", Game1.UINativeResolution.X -100, Game1.UINativeResolution.Y -100, rootElement));
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
