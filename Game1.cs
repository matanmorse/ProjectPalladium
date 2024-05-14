using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace ProjectPalladium
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Map _map;

        public Player happyFace;
        private Tilemap _tilemap;

        public ContentManager content;

        public static int screenWidth;
        public static int screenHeight;
        public static ContentManager contentManager;

        public static float scale = 4f;

        private Matrix _translation;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            contentManager = new ContentManager(base.Content.ServiceProvider, base.Content.RootDirectory);
            IsMouseVisible = true;

            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth = 1440;

            screenWidth = _graphics.PreferredBackBufferWidth;
            screenHeight = _graphics.PreferredBackBufferHeight;
        }

        private void CalculateTranslation()
        {

            var dx = ((screenWidth / 2) - happyFace.pos.X - (happyFace.sprite.spriteWidth * scale) / 2 );
            
            dx = MathHelper.Clamp(
                dx, 
                -(_map.tileMapSize.X * _map.tilesize * scale) + screenWidth - (_map.tilesize * scale / 2) ,
                _map.tilesize / 2 * scale);

            var dy = ((screenHeight / 2) - happyFace.pos.Y - (happyFace.sprite.spriteHeight * scale) / 2);


            dy = MathHelper.Clamp(
                dy,
                -(_map.tileMapSize.Y * _map.tilesize * scale) + screenHeight - (_map.tilesize * scale / 2),
                _map.tilesize / 2 * scale);

            _translation = Matrix.CreateTranslation(dx, dy, 0f);
        }

        protected override void Initialize()
        {
            base.Initialize();
            _map = new Map("test1.tmx");

            happyFace = new Player(new AnimatedSprite(16, 32, "playerplaceholder", "playerplaceholder"), Vector2.Zero, "Player");
            happyFace.Initialize();

            happyFace.setBounds(_map.tileMapSize, 16);

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            happyFace.Update(gameTime);

            CalculateTranslation();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _translation);

            _map.Draw(_spriteBatch);
            happyFace.Draw(_spriteBatch);

            _spriteBatch.End();

        }
    }
}
