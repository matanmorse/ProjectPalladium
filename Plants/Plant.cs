using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium.Utils;
using Microsoft.Xna.Framework;
using ProjectPalladium.Animation;
using ProjectPalladium.Buildings;
using System.Text.Json;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Items;
using ProjectPalladium;
using static ProjectPalladium.Map;

namespace ProjectPalladium.Plants
{
    public class Plant : GameObject
    {
        public AnimatedSprite animatedSprite;
        public Point SPRITE_SIZE = new Point(16, 32);

        private int growthStage = 0;
        public int GrowthStage
        {
            get { return growthStage; }
            set { growthStage = Math.Clamp(value, 0, totalGrowthStages - 1); }
        }
        public int totalGrowthStages;



        private const int GROWTH_INTERVAL = 30; // in-game minutes between growth stages

        public List<Renderable> sprites = new List<Renderable>();

        private int timeSinceGrowth = 0; // time since last growth in minutes
        public int TimeSinceLastGrowth
        {
            get { return timeSinceGrowth; }
        }

        private GameManager.GameWorldTime timeOfLastGrowth;
      
        public class PlantDeserializer
        {
            public string name { get; set; }
            public int growthstages { get; set; }

            public int width { get; set; }
            public int height { get; set; } 

            public Dictionary<string, ColliderDetails> colliders { get; set; }
        }

        public Plant(PlantSerialized pobj) : base(pobj.name, new Vector2(pobj.x, pobj.y))
        {
            this.name = pobj.name;
            this.globalPos = new Vector2(pobj.x, pobj.y);
            this.localPos = Util.GlobalPosToLocalPos(globalPos) + new Vector2(0, 1);
            this.growthStage = pobj.growthStage;

            this.timeSinceGrowth = pobj.timeSinceLastGrowth;
            this.timeOfLastGrowth = GameManager.time;

            sprite = new Renderable(name);
            DeserializeJsonData(name);
        }

        public Plant(string name, Vector2 tilePos) : base(name, tilePos)
        {
            this.localPos = tilePos;
            this.globalPos = Util.LocalPosToGlobalPos(tilePos);

            // division constant is 1.5f so player is behind plant at approx plant stem height
            layer = 0.1f + (Game1.LAYER_CONSTANT * (globalPos.Y + ((SPRITE_SIZE.Y / 1.25f) * Game1.scale))); // dynamic z-indexing based on y coordinates
            this.globalPos.Y -= Map.scaledTileSize; // need to be here to render correctly

            this.timeOfLastGrowth = GameManager.time;

            sprite = new Renderable(name);
            DeserializeJsonData(name);
        }

        public void DeserializeJsonData(string jsonName)
        {
            string jsonString = GetJsonString(jsonName);
            PlantDeserializer info = JsonSerializer.Deserialize<PlantDeserializer>(jsonString);

            this.name = info.name;
            if (info.colliders.ContainsKey("move"))
            {
                ColliderDetails bounds = info.colliders["move"];
                this.bounds = Util.makeRectFromPoints(bounds, globalPos);
            }
            else
            {
                this.bounds = Rectangle.Empty;
            }

            if (info.colliders.ContainsKey("behind"))
            {
                ColliderDetails walkBehind = info.colliders["behind"];
                this.walkBehind = Util.makeRectFromPoints(walkBehind, globalPos);
            }
            else
            {
                this.walkBehind = Rectangle.Empty;
            }

            this.totalGrowthStages = info.growthstages;
            ExtractSprites();
        }

        public void ExtractSprites()
        {
            int spriteWidth = SPRITE_SIZE.X;
            int spriteHeight = SPRITE_SIZE.Y;

            for (int i = 0; i < totalGrowthStages; i++)
            {
                int cols = sprite.Texture.Width / spriteWidth;
                int rows = sprite.Texture.Height / spriteHeight;

                int col = i % cols;
                int row = i / cols;
                Rectangle sourceRect = new Rectangle(col * spriteWidth, row * spriteHeight, spriteWidth, spriteHeight);

                sprites.Add(new Renderable(sprite.Texture, sourceRect));
            }

        }

        public void UpdateOnGameTime()
        {
            timeSinceGrowth = Util.CalculateMinutesDifference(GameManager.time, timeOfLastGrowth);
            while (timeSinceGrowth >= GROWTH_INTERVAL)
            {
                timeSinceGrowth -= GROWTH_INTERVAL;
                GrowthStage++;
                timeOfLastGrowth = GameManager.time;
            }
        }
        public void Harvest()
        {
            if (growthStage != totalGrowthStages - 1) return;
            Game1.player.inventory.AddItem(Item.GetItemFromRegistry("ectoplasmic gem"), 5);
            SceneManager.CurScene.Map.RemovePlant(localPos);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch b)
        {
            sprites[growthStage].Draw(b, globalPos, layer:layer);
        }
    }
}
