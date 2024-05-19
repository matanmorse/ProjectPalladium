using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectPalladium.TileMap;
using ProjectPalladium.Utils;


namespace ProjectPalladium
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static GraphicsDevice graphicsDevice;
        private Map _map;

        public Player player;
        private Tilemap _tilemap;

        public ContentManager content;

        public static int screenWidth;
        public static int screenHeight;
        public static ContentManager contentManager;

        public static float scale = 5f;

        private Matrix _translation;

        public static class layers
        {
            public const float tile = 0f;
            public const float player = 0.9f;
            public const float rectangles = 1f;
            public const float buildings = 0.1f;
        }
        public Game1()
        {

            _graphics = new GraphicsDeviceManager(this);
            

            Content.RootDirectory = "Content";
            contentManager = new ContentManager(base.Content.ServiceProvider, base.Content.RootDirectory);
            IsMouseVisible = true;

            _graphics.PreferredBackBufferHeight = 1440;
            _graphics.PreferredBackBufferWidth = 2560;

            screenWidth = _graphics.PreferredBackBufferWidth;
            screenHeight = _graphics.PreferredBackBufferHeight;
        }

        public static Vector2 TileToGlobalPos(Vector2 pos)
        {
            return new Vector2(pos.X * Map.tilesize, pos.Y * Map.tilesize);
        }

        public static Vector2 GlobalToTilePos(Vector2 pos)
        {
            return new Vector2(pos.X / Map.tilesize, pos.Y / Map.tilesize );
        }
        private void CalculateTranslation()
        {

            var dx = ((screenWidth / 2) - player.pos.X - (player.sprite.spriteWidth * scale) / 2 );
            
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

            _map = new Map("hollow.tmx");

            Vector2 playerPos = new Vector2(100, 100);
            player = new Player(new Animation.AnimatedSprite(16, 32, "mageanims", "mage"), playerPos, "Player", _map,
    new Rectangle((int)playerPos.X, (int)playerPos.Y, (int) (12 * Game1.scale), (int)(30 * Game1.scale)));

            player.Initialize();

            player.setBounds(_map.tileMapSize, 16);

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            graphicsDevice = GraphicsDevice;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Z)) { DebugParams.showColliders = DebugParams.showColliders ? false : true; }
            player.Update(gameTime);
            _map.Update(gameTime);
            CalculateTranslation();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
            _spriteBatch.Begin(SpriteSortMode.FrontToBack ,null, SamplerState.PointClamp, null,  null, null, _translation);

            _map.Draw(_spriteBatch);
            player.Draw(_spriteBatch);


            _spriteBatch.End();

        }
    }
}
