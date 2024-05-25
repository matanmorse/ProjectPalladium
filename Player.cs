using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using ProjectPalladium.Animation;
using ProjectPalladium.Items;
using Tutorial;
using System.Diagnostics;

namespace ProjectPalladium
{
    public class Player : Character
    {
        private UIManager uiManager;
        private Vector2 inputDir = Vector2.Zero;
        public Inventory inventory;
        public Player(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Rectangle boundingBox) :
            base(sprite, pos, name, startingMap, boundingBox)
        {
            currentMap.player = this;
            uiManager = Game1.UIManager;
            inventory = uiManager.inventoryUI.Inventory;
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

            if (Velocity.Y != 0 || Velocity.X != 0)
            {
                if (sprite.Animation != sprite.Animations["walk"])
                {
                    sprite.AnimationChangeDetected();
                    sprite.Animation = sprite.Animations["walk"];
                }
            }
            else
            {
                if (sprite.Animation != sprite.Animations["idle"])
                {
                    sprite.AnimationChangeDetected();
                    sprite.Animation = sprite.Animations["idle"];
                }
            }
            movePos();
        }

        public void doInputCheck()
        {
            if (Input.GetKeyDown(Keys.E))
            {
                uiManager.inventoryUI.ToggleShowing();
            }


            // debug code for adding and removing items
            if (Input.GetKeyDown(Keys.O))
            {
                inventory.AddItem(Item.Items["ectoplasmic gem"], 10);
            }
            if (Input.GetKeyDown(Keys.I))
            {
                inventory.RemoveItem(Item.Items["ectoplasmic gem"], 9);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            handleMovement();
            doInputCheck();
        }
    }

}
