using Microsoft.Xna.Framework;
using ProjectPalladium.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;
using ProjectPalladium.Utils;
using GameWorldTime = ProjectPalladium.GameManager.GameWorldTime;
using ProjectPalladium.UI;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectPalladium.Characters
{
    public class Villager : NPC
    {
        public class Schedule
        {
            [JsonPropertyName("name")]
            public string name { get; set; }

            [JsonPropertyName("schedule")]
            public Dictionary<string, ScheduleLocation> schedule { get; set; }
        }
        public class ScheduleLocation
        {
            [JsonPropertyName("position")]
            public int[] location { get; set;}

            [JsonPropertyName("map")]
            public string map { get; set;}
        }

        public struct ScheduleItem
        {
            public static ScheduleItem none = new ScheduleItem();
            public Vector2 location;
            public string mapName;
            public GameWorldTime time;
            public ScheduleItem(ScheduleLocation location, GameWorldTime time)
            {
                this.time = time;
                this.location = new Vector2(location.location[0], location.location[1]);
                this.mapName = location.map;
            }

            public override string ToString()
            {
                return "Map: " + mapName + "\nLocation: " + location + "\nTime: " + time;
            }
        }

        private Button interactButton;
        private LinkedList<ScheduleItem> schedule = new LinkedList<ScheduleItem>();
        private Schedule scheduleLoader;
        public string mapName;
        public ScheduleItem currentStop;
        public ScheduleItem nextStop;
        private const int interactDistacne = (int)(2 * Game1.scale);
        public Villager(string name) 
            : base(new AnimatedSprite(16, 32, "player/" + name + "anims", name),
                Vector2.Zero, name, SceneManager.CurScene.Map,
                new Vector2(-8, 0) * Game1.scale, new Vector2(16, 16) * Game1.scale)
        {
            this.Velocity = Vector2.Zero;

            interactButton = new Button(null, null, null, boundingBox.Location, boundingBox.Size, onRightClick:Interact);

            LoadSchedule();
        }

        private void Interact()
        {
            if (Game1.player.DialogueBoxOpen) return;
            int distx = (int) Math.Abs(pos.X - Game1.player.pos.X);
            int disty = (int)Math.Abs(pos.Y - Game1.player.pos.Y);
            if (distx > Map.scaledTileSize + interactDistacne || disty > Map.scaledTileSize + interactDistacne) return;
            UIManager.dialogBox.ShowDialog("It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout. The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here', making it look like readable English. Many desktop publishing packages and web page editors now use Lorem Ipsum as their default model text, and a search for 'lorem ipsum' will uncover many web sites still in their infancy. Various versions have evolved over the years, sometimes by accident, sometimes on purpose (injected humour and the like).\r\n\r\n");
        }
        private void LoadSchedule()
        {
            string jsonString = System.IO.File.ReadAllText("Content/npcs/" + name + ".json");
            scheduleLoader = JsonSerializer.Deserialize<Schedule>(jsonString);

            foreach (string key in scheduleLoader.schedule.Keys)
            {
                schedule.AddLast(new ScheduleItem(scheduleLoader.schedule[key], Util.ParseGameTimeString(key)));
            }
            GoToScheduleLocation(FindFirstLocation());

            
        }

        // find first schedule Item of appropriate time
        private ScheduleItem FindFirstLocation()
        {
            ScheduleItem prevItem = ScheduleItem.none;
            foreach (ScheduleItem item in schedule)
            {
                if (item.time > GameManager.time) break; // the previous item is the last one before the current time
                prevItem = item;
            }

            if (prevItem.Equals(ScheduleItem.none))
            {
                prevItem = schedule.First();
            }
            

            if (schedule.Find(prevItem).Next == null) nextStop = ScheduleItem.none;
            else nextStop = schedule.Find(prevItem).Next.Value;

            return prevItem;          
        }

        private void GoToScheduleLocation(ScheduleItem item)
        {
            this.pos = item.location * Game1.scale;
            this.mapName = item.mapName;
            this.currentStop = item;

            if (nextStop.Equals(ScheduleItem.none)) return;

            if (this.mapName != SceneManager.CurScene.Map.name) SceneManager.CurScene.Map.RemoveCharacter(this);

            if (schedule.Find(item) == null) return;
            if (schedule.Find(item).Next != null)
            {
                this.nextStop = schedule.Find(item).Next.Value;
            }
            else this.nextStop = ScheduleItem.none;

            SetFacingDir(Direction.up);
        }

        public virtual void UpdateOnGameTime()
        {
            if (nextStop.Equals(ScheduleItem.none)) return;
            if(nextStop.time >= GameManager.time)
            {
                GoToScheduleLocation(nextStop);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            interactButton.SetBounds(boundingBox);
            interactButton.Update(gameTime);
        }

        public override void Draw(SpriteBatch b)
        {
            base.Draw(b);
        }
    }
}
