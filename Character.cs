﻿
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Animation;
using ProjectPalladium.Utils;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProjectPalladium
{

    public class Character
    {
        protected Map currentMap;

        public Map CurrentMap{ get { return currentMap; } set { currentMap = value; } }
        protected bool movementLocked; 
        public bool MovementLocked
        {
            get { return movementLocked; } 
            set
            {
                movementLocked = value;
              
            }
        }

        public Rectangle boundingBox;
        public float layer = Game1.layers.player;
        public AnimatedSprite sprite;
        public Vector2 pos;
        public Vector2 prevpos;

        public float speed = 1.25f * Game1.scale;
        private Vector2 velocity;

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

        private int edgex;
        private int edgey;

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


        public Character(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Rectangle boundingBox)
        {
            this.sprite = sprite;
            this.pos = pos;
            this.name = name;
            currentMap = startingMap;
            this.boundingBox = boundingBox;
        }

        public virtual void Initialize()
        {
            // load sprite data
            sprite.LoadContent();
        }

        public virtual void Update(GameTime gameTime)
        {
            prevpos = pos;
            sprite.Update(gameTime);

        }

        public virtual void Draw(SpriteBatch b)
        {
           
            if (DebugParams.showColliders) { Util.DrawRectangle(boundingBox, b); }
            sprite.Draw(b, pos, flipped, layerDepth: layer);
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

            pos += velocity * speed;
            boundingBox.Location = new Point((int)(pos.X - sprite.scaledWidth / 2 + (sprite.scaledWidth - boundingBox.Width) / 2), (int)(pos.Y - sprite.scaledHeight / 2) + (sprite.scaledHeight - boundingBox.Height) / 2);

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
            boundingBox.Location = new Point((int)(pos.X - sprite.scaledWidth / 2 + (sprite.scaledWidth - boundingBox.Width) / 2),
            (int)(pos.Y - sprite.scaledHeight / 2) + (sprite.scaledHeight - boundingBox.Height) / 2);

            // check if this set of resolutions put us in collision with another object, if so resolve it
            List<Rectangle> stillColliding = currentMap.CheckCollisions(boundingBox);
            if (stillColliding.Count != 0) ResolveCollision(stillColliding, depth + 1);

        }

        // resolves a single collision
        public void ResolveCollision(Rectangle intersection)
        {
            // distances to the outside of the bounding box
            int distx = intersection.Size.X;
            int disty = intersection.Size.Y;


            bool moveToSide = distx < disty ? true : false;
            if (moveToSide)
            {
                pos.X = pos.X < intersection.Left ? intersection.Left - boundingBox.Width / 2 : intersection.Right + boundingBox.Width / 2;
            }
            else
            {
                pos.Y = pos.Y < intersection.Top ? intersection.Top - boundingBox.Height / 2 : intersection.Bottom + boundingBox.Height / 2;
            }
        }
        public override string ToString()   
        {
            return "Name: " + name;
        }
    }
}
