using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectPalladium.UI;
using ProjectPalladium.Utils;
using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using ProjectPalladium;
using ProjectPalladium.Animation;


namespace ProjectPalladium
{
    public class Game1 : Game
    {
        private static GraphicsDeviceManager _graphics;
        public static GraphicsDeviceManager graphics { get { return _graphics; } }

        public static bool isFullscreen;

        private SpriteBatch _spriteBatch;
        public static GraphicsDevice graphicsDevice;
        private Canvas _canvas;
        private Canvas _uiCanvas;

        public static Inventory inventory;

        public const float scale = 10f;

        public static Point NativeResolution = new Point(512 * (int) scale, 288 * (int) scale);
        public static Point UINativeResolution = new Point(1360 * 2, 768 * 2);

        
        public Map map;

        public static Player player;
        //private Tilemap _tilemap;

        public ContentManager content;

        public static int screenWidth;
        public static int screenHeight;
        public static float UITargetScale;
        public static float gameWorldTargetScale;

        public static Point hackyOffset;

        public static ContentManager contentManager;


        public static Matrix translation;
        private Matrix prevTranslation; 
        
        private static UIManager _uiManager;
        public static UIManager UIManager { get { return _uiManager; } }
        public static GameManager gameManager;

        public static ScreenShader shader;


        public static class layers
        {
            public const float tile = 0f;
            public const float player = 0.9f;
            public const float rectangles = 1f;
            public const float UI = 0.95f;
            public const float buildings = 0.1f;
        }
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

            Content.RootDirectory = "Content";
            contentManager = new ContentManager(base.Content.ServiceProvider, base.Content.RootDirectory);
            IsMouseVisible = true;

            // start with native resolution
            _graphics.PreferredBackBufferHeight = NativeResolution.Y;
            _graphics.PreferredBackBufferWidth = NativeResolution.X;

            screenWidth = _graphics.PreferredBackBufferWidth;
            screenHeight = _graphics.PreferredBackBufferHeight;

        }

        //update the prefferedBackBuffer variables when the window is changed
        private void OnResize(object sender, EventArgs e)
        {
            _canvas.SetDestinationRectangle();
            _uiCanvas.SetDestinationRectangle();
        }


        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            Input.Update();
        }
        private void CalculateTranslation()
        {
            prevTranslation = translation;

            var dx = ((screenWidth / 2) - player.lerpingCamera.X );
            
            dx = MathHelper.Clamp(
                dx, 
                -(map.tileMapSize.X * Map.scaledTileSize) + screenWidth,
                0);

            var dy = ((screenHeight / 2) - player.lerpingCamera.Y - (player.sprite.spriteHeight * scale) / 2);

            

            dy = MathHelper.Clamp(
                dy,
                -(map.tileMapSize.Y * Map.scaledTileSize) + screenHeight - (Map.scaledTileSize / 2),
                0);

           
            translation = Matrix.CreateTranslation((int) dx, (int) dy, 0f);
        }


        protected override void Initialize()
        {
            base.Initialize();

            graphics.HardwareModeSwitch = false;
           
            // by default we start in bordered fullscreen
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

            _graphics.ApplyChanges();
            _canvas.SetDestinationRectangle();
            _uiCanvas.SetDestinationRectangle();

            shader = new ScreenShader();
            shader.onFinishEffect += SceneManager.OnSceneTransitionFinished;

            // init game manager
            gameManager = new GameManager();

            // init UI
            _uiManager = new UIManager(new UIElement("root", null, 0, 0, null, isRoot: true, isBox: true));
            _uiManager.Initialize();

            // load map
            map = new Map("hollow.tmx");

            // init player object
            Vector2 playerPos = new Vector2(100, 100);
            player = new Player(new Animation.AnimatedSprite(16, 32, "mageanims", "mage"), playerPos, "Player", map,
            new Rectangle((int)playerPos.X, (int)playerPos.Y, (int) (12 * Game1.scale), (int)(30 * Game1.scale)));

            player.sprite.Owner = player;

            player.Initialize();
            player.setBounds(map.tileMapSize, 16);

            _uiManager.SetPlayer(player);

            //Send it to SceneManager
            Scene mainScene = new Scene(map, player);
            SceneManager.LoadScene(mainScene);

            inventory = UIManager.inventoryUI.Inventory;

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            graphicsDevice = GraphicsDevice;
            _canvas = new Canvas(GraphicsDevice, screenWidth, screenHeight, "gameworld");
            _uiCanvas = new Canvas(graphicsDevice, UINativeResolution.X, UINativeResolution.Y,"ui");
        }

        protected override void Update(GameTime gameTime)
        {
            GameManager.Update(gameTime);
            //Allows for GetKeyDown functionality; if you remove, it will break
            Input.Update();

            if (Input.GetKeyDown(Keys.Z)) { DebugParams.showColliders = true; }
            if (Input.GetKeyDown(Keys.X)) { DebugParams.showColliders = false; }
            if (Input.GetKeyDown(Keys.F6)) { 
                graphics.ToggleFullScreen(); _canvas.SetDestinationRectangle(); _uiCanvas.SetDestinationRectangle();}
            
            _uiManager.Update();
            SceneManager.Update(gameTime);

            CalculateTranslation();

            //Debug code for changing scenes
            if (Input.GetKeyDown(Keys.P))
            {
                Scene test2 = new Scene(new Map("test1.tmx"), player);
                SceneManager.LoadScene(test2);
            }
            if (Input.GetKeyDown(Keys.L))
            {
                Scene test3 = new Scene(new Map("wizardtower.tmx"), player);
                SceneManager.LoadScene(test3);
            }

            shader.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the back buffer with the background color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Render the game world to the _canvas
            _canvas.Activate();
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, translation);
            SceneManager.Draw(_spriteBatch);
            _spriteBatch.End();

            // Render the UI elements to the _uiCanvas
            _uiCanvas.Activate();
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            _uiManager.Draw(_spriteBatch);
            _spriteBatch.End();

            // Switch back to the default render target (the back buffer)
            GraphicsDevice.SetRenderTarget(null);

            // Draw the UI canvas to the back buffer
            _canvas.Draw(_spriteBatch);
            _uiCanvas.Draw(_spriteBatch);

            // last, apply shading effects
            _spriteBatch.Begin();
            shader.Draw(_spriteBatch);
            _spriteBatch.End();
        }
    }
}
