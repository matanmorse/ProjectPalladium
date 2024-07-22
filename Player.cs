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
        private LightObject playerLight;

        public bool dead;
        public bool usingItemLocked = false;
        private bool dialogueBoxOpen;
        public bool DialogueBoxOpen
        {
            get { return dialogueBoxOpen; }
            set
            {
                dialogueBoxOpen = value;
                movementLocked = value;
                if (value) SetToIdle();
            }
        }

        public override bool MovementLocked
        {
            get { return movementLocked; }
            set 
            {
                if (value) SetToIdle();
                base.MovementLocked = value;
            }
        }

        public Point feet; // lmao

        // mana + spellcasting info
        public const int MAX_MANA = 100;
        private int mana = MAX_MANA;

        public int Mana
        {
            get { return mana; }
            set
            {
                if (value > MAX_MANA) mana = MAX_MANA;
                else mana = value;
            }
        }
        public bool castingAttackSpell; // do we need to show the target indicator

        public Vector2 lerpingCamera;
        public Item ActiveItem { get { return _activeItem; } 
            
            set {
                if (value == Item.none || value == null) ActiveItemIndex = -1;
                usingItemLocked = true;
                _activeItem = value;
            }
        }

        private int activeItemIndex; // inventory index of active item
        public int ActiveItemIndex
        {
            get { return activeItemIndex; }
            set 
            {
                UIManager.toolbar.ResetItemSlot(activeItemIndex);
                activeItemIndex = value; 
            }
        }
        public Player(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Vector2 bBoxOffset, Vector2 bBoxSize) :
            base(sprite, pos, name, startingMap, bBoxOffset, bBoxSize)
        {
            this.subfolder = "player/";
            feet = Util.GetNearestTile(boundingBox.Center);
            currentMap.player = this;
            uiManager = Game1.UIManager;
            inventory = UIManager.inventoryUI.Inventory;
            invincFrames = 1000f;
        }

        public override void Initialize()
        {
            base.Initialize();

            playerLight = Lightmap.AddLightObject(pos, LightObject.LightTypes.circle, (int)(100 * Game1.scale), 1f, this);
        }
        protected override void Kill()
        {
            Debug.WriteLine("player is dead");
            dead = true;
            sprite.spriteTexture = null;
            movementLocked = true;
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
                UIManager.inventoryUI.ToggleShowing();

            }


            // debug code for adding and removing items
            if (Input.GetKeyDown(Keys.O))
            {
                SceneManager.CurScene.Map.AddEnemy("slime", pos);
            }
            if (Input.GetKeyDown(Keys.M))
            {
                Debug.WriteLine(activeItemIndex);
            }


            if (_activeItem != null && _activeItem != Item.none)
            {
               
                if (!usingItemLocked && !UIManager.inventoryUI.showing)
                {
                    // some items are used with left click, all others with right click.
                    if ((_activeItem is Tool || _activeItem is Placeable) && Input.GetLeftMouseClicked()) { _activeItem.Use(); }
                    else if (Input.GetRightMouseClicked()) { _activeItem.Use(); }
                }
                else
                {
                    usingItemLocked = false;
                }
            }

            if (Input.GetRightMouseClicked())
            {
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
            if (dead) return; // duh
            if (dialogueBoxOpen) movementLocked = true;

            lerpingCamera = Vector2.Lerp(lerpingCamera, pos , 0.1f);
            lerpingCamera.Round();
            base.Update(gameTime);
            feet = Util.GetNearestTile(boundingBox.Center);

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
