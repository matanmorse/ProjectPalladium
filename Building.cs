using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium;
using System.Text.Json;
using System.Diagnostics;

namespace ProjectPalladium
{

    public class Building
    {
        private Renderable sprite;

        private float opacityDecayFactor = 1f;
        private float opacity = 1f;
        private float layer = Game1.layers.buildings;
        public Rectangle bounds;
        public Rectangle walkBehind;

        private Vector2 globalPos;

        private Vector2 localPos;

        private bool playerBehind;
        public bool PlayerBehind
        {
            get
            {
                return playerBehind;
            }
            set {
                if (value)
                {
                    layer = 1f;
                }
                else
                {
                    layer = 0.1f;
                }
                playerBehind = value; 
            }
        }

        /* The name of the building (as defined by the "name" property of the .tmx file) is also the name of the json data file and the texture. */
        public Building(string name, Vector2 pos) 
        {


            this.sprite = new Renderable(name);
            this.localPos = pos;

            this.globalPos = Util.LocalPosToGlobalPos(pos);
            DeserializeJsonData(name);

        }

        private void DeserializeJsonData(string jsonPath)
        {
            string registryPath = "Content/" + jsonPath + ".json";
            string jsonString = System.IO.File.ReadAllText(registryPath);
            
            BuildingDeserializer info = JsonSerializer.Deserialize<BuildingDeserializer>(jsonString);

            foreach (var collider in info.colliders)
            {
                Debug.WriteLine(collider.Key);
            }
            ColliderDetails bounds = info.colliders["move"];
            ColliderDetails walkBehind = info.colliders["behind"];
            this.bounds = Util.makeRectFromPoints(bounds, globalPos);
            this.walkBehind = Util.makeRectFromPoints(walkBehind, globalPos);

        }

        public void Update(GameTime gameTime)
        {
            // Debug.WriteLine("building updating@ " + globalPos);
            if (playerBehind) opacity = MathHelper.Clamp(opacity -= (float) (opacityDecayFactor * gameTime.ElapsedGameTime.TotalSeconds), 0.5f, 1f);
            else opacity = MathHelper.Clamp(opacity += (float)(opacityDecayFactor * gameTime.ElapsedGameTime.TotalSeconds), 0.5f, 1f);
        }
        public void Draw(SpriteBatch b)
        {
            if (DebugParams.showColliders)
            {
                Util.DrawRectangle(bounds, b); Util.DrawRectangle(walkBehind, b);
            }
            sprite.Draw(b, globalPos, opacity:opacity, layer:layer);
        }

        
    }
}
