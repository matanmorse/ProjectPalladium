using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium
{
    public class Projectile
    {
        public Vector2 origin;
        public Renderable sprite;
        public Vector2 velocity;
        public Vector2 pos;
        public float speed;
        private float rotation;
        public Object owner;

        public Projectile(string textureName, Object owner, float speed, Vector2 pos, Vector2 velocity, float rotation) 
        {
            this.rotation = rotation;
            this.velocity = velocity;
            this.pos = pos;
            this.speed = speed;
            this.sprite = new Renderable(textureName);
            this.owner = owner;

            origin = sprite.Texture.Bounds.Size.ToVector2() / 2;
        }

        public void Update(GameTime gameTime)
        {
            this.pos += velocity * speed;
        }

        public void Draw(SpriteBatch b)
        {
            sprite.Draw(b, pos, layer: 1f, origin:origin, rotation: rotation);
        }
    }
}
