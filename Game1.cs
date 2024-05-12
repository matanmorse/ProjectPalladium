using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectPalladium
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        AnimatedSprite happyFace;
        public ContentManager content;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            content = new ContentManager(base.Content.ServiceProvider, base.Content.RootDirectory);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            happyFace = new AnimatedSprite(content, 0, 16, 16, "happyface");
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            //if (Keyboard.GetState().IsKeyDown(Keys.W))
            //    happyFace.pos.Y -= 1;

            //if (Keyboard.GetState().IsKeyDown(Keys.S))
            //    happyFace.pos.Y += 1;

            //if (Keyboard.GetState().IsKeyDown(Keys.A))
            //    happyFace.pos.X -= 1;

            //if (Keyboard.GetState().IsKeyDown(Keys.D))
            //    happyFace.pos.X += 1;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            happyFace.Draw(_spriteBatch, new Vector2(0, 0), 1f);

        }
    }
}
