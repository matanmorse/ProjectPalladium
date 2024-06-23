using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;

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
            if (Game1.player.boundingBox.Intersects(new Rectangle(clickedTile * new Point(Map.scaledTileSize), new Point(Map.scaledTileSize))))
            { Debug.WriteLine("stop"); return; }
            // don't let us place in a way that causes a collision
            // if the add was successfull, remove the item
            if (SceneManager.CurScene.Map.AddGameObject(name, clickedTile.ToVector2()))
            {
                int thisIndex = Game1.player.inventory.FindExactItemStack(this);
                Game1.player.inventory.RemoveItemAtIndex(thisIndex, 1);
                UIManager.toolbar.Reset();
            }

        }
    }
}
