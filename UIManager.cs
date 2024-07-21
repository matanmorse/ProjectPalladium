using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectPalladium.UI;
using static ProjectPalladium.UI.UIElement.OriginType;
using System.Diagnostics;
using ProjectPalladium.Utils;
namespace ProjectPalladium
{
    public class UIManager
    {
        public static UIElement rootElement;
        private Player player;
        public Player Player { get { return player; } }

        public static float defaultUIScale = 4f;

        public static InventoryUI inventoryUI;
        public static Toolbar toolbar;
        public CastingUI castingUI;
        public static TextRenderer debugText;
        public static MainDialogBox dialogBox;
        public static TimeDisplay gameInfo;

        public static float inventoryScale = 6f;
        public static float toolbarscale = 6f;

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
            inventoryUI = new InventoryUI("inventory", "inventory", Game1.UINativeResolution.X / 2, Game1.UINativeResolution.Y / 2, rootElement, originType: center, scale:inventoryScale);

            // toolbar
            toolbar = new Toolbar("toolbar", "toolbar", Game1.UINativeResolution.X / 2, Game1.UINativeResolution.Y - (int) (15 * toolbarscale) , rootElement, originType: center, scale:toolbarscale);
            toolbar.inv = inventoryUI.Inventory;

            // link inventory and toolbar
            inventoryUI.toolbar = toolbar;
            inventoryUI.showing = false;

            // mana bar and spellbook
            Manabar manabar = new Manabar();
            // rootElement.AddChild(new UIElement("spellbook", "spellbook", Game1.UINativeResolution.X -150, Game1.UINativeResolution.Y -100, rootElement));

            // Casting UI
            castingUI = new CastingUI(rootElement);

            // game info
            gameInfo = new TimeDisplay("Game Info", "TimeDisplay", rootElement);

            toolbar.UpdateToolbar();
            manabar.Initialize();

            dialogBox = new MainDialogBox("Main dialog box", "");
            dialogBox.Initialize();

            rootElement.AddChild(dialogBox);
            // setup debug menu
            debugText = new TextRenderer(new Vector2(Game1.UINativeResolution.X - (int)(100 * defaultUIScale), (int)(50 * defaultUIScale)), originType:TextRenderer.Origin.topLeft);
            debugText.showing = false;

        }
        public void SetPlayer(Player player) { this.player = player; }
        public void Update(GameTime gameTime)
        {
            rootElement.Update(gameTime);
        }

        public void Draw(SpriteBatch b)
        {
            debugText.Draw(b, "DEBUG\n " + "tile colliders: " + DebugParams.showTileColliders 
                + "\n character colliders: " + DebugParams.showCharacterColliders
                + "\n object colliders: " + DebugParams.showObjectColliders
                + "\n feet: " + DebugParams.showFootTile
                + "\n projectile colliders " + DebugParams.showProjectileColliders 
                + "\n elapsed millis " + DebugParams.elapsedMillis);

            rootElement.Draw(b);
        }
    }
}
