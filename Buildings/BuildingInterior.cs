using ProjectPalladium.Triggers;
using ProjectPalladium.Utils;
using Microsoft.Xna.Framework;
using ProjectPalladium.UI;
using System.Diagnostics;


namespace ProjectPalladium.Buildings
{
    public class BuildingInterior : Map
    {
        private Building exteriorBuilding; // the building object that exists on the parent map
        
        public BuildingInterior(string filename, Building exteriorBuilding) : base(filename)
        {
            this.exteriorBuilding = exteriorBuilding;

            // associate each ChangeSceneTrigger with its appropriate door
            foreach (Trigger t in triggers)
            {
                if (!(t is ChangeSceneTrigger)) continue;

                ChangeSceneTrigger cst = (ChangeSceneTrigger)t;
                Button door = exteriorBuilding.door;
                cst.SpawnPos = (door.bounds.Location.ToVector2() 
                    // to maintain scale-neutrality, we weirdly divide by scale here to multiply by it later, just so the spawnPos is given in pre-scaled game-world size
                    // this maintains compatibility with the tiled editor
                    + new Vector2(door.bounds.Size.X / 2, door.bounds.Size.Y / 2 + Game1.player.boundingBox.Size.Y)) / Game1.scale;
            }
        }
    }
}
