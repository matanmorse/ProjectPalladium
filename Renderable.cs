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
        private Texture2D _texture;
        private Rectangle sourceRect;
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
            LoadTexture(textureName);
        }
        public Renderable(Texture2D texture, Vector2 pos)
        {
            this.pos = pos;
            this._texture = texture;
        }

        public Renderable(Texture2D texture, Rectangle sourceRect)
        {
            this._texture = texture;
            this.sourceRect = sourceRect;
        }

        public void LoadTexture(String textureName)
        {
            if (textureName == null) { return; }
            _texture = Game1.contentManager.Load<Texture2D>(textureName);
        }

        public void Draw(SpriteBatch b, Vector2 pos)
        {
            b.Draw(_texture, pos, sourceRect, Color.White, 0f, Vector2.Zero, Game1.scale, SpriteEffects.None, 0f);
        }


    }
}
