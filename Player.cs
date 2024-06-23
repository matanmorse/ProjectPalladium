using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using ProjectPalladium.Animation;
using ProjectPalladium.Items;
using ProjectPalladium;
using System.Diagnostics;
using ProjectPalladium.Utils;
using ProjectPalladium.Plants;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Tools;
namespace ProjectPalladium
{
    public class Player : Character
    {
        private UIManager uiManager;
        private Vector2 inputDir = Vector2.Zero;
        public Inventory inventory;
        private Item _activeItem;
        public bool holdingTool;


        public bool usingItemLocked = false;
        public Point feet; // lmao

        // mana + spellcasting info
        public const int MAX_MANA = 100;
        public int mana = MAX_MANA;

        public Vector2 lerpingCamera;



        public Item ActiveItem { get { return _activeItem; } 
            
            set {
                usingItemLocked = true;
                _activeItem = value;
            }
        } 
        public Player(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Rectangle boundingBox) :
            base(sprite, pos, name, startingMap, boundingBox)
        {
            feet = Util.GetNearestTile(pos) + new Point(0, 1);
            currentMap.player = this;
            uiManager = Game1.UIManager;
            inventory = uiManager.inventoryUI.Inventory;

        }

        public void handleMovement()
        {

            if (movementLocked) return;

            inputDir = Vector2.Zero;
            setMovingUp(Keyboard.GetState().IsKeyDown(Keys.W));
            setMovingDown(Keyboard.GetState().IsKeyDown(Keys.S));
            setMovingLeft(Keyboard.GetState().IsKeyDown(Keys.A));
            setMovingRight(Keyboard.GetState().IsKeyDown(Keys.D));

            if (moveUp) inputDir.Y -= 1;
            if (moveDown) inputDir.Y += 1;
            if (moveLeft) inputDir.X -= 1;
            if (moveRight) inputDir.X += 1;

            // if (inputDir != Vector2.Zero) { inputDir.Normalize(); }
            if (moveUp && moveDown) inputDir.Y = Math.Sign(Velocity.Y);

            if (moveLeft && moveRight) inputDir.X = Math.Sign(Velocity.X);

            Velocity = inputDir;
            
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
            movePos();
        }

        public void Halt()
        {
            Velocity = Vector2.Zero;
            SetToIdle();
        }

        public void SetToIdle()
        {
            if (!(sprite.Animation.Name.Contains("idle")))
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
        public void doInputCheck()
        {
            
            if (Input.GetKeyDown(Keys.E))
            {
                uiManager.inventoryUI.ToggleShowing();
            }


            // debug code for adding and removing items
            if (Input.GetKeyDown(Keys.O))
            {
                currentMap.AddEnemy("mage", new Vector2(100, 100) * Game1.scale);
            }


            if (_activeItem != null && _activeItem != Item.none && Input.GetLeftMouseClicked())
            {
               
                if (!usingItemLocked)
                {
                    _activeItem.Use();
                }
                else
                {
                    usingItemLocked = false;
                }
            }

            if (Input.GetRightMouseClicked())
            {
                sprite.AddTimer(() => Debug.WriteLine("ding!"), 1000f);
                HandleRightClicks();
            }
        }

        public void HandleRightClicks()
        {
            // get game object at spot of click
            GameObject obj = SceneManager.CurScene.Map.FindGameObjectAtTile(Util.GetNearestTile(Input.gameWorldMousePos)); 
            if (obj is Plant) { (obj as Plant).Harvest(); }
        }

        public override void Update(GameTime gameTime)
        {
          
            lerpingCamera = Vector2.Lerp(lerpingCamera, pos , 0.3f);
            base.Update(gameTime);
            feet = Util.GetNearestTile(pos) + new Point(0, 1);

            handleMovement();
            doInputCheck();


            if (_activeItem != null && _activeItem != Item.none)
            {                
                _activeItem.Update();
            }
            // Debug.WriteLine(pos);
        }

        public override void Draw(SpriteBatch b)
        {
            if (holdingTool) { (ActiveItem as Tool).Draw(b); }
            base.Draw(b);
        }
    }

}
