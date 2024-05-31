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
namespace ProjectPalladium
{
    public class GameObject
    {
        protected float layer;
        public Rectangle bounds;
        public Rectangle walkBehind;
        protected Renderable sprite;
        protected Vector2 globalPos;
        
        protected Vector2 localPos;
        public Vector2 LocalPos
        {
            get { return localPos; }
        }

        protected bool playerBehind;
        public GameObject(string name, Vector2 pos) {
        layer = Game1.layers.buildings + 0.01f * pos.Y; // dynamic z-indexing based on y coordinates
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
                if (value)
                {
                    layer = Game1.layers.player + 0.1f;
                }
                else
                {
                    layer = Game1.layers.buildings + 0.01f * localPos.Y;
                }
                playerBehind = value;
            }
        }


        public string GetJsonString(string jsonName)
        {
            string registryPath = "Content/" + jsonName + ".json";
            string jsonString = System.IO.File.ReadAllText(registryPath);
            return jsonString;
        }


        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch b)
        {
            if (DebugParams.showColliders)
            {
                Util.DrawRectangle(bounds, b); Util.DrawRectangle(walkBehind, b);
            }
            sprite.Draw(b, globalPos, opacity: opacity, layer: layer);
        }
    }
}
