using Microsoft.Xna.Framework;
using ProjectPalladium.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;
using ProjectPalladium.Utils;
using GameWorldTime = ProjectPalladium.GameManager.GameWorldTime;

namespace ProjectPalladium.Characters
{
    public class Villager : NPC
    {
        LinkedList<ScheduleItem> schedule = new LinkedList<ScheduleItem>();
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

        Schedule scheduleLoader;
        public Villager(string name) 
            : base(new AnimatedSprite(16, 32, "player/" + name + "anims", name),
                Vector2.Zero, name, SceneManager.CurScene.Map,
                new Vector2(-8, 0) * Game1.scale, new Vector2(16, 16) * Game1.scale)
        {
            this.pos = new Vector2(100,100) * Game1.scale;
            this.Velocity = Vector2.Zero;

            LoadSchedule();
        }

        private void LoadSchedule()
        {
            string jsonString = System.IO.File.ReadAllText("Content/npcs/" + name + ".json");
            scheduleLoader = JsonSerializer.Deserialize<Schedule>(jsonString);

            foreach (string key in scheduleLoader.schedule.Keys)
            {
                schedule.AddLast(new ScheduleItem(scheduleLoader.schedule[key], Util.ParseGameTimeString(key)));
            }

            foreach(ScheduleItem item in  schedule)
            {
                // Debug.WriteLine(item);
            }

            FindFirstLocation();
        }

        private ScheduleItem FindFirstLocation()
        {
            foreach (ScheduleItem item in schedule)
            {
                // if (item.time < GameManager.time) ;
            }

            GameWorldTime startTime = new GameWorldTime(6, 30, true);
            GameWorldTime endTime = new GameWorldTime(2, 30, true);

            Debug.WriteLine(startTime < endTime);
            return new ScheduleItem();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            SetFacingDir(Direction.left);
        }
    }
}
