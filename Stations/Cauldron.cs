using Microsoft.Xna.Framework;
using ProjectPalladium.Buildings;
using ProjectPalladium.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium.Items;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
using ProjectPalladium.Animation;
using GameWorldTime = ProjectPalladium.GameManager.GameWorldTime;
namespace ProjectPalladium.Stations
{
    public class Cauldron : Station
    {

        private const int NUM_ITEMS = 3; // number of items that can be held in the cauldron and used in potions
        private StationUI cauldronUI;
        private Item[] heldItems = new Item[NUM_ITEMS];
        private bool brewing;

        private GameWorldTime brewStartTime;
        private int timeSinceBrewStart;
        private int brewTime = 10; // minutes for a potion to brew
        public bool Brewing
        {
            get { return brewing; }
            set
            {
                // to do when start brewing
                if (value)
                {
                    brewStartTime = GameManager.time;
                    animatedSprite.changeAnimation("brew");
                }
                // to do when end brewing
                else animatedSprite.changeAnimation("idle");
                brewing = value;
            }
        }

      
    
        public Cauldron(string name, Vector2 pos) : base(name, pos, "cauldron", textureName:"cauldronplaced")
        {
            for (int i = 0 ; i < NUM_ITEMS; i++)
            {
                heldItems[i] = Item.none;
            }


            Brewing = false;
            
            cauldronUI = new StationUI(this);
            UpdateUI();
            button.onRightClick = HandleRightClicks;
            button.onClick = RemoveItem;
        }

        public override void UpdateOnGameTime()
        {
            if (!Brewing) return;

            base.UpdateOnGameTime();
            timeSinceBrewStart = Util.CalculateMinutesDifference(GameManager.time, brewStartTime);
            if (timeSinceBrewStart >= brewTime)
            {
                FinishBrewing();
            }
        }

        public void FinishBrewing()
        {
            Brewing = false;
            Completed = true;

            Potion result = new Potion(heldItems);

            for (int i = 0; i < NUM_ITEMS; i++) heldItems[i] = Item.none; // empty the cauldron

            heldItems[1] = result;
            UpdateUI();
        }

        public void HandleRightClicks()
        {
            if (Brewing) return; // right clicking on a brewing cauldron does nothing
            if (Completed) // get the resultant item
            {
                ReturnItems(); 
                Completed = false; 
                return;
            }
            // default behavior is to add an item
            AddItem();
        }
        public void AddItem()
        {
            Item currentItem = Game1.player.ActiveItem;
            if (currentItem == null || currentItem == Item.none || !currentItem.potionIngredient) return;

            int firstEmptyIndex = Array.FindIndex(heldItems, x => x == Item.none); // find first empty item
            if (firstEmptyIndex == -1) return; // cauldron is full

            heldItems[firstEmptyIndex] = currentItem;
            Game1.player.inventory.RemoveCurrentItem(1);

            UpdateUI();
        }

        /* Pop an item off the stack */
        public void RemoveItem()
        {
            if (brewing) return;
            int lastItemIndex = Array.FindLastIndex(heldItems, x => x != Item.none);
            if (lastItemIndex == -1) return; // cauldron is empty

            Game1.player.inventory.AddItem(heldItems[lastItemIndex], 1);
            heldItems[lastItemIndex] = Item.none;

            UpdateUI();
        }
        public void TryBrew()
        {
            if (Array.FindAll<Item>(heldItems, x => x != Item.none).Length == NUM_ITEMS)
            {
                Brewing = true;
            }
        }
        public override void Remove()
        {
            if (!brewing)
            {
                ReturnItems();
            }


            base.Remove();
        }

        // returns all items in the cauldron to the player's inventory
        public void ReturnItems()
        {
            // return items to player's inventory
            for (int i = 0; i < NUM_ITEMS;  i++) 
            {
                if (heldItems[i] == Item.none) continue;
                Game1.player.inventory.AddItem(heldItems[i], 1);
                heldItems[i] = Item.none;
            }
        }
        public void UpdateUI()
        {
            for( int i = 0 ;i < NUM_ITEMS; i++)
            {
                cauldronUI.items[i].Item = heldItems[i];
            }
        }
        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch b)
        {
            
            // if there is at least 1 held item, draw the UI
            if (!(Array.FindIndex(heldItems, x => x != Item.none) == -1))
            {
                cauldronUI.Draw(b);
            }

            animatedSprite.Draw(b, globalPos, layer);
        }
    }
}
