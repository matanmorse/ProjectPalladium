using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectPalladium.Utils;

namespace ProjectPalladium.Spells
{
    public class Spell
    {
        public string name;
        public string description;
        public string spellPath;
        public delegate void SpellHandler();
        SpellHandler onCast;
        
        public static Spell none = new Spell("", "", "", null);

        public static Dictionary<string, Spell> spells = new Dictionary<string, Spell>()
        {
            {"tillearth", new Spell("Till Earth", "Tills some earth", "LL", TillEarth) },
            {"iceblast", new Spell("Ice blast", "Blasts some ice", "RR", DoNothing) }
        };

        public Spell(string name, string description, string spellPath, SpellHandler onCast)
        {
            this.name = name;
            this.description = description;
            this.spellPath = spellPath;
            this.onCast = onCast;
        }

        public void Cast()
        {
            onCast();
        }

        public static void TillEarth()
        {
            Map map = SceneManager.CurScene.Map;
            Player player = SceneManager.CurScene.Player;
            if (map.tillLayer == null) return;
            Point playerPos = Util.GetTileFromPos(player.pos) + new Point(0, 1); // get tile of player's feet, not head

            Tilemap tillable = map.tilemaps.FirstOrDefault(i => i.name.ToLower() == "tillable");
            if (tillable.Layer[playerPos.X, playerPos.Y] == Renderable.empty) return; // empty tile
            
            map.tillLayer.SetTileData(playerPos, new Renderable("TilledDirt"));
        }

        public static void DoNothing()
        {

        }
    }
}
