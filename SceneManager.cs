﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Buildings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ProjectPalladium
{
    internal class SceneManager
    {
        private static Scene curScene;
        public static Scene CurScene { get { return curScene; }  }
        public static void LoadScene(Scene scene)
        {

            curScene = scene ?? throw new Exception("Scene is null");
            curScene.Player.CurrentMap = curScene.Map;

            curScene.Map.player = curScene.Player;

            curScene.Player.pos = curScene.Map.spawnLocation * Game1.scale;
            curScene.Map.player.lerpingCamera = curScene.Player.pos; // avoid camera movement when changing scenes, less motion sick

        }

        // load scene at non-default spawn position
        public static void LoadScene (Scene scene, Vector2 pos)
        {
            LoadScene(scene);
            curScene.Player.pos = pos;
            curScene.Map.player.lerpingCamera = curScene.Player.pos; // avoid camera movement when changing scenes, less motion sick
        }
        public static void ChangeScene(string sceneName)
        {
            Map map = new Map(sceneName + ".tmx");

            Scene newScene = new Scene(map, curScene.Player);
            LoadScene(newScene);
        }

        public static void ChangeScene(string sceneName, Vector2 pos)
        {
            Map map = new Map(sceneName + ".tmx");
            
            Scene newScene = new Scene(map, curScene.Player);
            LoadScene(newScene, pos);
        }

        public static void EnterBuilding(string sceneName, Building exteriorBuilding)
        {
            BuildingInterior building = new BuildingInterior(sceneName, exteriorBuilding);
            Scene newScene = new Scene(building, curScene.Player);
            LoadScene(newScene);
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
