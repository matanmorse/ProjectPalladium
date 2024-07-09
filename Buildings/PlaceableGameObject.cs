using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium.Items;
using ProjectPalladium.UI;
using System.Diagnostics;

namespace ProjectPalladium.Buildings
{
    public class PlaceableGameObject : GameObject
    {
        protected Button button;
        public PlaceableGameObject(string name, Vector2 pos, string textureName = "") : base(name, pos, textureName)
        {
            button = new Button(null, Remove, null, bounds.Location,bounds.Size);
        }


        public override void Update(GameTime gameTime)
        {
            button.Update();
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
