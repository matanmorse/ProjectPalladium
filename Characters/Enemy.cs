using Microsoft.Xna.Framework;
using ProjectPalladium.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Characters
{
    public class Enemy : NPC
    {
        public int health = 10;
        public Enemy(AnimatedSprite sprite, Vector2 pos, string name, Map startingMap, Rectangle boundingBox) : base(sprite, pos, name, startingMap, boundingBox)
        {
        }
        
        public void Update()
        {

        }
    }
}
