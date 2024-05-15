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
using ProjectPalladium.Content;
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
        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }

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
            sprite.Draw(b, pos, 1f);
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
            edgex = (mapSize.X * tileSize * (int)Game1.scale) - (int)((sprite.spriteWidth / 2) * Game1.scale);
            edgey = (mapSize.Y * tileSize * (int)Game1.scale) - (int)((sprite.spriteHeight / 2) * Game1.scale);
        }
        public void movePos()
        {

            pos += velocity * speed;
            boundingBox.Location = new Point((int)(pos.X - sprite.spriteWidth / 2 * Game1.scale), (int)(pos.Y - sprite.spriteHeight / 2 * Game1.scale));

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
                pos.X = pos.X < collided.Left ? collided.Left - (sprite.scaledWidth / 2)    : collided.Right + (sprite.scaledWidth / 2);
            }
            else
            {
                pos.Y = pos.Y < collided.Top ? collided.Top - (sprite.scaledHeight / 2) : collided.Bottom + (sprite.scaledHeight / 2);
            }
            boundingBox.Location = new Point((int)(pos.X - sprite.spriteWidth / 2 * Game1.scale), (int)(pos.Y - sprite.spriteHeight / 2 * Game1.scale));
            // TODO: fix jank collisions, but in the meantime if this didn't acually fix just put it at the top right
            if (currentMap.CheckCollisions(boundingBox) != Rectangle.Empty) { pos.X = collided.Right; pos.Y = collided.Top; }
        }

        public override string ToString()
        {
            return "Name: " + name;
        }
    }
}
