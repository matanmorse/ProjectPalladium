using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

/* Render items are first drawn here before being the canvas is drawn to the screen */
namespace ProjectPalladium
{
    public class Canvas
    {
        Matrix scaleMatrix;
        private RenderTarget2D _renderTarget;
        public string name;
        public RenderTarget2D RenderTarget { get { return _renderTarget; }  set { _renderTarget = value; } }
        private readonly GraphicsDevice _graphicsDevice;
        public Rectangle _destinationRectangle;
        public float scale = 1f;

        public Canvas(GraphicsDevice graphicsDevice, int width, int height, string name)
        {
            _graphicsDevice = graphicsDevice;
            _renderTarget = new RenderTarget2D(graphicsDevice, width, height);
            SetDestinationRectangle();
            this.name = name;
        }

        // calculate the appropriate destination rectangle for the rendertarget
        public void SetDestinationRectangle()
        {
            var screensize = Game1.graphicsDevice.PresentationParameters.Bounds;

            // get the scale as the ratio of the screen size to the native size
            float scaleX = (float) screensize.Width / _renderTarget.Width;
            float scaleY = (float)screensize.Height / _renderTarget.Height;

          
            float scale = Math.Min(scaleX, scaleY);
            // if this is the ui canvas we need the scale for mouse transforms
            if (name == "ui")
            {
                Game1.UITargetScale = scale;
            }
            if (name == "gameworld")
            {
                Game1.gameWorldTargetScale = scale;
            }

            int newWidth = (int)(_renderTarget.Width * scale);
            int newHeight = (int)(_renderTarget.Height * scale);

           
            int posX = (screensize.Width - newWidth) / 2;
            int posY = (screensize.Height - newHeight) / 2;

            _destinationRectangle = new Rectangle (posX, posY, newWidth, newHeight);
        }

        public void Activate()
        {
            _graphicsDevice.SetRenderTarget(_renderTarget);
            _graphicsDevice.Clear(Color.Transparent); // Clear with a transparent color
        }

        // draw whole canvas to precalculated rectangle
        public void Draw(SpriteBatch b)
        {
            if (name == "ui") { scaleMatrix = Matrix.CreateScale(2f); }
            b.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            b.Draw(_renderTarget, _destinationRectangle, Color.White);
            b.End();
        }

        public void DrawInMiddleOfSpriteBatch(SpriteBatch b)
        {
            b.Draw(_renderTarget, _destinationRectangle, Color.White);
        }
    }
}
