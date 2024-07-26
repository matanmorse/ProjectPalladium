using ProjectPalladium.Characters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectPalladium.Locations
{
    public class Dungeon : Map
    {
        Func<bool> winCondition = () => SceneManager.CurScene.Characters.Count(x => x is Enemy) == 0;
        bool won = false;
        Tilemap gate;

        public Dungeon(string filename) : base(filename)
        {
            Tilemap gate = tilemaps.Find(x => x.name == "gate");
        }

        public override void OnLoad()
        {
            Enemy.RemoveAllDangers();
            Enemy.AddTilemapDangers();
            AddEnemy("slime", Game1.player.pos + new Vector2(100, 100));
        }
        public override void OnEnemyKilled()
        {
            if (won) return;
            base.OnEnemyKilled();
            if (winCondition())
            {
                won = true;
                RemoveTilemap("gate");
            }
        }
    }
}
