using Microsoft.Xna.Framework;
using ProjectPalladium.Items;
using ProjectPalladium.UI;
using System.Diagnostics;

namespace ProjectPalladium.Buildings
{
    public class PlaceableGameObject : GameObject
    {
        private float millisToPerformClicks = 400f; // milliseconds to do required number of left clicks to remove building
        private int numberOfClicks = 3; // clicks to be done within interval to remove
        private float timeOfFirstClick;
        private int clickCount = 0; // current number of clicks

        protected Button button;
        public PlaceableGameObject(string name, Vector2 pos, string textureName = "") : base(name, pos, textureName)
        {
            button = new Button(null, null, null, bounds.Location,bounds.Size);
        }

        public void TrackLeftClicks(GameTime gameTime)
        {
            if (!button.mouseOver) { return; }
            if (!button.CheckLeftClick()) { return; }

            // on first click, start the timer
            float currentTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            if (clickCount == 0) {
                timeOfFirstClick = currentTime;
                clickCount++;
                return;
            }
            if (currentTime - timeOfFirstClick > millisToPerformClicks) // reset the timer
            {
                clickCount = 0;
                return;
            }
            clickCount++;
            if (clickCount >= numberOfClicks)
            {
                Remove();
            }
        }

        public override void Update(GameTime gameTime)
        {
            TrackLeftClicks(gameTime);
            button.Update(gameTime);
        }

       

        /* When the player picks up the object */
        public virtual void Remove()
        {
            Map curMap = SceneManager.CurScene.Map;
            if (curMap.RemoveGameObject(this))
            {
                // give the player back the item
                curMap.player.inventory.AddItem(Item.GetItemFromRegistry(name.ToLower()), 1);
            }
        }
    }
}
