using ProjectPalladium.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium
{
    public class QuestEvents
    {
        public static void GoToArena()
        {
            Debug.WriteLine("Go to arena!");
            SceneManager.EnterDungeon("dungeon.tmx");
        }

        public static Button.OnClick GetButtonDelegateMethod(string name)
        {
            if (name == null) return null;
            MethodInfo methodInfo = typeof(QuestEvents).GetMethod(name);
            return (Button.OnClick)methodInfo.CreateDelegate(typeof(Button.OnClick));
        }

    }
}
