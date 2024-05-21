using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ProjectPalladium
{
    internal class SceneManager
    {
        private static Scene curScene;
        public static void LoadScene(Scene scene)
        {
            curScene = scene ?? throw new Exception("Scene is null");
            curScene.Player.pos = curScene.metadata.spawnLocation;
        }
        public static void Update(GameTime gameTime)
        {
            if (curScene == null) { return; }
            curScene.Map.Update(gameTime);
            curScene.Player.Update(gameTime);
        }
        public static void Draw(SpriteBatch _spriteBatch)
        {
            if (curScene == null) { return; }
            curScene.Map.Draw(_spriteBatch);
            curScene.Player.Draw(_spriteBatch);
        }
    }
}
