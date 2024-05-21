using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium
{
    internal class SceneManager
    {
        private static Game1 game;
        public static void Initialize(Game1 game1)
        {
            game = game1;
        }
        public static void LoadScene(Scene scene)
        {
            if (game == null) { throw new Exception("No Game in SceneManager"); }
            if (scene == null) { throw new Exception("Scene is null"); }

            game.player = scene.Player;
            game.player.pos = scene.metadata.spawnLocation;
            game.map = scene.Map;
        }
    }
}
