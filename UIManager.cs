using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectPalladium.UI;
using static ProjectPalladium.UI.UIElement.OriginType;
using ProjectPalladium;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using ProjectPalladium.Utils;
using System.Security.Cryptography;
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
        public static DialogBox dialogBox; 

        public static float inventoryScale = 5f;
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
            toolbar = new Toolbar("toolbar", "toolbar", Game1.UINativeResolution.X / 2, Game1.UINativeResolution.Y - (int) (10 * toolbarscale) , rootElement, originType: center, scale:toolbarscale);
            toolbar.inv = inventoryUI.Inventory;

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

            // Nightmare code to center the dialog box on the toolbar
            Point dialogueBoxSize = new Point((int)(Game1.UINativeResolution.X * 0.5f), (int)(100 * defaultUIScale));
            Point dialogBoxPos = new Point(toolbar.globalPos.X - ((dialogueBoxSize.X + (int)((DialogBox.padding.X - 1) * defaultUIScale)) / 2), (int)(340 * defaultUIScale) - dialogueBoxSize.Y);
            dialogBox = new DialogBox("Main dialog box", dialogBoxPos, "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of \"de Finibus Bonorum et Malorum\" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, \"Lorem ipsum dolor sit amet..\", comes from a line in section 1.10.32.\r\n\r\nThe standard chunk of Lorem Ipsum used since the 1500s is reproduced below for those interested. Sections 1.10.32 and 1.10.33 from \"de Finibus Bonorum et Malorum\" by Cicero are also reproduced in their exact original form, accompanied by English versions from the 1914 translation by H. Rackham.\r\n\r\n", rootElement);
            dialogBox.SetSize(dialogueBoxSize);

            rootElement.AddChild(dialogBox);
            // setup debug menu
            debugText = new TextRenderer(new Vector2(Game1.UINativeResolution.X - (int)(100 * defaultUIScale), (int)(50 * defaultUIScale)), originType:TextRenderer.Origin.topLeft);
            debugText.showing = false;

        }
        public void SetPlayer(Player player) { this.player = player; }
        public void Update()
        {
            rootElement.Update();
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
