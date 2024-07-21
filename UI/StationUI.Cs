using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Items;
using ProjectPalladium.Utils;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using ProjectPalladium.Stations;

namespace ProjectPalladium.UI
{
    /* UI to display Items over a station (cauldron as of 7/9) */
    public class StationUI
    {

        public class StationItemSlot
        {
            private Renderable sprite;
            private Vector2 pos;
            private Item item;

            public Item Item { get { return item; } 
            set
                {
                    item = value;
                    this.sprite = new Renderable(item.textureName);
                }
            }

            private static Renderable stationItemSlot = new Renderable("stationitemslot");

            public StationItemSlot(string textureName, Item item, int slotPos, Vector2 startPos, Vector2 itemDisplaySize)
            {
                this.sprite = new Renderable(textureName);
                this.pos = startPos;
                this.item = item;
            }

            public void Draw(SpriteBatch b)
            {
                stationItemSlot.Draw(b, pos, layer: Game1.layers.UI - 0.01f, scale: StationUIScale);
                item.Draw(b, pos, scale: StationUIScale, origin:Vector2.Zero);

            }

        }

        
        public Vector2 pos;

        public Vector2 itemDisplaySize;

        private const int NUM_ITEMS = 3;
        public StationItemSlot[] items = new StationItemSlot[NUM_ITEMS];

        public Station parent;

        public static float StationUIScale;
        public StationUI(Station parent)
        {
            this.parent = parent;

            StationUIScale = Game1.scale * 0.5f; // station UI is 3/4 of normal UI

            float bottomOfStation = (parent.sprite.size.Y * Game1.scale) - (8 * StationUIScale);
            Vector2 ITEMDISPLAY_START_OFFSET = new Vector2(0, bottomOfStation);
            this.pos = parent.globalPos + ITEMDISPLAY_START_OFFSET;

            // set up display to be centered on the center of the station with correct number of slots
            itemDisplaySize = new Vector2( 17 , 0) * StationUIScale;

            

            float totalWidth = (NUM_ITEMS - 1) * itemDisplaySize.X;
            float centerOfStation = parent.globalPos.X + ((parent.animatedSprite.spriteWidth / 2) * Game1.scale);
            float startX = 4 + centerOfStation - (totalWidth / 2);
            for (int i = 0; i < NUM_ITEMS; i++)
            {
                float xCoord = startX + (i * itemDisplaySize.X ) - ( (itemDisplaySize.X / 2)) ;
                items[i] = new StationItemSlot(null, Item.none, i, new Vector2(xCoord, pos.Y), itemDisplaySize);
            }
        }

        public void Draw(SpriteBatch b)
        {
            if (parent.Completed)
            {
                items[1].Draw(b); // only draw the center item slot when completed
            }
            else // otherwise draw all item slots
            {
                foreach (StationItemSlot sprite in items)
                {
                    sprite.Draw(b);
                }
            }
          
        }
    }
}
