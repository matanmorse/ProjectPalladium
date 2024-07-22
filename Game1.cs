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
using System.Collections.Generic;
using ProjectPalladium.Characters;
using ProjectPalladium.Items;


namespace ProjectPalladium
{
    public class Game1 : Game
    {


        public static Game1 instance;

        

        private static GraphicsDeviceManager _graphics;
        public static GraphicsDeviceManager graphics 
        { 
            get { return _graphics; } 
        }

        public static bool isFullscreen;

        private SpriteBatch _spriteBatch;
        public static GraphicsDevice graphicsDevice;
        private Canvas _canvas;
        private Canvas _uiCanvas;
        private Canvas _lightmap;

        public static Inventory inventory;

        public const float scale = 20f;
        public static int targetFPS = 144;
        public static Point NativeResolution = new Point(512 * (int) scale, 288 * (int) scale);
        public static Point UINativeResolution = new Point(1360 * 2, 768 * 2);

        private List<float> frameTimes = new List<float>();
        private const float windowSizeSeconds = 1.0f;
        private float elapsedTime = 0.0f;
        private float averageFPS = 0.0f;

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
        public static ScreenShader gameWorldShader; // excludes UI

        public static float LAYER_CONSTANT = 0.00001f;

        private BlendState maxBrightnessBlendState = new BlendState()
        {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.Min,
            AlphaSourceBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
            AlphaBlendFunction = BlendFunction.Min
        };

        public static class layers
        {
            public const float tile = 0f;
            public const float player = 0.1f;
            public const float rectangles = 1f;
            public const float UI = 0.9f;
            public const float buildings = 0.1f;
        }
        public Game1()
        {
            instance = this;

            _graphics = new GraphicsDeviceManager(this);

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / targetFPS);
            _graphics.SynchronizeWithVerticalRetrace = false;

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
            Window.Position = new Point(0, 0);
            Content.RootDirectory = "Content";
            contentManager = new ContentManager(base.Content.ServiceProvider, base.Content.RootDirectory);
            IsMouseVisible = true;

            
            // start with native resolution
            _graphics.PreferredBackBufferHeight = NativeResolution.Y;
            _graphics.PreferredBackBufferWidth = NativeResolution.X;

            screenWidth = _graphics.PreferredBackBufferWidth;
            screenHeight = _graphics.PreferredBackBufferHeight;

        }
        
        public void ToggleMouseShowing(bool x)
        {
            IsMouseVisible = x;
        }

        //update the prefferedBackBuffer variables when the window is changed
        private void OnResize(object sender, EventArgs e)
        {
            _canvas.SetDestinationRectangle();
            _uiCanvas.SetDestinationRectangle();
            _lightmap.SetDestinationRectangle();
            
        }

        private void DoFrameRateCalculation(GameTime gameTime)
        {
            // Calculate frame time in seconds
            float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Add frame time to the list
            frameTimes.Add(frameTime);

            // Update elapsed time
            elapsedTime += frameTime;

            // Remove frame times that are outside the one-second window
            while (elapsedTime > windowSizeSeconds && frameTimes.Count > 0)
            {
                elapsedTime -= frameTimes[0];
                frameTimes.RemoveAt(0);
            }

            // Calculate average FPS
            if (elapsedTime > 0 && frameTimes.Count > 0)
            {
                averageFPS = MathF.Round(frameTimes.Count / elapsedTime, 3);
                DebugParams.elapsedMillis = averageFPS;
            }
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
                -(player.edgex) + screenWidth,
                0);

            var dy = ((screenHeight / 2) - player.lerpingCamera.Y - (player.sprite.spriteHeight * scale) / 2);

            

