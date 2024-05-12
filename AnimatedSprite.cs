using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public float scale = 4f;
        public Texture2D spriteTexture;
        public Rectangle sourceRect;
        ContentManager contentManger;
        private int currentFrame;
        public int spriteWidth;
        public int spriteHeight;
        private float timer = 0;
        public int CurrentFrame
        {
            get => currentFrame; 
            set
            {
                currentFrame = value;
                UpdateSourceRect();
            }
        }

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
            
            b.Begin(samplerState:SamplerState.PointClamp);
            b.Draw(spriteTexture, pos, sourceRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth); ;
            b.End();
        }

        public void Animate(GameTime gameTime, int startFrame, int numOfFrames, float interval)
        {
            if (currentFrame > startFrame + numOfFrames || currentFrame < startFrame) // if the frame of the sprite is out of the range of the animation, start animation over
            {
                Debug.WriteLine(currentFrame);
                CurrentFrame = startFrame;
            }

            timer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer > interval) {
                timer = 0;
                CurrentFrame++;

                if (currentFrame > startFrame + numOfFrames) {
                    // loop to start of animation
                    CurrentFrame = startFrame;
                }
            }
        }
    }
}
