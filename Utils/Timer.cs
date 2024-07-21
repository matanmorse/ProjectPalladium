using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProjectPalladium.Utils
{
    public class Timer
    {
        private float startTime;
        public float time;

        private List<Timer> stack;
        public delegate void Callback();

        Callback callback;
        public Timer(float startTime, float time, Callback callback, List<Timer> stack)
        {
            this.startTime = startTime;
            this.time = time;
            this.callback = callback;
            this.stack = stack;
        }

        public void Update(GameTime gameTime)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (time < 0)
            {
                callback.Invoke();
                return;
            }
        }

        public static void TimerUpdates(List<Timer> timers)
        {
            List<Timer> expiredTimers = new List<Timer>();
            foreach (Timer t in timers)
            {
                if (t.time < 0) expiredTimers.Add(t);
            }
            foreach(Timer t in expiredTimers)
            {
                timers.Remove(t);
            }
        }
    }
}