            dy = MathHelper.Clamp(
                dy,
                -(player.edgey) + screenHeight - (Map.scaledTileSize / 2),
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
            _lightmap.SetDestinationRectangle();

            shader = new ScreenShader();
            shader.onFinishEffect += SceneManager.OnSceneTransitionFinished;

            gameWorldShader = new ScreenShader();
            // init game manager
            gameManager = new GameManager();

            // init UI
            _uiManager = new UIManager(new UIElement("root", null, 0, 0, null, isRoot: true, isBox: true));
            _uiManager.Initialize();

            // load map
            map = new Map("hollowdefault.tmx");
            SceneManager.hollow = map;

            // init player object
            Vector2 playerPos = new Vector2(100, 100);
            player = new Player(new Animation.AnimatedSprite(16, 32, "mageanims", "mage"), playerPos, "Player", map,
            new Vector2(-8, 0) * Game1.scale, new Vector2(16,16) * Game1.scale);

            player.sprite.Owner = player;

            // player.Initialize();
            player.setBounds(map.tileMapSize, 16);

            _uiManager.SetPlayer(player);

            //Send it to SceneManager
            Scene hollow = new Scene(map, player);
            SceneManager.LoadScene(hollow);

            inventory = UIManager.inventoryUI.Inventory;

            GameManager.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            graphicsDevice = GraphicsDevice;
            _canvas = new Canvas(GraphicsDevice, screenWidth, screenHeight, "gameworld");
            _uiCanvas = new Canvas(graphicsDevice, UINativeResolution.X, UINativeResolution.Y,"ui");
            _lightmap = new Canvas(GraphicsDevice, screenWidth, screenHeight, "lightmap");
        }

        protected override void Update(GameTime gameTime)
        {

            GameManager.Update(gameTime);
            //Allows for GetKeyDown functionality; if you remove, it will break
            Input.Update();

            // Debug settings
            if (Input.GetKeyDown(Keys.Z)) { UIManager.debugText.showing = !UIManager.debugText.showing; }
            if (Input.GetKeyDown(Keys.F1)) { DebugParams.showTileColliders = !DebugParams.showTileColliders; }
            if (Input.GetKeyDown(Keys.F2)) { DebugParams.showCharacterColliders = !DebugParams.showCharacterColliders; }
            if (Input.GetKeyDown(Keys.F3)) { DebugParams.showObjectColliders = !DebugParams.showObjectColliders; }
            if (Input.GetKeyDown(Keys.F4)) { DebugParams.showFootTile = !DebugParams.showFootTile; }
            if (Input.GetKeyDown(Keys.F5)) { DebugParams.showProjectileColliders = !DebugParams.showProjectileColliders; }
            // DebugParams.elapsedMillis = (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Input.GetKeyDown(Keys.F6)) { 
                graphics.ToggleFullScreen(); _canvas.SetDestinationRectangle(); _uiCanvas.SetDestinationRectangle();}
            
            _uiManager.Update(gameTime);

            if (!GameManager.paused) SceneManager.Update(gameTime);
            else player.doInputCheck(); // only update input to allow player to unpause

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
            gameWorldShader.Update(gameTime);
            Lightmap.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            
            DoFrameRateCalculation(gameTime);
            
            // Clear the back buffer with the background color
            GraphicsDevice.Clear(Color.CornflowerBlue);


            // Render the game world to the _canvas
            _canvas.Activate();

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, translation);
            SceneManager.Draw(_spriteBatch);
            Lightmap.Draw(_spriteBatch);
            Enemy.DrawStatic(_spriteBatch);
            _spriteBatch.End();

            // apply game world shading effects
            _spriteBatch.Begin(blendState:BlendState.NonPremultiplied);
            gameWorldShader.Draw(_spriteBatch);
            _spriteBatch.End();

            // Render the UI elements to the _uiCanvas
            _uiCanvas.Activate();
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, null);
            _uiManager.Draw(_spriteBatch);
            _spriteBatch.End();

            _lightmap.Activate();
            _spriteBatch.Begin();
            shader.Draw(_spriteBatch);
            _spriteBatch.End();


            // Switch back to the default render target (the back buffer)
            GraphicsDevice.SetRenderTarget(null);

            // Draw the UI canvas to the back buffer
            _canvas.Draw(_spriteBatch);
            _uiCanvas.Draw(_spriteBatch);
            _lightmap.Draw(_spriteBatch);

        }
    }
}
