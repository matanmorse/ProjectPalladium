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
        public Point size;
        private Texture2D _texture;
        private Rectangle sourceRect;
       

        public string textureName;
       

        public Renderable(Texture2D texture, Rectangle sourceRect)
        {
            this._texture = texture;
            this.sourceRect = sourceRect;
            size = new Point(sourceRect.Width, sourceRect.Height);
        }

        public Renderable (string textureName)
        {
            this.textureName = textureName;
            LoadTexture(textureName);
        }

        public void LoadTexture(String textureName)
        {
            if (textureName == null) { return; }
            _texture = Game1.contentManager.Load<Texture2D>(textureName);
            size = new Point(_texture.Width, _texture.Height);
            sourceRect = new Rectangle(new Point (0,0) , size);
        }

        public void Draw(SpriteBatch b, Vector2 pos)
        {
            b.Draw(_texture, pos, sourceRect, Color.White, 0f, Vector2.Zero, Game1.scale, SpriteEffects.None, 0f);
        }


    }
}
