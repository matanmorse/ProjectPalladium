using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using ProjectPalladium.UI;
using System.Runtime.CompilerServices;
using ProjectPalladium.Spells;
using static ProjectPalladium.Spells.Spell;
using static ProjectPalladium.Utils.Timer.Callback;
using System.Reflection;
using System.Data.SqlTypes;
using ProjectPalladium.Tools;
using System.Threading;
using ProjectPalladium.Utils;
using Timer = ProjectPalladium.Utils.Timer;

namespace ProjectPalladium.Animation
{
    public class AnimatedSprite
    {
        public Texture2D spriteTexture;
        public Vector2 origin;

        public List<Timer> timers = new List<Timer>();

        public bool doingSyncedAnimation;

        private Character owner;
        public Character Owner { get { return owner; } set { this.owner = value; } }  

        private Animation _animation;
        public Animation Animation
        {
            get { return _animation; }
            set
            {
                _animation = value;
            }
        }

        public ToolSprite toolSprite;

        public bool playingOnce;

        private int currentFrame;
        public int spriteWidth;
        public int spriteHeight;

        // the function to invoke when the animation is finished, if any
        public Delegate onAnimationFinished;

        public int scaledWidth;
        public int scaledHeight;

        private string animationsRegistryName;

        private Rectangle defaultFrame;

        private bool animationLocked;
        public bool AnimationLocked
        {
            get { return animationLocked; }
            set
            {
                this.animationLocked = value;
                owner.MovementLocked = value; 
            }
        }

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

        public void AddTimer(Timer.Callback callback, float time )
        {
            timers.Add(new Timer(0f, time, callback, timers));
        }

        public void AddTimer(Timer.Callback doNow, Timer.Callback callback, float time)
        {
            doNow.Invoke();
            timers.Add(new Timer(0f, time, callback, timers));
        }


        public void DoToolAnimation ()
        {
            toolSprite = ((owner as Player).ActiveItem as Tool).sprite;

            doingSyncedAnimation = true;
            toolSprite.DoToolAnimation();
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

        public void changeAnimation (string animationName)
        {
            if (AnimationLocked) return;
            if (_animation != animations[animationName])
            {
                _animation = animations[animationName];
                AnimationLocked = _animation.locking;

                AnimationChangeDetected();
            }
        }

        public void PlayAnimationOnce<T>(string animationName, T onFinish) where T: Delegate
        {
            onAnimationFinished = onFinish;
            playingOnce = true;
            changeAnimation(animationName);
        }

        // called when PlayAnimationOnce concludes
        public void AnimationEnded() {

            if (doingSyncedAnimation)
            {
                toolSprite.EndAnim();
                doingSyncedAnimation = false;
            }

            playingOnce = false;
            if (animationLocked) animationLocked = false;

            // TODO: add more advanced logic for what animation to return to.
            changeAnimation("idle");
        }

        public void DoAnimationAction (GameTime gameTime)
        {
            // if behavior is given, invoke end-of-animation behavior
            if (onAnimationFinished != null)
            {
                onAnimationFinished.DynamicInvoke();
                onAnimationFinished = null;
            }
        }
        public void LoadContent()
        {
            // open the animation metadata json
            string registryPath = "Content/animations/" + animationsRegistryName + ".json";
            string jsonString = File.ReadAllText(registryPath);

            AnimationDeserializer dsrlzdAnimData = JsonSerializer.Deserialize<AnimationDeserializer>(jsonString);
            foreach (var anim in dsrlzdAnimData.animations)
            {
                animations.Add(anim.Key, new Animation(anim.Key, anim.Value.startFrame, anim.Value.numFrames, anim.Value.intervals, this, anim.Value.locking, anim.Value.actionFrame));
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
            foreach(Timer t in timers) { t.Update(gameTime); }
            Timer.TimerUpdates(timers);

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
        public void Draw(SpriteBatch b, Vector2 pos, Color color, bool flipped = false, float layerDepth = 1f)
        {
            pos = new Vector2(MathF.Floor(pos.X), MathF.Floor(pos.Y));
            if (_animation == null) { return; }
            if (flipped)
            {
                b.Draw(spriteTexture, pos, _animation.getCurrentFrame(), color, 0f, origin, Game1.scale, SpriteEffects.FlipHorizontally, layerDepth);
            }
            else
            {
                b.Draw(spriteTexture, pos, _animation.getCurrentFrame(), color, 0f, origin, Game1.scale, SpriteEffects.None, layerDepth);
            }
        }

    }
}
