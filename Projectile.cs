using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Characters;
using ProjectPalladium.Utils;
using System;
using System.Diagnostics;
using Circle = ProjectPalladium.Utils.Util.Circle;

namespace ProjectPalladium
{
    public class Projectile 
    {
        public int knockbackFactor = 5;
        public int baseDamage = 5;
        public Vector2 origin;
        public Renderable sprite;
        public Vector2 velocity;
        private Vector2 pos;

        private const float MAX_DISTANCE = 100 * Game1.scale;
        public Vector2 Pos { get { return pos; } set { this.pos = value; hitbox.Pos = value.ToPoint(); } }
        public float speed;
        private float rotation;
        public Object owner;

        public Vector2 distanceTraveled = Vector2.Zero;
        public Vector2 startPos; 

        public Circle hitbox;
        public Projectile(string textureName, Object owner, float speed, Vector2 pos, Vector2 velocity, float rotation) 
        {
            this.rotation = rotation;
            this.velocity = velocity;
            this.pos = pos;
            this.startPos = pos;
            this.speed = speed;
            this.sprite = new Renderable(textureName);
            this.owner = owner;

            hitbox = new Circle(pos, 50);
            origin = sprite.Texture.Bounds.Size.ToVector2() / 2;
        }

        public void Destroy() 
        { 
            if (owner is Player) { (owner as Player).RemoveProjectile(this); }
        }
        public void Update(GameTime gameTime)
        {
            distanceTraveled = pos - startPos;
            if (distanceTraveled.Length() > MAX_DISTANCE) { Destroy(); return   ; }

            Object hit = SceneManager.CurScene.Map.checkCollisions(this);
            if (hit != null)
            {
                Debug.WriteLine(hit);
                Debug.WriteLine(Game1.player.boundingBox);
                if (hit is Enemy)
                {
                    (hit as Enemy).GetHit(this);
                }
                Destroy(); return;
            }
            
            this.Pos += velocity * speed;
        }

       

        public void Draw(SpriteBatch b)
        {
            if (DebugParams.showProjectileColliders) hitbox.DrawCircle(b);
            sprite.Draw(b, pos, layer: 0.9f, origin:origin, rotation: rotation);
        }
    }
}
