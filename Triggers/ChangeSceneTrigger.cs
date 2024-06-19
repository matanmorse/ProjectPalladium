using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium.Utils;
using static ProjectPalladium.Map;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

/* Trigger that changes scene when walked on */
namespace ProjectPalladium.Triggers
{
    public class ChangeSceneTrigger : Trigger
    {
        public onEnterFunc OnEnter;
        private Vector2 spawnPos = Vector2.Zero; // where on the sceneToGoTo the player should spawn
        public Vector2 SpawnPos { get { return spawnPos; } set { this.spawnPos = value; } }
        private string sceneToGoTo;
        private string linkID; // the associated entrance/exit to the new scene
        public ChangeSceneTrigger(string name, Rectangle bounds, string sceneToGoTo, string linkID="") : base(name, bounds, null)
        {
            this.linkID = linkID;
            this.sceneToGoTo = sceneToGoTo;
            onEnter = OnEnterTrigger;
            spawnPos = GetAssociatedSpawnPosFromLink(sceneToGoTo, linkID); // a blank linkID indicates a spawn position that will be set dynamically later
        }

        public void OnEnterTrigger()
        {
            
            SceneManager.ChangeScene(sceneToGoTo, spawnPos);
        }
    }
}
