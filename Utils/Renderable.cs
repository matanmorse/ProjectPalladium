using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace ProjectPalladium.Utils
{
    public class Renderable
    {
        public Point size;
        private Texture2D _texture;
        public Texture2D Texture { get { return _texture; } }
        private Rectangle sourceRect;

        private Vector2 zero = Vector2.Zero;

        public string textureName;
        public float rotation;

        public static Renderable empty = new Renderable(null, Rectangle.Empty);
        public Renderable(Texture2D texture, Rectangle sourceRect, float rotation = 0f)
        {
            this.rotation = rotation;
            _texture = texture;
            this.sourceRect = sourceRect;
            size = new Point(sourceRect.Width, sourceRect.Height);
        }

        public Renderable(string textureName, float rotation = 0f)
        {
            this.rotation = rotation;
            this.textureName = textureName;
            LoadTexture(textureName);
        }

        public void LoadTexture(string textureName)
        {
            if (textureName == null || textureName == "") { return; }
            _texture = Game1.contentManager.Load<Texture2D>(textureName);
            size = new Point(_texture.Width, _texture.Height);
            sourceRect = new Rectangle(new Point(0, 0), size);
        }

        public void Draw(SpriteBatch b, Vector2 pos, float opacity = 1f, float layer = 0f, float scale = 1f)
        {
            if (Texture == null) return;
            b.Draw(_texture, pos, sourceRect, Color.White * opacity, rotation, Vector2.Zero, scale, SpriteEffects.None, layer);
        }

        // draw renderable with custom origin
        public void Draw(SpriteBatch b, Vector2 pos, Vector2 origin, float layer = 0f, float opacity = 1f, float scale = 1f)
        {
            b.Draw(_texture, pos, sourceRect, Color.White * opacity, rotation, origin, scale, SpriteEffects.None, layer);
        }

       

    }
}
