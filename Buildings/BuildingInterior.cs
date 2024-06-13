using ProjectPalladium.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium.Utils;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using ProjectPalladium.UI;


namespace ProjectPalladium.Buildings
{
    public class BuildingInterior : Map
    {
        private Building exteriorBuilding; // the building object that exists on the parent map
        
        public BuildingInterior(string filename, Building exteriorBuilding) : base(filename)
        {
            this.exteriorBuilding = exteriorBuilding;

            // associate each ChangeSceneTrigger with its appropriate door
            foreach (Utils.Trigger t in triggers)
            {
                if (!(t is ChangeSceneTrigger)) continue;

                ChangeSceneTrigger cst = (ChangeSceneTrigger)t;
                Button door = exteriorBuilding.door;
                cst.SpawnPos = door.bounds.Location.ToVector2() + new Vector2(door.bounds.Size.X / 2, door.bounds.Size.Y + (2 * Game1.scale));
            }
        }
    }
}
