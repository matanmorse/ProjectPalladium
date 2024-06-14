using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectPalladium.Utils;
using ProjectPalladium.Plants;
using ProjectPalladium.Tools;

namespace ProjectPalladium.Spells
{
    public class Spell
    {
        public string name;
        public string description;
        public string spellPath;
        public delegate void SpellHandler();
        SpellHandler onCast;
        public int manaCost;
        
        public static Spell none = new Spell("", "", "", 0, null);

        public static Dictionary<string, Spell> spells = new Dictionary<string, Spell>()
        {
            {"tillearth", new Spell("Till Earth", "Tills some earth", "LL", 10, TillEarth) },
            {"iceblast", new Spell("Ice blast", "Blasts some ice", "RR", 10, DoNothing) },
            {"growth", new Spell("Growth", "Grows a plant", "LLUR", 10,Growth) },
            {"chronoshift", new Spell("Chrono Shift", "Advance Time", "DDRUUL", 10, ChronoShift) }
        };

        public Spell(string name, string description, string spellPath, int manaCost, SpellHandler onCast)
        {
            this.name = name;
            this.description = description;
            this.spellPath = spellPath;
            this.manaCost = manaCost;
            this.onCast = onCast;
        }

        public void Cast()
        {
            
            string name = this.name.Replace(" ", "").ToLower();
            GenericSpellHandler(name);
        }

        public static void ChronoShift()
        {
            for (int i = 0; i < 5; i++)
            {
                GameManager.time.Minute += 10;
            }
            SceneManager.CurScene.Map.UpdateOnGameTime();
        }
        public static void TillEarth()
        {

            Map map = SceneManager.CurScene.Map;
            Player player = SceneManager.CurScene.Player;

            if (map.tillLayer == null) return;
            Point playerPos = Util.GetTileFromPos(player.pos) + new Point(0, 1); // get tile of player's feet, not head

            Tilemap tillable = map.tilemaps.FirstOrDefault(i => i.name.ToLower() == "tillable");
            if (tillable.Layer[player.feet.X, player.feet.Y] == Renderable.empty) return; // empty tile

            map.tillLayer.SetTileData(playerPos, new Renderable("TilledDirt"));
        }

        public static void Growth()
        {
            Map map = SceneManager.CurScene.Map;
            Player player = SceneManager.CurScene.Player;

            GameObject objAtFeet = map.FindGameObjectAtTile(player.feet);

            if (objAtFeet == null) return;
            if (!(objAtFeet is Plant)) return;

            Plant plant = objAtFeet as Plant;
            plant.GrowthStage += 1;


        }
        // logic to be called after any spell is cast, returns if spell failed or not
        public static bool GenericSpellHandler(string spellName)
        {
            Spell spell = spells[spellName];

            // check if player has enough mana
            if (Game1.player.mana < spell.manaCost) { return false; }
            Game1.player.mana -= spell.manaCost;

            // play the cast animiation, pass spell function to perform when animation is finished
            Game1.player.flipped = false; // ensure player is not flipped
            Game1.player.sprite.PlayAnimationOnce("cast", spell.onCast);
            Game1.player.sprite.DoToolAnimation();

            return true;
        }

        public static void DoNothing()
        {

        }
    }
}
