using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Animation;
using ProjectPalladium.UI;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Characters
{
    public class Enemy : NPC
    {
        public int health = 10;
        private bool invincible;
        public Mode mode;
        private Direction movingDir;

        // interest values for each direction of context-based steering
        private float[] interests = new float[8] { 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f };
        private enum Direction
        {
            Up,
            UpRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left,
            UpLeft,
        }

        // unit vectors in 8 cardinal directions
        private Dictionary<Direction, Vector2> UnitDirections = new Dictionary<Direction, Vector2>()
        {
            {Direction.Up, Vector2.Normalize(new Vector2(0, -1)) },
            {Direction.UpRight, Vector2.Normalize(new Vector2(1, -1)) },
            {Direction.Right, Vector2.Normalize(new Vector2(1, 0)) },
            {Direction.DownRight, Vector2.Normalize(new Vector2(1, 1)) },
            {Direction.Down,  Vector2.Normalize(new Vector2(0, 1)) },
            {Direction.DownLeft, Vector2.Normalize(new Vector2(-1, 1)) },
            {Direction.Left, Vector2.Normalize(new Vector2(-1, 0)) },
            {Direction.UpLeft, Vector2.Normalize(new Vector2(-1, -1)) },
        };


        public enum Mode
        {
            Idle,
            Pursue
        }
        public Enemy(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Vector2 bBoxOffset, Vector2 bBoxSize) : base(sprite, pos, name, startingMap, bBoxOffset, bBoxSize)
        {
            mode = Mode.Idle;
        }
        
        public void WalkTowardsPlayer()
        {
            if (movementLocked) return;
            Velocity = Vector2.Normalize(SceneManager.CurScene.Player.pos - pos);
        }
        public override void Update(GameTime gameTime)
        {

            DoModeActions();
            UpdateContextSteering();
            // NOTE: Any collision logic for purposes of combat MUST occur before the movement step, because after collision resolution concludes
            // no characters will collide with each other
            base.Update(gameTime);
            // boundingBox.Location = new Point((int)pos.X - sprite.scaledWidth / 2, (int) (pos.Y - sprite.scaledHeight / 2));
        }

        protected void UpdateContextSteering()
        {
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                float dot = Vector2.Dot(Vector2.Normalize(Game1.player.pos - pos), UnitDirections[dir]);
                float distToPlayer = (SceneManager.CurScene.Player.pos - pos).Length();
                

               
                interests[(int)dir] = dot;
            }
            Move();
        }

        protected void Move()
        {
            if (movementLocked) return;

            int highestIndex = Array.IndexOf(interests, interests.Max());
            movingDir = (Direction)highestIndex;

            Velocity = UnitDirections[(Direction) highestIndex];
            boundingBox.Location = pos.ToPoint();
        }
        protected void DoModeActions()
        {
            FindMode();
            switch (mode)
            {
                case (Mode.Idle):
                    {
                        Velocity = Vector2.Zero;
                        break;
                    }

                case (Mode.Pursue): WalkTowardsPlayer(); break;
            }

        }

        protected void FindMode()
        {
            float distToPlayer = (SceneManager.CurScene.Player.pos - pos).Length();
            if (distToPlayer < 100 * Game1.scale) { mode = Mode.Pursue; }

            else { mode = Mode.Idle; }
        }
        protected override void FindLocomotionAnimation()
        {
            if (mode == Mode.Idle)
            {
                if (sprite.Animation.Name != "idle")
                {
                    sprite.changeAnimation("idle");
                }
            }
            if (mode == Mode.Pursue)
            {
                if (sprite.Animation.Name != "walk")
                {
                    sprite.changeAnimation("walk");
                }
            }
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

        public override void Draw(SpriteBatch b)
        {
            base.Draw(b);
            if (DebugParams.showCharacterColliders)
            {
                foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                {
                    Color color = Color.Green;

                    if (interests[(int)dir] < 0) color = Color.Red;
                    if (dir == movingDir) color = Color.Blue;

                    float interest = Math.Abs(interests[(int)dir]);

                    Vector2 start = pos + (UnitDirections[dir] * 75);
                    Vector2 end = pos + (UnitDirections[dir] * 75) + interest * 300 * UnitDirections[dir];


                    Util.DrawLine(b, start, end, color);
                }
            }
        }
    }
}
