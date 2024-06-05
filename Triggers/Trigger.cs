using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectPalladium.Utils
{
    public class Trigger
    {
        Rectangle bounds;
        public delegate void onEnterFunc();
        public onEnterFunc onEnter;
        public string name;

        public Trigger (string name, Rectangle bounds, onEnterFunc onEnter)
        {
            this.name = name;
            this.bounds = bounds;
            this.onEnter = onEnter;
        }

        public void CheckEnter()
        {
            if (bounds.Contains(SceneManager.CurScene.Player.pos)) 
            {

                onEnter.DynamicInvoke();
            }
        }
    }
}
