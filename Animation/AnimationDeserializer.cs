﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectPalladium.Animation
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

        [JsonPropertyName("intervals")]
        public float[] intervals { get; set; }

        [JsonPropertyName("locking")]
        public bool locking { get; set; }

        [JsonPropertyName("action")]
        public int actionFrame { get; set; }
    }
}