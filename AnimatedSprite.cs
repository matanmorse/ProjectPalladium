using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectPalladium
{
    public class AnimatedSprite
    {
        public Texture2D spriteTexture;
        public Rectangle sourceRect;
        ContentManager contentManger;
        private int currentFrame;
        private int spriteWidth;
        private int spriteHeight;

        public AnimatedSprite(ContentManager content, int currentFrame, int spriteWidth, int spriteHeight, string textureName)
        {
            this.contentManger = content;
            this.currentFrame = currentFrame;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            LoadTexture(textureName);
        }

        // Loads texture into memory
        public void LoadTexture(String textureName)
        {
            if (textureName == null) { return; }
            spriteTexture = contentManger.Load<Texture2D>(textureName);
            UpdateSourceRect();
        }

        // update the source on the spritesheet given a current frame of animation
        public void UpdateSourceRect()
        {
            sourceRect = new Rectangle(spriteWidth * currentFrame % spriteTexture.Width, currentFrame * spriteWidth / spriteTexture.Width * spriteHeight,
                spriteWidth, spriteHeight);
        }

        // draw the given sprite using sourceRect
        public void Draw(SpriteBatch b, Vector2 pos, float layerDepth)
        {
            b.Begin();
            Console.WriteLine(this.sourceRect);
            b.Draw(spriteTexture, pos, sourceRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth); ;
            b.End();

        }
    }
}
