using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Animation;
using ProjectPalladium.UI;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Data;
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
        private bool dying;
        public Mode mode;
        private Direction movingDir;
        private Vector2 smoothedDirection = Vector2.Zero;
        private static Vector2 centerOfPlayersHitbox = Game1.player.boundingBox.Center.ToVector2();

        private static List<Danger> dangers = new List<Danger>();

        private bool canAttack = true;

        // interest values for each direction of context-based steering
        private float[] interests = new float[8] { 0f, 0f, 0f, 0f, 0f,0f, 0f,0f };
        public enum Direction
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

        public enum DecayType { 
            InverseExponent,
            InverseSquare,
        }
        public struct Danger
        {
            public DecayType decayType;
            public float weight;
            public Vector2 location;
            public float maxInfluenceRadius;

            public Danger(DecayType decayType, float weight, Vector2 location, float maxInfluenceRadius)
            {
                this.maxInfluenceRadius = maxInfluenceRadius * Game1.scale;
                this.decayType = decayType;
                this.location = location;
                this.weight = weight;
            }

            // calculate the interest for this danger given a location,direction being considered, and distance
            public float CalculateInterest(float distToDanger, Vector2 otherLocation, Direction dir)
            {
                if (distToDanger > maxInfluenceRadius) return 0f;

                // calculate the distanceweight
                float distWeight;
                if (decayType == DecayType.InverseExponent)
                {
                    float k = 0.005f;
                    float a = 10f;
                    distWeight = a / (MathF.Exp(k * distToDanger));
                }
                if (decayType == DecayType.InverseSquare)
                {
                    float a = 3f;
                    distToDanger /= Game1.scale * 2;
                    distWeight = a / (distToDanger * distToDanger);
                }
                else distWeight = 1f;

                Vector2 awayVector = otherLocation - location;
                return Vector2.Dot(awayVector, UnitDirections[dir]) * weight * distWeight;
            }
        }


        // unit vectors in 8 cardinal directions
        public static Dictionary<Direction, Vector2> UnitDirections = new Dictionary<Direction, Vector2>()
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
            Pursue,
            Attack
        }
        public Enemy(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Vector2 bBoxOffset, Vector2 bBoxSize) : base(sprite, pos, name, startingMap, bBoxOffset, bBoxSize)
        {
            mode = Mode.Idle;

            if (dangers.Count == 0) { AddTilemapDangers(); }   
        }


        public override void Update(GameTime gameTime)
        {

            if (health < 0) SendRemoveMapCall();
            DoModeActions();
            
            // NOTE: Any collision logic for purposes of combat MUST occur before the movement step, because after collision resolution concludes
            // no characters will collide with each other
            base.Update(gameTime);
            // boundingBox.Location = new Point((int)pos.X - sprite.scaledWidth / 2, (int) (pos.Y - sprite.scaledHeight / 2));
        }

        public static void UpdateStaticItems()
        {
            
            centerOfPlayersHitbox = Game1.player.boundingBox.Center.ToVector2();

            
        }

        public static void AddTilemapDangers()
        {
            foreach (Tilemap t in SceneManager.CurScene.Map.collidingTilemaps)
            {
                foreach (Rectangle r in t.colliders)
                {
                    dangers.Add(new Danger(DecayType.InverseSquare, 0.1f, r.Center.ToVector2(), Map.tilesize + 10 ));
                }
            }
        }
        protected void UpdateContextSteering()
        {
            Vector2 centerOfHitbox = this.boundingBox.Center.ToVector2();
            Vector2 toPlayerVector = Vector2.Normalize(centerOfPlayersHitbox - pos);
            Vector2 awayFromPlayerVector = -toPlayerVector;
            float distToPlayer = (centerOfPlayersHitbox - centerOfHitbox).Length();

            /* Some explanation of the math here: 
             
             k is the steepness constant, increasing it means the weight increases faster when the player moves closer
             a is the base, this is the maximum value of the weight.
             the distance to the player is the contributing factor
            */
            float k = 0.009f;
            float a = 25f;
            float exponentialDistanceWeight = a / (MathF.Exp(k * distToPlayer));

            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                float interest = 0f;
                float awayFromPlayer = Vector2.Dot(awayFromPlayerVector, UnitDirections[dir]) * exponentialDistanceWeight; // less weight
                foreach(Danger d in dangers)
                {
                    float distToDanger = (d.location - centerOfHitbox).Length();
                    interest += d.CalculateInterest(distToDanger, centerOfHitbox, dir);
                }
                if (distToPlayer < 50 * Game1.scale)
                {
                    if (canAttack)
                    {
                        mode = Mode.Attack; return;
                    }
                    float cross = Util.Cross(toPlayerVector, UnitDirections[dir]);
                    interest += cross + awayFromPlayer;
                }
                else
                {
                    float dot = Vector2.Dot(toPlayerVector, UnitDirections[dir]);
                    interest += dot + awayFromPlayer;
                }

                interests[(int) dir] = interest;    
            }

        }

        protected void Move()
        {

            if (movementLocked || gettingKnockedBack) return;

            int highestIndex = Array.IndexOf(interests, interests.Max());
            movingDir = (Direction)highestIndex;

            // Debug.WriteLine(movingDir + " " + interests[highestIndex]);

            // apply direction smoothing
            float smoothingFactor = 0.03f;
            smoothedDirection = smoothingFactor * UnitDirections[movingDir] + (1 - smoothingFactor) * smoothedDirection;
            smoothedDirection.Normalize();
            Velocity = smoothedDirection;
        }
        protected void DoModeActions()
        {
            FindMode();
            UpdateContextSteering();
            switch (mode)
            {
                case (Mode.Idle):
                    {
                        Velocity = Vector2.Zero;
                        break;
                    }

                case (Mode.Pursue):
                    Move(); break;
                case (Mode.Attack):
                    StartAttack(); break;
            }

        }

        private void StartAttack()
        {
            if (!canAttack) return;
            movementLocked = true;
            GameManager.TimerManager.AddTimer(() =>
            {
                movementLocked = false;
                canAttack = false;
                Attack();
            }, 1000f);

            GameManager.TimerManager.AddTimer(() => mode = Mode.Pursue, 1500f);
            GameManager.TimerManager.AddTimer(() => canAttack = true, 7500f);
        }
        
        private void Attack()
        {

            float attackSpeed = 5f;
            Vector2 toPlayerVector = Vector2.Normalize(centerOfPlayersHitbox - pos);
            Velocity = toPlayerVector * attackSpeed;


        }
        protected void FindMode()
        {
            if (mode == Mode.Attack) { return;  }
            float distToPlayer = (SceneManager.CurScene.Player.pos - pos).Length();
            if (distToPlayer < 300 * Game1.scale) { mode = Mode.Pursue; }

            else { mode = Mode.Idle; }
        }
        protected override void FindLocomotionAnimation()
        {
            if (movementLocked) return;
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
            dying = true;
            Velocity = Vector2.Zero;
            movementLocked = true;
            sprite.PlayAnimationOnce("die", SendRemoveMapCall);
        }

        public void SendRemoveMapCall() {
            SceneManager.CurScene.Map.RemoveCharacter(this);
            dying = false;
        }
        public void GetHit(int damage)
        {
            if (invincible || dying) return;

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
            }, 100f);
            
        }

        public void GetHit(Projectile p)
        {
            GetHit(p.baseDamage);

            // apply knockback effects
            Vector2 knockback = p.velocity * p.knockbackFactor;

            Velocity = knockback;
            gettingKnockedBack = true;

            sprite.AddTimer(() =>
            {
                Velocity = Vector2.Zero; gettingKnockedBack = false;
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
            foreach (Danger d in dangers)
            {
                if ( (d.location - Game1.player.pos).Length() < 1000 && DebugParams.showTileColliders) 
                new Util.Circle(d.location, (int)d.maxInfluenceRadius).DrawCircle(b);
            }

            if (DebugParams.showCharacterColliders)
            {
                foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                {
                    Color color;
                    if (interests[(int)dir] < 0) color = Color.Red;
                    else if (dir == movingDir) color = Color.Blue;
                    else color = Color.Green;

                    float interest = Math.Abs(interests[(int)dir]);

                    Vector2 start = pos + (UnitDirections[dir] * 75);
                    Vector2 end = pos + (UnitDirections[dir] * 75) + interest * 300 * UnitDirections[dir];


                    Util.DrawLine(b, start, end, color);
                }
            }
        }
    }
}
