using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium
{
    internal class Scene
    {
        public Map Map { get; set; }
        public Player Player { get; set; }
        public List<Character> Characters { get; set; }
       

        public Scene(Map map, Player player, List<Character> characters = null)
        {
            this.Map = map;
            this.Player = player;
            this.Characters = characters == null ? new List<Character>() : characters;
        }

        public override string ToString()
        {
            return Map.ToString();
        }
    }
}
