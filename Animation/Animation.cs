using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectPalladium.Animation
{
    public class Animation
    {

        private string _name;
        public string Name { get { return _name; } set { _name = value; } }

        public int startFrame;

        public int numFrames;

        public int currentFrame;

        public float[] intervals;

        public float timer = 0f;

        // if an action is to be performed during the animaiton, what frame to do it on.
        public int actionFrame;

        public AnimationFrame[] frames;
        private AnimatedSprite sprite;

        public bool locking; // is the sprite movement/animation locked during this animation?
        public struct AnimationFrame
        {
            public Rectangle sourceRect;
            public float delay;
            public AnimationFrame(int frame, AnimatedSprite sprite, float frameDelay=1000f)
            {
                int spriteWidth = sprite.spriteWidth;
                int spriteHeight = sprite.spriteHeight;
                Texture2D spriteTexture = sprite.spriteTexture;
                sourceRect = new Rectangle(spriteWidth * frame % spriteTexture.Width, frame * spriteWidth / spriteTexture.Width * spriteHeight,
                
                    
                spriteWidth, spriteHeight);
                delay = frameDelay;
            }

        }
        public Animation(string name, int startFrame, int numFrames, float[] intervals, AnimatedSprite sprite, bool locking, int actionFrame)
        {
            Name = name;
            this.numFrames = numFrames;
            this.startFrame = startFrame;
            currentFrame = 0;
            this.intervals = intervals;
            this.sprite = sprite;
            this.locking = locking;
            this.actionFrame = actionFrame;


            frames = new AnimationFrame[numFrames + 1];


            // populate the frames array with the source rectangles of every frame of the animation
            for (int i = 0; i <= numFrames; i++)
            {
                frames[i] = new AnimationFrame(i + startFrame, sprite, intervals[i]);
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


            if (timer > frames[currentFrame].delay)
            {
               
                // if we're at the end of an animation that only plays once, end the animation
                if ( (currentFrame + 1 >= frames.Length) && sprite.playingOnce) {
                    sprite.AnimationEnded();
                    return;
                }
                
                currentFrame = (currentFrame + 1) % frames.Length;
                timer = 0f;

                if (sprite.doingSyncedAnimation)
                {
                    sprite.toolSprite.NextFrame();
                }
                // if we're on the action frame, perform the action (after increment so perform at start of frame, not end)
                if (currentFrame == actionFrame && actionFrame != 0)
                {
                    sprite.DoAnimationAction(gameTime);
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
