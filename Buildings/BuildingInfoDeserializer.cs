using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Buildings
{
    public class BuildingDeserializer
    {
        public string name { get; set; }
        public Dictionary<string, ColliderDetails> colliders { get; set; }
    }

    public class ColliderDetails
    {
        public int[] size { get; set; }
        public int[] location { get; set; }


    }
}
