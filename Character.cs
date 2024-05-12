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
        private float speed = 5f;
        private Vector2 velocity;
        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }

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
        public void movePos()
        {
            int edgex = Game1.screenWidth - (int)(sprite.spriteWidth * sprite.scale);
            int edgey = Game1.screenHeight - (int)(sprite.spriteHeight * sprite.scale);
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
