using ProjectPalladium.Utils;
using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using ProjectPalladium.Buildings;

namespace ProjectPalladium.Items
{
    /* All items (stations, furniture, etc) that can be placed in the gameworld */
    public class Placeable : Item
    {

        public Type worldObjectType;
        public Placeable(int id, string name, string textureName, int quantity, string description, Type worldObjectType = null) 
            : base(id, name, textureName, quantity, description, 1) 
        { 
            this.worldObjectType = worldObjectType == null ? typeof(PlaceableGameObject) : worldObjectType;
        }

        public override void Use()
        {
            Point clickedTile = Util.GetNearestTile(Input.gameWorldMousePos);
            if (!Util.IsTileWithinOneTileOfPlayer(clickedTile)) { return; }

            // don't let us place in a way that causes a collision
            if (Game1.player.boundingBox.Intersects(new Rectangle(clickedTile * new Point(Map.scaledTileSize), new Point(Map.scaledTileSize))))
            { return; }


            Vector2 position = clickedTile.ToVector2();
            string textureName = name.Replace(" ", "").ToLower() + "placed";



            // if the add was successfull, remove the item
            if (SceneManager.CurScene.Map.AddGameObject(name, clickedTile.ToVector2(), worldObjectType))
            {
                int thisIndex = Game1.player.inventory.FindExactItemStack(this);
                Game1.player.inventory.RemoveCurrentItem(1);
                UIManager.toolbar.Reset();
            }


        }
    }
}
