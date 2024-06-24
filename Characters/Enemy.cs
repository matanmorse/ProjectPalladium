using Microsoft.Xna.Framework;
using ProjectPalladium.Animation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Characters
{
    public class Enemy : NPC
    {
        public int health = 10;
        private bool invincible;


        public Enemy(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Rectangle boundingBox) : base(sprite, pos, name, startingMap, boundingBox)
        {
        }
        
        public override void Update(GameTime gameTime)
        {
            

         

          

            if (GetCharacterCollisions() > 0)
            {
                GetHit(1);
            }
            // NOTE: Any collision logic for purposes of combat MUST occur before the movement step, because after collision resolution concludes
            // no characters will collide with each other
            base.Update(gameTime);
        }
        public int GetCharacterCollisions()
        {
            return SceneManager.CurScene.CheckCharacterCollisions(this); 
        }

        /* Remove this enemy from the game >:) */
        public void Kill()
        {
            Debug.WriteLine(name + " was killed");
            SceneManager.CurScene.Map.RemoveCharacter(this);
        }
        public void GetHit(int damage)
        {
            if (invincible) return;

            DoHitEffect();
            health -= damage;

            if (health <= 0) { Kill(); return; }

            sprite.AddTimer(() =>
            {
                invincible = true;
            },
            () =>
            {
                invincible = false;
            }, 1000f);
            
            Debug.WriteLine("hit!" + " Health: " + health);
        }

        public void GetHit(Projectile p)
        {
            GetHit(p.baseDamage);

            // apply knockback effects
            Vector2 knockback = p.velocity * p.knockbackFactor;

            Velocity = knockback;
            movementLocked = true;

            sprite.AddTimer(() =>
            {
                Velocity = Vector2.Zero; movementLocked = false;
            }, 100f);

        }

        private void DoHitEffect()
        {
            tintColor = Color.Red;
            sprite.AddTimer(() =>
            {
                tintColor = Color.White;
            }
            , 150f);
        }
    }
}
