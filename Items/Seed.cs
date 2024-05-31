using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium;
using ProjectPalladium.Utils;
namespace ProjectPalladium.Items
{
    public class Seed : Item
    {
        private string plantName;
        public Seed(int id, string name, string textureName, int quantity, string description, int stacksize, string plantName)
            : base(id, name, textureName, quantity, description, stacksize)
        {
            this.plantName = plantName;
        }

        public override void Use()
        {
            Point clickedTile = Util.GetNearestTile(Input.gameWorldMousePos);
            if (!Util.IsTileWithinOneTileOfPlayer(clickedTile)) { return; }

            // if the add was successfull, remove the seed
            if (SceneManager.CurScene.Map.AddPlant(plantName, clickedTile.ToVector2()))
            {
                int thisIndex = Game1.player.inventory.FindExactItemStack(this);
                Game1.player.inventory.RemoveItemAtIndex(thisIndex, 1);
            }

        }
    }
}
