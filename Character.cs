using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Formats.Asn1.AsnWriter;

namespace ProjectPalladium
{

    public class Character
    {
        public AnimatedSprite sprite;
        public Vector2 pos;
        private float speed = 2f * Game1.scale;
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

 
        public Character(AnimatedSprite sprite, Vector2 pos, String name) {
            this.sprite = sprite;
            this.pos = pos;
            this.name = name;
            
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
            edgex = (mapSize.X * tileSize * (int) Game1.scale) - (int)(sprite.spriteWidth * Game1.scale);
            edgey = (mapSize.Y * tileSize * (int) Game1.scale) - (int)(sprite.spriteHeight * Game1.scale);
        }
        public void movePos()
        {

            pos += velocity * speed;
            if (pos.X < 0) pos.X = 0;
            if (pos.X > edgex) pos.X = edgex;
            if (pos.Y < 0) pos.Y = 0;
            if (pos.Y > edgey) pos.Y = edgey;
        }

       
        public override string ToString()
        {
            return "Name: " + name;
        }
    }
}
