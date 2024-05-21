using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectPalladium.UI;
using ProjectPalladium.Utils;
using System;
using System.Diagnostics;


namespace ProjectPalladium
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static GraphicsDevice graphicsDevice;
        private Canvas _canvas;

        public static Point NativeResolution = new Point(512, 288);

        private Map _map;

        public Player player;

        public ContentManager content;

        public static int screenWidth;
        public static int screenHeight;
        public static ContentManager contentManager;

        public static float scale = 1f;

        private Matrix _translation;

        private UIManager _uiManager;

        public static class layers
        {
            public const float tile = 0f;
            public const float player = 0.9f;
            public const float rectangles = 1f;
            public const float UI = 1f;
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

        // update the prefferedBackBuffer variables when the window is changed
        private void OnResize(object sender, EventArgs e)
        {
            _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.Viewport.Width;
            _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Viewport.Height;

            _graphics.ApplyChanges();
            _canvas.SetDestinationRectangle();
        }

        private void SetFullScreen()
        {
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

            Window.IsBorderless = true;
            _graphics.ApplyChanges();
            _canvas.SetDestinationRectangle();
        }
        private void CalculateTranslation()
        {

            var dx = ((screenWidth / 2) - player.pos.X );
            
            dx = MathHelper.Clamp(
                dx, 
                -(_map.tileMapSize.X * _map.scaledTileSize) + screenWidth,
                0);

            var dy = ((screenHeight / 2) - player.pos.Y - (player.sprite.spriteHeight * scale) / 2);


            dy = MathHelper.Clamp(
                dy,
                -(_map.tileMapSize.Y * Map.tilesize * scale) + screenHeight - (Map.tilesize * scale / 2),
                0);

            _translation = Matrix.CreateTranslation(dx, dy, 0f);
        }

        protected override void Initialize()
        {
            base.Initialize();

            // by default we start in bordered fullscreen
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.ApplyChanges();
            _canvas.SetDestinationRectangle();

            // load map
            _map = new Map("hollow.tmx");

            // init player object
            Vector2 playerPos = new Vector2(100, 100);
            player = new Player(new Animation.AnimatedSprite(16, 32, "mageanims", "mage"), playerPos, "Player", _map,
            new Rectangle((int)playerPos.X, (int)playerPos.Y, (int) (12 * Game1.scale), (int)(30 * Game1.scale)));
            player.Initialize();
            player.setBounds(_map.tileMapSize, 16);

            // init UI
            
            _uiManager = new UIManager(new UIElement("root",null, 0, 0, null, isRoot: true, isBox:true));
            
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            graphicsDevice = GraphicsDevice;
            _canvas = new Canvas(GraphicsDevice, screenWidth, screenHeight);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Z)) { DebugParams.showColliders = true; }
            if (Keyboard.GetState().IsKeyDown(Keys.X)) { DebugParams.showColliders = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.F6)) { SetFullScreen(); }
            if (Keyboard.GetState().IsKeyDown(Keys.F7)) { Window.IsBorderless = false; }

            player.Update(gameTime);
            _map.Update(gameTime);
            _uiManager.Update();

            CalculateTranslation();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _canvas.Activate(); // start drawing to the canvas
            _spriteBatch.Begin(SpriteSortMode.FrontToBack ,null, SamplerState.PointClamp, null,  null, null, _translation);

            _map.Draw(_spriteBatch);
            player.Draw(_spriteBatch);
            _spriteBatch.End();
            
            // draw UI elements (always fixed on screen)
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            _uiManager.Draw(_spriteBatch);
            _spriteBatch.End();

            _canvas.Draw(_spriteBatch);

        }
    }
}
