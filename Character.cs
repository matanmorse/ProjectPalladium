using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Formats.Asn1.AsnWriter;

namespace ProjectPalladium
{

    public class Character
    {
        protected Map currentMap;

        public Rectangle boundingBox;

        public AnimatedSprite sprite;
        public Vector2 pos;
        public float speed = 2f * Game1.scale;
        private Vector2 velocity;

        public bool flipped;

        public Vector2 Velocity { get { return velocity; } set { velocity = value; 
                if (value.X != 0) flipped = velocity.X > 0 ? true : false; } }

        private int edgex;
        private int edgey;

        public string name;
        public bool moveLeft, moveRight, moveUp, moveDown;
        public float Speed {
            get { return speed; }
            set {
                if (speed < 1) speed = 1;
                speed = value;
            }
        }


        public Character(AnimatedSprite sprite, Vector2 pos, String name, Map startingMap, Rectangle boundingBox)
        {
            this.sprite = sprite;
            this.pos = pos;
            this.name = name;
            this.currentMap = startingMap;
            this.boundingBox = boundingBox;
        }

        public virtual void Initialize()
        {
            // load sprite data
            sprite.LoadContent();
        }

        public virtual void Update(GameTime gameTime)
        {
            
            sprite.Update(gameTime);
            
        }

        public virtual void Draw(SpriteBatch b)
        {
            if (DebugParams.showColliders) { Util.DrawRectangle(boundingBox, b); }
            sprite.Draw(b, pos, 1f, flipped);
        }
        public void setMovingUp(bool b) {
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
            edgex = (mapSize.X * tileSize * (int)Game1.scale) - (int)((sprite.scaledWidth / 2));
            edgey = (mapSize.Y * tileSize * (int)Game1.scale) - (int)((sprite.scaledHeight / 2));
        }
        public void movePos()
        {

            pos += velocity * speed;
            boundingBox.Location = new Point((int)(pos.X - (sprite.scaledWidth / 2) + ((sprite.scaledWidth - boundingBox.Width) / 2)), (int)(pos.Y - sprite.scaledHeight / 2) + ((sprite.scaledHeight - boundingBox.Height) / 2));

            if (pos.X - (sprite.spriteWidth * Game1.scale / 2) < 0) pos.X = (sprite.spriteWidth * Game1.scale / 2);
            if (pos.X > edgex) pos.X = edgex;
            if (pos.Y - (sprite.spriteHeight * Game1.scale / 2) < 0) pos.Y = (sprite.spriteHeight * Game1.scale / 2);
            if (pos.Y > edgey) pos.Y = edgey;


            // get collision if one occurs, then resolve it
            Rectangle collided = currentMap.CheckCollisions(boundingBox);

            if (collided != Rectangle.Empty)
            {
                velocity = Vector2.Zero;
                
                ResolveCollision(collided, Rectangle.Intersect(boundingBox, collided));
            }
            
        }

        public void ResolveCollision(Rectangle collided, Rectangle interSection)
        {
            // distances to the outside of the bounding box
            int distx = interSection.Size.X;
            int disty = interSection.Size.Y;


            bool moveToSide = distx < disty ? true : false;
            if (moveToSide)
            {
                pos.X = pos.X < collided.Left ? collided.Left - (boundingBox.Width / 2)    : collided.Right + (boundingBox.Width / 2);
            }
            else
            {
                pos.Y = pos.Y < collided.Top ? collided.Top - (boundingBox.Height / 2) : collided.Bottom + (boundingBox.Height / 2);
            }
            boundingBox.Location = new Point((int)(pos.X - (sprite.scaledWidth / 2) + ((sprite.scaledWidth - boundingBox.Width) / 2)), 
                (int)(pos.Y - sprite.scaledHeight / 2) + ((sprite.scaledHeight - boundingBox.Height) / 2));
            // TODO: fix jank collisions, but in the meantime if this didn't acually fix just put it at the top right
            Rectangle stillCollides = currentMap.CheckCollisions(boundingBox);
            if ( stillCollides != Rectangle.Empty) { ResolveCollision(stillCollides, Rectangle.Intersect(stillCollides, boundingBox));  }
                
        }

        public override string ToString()
        {
            return "Name: " + name;
        }
    }
}
