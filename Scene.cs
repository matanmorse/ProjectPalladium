using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium
{
    internal class Scene
    {
        public Map Map { get; }
        public Player Player { get; }
        public List<Character> Characters { get; }
        public struct Metadata
        {
            public Vector2 spawnLocation { get; set; }
        }
        public Metadata metadata { get; }

        public Scene(Map map, Player player, Metadata metadata, List<Character> characters = null)
        {
            this.Map = map;
            this.Player = player;
            this.Characters = characters == null ? new List<Character>() : characters;
            this.metadata = metadata ;
        }

    }
}
