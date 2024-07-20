using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using ProjectPalladium.Plants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Timer = ProjectPalladium.Utils.Timer;
namespace ProjectPalladium
{
    public class GameManager
    {

        public static bool paused = false; // is the whole game paused

        // generic Timer Manager
        public static List<Timer> timersToAdd = new List<Timer>();
        public class TimerManager
        {
            public static List<Timer> timers = new List<Timer>();
            public TimerManager() { }
            public static void Update(GameTime gameTime)
            {
                foreach (Timer timer in timers)
                {
                    timer.Update(gameTime);
                }
                Timer.TimerUpdates(timers);
                foreach (Timer t in timersToAdd)
                {
                    timers.Add(t);
                }
                timersToAdd.Clear();
            }

            public static void AddTimer(Timer.Callback onStart, Timer.Callback callback, float interval)
            {
                onStart.Invoke();
                AddTimer(callback, interval);
            }

            public static void AddTimer(Timer.Callback callback, float interval)
            {
                timersToAdd.Add(new Timer(0f, interval, callback, timers));
            }
        }

        public struct GameWorldTime
        {
            public bool paused = false; // is the game time paused

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
                    FixTime();
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
        private const float MILLIS_PER_TEN_GAMEMINUTES = 5000f; // ten seconds per in-game ten minutes
        private static float timer = 0f;
        private const int MINUTES_IN_HOUR = 60;
        public GameManager()
        {
            time = new GameWorldTime(hour: 6, minute: 0, isAM: true); // start at 6:00am
        }
        public static void Update(GameTime gameTime)
        {
            TimerManager.Update(gameTime);
            timer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;


            if (timer > MILLIS_PER_TEN_GAMEMINUTES && !time.paused && !paused)
            {
                timer = 0;
                DoTenMinuteTick();
            }
        }

        public static void DoTenMinuteTick()
        {
            time.Minute += 10;
            
            SceneManager.CurScene.Map.UpdateOnGameTime();

            // make sure hollow is getting updated too  
            if (SceneManager.CurScene.Map != SceneManager.hollow) 
            {
                SceneManager.hollow.UpdateOnGameTime();
            }
        }

        public static void FixTime()
        {
            if (time.Minute >= MINUTES_IN_HOUR)
            {
                int remainder = time.Minute - MINUTES_IN_HOUR;
                time.Minute = 0;
                time.Hour++;
                if (time.Hour == 12) time.isAM = !time.isAM;
                if (time.Hour >= 13) time.Hour = 1;


                while (remainder > 0)
                {
                    time.Hour++;
                    if (time.Hour == 12) time.isAM = !time.isAM;
                    if (time.Hour >= 13) time.Hour = 1;

                    remainder -= MINUTES_IN_HOUR;
                }
                if (remainder > 0) { time.Minute += remainder; }
            }
        }
    }
}
