using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectPalladium
{
    public class GameManager
    {
        public struct GameWorldTime
        {
            private int hour;
            public int Hour
            {
                get { return hour; }
                set
                {
                    this.hour = value;
                }
            }
            private int minute;
            public int Minute
            {
                get { return minute; }
                set
                {
                    this.minute = value;
                }
            }
            public bool isAM;

            public GameWorldTime(int hour = 6, int minute = 0, bool isAM = true)
            {
                this.hour = hour;
                this.minute = minute;
                this.isAM = isAM;
            }

            public override string ToString()
            {
                return hour + ":" + (minute == 0 ? "00" : minute) + (isAM ? "AM" : "PM");   
            }
        }

        public static GameWorldTime time;
        private const float MILLIS_PER_TEN_GAMEMINUTES = 10000f; // ten seconds per in-game ten minutes
        private static float timer = 0f;
        public GameManager()
        {
            time = new GameWorldTime(hour: 6, minute: 0, isAM: true); // start at 6:00am
        }
        public static void Update(GameTime gameTime)
        {
            timer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > MILLIS_PER_TEN_GAMEMINUTES)
            {
                timer = 0;
                DoTenMinuteTick();
            }
        }

        public static void DoTenMinuteTick()
        {
            time.Minute += 10;
            if (time.Minute == 60)
            {
                time.Minute = 0;
                time.Hour++;

                if (time.Hour == 12) time.isAM = !time.isAM;

                if (time.Hour == 13)
                {
                    time.Hour = 1; 
                }
            }
            SceneManager.CurScene.Map.DoTenMinuteTick();
        }
    }
}
