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
        private float invincibilityTimer = 0f;
        private bool invincible;

        private float hitEffectTimer = 0f;

        public Enemy(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Rectangle boundingBox) : base(sprite, pos, name, startingMap, boundingBox)
        {
        }
        
        public override void Update(GameTime gameTime)
        {
            

            if (invincibilityTimer > 0f)
            {
                invincibilityTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                if (invincible) {  invincible = false; }
                invincibilityTimer = 0f;
            }

          

            if (GetCharacterCollisions() > 0)
            {
                GetHit();
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
        private void GetHit()
        {
            if (invincible) return;

            DoHitEffect();
            health -= 1;
            if (health <= 0) { Kill(); return; }

            invincible = true;
            invincibilityTimer = 1000f;
            Debug.WriteLine("hit!" + " Health: " + health);
        }

        private void DoHitEffect()
        {
            tintColor = Color.Red;
            sprite.AddTimer(() =>
            {
                tintColor = Color.White;
            }
            , 1000f);
        }
    }
}
