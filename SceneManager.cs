using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Buildings;
using System;
using System.Diagnostics;


namespace ProjectPalladium
{
    internal class SceneManager
    {

        private static Scene curScene;
        private static Scene storedScene;
        private static Vector2 storedPos;
        public static Map hollow; // the hollow is always loaded
        public static Scene CurScene { get { return curScene; }  }
        public static void LoadScene(Scene scene)
        {
            curScene = scene ?? throw new Exception("Scene is null");
            curScene.Player.CurrentMap = curScene.Map;

            curScene.Map.player = curScene.Player;
            curScene.Player.pos = curScene.Map.spawnLocation * Game1.scale;
            curScene.Map.player.lerpingCamera = curScene.Player.pos; // avoid camera movement when changing scenes, less motion sick

            // curScene.Map.OnLoad();
        }

        // load scene at non-default spawn position
        public static void LoadScene (Scene scene, Vector2 pos)
        {
            storedScene = scene;
            storedPos = pos * Game1.scale;
        }
      
        public static void ChangeScene(string sceneName, Vector2 pos)
        {
            // if we're attempting to load the hollow, use the version that's already stored before loading from disc
            if (sceneName == "hollow")
            {
                if (hollow != null)
                {
                    hollow.Save("hollow"); // save changes made outside scene
                    Scene hollowScene = new Scene(hollow, curScene.Player);

                    LoadScene(hollowScene, pos);
                    Game1.shader.DoSceneTransition();

                    return;

                }
            }
            Map map = new Map(sceneName + ".tmx");
            pos = map.spawnLocation;

            Scene newScene = new Scene(map, curScene.Player);
            LoadScene(newScene, pos);
            Game1.shader.DoSceneTransition();

        }

        public static void EnterBuilding(string sceneName, Building exteriorBuilding)
        {
            BuildingInterior building = new BuildingInterior(sceneName, exteriorBuilding);
            Scene newScene = new Scene(building, curScene.Player);

            // save the current version of the map, if we're on the default save to the user's version
            CurScene.Map.Save(CurScene.Map.filename.Replace("default", "")); 

            storedScene = newScene;
            Game1.shader.DoSceneTransition();
        }

        public static void OnSceneTransitionFinished()
        {

            LoadScene(storedScene);
            storedScene = null;

            if (storedPos != Vector2.Zero)
            {
                curScene.Player.pos = storedPos;
                curScene.Player.lerpingCamera = storedPos;
                storedPos = Vector2.Zero;
            }
   
        }
        public static void Update(GameTime gameTime)
        {
            if (curScene == null) { return; }
            curScene.Player.Update(gameTime);
            curScene.Map.Update(gameTime);
        }
        public static void Draw(SpriteBatch _spriteBatch)
        {
            if (curScene == null) { return; }
            curScene.Map.Draw(_spriteBatch);
            curScene.Player.Draw(_spriteBatch);

            
        }
    }
}
