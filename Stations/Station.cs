using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Animation;
using ProjectPalladium.Buildings;
using ProjectPalladium.Utils;
using System.Diagnostics;

namespace ProjectPalladium.Stations
{
    public class Station : PlaceableGameObject
    {
        new public AnimatedSprite animatedSprite;

        public Renderable sprite;
        private bool completed = false;
        public bool Completed
        {
            get { return completed; }
            set
            {
                completed = value;
            }
        }

        public Station(string name, Vector2 pos, string registryName, string textureName = "", bool nonAnimated=false) : base(name, pos, textureName)
        {
            if (nonAnimated)
            {
                sprite = new Renderable(subfolder + textureName);
            }
        }

        public override void Draw(SpriteBatch b)
        {
            if (sprite != null)
            {
                sprite.Draw(b, globalPos, layer:layer);
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (animatedSprite != null) animatedSprite.Update(gameTime);

        }

        public virtual void UpdateOnGameTime()
        {

        }
    }
}
