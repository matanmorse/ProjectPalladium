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
        private Button button;
        public PlaceableGameObject(string name, Vector2 pos, string textureName = "") : base(name, pos, textureName)
        {
            button = new Button(null, null, null, bounds.Location,bounds.Size, onRightClick:Remove);
        }


        public override void Update(GameTime gameTime)
        {
            button.Update();
        }

        /* When the player picks up the object */
        public void Remove()
        {
            Debug.WriteLine("remove");
            Map curMap = SceneManager.CurScene.Map;
            if (curMap.RemoveGameObject(this))
            {
                // give the player back the item
                curMap.player.inventory.AddItem(Item.GetItemFromRegistry(name.ToLower()), 1);
            }
        }
    }
}
