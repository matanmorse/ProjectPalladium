using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectPalladium
{
    public class Renderable
    {
        public Vector2 pos;
        

        private Vector2 size;

        public Vector2 Size
        {
            get { return Size; }
            set
            {
                if (value.X >= 1 && value.Y >= 1)
                {
                    this.size = value;
                }
            }
        }

        public String textureName;
        public Renderable(Vector2 pos, Vector2 size, String textureName)
        {
            this.pos = pos;
            this.Size = size;
            this.textureName = textureName;
        }

  


    }
}
