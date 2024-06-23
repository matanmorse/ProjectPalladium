using Microsoft.Xna.Framework;
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
        public enum Direction
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

        public NPC(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Rectangle boundingBox) : base(sprite, pos, name, startingMap, boundingBox)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (t < 10)
            {
                Move(Direction.right);
            }
            else if (t < 20)
            {
                Move(Direction.down);
            }
            else if (t < 30)
            {
                Move(Direction.left);
            }
            else if (t < 40)
            {
                Move(Direction.up);
            }
            else
            {
                t = 0;
            }
            t++;
            FindLocomotionAnimation();
            movePos();
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

        public void FindLocomotionAnimation()
        {
            if (Velocity.X != 0)
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
