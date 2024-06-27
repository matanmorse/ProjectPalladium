using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public int CheckCharacterCollisions(Character character)
        {
            int numCollisions = 0;
            // since the characters' bounding box never actually collides, to detect hits we use a slightly larger bounding box
            Rectangle slightlyBiggerBounds = new Rectangle(character.boundingBox.Location - new Point(1), character.boundingBox.Size + new Point(2));
            // first, check collisions with the player
            if (!(character is Player))
            {
                if (slightlyBiggerBounds.Intersects(Player.boundingBox)) { numCollisions++; }
            }
            // next with all npcs
            foreach (Character character2 in this.Characters)
            {
                if (character2 == character) continue; 

                if (slightlyBiggerBounds.Intersects(character.boundingBox))
                {
                    numCollisions++;
                }
            }
            return numCollisions;
        }

        
    }
}
