﻿using Microsoft.Xna.Framework;
using ProjectPalladium.Animation;
using ProjectPalladium.Buildings;

namespace ProjectPalladium.Stations
{
    public class Station : PlaceableGameObject
    {
        new public AnimatedSprite animatedSprite;

        private bool completed = false;
        public bool Completed
        {
            get { return completed; }
            set
            {
                completed = value;
            }
        }

        public Station(string name, Vector2 pos, string registryName, string textureName = "") : base(name, pos, textureName)
        {
            animatedSprite = new AnimatedSprite(32, 32, subfolder + textureName, registryName, Vector2.Zero);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            animatedSprite.Update(gameTime);
        }

        public virtual void UpdateOnGameTime()
        {

        }
    }
}
