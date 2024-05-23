using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

/* Render items are first drawn here before being the canvas is drawn to the screen */
namespace ProjectPalladium
{
    public class Canvas
    {
        private RenderTarget2D _renderTarget;
        public RenderTarget2D RenderTarget { get { return _renderTarget; }  set { _renderTarget = value; } }
        private readonly GraphicsDevice _graphicsDevice;
        private Rectangle _destinationRectangle;

        public Canvas(GraphicsDevice graphicsDevice, int width, int height)
        {
            _graphicsDevice = graphicsDevice;
            _renderTarget = new RenderTarget2D(graphicsDevice, width, height);
            SetDestinationRectangle();
            
        }

        // calculate the appropriate destination rectangle for the rendertarget
        public void SetDestinationRectangle()
        {
            var screensize = Game1.graphicsDevice.PresentationParameters.Bounds;

            // get the scale as the ratio of the screen size to the native size
            float scaleX = (float) screensize.Width / _renderTarget.Width;
            float scaleY = (float)screensize.Height / _renderTarget.Height;


            float scale = Math.Min(scaleX, scaleY);
            Game1.targetScale = scale;

            int newWidth = (int)(_renderTarget.Width * scale);
            int newHeight = (int)(_renderTarget.Height * scale);

           
            int posX = (screensize.Width - newWidth) / 2;
            int posY = (screensize.Height - newHeight) / 2;

            _destinationRectangle = new Rectangle (posX, posY, newWidth, newHeight);
        }

        public void Activate()
        {
            _graphicsDevice.SetRenderTarget(_renderTarget);
            _graphicsDevice.Clear(Color.DarkBlue);
        }

        // draw whole canvas to precalculated rectangle
        public void Draw(SpriteBatch b)
        {
            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(Color.Black);
            b.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, null);
            b.Draw(_renderTarget, _destinationRectangle, Color.White);
            b.End();
        }
    }
}
