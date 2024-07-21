
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Animation;
using ProjectPalladium.Characters;
using ProjectPalladium.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace ProjectPalladium
{

    public class Character
    {
        protected string subfolder = ""; // subfolder for character data
        protected Map currentMap;
        public Color tintColor = Color.White;

        public int health = 10;
        protected bool invincible;
        protected bool dying;

        protected float invincFrames = 100f; // how long is this character invincible for after getting hit?

        public Map CurrentMap{ get { return currentMap; } set { currentMap = value; } }
        protected bool movementLocked;
        protected bool gettingKnockedBack;

        public virtual bool MovementLocked
        {
            get { return movementLocked; }
            set
            {
                if (dying)
                {
                    movementLocked = true; return;
                }

                movementLocked = value;
            }
        }


        public Rectangle boundingBox;
        public Rectangle prevBoundingBox;

        public float layer = Game1.layers.player;
        public AnimatedSprite sprite;
        public Vector2 pos;
        public Vector2 prevpos;

        public float speed = 1.1f * Game1.scale / (Game1.targetFPS / 40);
        private Vector2 velocity;

        private List<Projectile> projectiles = new List<Projectile>(); // list of projectiles belonging to this character
        List<Projectile> projectilesToRemove = new List<Projectile>();

        Rectangle intersection;
        List<Rectangle> intersections;

        public bool flipped;

        public Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                if (value.X != 0) flipped = velocity.X > 0 ? false : true;
            }
        }

        private Vector2 bBoxOffset;
        public int edgex;
        public int edgey;

        public string name;
        public bool moveLeft, moveRight, moveUp, moveDown;
        public float Speed
        {
            get { return speed; }
            set
            {
                if (speed < 1) speed = 1;
                speed = value;
            }
        }


        public Character(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Vector2 bBoxOffset, Vector2 bBoxSize)
        {
            this.sprite = sprite;
            this.pos = pos;
            this.name = name;
            currentMap = startingMap;
            this.bBoxOffset = bBoxOffset;
            this.boundingBox = new Rectangle(pos.ToPoint(), bBoxSize.ToPoint());
            sprite.Owner = this;

            Initialize();
        }

        public virtual void Initialize()
        {
            setBounds(currentMap.tileMapSize, 16);
        }

        public virtual void Update(GameTime gameTime)
        {
            layer = 0.1f + (Game1.LAYER_CONSTANT * (pos.Y + sprite.scaledHeight));

            foreach (Projectile p in projectiles) { p.Update(gameTime); }
            if (projectilesToRemove.Count > 0)
            {
                foreach(Projectile p in projectilesToRemove) { projectiles.Remove(p); }
                projectilesToRemove.Clear();
            }

            prevpos = pos;
            prevBoundingBox = boundingBox;
            sprite.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch b)
        {
           
            if (DebugParams.showCharacterColliders) { Util.DrawRectangle(boundingBox,b); }
           foreach(Projectile p in projectiles) { p.Draw(b); }
            sprite.Draw(b, pos, tintColor, flipped, layerDepth: layer);
        }
        public void setMovingUp(bool b)
        {
            moveUp = b;
        }
        public void setMovingDown(bool b)
        {
            moveDown = b;
        }
        public void setMovingLeft(bool b)
        {
            moveLeft = b;
        }
        public void setMovingRight(bool b)
        {
            moveRight = b;
        }

        public void setBounds(Point mapSize, int tileSize)
        {
            edgex = mapSize.X * tileSize * (int)Game1.scale - sprite.scaledWidth / 2;
            edgey = mapSize.Y * tileSize * (int)Game1.scale - sprite.scaledHeight / 2;
        }
        public void movePos()
        {
            if (movementLocked) return;
            pos += velocity * speed;
            boundingBox.Location = (pos + bBoxOffset).ToPoint();

            if (pos.X - sprite.scaledWidth / 2 < 0) pos.X = sprite.scaledWidth / 2;
            if (pos.X > edgex) pos.X = edgex;
            if (pos.Y - sprite.scaledHeight / 2 < 0) pos.Y = sprite.scaledHeight / 2;
            if (pos.Y > edgey) pos.Y = edgey;


            // get collision if one occurs, then resolve it
            List<Rectangle> collided = currentMap.CheckCollisions(boundingBox);

            if (collided.Count != 0)
            {
                velocity = Vector2.Zero;

                ResolveCollision(collided);
            }

        }

        public void ResolveCollision(List<Rectangle> intersections, int depth = 0)
        {
            
            this.intersections = intersections;

            // if all the rectangles are the same width/height, i.e. a set of rectangles lying on the same axis, we should combine them and resolve them as one to avoid issues
            bool allSameWidth = true;
            bool allSameHeight = true;
            int width = 0;
            int height = 0;
            foreach (Rectangle r in intersections)
            {
                if (width == 0) width = r.Width;
                else
                {
                    if (r.Width != width)
                    {
                        allSameWidth = false;
                        break;
                    }
                }
            }
            foreach (Rectangle r in intersections)
            {
                if (height == 0) height = r.Height;
                else
                {
                    if (r.Height != height)
                    {
                        allSameHeight = false;
                        break;
                    }
                }
            }

            // calculate combined rectangle
            Rectangle total = Rectangle.Empty;
            if (allSameWidth || allSameHeight)
            {
                foreach(Rectangle r in intersections)
                {
                    total = total == Rectangle.Empty ? r : Rectangle.Union(r, total);
                }
                this.intersection = total;
                ResolveCollision(total);
            }


            if (total != Rectangle.Empty) {
            }
            else
            {
                foreach (Rectangle intersection in intersections)
                {
                    ResolveCollision(intersection);
                }
            }

            // avoid stack overflow from recursive resolution
            if (depth > 100) return;

            // update bounding box location
            boundingBox.Location = (pos + bBoxOffset).ToPoint();

            // check if this set of resolutions put us in collision with another object, if so resolve it
            List<Rectangle> stillColliding = currentMap.CheckCollisions(boundingBox);
            if (stillColliding.Count != 0) ResolveCollision(stillColliding, depth + 1);

        }

        // resolves a single collision
        public void ResolveCollision(Rectangle intersection)
        {
            int distx = intersection.Width;
            int disty = intersection.Height;

            bool moveToSide = distx < disty;


            if (moveToSide)
            {
                boundingBox.X = boundingBox.X < intersection.Left ? intersection.Left - boundingBox.Width : intersection.Right;
            }
            else
            {
                boundingBox.Y = boundingBox.Y < intersection.Top ? intersection.Top - boundingBox.Height: intersection.Bottom;
            }
            pos = boundingBox.Location.ToVector2() - bBoxOffset;

            // boundingBox.Location = (pos + bBoxOffset).ToPoint();
        }

        public void AddProjectile(string name, Vector2 vel, float rotation)
        {
            projectiles.Add(new Projectile(name, this, 15f, pos, vel, rotation));
        }

        public bool RemoveProjectile(Projectile p)
        {
            foreach(Projectile x in projectiles)
            {
                if (x == p) { projectilesToRemove.Add(p); return true; }
            }
            return false;
        }

        public void GetHit(Enemy.Attack atk)
        {
            if (invincible || dying) return;

            Debug.WriteLine(name + " got hit for " + atk.damage + " damage.");
            GetHit(atk.damage);
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
            }, invincFrames);
        }

        /* Remove this enemy from the game >:) */
        protected virtual void Kill()
        {
            dying = true;
            Velocity = Vector2.Zero;
            movementLocked = true;
            boundingBox = Rectangle.Empty;
            sprite.PlayAnimationOnce("die", SendRemoveMapCall);
        }

        public void SendRemoveMapCall()
        {
            SceneManager.CurScene.Map.RemoveCharacter(this);
            dying = false;
        }
        protected void DoHitEffect()
        {
            tintColor = Color.Red;
            sprite.AddTimer(() =>
            {
                tintColor = Color.White;
            }
            , 150f);
        }

        public override string ToString()   
        {
            return "Name: " + name;
        }
    }
}
