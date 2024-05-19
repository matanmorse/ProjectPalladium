using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjectPalladium.Animation
{
    public class Animation
    {

        private string _name;
        public string Name { get { return _name; } set { _name = value; } }

        public int startFrame;

        public int numFrames;

        public int currentFrame;

        public float interval;

        public float timer = 0f;

        public AnimationFrame[] frames;
        private AnimatedSprite sprite;
        public struct AnimationFrame
        {
            public Rectangle sourceRect;
            public AnimationFrame(int frame, AnimatedSprite sprite)
            {
                int spriteWidth = sprite.spriteWidth;
                int spriteHeight = sprite.spriteHeight;
                Texture2D spriteTexture = sprite.spriteTexture;
                sourceRect = new Rectangle(spriteWidth * frame % spriteTexture.Width, frame * spriteWidth / spriteTexture.Width * spriteHeight,
                spriteWidth, spriteHeight);
            }

        }
        public Animation(string name, int startFrame, int numFrames, float interval, AnimatedSprite sprite)
        {
            Name = name;
            this.numFrames = numFrames;
            this.startFrame = startFrame;
            currentFrame = 0;
            this.interval = interval;
            this.sprite = sprite;

            frames = new AnimationFrame[numFrames + 1];

            // populate the frames array with the source rectangles of every frame of the animation
            for (int i = 0; i <= numFrames; i++)
            {
                frames[i] = new AnimationFrame(i + startFrame, sprite);
            }
        }

        /*  Resets the Animation back to its default parameters (i.e. the start of the animation) */
        public void Reset()
        {
            currentFrame = 0;
            timer = 0f;
        }
        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;


            if (timer > interval)
            {
                currentFrame = (currentFrame + 1) % frames.Length;
                timer = 0f;
                if (currentFrame > startFrame + numFrames)
                {
                    // loop to start of animation
                    currentFrame = startFrame;
                }
            }
        }

        public Rectangle getCurrentFrame()
        {
            return frames[currentFrame].sourceRect;
        }

        public override string ToString()
        {
            return "Animation: " + Name + " Starting at " + startFrame + " ends at " + (startFrame + numFrames);
        }
    }
}
