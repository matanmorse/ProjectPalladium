using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ProjectPalladium.Stations;
using static ProjectPalladium.Map;
using static System.Formats.Asn1.AsnWriter;
using System.Transactions;
using ProjectPalladium.Animation;

namespace ProjectPalladium
{
    public class LightObject
    {
        public enum LightTypes
        {
            circle,
            rectangle,
        }

        Object owner;
        private LightTypes type;
        private Texture2D texture;
        public Vector2 pos;
        private float strength;
        private int size;

        public LightObject(Vector2 pos, LightTypes type, int size, float strength, Object owner)
        {
            this.pos = pos;
            this.strength = strength;
            this.type = type;
            this.owner = owner;
            this.size = size;
            if (type == LightTypes.circle)
            {
                AddCircleTexture(size);
            }
        }
        Texture2D AddCircleTexture(int radius)
        {
            int diameter = radius * 2;
             texture = new Texture2D(Game1.graphicsDevice, diameter, diameter);
            Color[] colorData = new Color[diameter * diameter];
            Color defaultColor = Color.White;
            Vector2 center = new Vector2(radius, radius);
            float maxDistance = radius;
            float maxIntensity = 1f;   
            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    Vector2 position = new Vector2(x, y);
                    float distance = Vector2.Distance(center, position);

                    if (distance <= maxDistance)
                    {
                        float intensity = MathHelper.Clamp((float)Math.Pow(1 - distance / maxDistance, 2), 0, 1);
                        colorData[y * diameter + x] = new Color(defaultColor.R, defaultColor.G, defaultColor.B, intensity);
                    }
                    else
                    {
                        colorData[y * diameter + x] = Color.Transparent;
                    }
                }
            }
            texture.SetData(colorData);
            return texture;
        }

        public void Update(GameTime gameTime)
        {
            // if this is attached to an attachable object, be sure to track it's position.
            if (owner is Character) { this.pos = ((owner as Character).boundingBox.Center.ToVector2() - new Vector2(size)); }
        }
        public void Draw(SpriteBatch b)
        {
            b.Draw(texture, pos, texture.Bounds, new Color(0, 30, 87), 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth:1f);
        }
    }
    public class Lightmap
    {
        private static List<LightObject> lightObjects = new List<LightObject>();
        public static ScreenShader shadow;

       
        public static void AddLightObject(LightObject lightObject)
        {
            lightObjects.Add(lightObject);
        }

        public static LightObject AddLightObject(Vector2 pos, LightObject.LightTypes type, int size, float strength, Object owner)
        {
            pos -= new Vector2(size / 2);
            LightObject obj = new LightObject(pos, type, size, strength, owner);
            lightObjects.Add(obj);
            return obj;
        }

        public static void Initialize()
        {
            shadow = new ScreenShader(Color.White);
        }

        public static void SetBrightnessWithoutTransition(float brightness)
        {
            shadow.transitioning = false; // if already in a transition, don't complete it
            brightness = Math.Clamp(brightness, 0f, 1f);
            shadow.SetAlpha(brightness);
        }
        public static void SetWorldBrightness(float brightness)
        {
            shadow.SetAlphaWithTransition(brightness, 0.025f);            
        }
        public static void Update(GameTime gameTime)
        {
            shadow.Update(gameTime);
            foreach (LightObject lightObject in lightObjects)
            {
                lightObject.Update(gameTime);
            }
        }

        public static void Draw(SpriteBatch b)
        {
            if (shadow.Brightness == 0f) return; // don't render lighting effects when at max brightness

            b.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            shadow.Draw(b);
            b.End();

            b.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, transformMatrix: Game1.translation);
            foreach (LightObject lightObject in lightObjects) lightObject.Draw(b);
            b.End();
        }
    }
}
