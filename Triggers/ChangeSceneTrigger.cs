using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium.Utils;

/* Trigger that changes scene when walked on */
namespace ProjectPalladium.Triggers
{
    public class ChangeSceneTrigger : Trigger
    {
        public onEnterFunc OnEnter;
        private Vector2 spawnPos = new Vector2(400, 400) * Game1.scale; // where on the sceneToGoTo the player should spawn
        public Vector2 SpawnPos { get { return spawnPos; } set { this.spawnPos = value; } }
        private string sceneToGoTo;
        public ChangeSceneTrigger(string name, Rectangle bounds, string sceneToGoTo) : base(name, bounds, null)
        {
            this.sceneToGoTo = sceneToGoTo;
            onEnter = OnEnterTrigger;
        }

        public void OnEnterTrigger()
        {
            SceneManager.ChangeScene(sceneToGoTo, spawnPos);
        }
    }
}
