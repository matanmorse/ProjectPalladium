using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace ProjectPalladium
{
    public class Player : Character
    {
        private Vector2 inputDir = Vector2.Zero;

        public Player(AnimatedSprite sprite, Vector2 pos, string name) : base(sprite, pos, name)
        {
           
        }

        public void handleMovement()
        {
            inputDir = Vector2.Zero;

            setMovingUp(Keyboard.GetState().IsKeyDown(Keys.W));
            setMovingDown(Keyboard.GetState().IsKeyDown(Keys.S));
            setMovingLeft(Keyboard.GetState().IsKeyDown(Keys.A));
            setMovingRight(Keyboard.GetState().IsKeyDown(Keys.D));

            if (moveUp) inputDir.Y -= 1;
            
            if (moveDown) inputDir.Y += 1;
            
            if (moveLeft) inputDir.X -= 1;
            
            if (moveRight) inputDir.X += 1;
            

            if (moveUp && moveDown) inputDir.Y = Math.Sign(Velocity.Y);
            
            if (moveLeft && moveRight) inputDir.X = Math.Sign(Velocity.X);
            

            Velocity = inputDir;
            movePos();
        }
    }

}
