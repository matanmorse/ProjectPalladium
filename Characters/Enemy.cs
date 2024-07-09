using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectPalladium.Animation;
using ProjectPalladium.Buildings;
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
using Animation = ProjectPalladium.Animation.Animation;
using Circle = ProjectPalladium.Utils.Util.Circle;

namespace ProjectPalladium.Characters
{
    public class Enemy : NPC
    {
        public Mode mode;
        private Direction movingDir;
        private Vector2 smoothedDirection = Vector2.Zero;
        private static Vector2 centerOfPlayersHitbox = Game1.player.boundingBox.Center.ToVector2();

        Danger thisDanger; // the danger for this entity for other entities

        private Attack currentAttack; // what attack is currently being performed?

        private Vector2 oldPlayerPos;

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
            InverseLinear
        }
        public struct Danger
        {
            public DecayType decayType;
            public float weight;

            private float x;
            private float y;

            public Vector2 location {
                get { return new Vector2(x, y); }
                set { x = value.X; y = value.Y; maxInfluenceArea.Pos = new Point((int)x, (int)y); }
            }


            public float maxInfluenceRadius;
            public Circle maxInfluenceArea;
            public Danger(DecayType decayType, float weight, Vector2 location, float maxInfluenceRadius)
            {
                this.maxInfluenceRadius = maxInfluenceRadius * Game1.scale;
                this.maxInfluenceArea = new Circle(location, (int)this.maxInfluenceRadius);

                this.decayType = decayType;
                this.x = location.X;
                this.y = location.Y;

                this.weight = weight;
            }

            // calculate the interest for this danger given a location,direction being considered, and distance
            public float CalculateInterest(Enemy owner, Direction dir)
            {
                if (!(maxInfluenceArea.Intersects(owner.boundingBox))) return 0f;
           
                float distToDanger = (location - owner.boundingBox.Center.ToVector2()).Length();
                // calculate the distanceweight
                float distWeight;
                if (decayType == DecayType.InverseExponent)
                {
                    float k = 0.005f;
                    float a = 10f;
                    distWeight = a / (MathF.Exp(k * distToDanger));
                }
                else if (decayType == DecayType.InverseSquare)
                {
                    float a = 3f;
                    distToDanger /= Game1.scale * 2;
                    distWeight = a / (distToDanger * distToDanger);
                }
                else if (decayType == DecayType.InverseLinear)
                {
                    float a = 0.95f;
                    distWeight = a / distToDanger;
                }
                else distWeight = 1f;

                Vector2 awayVector = owner.boundingBox.Center.ToVector2() - location;
                return Vector2.Dot(awayVector, UnitDirections[dir]) * weight * distWeight;
            }

            public bool Equals(Danger other)
            {
                return (other.location == location);
            }
        }

        private Dictionary<string, Attack> Attacks = new Dictionary<string, Attack>();

        public struct Attack
        {
            public static Attack None = new Attack();

            public delegate void Action();
            public Action action; // function invoked after charging
            public float chargeTime; // how long does the enenmy charge the attack for before attacking
            public float duration; // how long after charging is the enemy in "attack" mode
            public float lagTime; // how long after this attack before enemy can attack again
            public Animation.Animation animation; // animation for this attack
            public int damage;
            public Attack(Action action, float chargeTime, float duration, float lagTime, int damage, Animation.Animation anim=null)
            {
                this.damage = damage;
                this.action = action;
                this.chargeTime = chargeTime;
                this.duration = duration;
                this.lagTime = lagTime;
                this.animation = anim;
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

            this.thisDanger = new Danger(DecayType.InverseSquare, 0.3f, this.boundingBox.Center.ToVector2(), (this.boundingBox.Width + (2 * Game1.scale) ) / Game1.scale);
            dangers.Add(this.thisDanger);


            Attacks.Add("jump", new Attack(Jump, 250f, 500f, 5000f, 0, sprite.Animations["idle"]));

        }


        public override void Update(GameTime gameTime)
        {

            int myDangerIndex = dangers.FindIndex(x => x.location == this.boundingBox.Center.ToVector2());
            if (myDangerIndex != -1)
            {
                thisDanger = dangers[myDangerIndex];
            }

            if (health < 0) SendRemoveMapCall();
            DoModeActions();
            
            // NOTE: Any collision logic for purposes of combat MUST occur before the movement step, because after collision resolution concludes
            // no characters will collide with each other
            base.Update(gameTime);

            thisDanger.location = boundingBox.Center.ToVector2();

            if (myDangerIndex != -1)
            {
                dangers[myDangerIndex] = thisDanger;
            }
        }

