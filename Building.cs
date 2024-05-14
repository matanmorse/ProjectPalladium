using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace ProjectPalladium
{

    public class Building
    {
        private Renderable sprite;
       

        public Rectangle bounds;
        private Vector2 globalPos;

        public Building(string textureName, Vector2 pos) 
        {
            this.sprite = new Renderable(textureName);
            this.globalPos = pos;
            this.bounds = new Rectangle(new Point((int) globalPos.X, (int) globalPos.Y), new Point((int)(sprite.size.X * Game1.scale), (int) (sprite.size.Y * Game1.scale)));
        }

        public void Draw(SpriteBatch b)
        {
            sprite.Draw(b, globalPos);
        }
    }
}
