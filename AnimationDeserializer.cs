using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectPalladium.Content
{
    public class AnimationDeserializer
    {
        [JsonPropertyName("name")]
        public string name { get; set; }

        public Dictionary<string, AnimationDetails> animations { get; set; }

    }

    public class AnimationDetails
    {
        [JsonPropertyName("startframe")]
        public int startFrame { get; set; }

        [JsonPropertyName("numframes")]
        public int numFrames { get; set; }
    }
}