        public static void UpdateStaticItems()
        {
            if (dangers.Count == 0) { AddDangers(); }
            centerOfPlayersHitbox = Game1.player.boundingBox.Center.ToVector2();
        }

        public static void AddDangers()
        {
            AddTilemapDangers();
            AddBuildingDangers();
        }

       
        public static void AddBuildingDangers()
        {
            foreach (Building b in SceneManager.CurScene.Map.buildings)
            {
                AddDanger(DecayType.InverseLinear, 1f, b.bounds.Center.ToVector2(), (Math.Max(b.bounds.Width, b.bounds.Height) - (10 * Game1.scale)) / Game1.scale);
            }
        }

        
        public static void AddDanger(DecayType dType, float weight, Vector2 pos, float maxInfluenceRadius)
        {
            dangers.Add(new Danger(dType, weight, pos, maxInfluenceRadius));
        }
        public static void RemoveDanger(GameObject g)
        {
            dangers.Remove(dangers.Find(x => x.location == g.bounds.Center.ToVector2()));
        }
        public void RemoveDanger()
        {
            dangers.Remove(dangers.Find(x => x.location == thisDanger.location));
        }
        public static void RemoveAllDangers()
        {
            dangers.Clear();
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
                    if (d.Equals(thisDanger)) continue; // don't do danger calculations on ourself

                    interest += d.CalculateInterest(this, dir);
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
                case (Mode.Attack ):
                    if (canAttack)
                    {
                        StartAttack(Attacks["jump"]);
                    }
                    if (CollidingWithPlayer())
                    {
                        Game1.player.GetHit(currentAttack);
                    }
                    
                    break;
            }

        }

        private void StartAttack(Attack atk)
        {
            
            if (!canAttack) return;
            Velocity = Vector2.Zero;
            oldPlayerPos = SceneManager.CurScene.Map.player.boundingBox.Center.ToVector2();
            movementLocked = true;
            canAttack = false;

            currentAttack = atk;
            if (atk.animation != null)
            {
                sprite.changeAnimation(atk.animation.Name);
            }

            // this is done at the end of charging
            GameManager.TimerManager.AddTimer(() =>
            {
                movementLocked = false;
                atk.action.Invoke();
            }, atk.chargeTime);


            // this is done at the end of the attack duration
            GameManager.TimerManager.AddTimer(() => 
            {
                mode = Mode.Pursue;
                currentAttack = Attack.None;
            }
            , atk.chargeTime + atk.duration);

            // this is done when the enemy can attack again
            GameManager.TimerManager.AddTimer(() => canAttack = true, atk.chargeTime + atk.lagTime);
        }
        
        private void Jump()
        {
            Vector2 posAtStart = pos;
            float attackSpeed = 5f;
            Vector2 toPlayerVector = Vector2.Normalize(oldPlayerPos - pos);
            Velocity = toPlayerVector * attackSpeed;

            GameManager.TimerManager.AddTimer(() => Velocity = Vector2.Normalize(posAtStart - pos) * attackSpeed, currentAttack.duration / 2);
        }
        protected void FindMode()
        {
            if (Game1.player.dead) { mode = Mode.Idle; return; }
            if (mode == Mode.Attack) { return;  }
            float distToPlayer = (SceneManager.CurScene.Player.pos - pos).Length();
            if (distToPlayer < 200 * Game1.scale) { mode = Mode.Pursue; }

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

        protected override void Kill()
        {
            base.Kill();
            RemoveDanger();
        }
     

        private bool CollidingWithPlayer()
        {
            Rectangle slightlyBiggerBounds = new Rectangle(boundingBox.Location - new Point(1), boundingBox.Size + new Point(2));
            return slightlyBiggerBounds.Intersects(Game1.player.boundingBox);
        }

        public static void DrawStatic(SpriteBatch b)
        {
            if (DebugParams.showCharacterColliders)
            {
                foreach (Danger d in dangers)
                {
                    if ((d.location - Game1.player.pos).Length() < 1000)
                        d.maxInfluenceArea.DrawCircle(b);
                }
            }
        }
        public override void Draw(SpriteBatch b)
        {
            base.Draw(b);
            
            // debug, draw gizmos for brain
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
