using System.Diagnostics;

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
