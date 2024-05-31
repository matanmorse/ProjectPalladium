using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using System.Diagnostics;
using ProjectPalladium.Utils;

namespace ProjectPalladium.Buildings
{

    public class Building : GameObject
    {

        private float opacityDecayFactor = 1f;

        /* The name of the building (as defined by the "name" property of the .tmx file) is also the name of the json data file and the texture. */
        public Building(string name, Vector2 pos) : base(name, pos)
        {
            sprite = new Renderable(name);
            localPos = pos;

            globalPos = Util.LocalPosToGlobalPos(pos);
            DeserializeJsonData(name);

        }

        private void DeserializeJsonData(string jsonPath)
        {
            string jsonString = GetJsonString(jsonPath);
            BuildingDeserializer info = JsonSerializer.Deserialize<BuildingDeserializer>(jsonString);

            ColliderDetails bounds = info.colliders["move"];
            ColliderDetails walkBehind = info.colliders["behind"];

            this.bounds = Util.makeRectFromPoints(bounds, globalPos);
            this.walkBehind = Util.makeRectFromPoints(walkBehind, globalPos);
        }

        public override void Update(GameTime gameTime)
        {
            // Debug.WriteLine("building updating@ " + globalPos);
            if (playerBehind) opacity = MathHelper.Clamp(opacity -= (float)(opacityDecayFactor * gameTime.ElapsedGameTime.TotalSeconds), 0.5f, 1f);
            else opacity = MathHelper.Clamp(opacity += (float)(opacityDecayFactor * gameTime.ElapsedGameTime.TotalSeconds), 0.5f, 1f);
        }


    }
}
