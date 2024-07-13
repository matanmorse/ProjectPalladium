using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Effects
{
    public interface IPotionEffect
    {
        void ApplyEffect(float strength, int duration);
    }

    public class RestoreManaEffect : IPotionEffect
    {
        public void ApplyEffect(float strength, int duration) 
        {
            Debug.WriteLine("restoring mana!");
            Game1.player.Mana += (int)strength;
        }
    }
}
