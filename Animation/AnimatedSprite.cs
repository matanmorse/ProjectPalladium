using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ProjectPalladium.Animation
{
    public class AnimatedSprite
    {
        public Texture2D spriteTexture;
        public Rectangle sourceRect;
        private Vector2 origin;

        private Animation _animation;
        public Animation Animation
        {
            get { return _animation; }
            set
            {
                _animation = value;
            }
        }

        private int currentFrame;
        public int spriteWidth;
        public int spriteHeight;
        private float timer = 0;

        public int scaledWidth;
        public int scaledHeight;

        private string animationsRegistryName;

        private Rectangle defaultFrame;

        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        public Dictionary<string, Animation> Animations { get { return animations; } }
        public int CurrentFrame
        {
            get => currentFrame;
            set
            {
                currentFrame = value;
            }
        }


        public AnimatedSprite(int spriteWidth, int spriteHeight, string textureName, string registryName)
        {
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;

            scaledWidth = (int)(spriteWidth * Game1.scale);
            scaledHeight = (int)(spriteHeight * Game1.scale);

            origin = new Vector2(spriteWidth / 2, spriteHeight / 2);
            initSprite(registryName);
            LoadTexture(textureName);
        }


        public void AnimationChangeDetected()
        {
            if (_animation == null) { return; }
            _animation.Reset();
        }
        public void initSprite(string registryName)
        {
            defaultFrame = new Rectangle(0, 0, spriteWidth, spriteHeight);
            animationsRegistryName = registryName;
        }

        public void LoadContent()
        {
            // open the animation metadata json
            string registryPath = "Content/" + animationsRegistryName + ".json";
            string jsonString = File.ReadAllText(registryPath);

            AnimationDeserializer dsrlzdAnimData = JsonSerializer.Deserialize<AnimationDeserializer>(jsonString);
            foreach (var anim in dsrlzdAnimData.animations)
            {
                animations.Add(anim.Key, new Animation(anim.Key, anim.Value.startFrame, anim.Value.numFrames, anim.Value.intervals, this));
            }

            // start on the idle animation
            _animation = animations["idle"];
        }

        // Loads texture into memory
        public void LoadTexture(string textureName)
        {
            if (textureName == null) { return; }
            spriteTexture = Game1.contentManager.Load<Texture2D>(textureName);
        }


        public void Update(GameTime gameTime)
        {
            _animation.Update(gameTime);
        }

        // draw the given sprite using sourceRect
        public void Draw(SpriteBatch b, Vector2 pos, float layerDepth)
        {
            if (_animation == null) { return; }
            b.Draw(spriteTexture, pos, _animation.getCurrentFrame(), Color.White, 0f, origin, Game1.scale, SpriteEffects.None, layerDepth);
        }
        public void Draw(SpriteBatch b, Vector2 pos, bool flipped=false, float layerDepth = 1f)
        {
            pos = new Vector2(MathF.Floor(pos.X), MathF.Floor(pos.Y));
            if (_animation == null) { return; }
            if (flipped)
            {
                b.Draw(spriteTexture, pos, _animation.getCurrentFrame(), Color.White, 0f, origin, Game1.scale, SpriteEffects.FlipHorizontally, layerDepth);
            }
            else
            {
                b.Draw(spriteTexture, pos, _animation.getCurrentFrame(), Color.White, 0f, origin, Game1.scale, SpriteEffects.None, layerDepth);
            }
        }

    }
}
