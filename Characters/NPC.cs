﻿using Microsoft.Xna.Framework;
using ProjectPalladium.Animation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Characters
{
    public class NPC : Character
    {
        int t = 0;
        private enum Direction
        {
            up,
            down,
            right,
            left,
        }

        private Dictionary<Direction, Point> dirs = new Dictionary<Direction, Point>()
        {
            { Direction.up, new Point(0, -1) },
            { Direction.down, new Point(0, 1) },
            { Direction.left, new Point(-1, 0) },
            { Direction.right, new Point(1, 0) },
        };

        public NPC(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Vector2 bBoxOffset, Vector2 bBoxSize) : base(sprite, pos, name, startingMap, bBoxOffset, bBoxSize)
        {
            speed = 2f;
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);


            FindLocomotionAnimation();

            movePos();

        }

        protected void Idle()
        {
            if (movementLocked) { return; }
            if (t < 50)
            {
                Move(Direction.right);
            }
            else if (t < 100)
            {
                Move(Direction.down);
            }
            else if (t < 150)
            {
                Move(Direction.left);
            }
            else if (t < 200)
            {
                Move(Direction.up);
            }
            else
            {
                t = 0;
            }
            t++;

        }
        private void Move(Direction dir)
        {
            Velocity = dirs[dir].ToVector2();
        }

        public void SetToIdle()
        {
            if (!sprite.Animation.Name.Contains("idle"))
            {
                if (sprite.Animation == sprite.Animations["walk-side"])
                {
                    sprite.changeAnimation("idle-side");
                }
                else if (sprite.Animation == sprite.Animations["walk-front"])
                {
                    sprite.changeAnimation("idle");
                }
                else if (sprite.Animation == sprite.Animations["walk-back"])
                {
                    sprite.changeAnimation("idle-back");
                }
            }
        }

        protected virtual void FindLocomotionAnimation()
        {
            if (movementLocked) return;

            

            if (Math.Abs(Velocity.X) >= Math.Abs(Velocity.Y)) 
            {
                sprite.changeAnimation("walk-side");
            }
            else if (Velocity.Y > 0)
            {
                sprite.changeAnimation("walk-front");
            }
            else if (Velocity.Y < 0)
            {
                sprite.changeAnimation("walk-back");
            }
            else // sprite isn't moving, need to do some kind of idle animation
            {
                SetToIdle();
            }
        }
    }
}
