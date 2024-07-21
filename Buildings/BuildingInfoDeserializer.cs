using System.Collections.Generic;

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
