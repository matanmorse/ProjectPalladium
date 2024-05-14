using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json.Linq;

namespace ProjectPalladium
{
    public class AnimatedSprite
    {
        public Texture2D spriteTexture;
        public Rectangle sourceRect;

        private Animation _animation;
        public Animation Animation { get { return _animation; } set { _animation = value; } }

        private int currentFrame;
        public int spriteWidth;
        public int spriteHeight;
        private float timer = 0;

        private String animationsRegistryName;
        
        private Rectangle defaultFrame;

        private List<Animation> animations = new List<Animation>();
        public List<Animation> Animations { get { return animations; } }
        public int CurrentFrame
        {
            get => currentFrame; 
            set
            {
                currentFrame = value;
            }
        }


        public AnimatedSprite(Texture2D texture, int spriteWidth, int spriteHeight, Rectangle sourceRect, string registryName)
        {
            this.spriteTexture = texture;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            this.sourceRect = sourceRect;
            initSprite(registryName);
        }

        public AnimatedSprite(Texture2D texture, int spriteWidth, int spriteHeight, string registryName)
        {
            spriteTexture = texture;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            initSprite(registryName);

        }
        public AnimatedSprite(int currentFrame, int spriteWidth, int spriteHeight, string textureName, string registryName)
        {
         
            this.currentFrame = currentFrame;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            initSprite(registryName);
            LoadTexture(textureName);
        }

        
        public void initSprite(string registryName)
        {
            defaultFrame = new Rectangle(0, 0, spriteWidth, spriteHeight);
            animationsRegistryName = registryName;
        }

        public void LoadContent()
        {
            string registryPath = "Content/" + animationsRegistryName + ".json";
            string json = System.IO.File.ReadAllText(registryPath);

            JObject data = JObject.Parse(json);
            JToken anims = data["animations"];
            foreach (JToken anim in anims.Values())
            {
                int startFrame = (int) anim["startframe"];
                int numFrames = (int) anim["numframes"];
                String name = ((JProperty)anim.Parent).Name;
                Debug.WriteLine(name);
                Animation newAnim = new Animation( name, startFrame, numFrames, 1000f, this); ;
                animations.Add(newAnim);
            }
            // Debug.WriteLine(animations[0]);
            _animation = animations[0];
            // Debug.WriteLine(_animation);
        }

        // Loads texture into memory
        public void LoadTexture(String textureName)
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
            b.Draw(spriteTexture, pos, _animation.getCurrentFrame(), Color.White, 0f, Vector2.Zero, Game1.scale, SpriteEffects.None, layerDepth);
        }

    }
}
