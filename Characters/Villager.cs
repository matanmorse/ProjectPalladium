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
using System.ComponentModel;

namespace ProjectPalladium.Characters
{
    public class Villager : NPC
    {
        public class Info
        {
            [JsonPropertyName("name")]
            public string name { get; set; }

            [JsonPropertyName("schedule")]
            public Dictionary<string, ScheduleLocation> schedule { get; set; }

            [JsonPropertyName("dialogue")]
            public Dictionary<string, DialogueOption> dialogue { get; set; }

        }

        public class DialogueOption
        {
            [JsonPropertyName("requirements")]
            public Dictionary<string,string> requirements { get; set; }

            [JsonPropertyName("text")]
            public string[] text { get; set; }

            [JsonPropertyName("alwaysoffers")]
            public bool alwaysOffers { get; set; }

            [JsonPropertyName("priority")]
            public int priority { get; set; }

            [JsonPropertyName("onlysayonce")]
            public bool onlySayOnce { get; set; }

            [JsonPropertyName("active")]
            public bool active { get; set; }
            
        }
        public class ScheduleLocation
        {
            [JsonPropertyName("position")]
            public int[] location { get; set;}

            [JsonPropertyName("map")]
            public string map { get; set;}

            [JsonPropertyName("name")]
            public string name { get; set; }

        }

        public struct ScheduleItem
        {
            public static ScheduleItem none = new ScheduleItem();
            public Vector2 location;
            public string mapName;
            public GameWorldTime time;
            public string name;
            public ScheduleItem(ScheduleLocation location, GameWorldTime time)
            {
                this.time = time;
                this.location = new Vector2(location.location[0], location.location[1]);
                this.mapName = location.map;
                this.name = location.name;
            }

            public override string ToString()
            {
                return "Map: " + mapName + "\nLocation: " + location + "\nTime: " + time;
            }
        }

        private Random rand = new Random();
        private Button interactButton;
        private LinkedList<ScheduleItem> schedule = new LinkedList<ScheduleItem>();
        private Info info;
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

            // load deserializer
            string jsonString = System.IO.File.ReadAllText("Content/npcs/" + name + ".json");
            info = JsonSerializer.Deserialize<Info>(jsonString);

            LoadSchedule();
            LoadDialogue();
            
        }

        private void Interact()
        {
            if (Game1.player.DialogueBoxOpen) return;
            int distx = (int) Math.Abs(pos.X - Game1.player.pos.X);
            int disty = (int)Math.Abs(pos.Y - Game1.player.pos.Y);
            if (distx > Map.scaledTileSize + interactDistacne || disty > Map.scaledTileSize + interactDistacne) return;

            string[] dialogue = FindSuitableDialogueOption();
            if (dialogue.Length == 0) return;

            UIManager.dialogBox.ShowDialog(dialogue);
        }
        private void LoadSchedule()
        {

            foreach (string key in info.schedule.Keys)
            {
                schedule.AddLast(new ScheduleItem(info.schedule[key], Util.ParseGameTimeString(key)));
            }
            GoToScheduleLocation(FindFirstLocation());
        }

        private void LoadDialogue()
        {
            foreach (DialogueOption opt in info.dialogue.Values)
            {
                opt.active = true;
            }
        }

        private string[] FindSuitableDialogueOption()
        {
            List<DialogueOption> suitableOptions = new List<DialogueOption>(); // list of all options that satisfy the requirements
            Dictionary<string, DialogueOption> dialogueOptions = info.dialogue;
            
            foreach(string key in dialogueOptions.Keys)
            {
                DialogueOption option = dialogueOptions[key];

                if (!option.active) continue;
                if (option.requirements == null) { suitableOptions.Add(option); continue; }

                if (option.requirements.ContainsKey("stopName"))
                {
                    if (this.currentStop.name != option.requirements["stopName"]) continue;
                }

                if (option.requirements.ContainsKey("before"))
                {
                    if (GameManager.time > Util.ParseGameTimeString(option.requirements["before"])) continue;
                }

                if (option.requirements.ContainsKey("after"))
                {
                    if (GameManager.time < Util.ParseGameTimeString(option.requirements["after"])) continue;
                }

                if (option.requirements.ContainsKey("mapName"))
                {
                    if (this.currentStop.mapName != option.requirements["mapName"]) continue;
                }

                suitableOptions.Add(option);
            }

            if (suitableOptions.Count == 0) { return new string[0]; } // no suitable options

            DialogueOption chosenOption;
            // if there are suitable options that are always returned when eligible, return the one with the highest priority
            if (suitableOptions.Any(x => x.alwaysOffers))
            {
                List<DialogueOption> alwaysOffersOptions = suitableOptions.FindAll(x => x.alwaysOffers);
                int maxPrioValue = alwaysOffersOptions.Max(x => x.priority);
                chosenOption = alwaysOffersOptions.Find(x => x.priority == maxPrioValue);
            }
            else
            {
                // otherwise, just pick a random one
                chosenOption = suitableOptions[(rand.Next(0, suitableOptions.Count()))];
            }

            if (chosenOption.onlySayOnce) { chosenOption.active = false; }

            return chosenOption.text;
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
