using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectPalladium.UI;
using static ProjectPalladium.UI.UIElement.OriginType;
using ProjectPalladium;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
namespace ProjectPalladium
{
    public class UIManager
    {
        public UIElement rootElement;
        private Player player;
        public Player Player { get { return player; } }

        public static float defaultUIScale = 4f;

        public InventoryUI inventoryUI;
        public static Toolbar toolbar;
        public CastingUI castingUI;
        
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
            float inventoryScale = 5f;
            inventoryUI = new InventoryUI("inventory", "inventory", Game1.UINativeResolution.X / 2, Game1.UINativeResolution.Y / 2, rootElement, originType: center, scale:inventoryScale);

            // toolbar
            float toolbarscale = 6f;
            toolbar = new Toolbar("toolbar", "toolbar", Game1.UINativeResolution.X / 2, Game1.UINativeResolution.Y - (int) (10 * toolbarscale) , rootElement, originType: center, scale:toolbarscale);
            toolbar.inv = inventoryUI.Inventory;
            Debug.WriteLine(toolbar.globalPos);

            // link inventory and toolbar
            inventoryUI.toolbar = toolbar;
            inventoryUI.showing = false;

            rootElement.AddChild(inventoryUI);
            rootElement.AddChild(toolbar);

            // mana bar and spellbook
            Manabar manabar = new Manabar("manabar", "manabar", rootElement);

            rootElement.AddChild(manabar);
            rootElement.AddChild(new UIElement("spellbook", "spellbook", Game1.UINativeResolution.X -150, Game1.UINativeResolution.Y -100, rootElement));

            // Casting UI
            castingUI = new CastingUI(rootElement);
            rootElement.AddChild(castingUI);

            // game info
            TimeDisplay gameInfo = new TimeDisplay("Game Info", "TimeDisplay", rootElement);
            rootElement.AddChild(gameInfo);

            toolbar.UpdateToolbar();
            manabar.Initialize();
            
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
