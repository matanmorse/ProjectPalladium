using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectPalladium.Items
{
    /* All items (stations, furniture, etc) that can be placed in the gameworld */
    public class Placeable : Item
    {
        public Placeable(int id, string name, string textureName, int quantity, string description) 
            : base(id, name, textureName, quantity, description, 1) 
        { 
        
        }

        public override void Use()
        {
            Point clickedTile = Util.GetNearestTile(Input.gameWorldMousePos);
            if (!Util.IsTileWithinOneTileOfPlayer(clickedTile)) { return; }

            // if the add was successfull, remove the seed
            if (SceneManager.CurScene.Map.AddGameObject(name, clickedTile.ToVector2()))
            {
                int thisIndex = Game1.player.inventory.FindExactItemStack(this);
                Game1.player.inventory.RemoveItemAtIndex(thisIndex, 1);
            }

        }
    }
}
