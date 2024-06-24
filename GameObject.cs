using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Buildings;
using System.Text.Json;
using ProjectPalladium.UI;
using System.Diagnostics;
namespace ProjectPalladium
{
    public class GameObject
    {
        protected float layer;
        public Rectangle bounds;
        public Rectangle walkBehind;
        public Renderable sprite;
        public Vector2 globalPos;

        public Point tileDimensions = Point.Zero;

        protected Vector2 localPos;

        public Vector2 LocalPos
        {
            get { return localPos; }
        }

        protected string name;


        protected bool playerBehind;
        public GameObject(string name, Vector2 pos, string textureName="") {
            this.name = name;
            sprite = new Renderable(textureName);


            if (sprite.Texture == null) { return;  }
            this.tileDimensions = new Point(sprite.Texture.Width / Map.tilesize, sprite.Texture.Height / Map.tilesize);
            this.localPos = new Vector2(pos.X, pos.Y ); // shift upward so that the origin of the object is bottom-left
            this.globalPos = Util.LocalPosToGlobalPos(localPos);
            layer = 0.1f + (Game1.LAYER_CONSTANT * (globalPos.Y + sprite.size.Y * Game1.scale)); // dynamic z-indexing based on y coordinates

            this.globalPos.Y -= (sprite.Texture.Height / 2) * Game1.scale;



            DeserializeJsonData(name);
        }

        protected float opacity = 1f;
        public bool PlayerBehind
        {
            get
            {
                return playerBehind;
            }
            set
            {
                playerBehind = value;
            }
        }


        public string GetJsonString(string jsonName)
        {
            jsonName = jsonName.ToLower().Replace(" ", "");

            string subFolder = this.GetType().Name.ToLower() + "s/";
            string registryPath = "Content/" + subFolder + jsonName + ".json";
            string jsonString = System.IO.File.ReadAllText(registryPath);
            return jsonString;
        }

        private void DeserializeJsonData(string jsonPath)
        {
            string jsonString = GetJsonString(jsonPath);
            BuildingDeserializer info = JsonSerializer.Deserialize<BuildingDeserializer>(jsonString);

            if (info.colliders.ContainsKey("move"))
            {
                ColliderDetails bounds = info.colliders["move"];
                this.bounds = Util.makeRectFromPoints(bounds, globalPos);
            }
            ColliderDetails walkBehind = info.colliders["behind"];

            this.walkBehind = Util.makeRectFromPoints(walkBehind, globalPos);

        }



        public virtual void Update(GameTime gameTime) {

        }
        public virtual void Draw(SpriteBatch b)
        {
            if (DebugParams.showObjectColliders)
            {
                Util.DrawRectangle(bounds, b); Util.DrawRectangle(walkBehind, b);
            }
            sprite.Draw(b, globalPos, opacity: opacity, layer: layer);
        }
    }
}
