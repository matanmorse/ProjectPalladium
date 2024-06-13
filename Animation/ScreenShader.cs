using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Animation
{
    /* Controls logic for screen brightness for scene transitions, etc. */
    public  class ScreenShader
    {
        private static Texture2D overlayTexture = new Texture2D(Game1.graphicsDevice, 1, 1);
        private Color overlayColor = new Color(0, 0, 0, 0.0f); // Black with 50% opacity
        private float t = 0f;
        private float alphaValue;
        private ShaderEffect currentEffect;
        public event Action onFinishEffect;

        private struct ShaderEffect
        {
            public float totalMillis;
            public string name;
            public ShaderEffect(string name, float totalMillis)
            {
                this.name = name;
                this.totalMillis = totalMillis;
            }
        }

        private static Dictionary<string, ShaderEffect> effects = new Dictionary<string, ShaderEffect>()
        {
            {"none", new ShaderEffect("none", 0f) },
            {"scenetransition", new ShaderEffect("scenetransition", 500f) },
            {"enteringscene", new ShaderEffect("enteringscene", 500f) }
        };

        public ScreenShader() {
            overlayTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            if (currentEffect.name == "none") return;
            switch(currentEffect.name)
            {
                case "scenetransition": DoSceneTransition(gameTime); break;
                case "enteringscene": DoEnterScene(gameTime); break;
            }


            UpdateAlphaValue();
        }

        public void DoSceneTransition()
        {
            Game1.player.Halt();
            Game1.player.MovementLocked = true; // lock the player's movement
            currentEffect = effects["scenetransition"];
        }

        public void DoEnterScene()
        {
            Game1.player.Halt();
            Game1.player.MovementLocked = true; // lock the player's movement
            currentEffect = effects["enteringscene"];
        }

        public void DoSceneTransition(GameTime gameTime)
        {
           
            t += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            alphaValue = t / currentEffect.totalMillis; // get the normalized alpha value
            if (t > currentEffect.totalMillis)
            {
                onFinishEffect.Invoke(); // do whatever at the end of the effect
                ResetEffect();
                DoEnterScene();
            }
        }

        public void DoEnterScene(GameTime gameTime)
        {

            t += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float normalized = t / currentEffect.totalMillis; // get the normalized  value
            alphaValue = 1f - normalized;

            if (t > currentEffect.totalMillis)
            {
                ResetEffect();
            }
        }

        private void ResetEffect()
        {
            t = 0f;
            UpdateAlphaValue();
            currentEffect = effects["none"];
            Game1.player.MovementLocked = false; // unlock player's movement at end of effect
        }

        private void UpdateAlphaValue()
        {
            overlayColor = new Color(0, 0, 0, alphaValue);
        }

        public void Draw(SpriteBatch b)
        {
            b.Draw(overlayTexture, new Rectangle(0, 0, Game1.graphicsDevice.Viewport.Width, Game1.graphicsDevice.Viewport.Height), overlayColor);
        }
    }
}
