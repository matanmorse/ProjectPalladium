using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectPalladium.UI;
using static ProjectPalladium.UI.UIElement.OriginType;
using ProjectPalladium;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
namespace ProjectPalladium
{
    public class UIManager
    {
        public UIElement rootElement;
        private Player player;
        public Player Player { get { return player; } }

        public static float defaultUIScale = 2f;

        public InventoryUI inventoryUI;
        public Toolbar toolbar;

        public enum UILayers
        {
            first,
            second, 
            third, 
            fourth
        }

        public UIManager(UIElement root) { 
           rootElement = root;
        }

        

        public void Initialize()
        {
            // inventory screen
            inventoryUI = new InventoryUI("inventory", "inventory", Game1.UINativeResolution.X / 2, Game1.UINativeResolution.Y / 2, rootElement, originType: center, scale:3f);

            // toolbar
            toolbar = new Toolbar("toolbar", "toolbar", Game1.UINativeResolution.X / 2, Game1.UINativeResolution.Y - 30 , rootElement, originType: center, scale:3f);
            toolbar.inv = inventoryUI.Inventory;
            Debug.WriteLine(toolbar.globalPos);

            // link inventory and toolbar
            inventoryUI.toolbar = toolbar;
            inventoryUI.showing = false;

            rootElement.AddChild(inventoryUI);
            rootElement.AddChild(toolbar);

            // mana bar and spellbook
            rootElement.AddChild("mana bar", "manabar", 10, Game1.UINativeResolution.Y - 150);
            rootElement.AddChild(new UIElement("spellbook", "spellbook", Game1.UINativeResolution.X -100, Game1.UINativeResolution.Y -100, rootElement));

            // Casting UI
            rootElement.AddChild(new CastingUI(rootElement));

            toolbar.UpdateToolbar();
            
        }
        public void SetPlayer(Player player) { this.player = player; }
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
