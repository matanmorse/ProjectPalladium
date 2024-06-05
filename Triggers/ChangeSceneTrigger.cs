using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium.Utils;

namespace ProjectPalladium.Triggers
{
    public class ChangeSceneTrigger : Trigger
    {
        public onEnterFunc OnEnter;

        private string sceneToGoTo;
        public ChangeSceneTrigger(string name, Rectangle bounds, string sceneToGoTo) : base(name, bounds, null)
        {
            this.sceneToGoTo = sceneToGoTo;
            onEnter = OnEnterTrigger;
        }

        public void OnEnterTrigger()
        {
            SceneManager.ChangeScene(sceneToGoTo);
        }
    }
}
