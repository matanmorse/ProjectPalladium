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

namespace ProjectPalladium.Stations
{
    public class Cauldron : Station
    {
        private const int NUM_ITEMS = 3; // number of items that can be held in the cauldron and used in potions
        private StationUI cauldronUI;
        private Item[] heldItems = new Item[NUM_ITEMS];
        private bool brewing;
        public bool Brewing
        {
            get { return brewing; }
            set
            {
                if (value) animatedSprite.changeAnimation("brew");
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
            button.onRightClick = AddItem;
        }


        public void AddItem()
        {
            Item currentItem = Game1.player.ActiveItem;
            if (currentItem == null || currentItem == Item.none || !currentItem.potionIngredient) return;

            Debug.WriteLine("adding ingredient");

            int firstEmptyIndex = Array.FindIndex(heldItems, x => x == Item.none); // find first empty item
            if (firstEmptyIndex == -1) return; // cauldron is full

            heldItems[firstEmptyIndex] = currentItem;
            int index = Game1.player.inventory.FindExactItemStack(currentItem);
            Game1.player.inventory.RemoveItemAtIndex(index, 1);

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
            // return items to player's inventory
            foreach (Item item in heldItems)
            {
                if (item == Item.none) continue;
                Game1.player.inventory.AddItem(item, 1);
            }
            base.Remove();
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
