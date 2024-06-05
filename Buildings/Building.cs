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
using ProjectPalladium.UI;

namespace ProjectPalladium.Buildings
{

    public class Building : GameObject
    {
        private Button door;
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
            ColliderDetails doorInfo = info.colliders["door"];

            this.bounds = Util.makeRectFromPoints(bounds, globalPos);
            this.walkBehind = Util.makeRectFromPoints(walkBehind, globalPos);
            Rectangle doorBounds = Util.makeRectFromPoints(doorInfo, globalPos);

            door = new Button(null, null, null, doorBounds.Location, doorBounds.Size, onRightClick:EnterBuilding);
        }

        public void EnterBuilding()
        {
            if (!door.bounds.Intersects(Game1.player.boundingBox)) return; // check if player is standing over door
            SceneManager.ChangeScene(name); // the tmx file of the building is the same as the building name
        }

        public override void Draw(SpriteBatch b)
        {
            base.Draw(b);
        }
        public override void Update(GameTime gameTime)
        {
            door.Update();
            if (playerBehind) opacity = MathHelper.Clamp(opacity -= (float)(opacityDecayFactor * gameTime.ElapsedGameTime.TotalSeconds), 0.5f, 1f);
            else opacity = MathHelper.Clamp(opacity += (float)(opacityDecayFactor * gameTime.ElapsedGameTime.TotalSeconds), 0.5f, 1f);
        }


    }
}
