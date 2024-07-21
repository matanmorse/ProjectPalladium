using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectPalladium.Utils;
using ProjectPalladium.Plants;
using TimerManager = ProjectPalladium.GameManager.TimerManager;
using ProjectPalladium.Stations;
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
        public School school;

        public static Spell none = new Spell("", "", School.None, "", 0, null);

        public static Dictionary<string, Spell> spells = new Dictionary<string, Spell>()
        {
            {"tillearth", new Spell("Till Earth", "Tills some earth", School.Transmutation, "LL", 10, TillEarth) },
            {"iceblast", new Spell("Ice blast", "Blasts some ice", School.Evocation, "RR", 10, IceBlast) },
            {"growth", new Spell("Growth", "Grows a plant",School.Transmutation, "LLUR" , 10,Growth) },
            {"chronoshift", new Spell("Chrono Shift", "Advance Time", School.Chronomancy, "DDRUUL", 10, ChronoShift) },
            {"animatecauldron", new Spell("Animate Cauldron", "Bring a cauldron to life",  School.Conjuration, "DDD", 10, AnimateCauldron) }
        };

        public enum School
        {
            None,
            Evocation,
            Transmutation,
            Chronomancy,
            Conjuration,
        }
        public Spell(string name, string description, School school, string spellPath, int manaCost, SpellHandler onCast)
        {
            this.school = school;
            this.name = name;
            this.description = description;
            this.spellPath = spellPath;
            this.manaCost = manaCost;
            this.onCast = onCast;
        }

        
        public static void OnCastEvocationSpell()
        {
            SceneManager.CurScene.Player.castingAttackSpell = false;
            Game1.instance.ToggleMouseShowing(true);
        }
        public void Cast()
        {

            string name = this.name.Replace(" ", "").ToLower();
            GenericSpellHandler(name);
        }

        public static void IceBlast()
        {
            OnCastEvocationSpell();

            Player p = SceneManager.CurScene.Player;

            Vector2 dir = Vector2.Normalize((Input.gameWorldMousePos.ToVector2() - p.pos));


            float rotation = (float)Math.Acos(Vector2.Dot(dir, Vector2.UnitX));
            if (dir.Y < 0) rotation = -rotation;

            int shots = 3;

            for (int i = 0; i < shots; i++)
            {
                TimerManager.AddTimer(
                    () =>
                    {
                        p.AddProjectile("iceblast", dir, rotation);
                    },
                    i * 500f);
            }
        }




        public static void AnimateCauldron()
        {
            foreach (GameObject obj in SceneManager.CurScene.Map.gameObjects)
            {
                if (!(obj is Cauldron)) continue;

                int rectRange = 50;

                Player p = Game1.player;
                Cauldron cauld = obj as Cauldron;

                // if player is within range, start brewing
                Rectangle areaOfEffect = Util.MakeRangeRect(p.boundingBox.Center, rectRange);
                if (!(areaOfEffect.Intersects(cauld.bounds)) || cauld.Brewing) continue;
               

                cauld.TryBrew();
            }
        }
        public static void ChronoShift()
        {
            for (int i = 0; i < 5; i++)
            {
                GameManager.DoTenMinuteTick();
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

            // by convention the first tile index 1 is the till tile
            map.tillLayer.SetTileData(playerPos, map.tillLayer.tileIndex[1]);
            Debug.WriteLine(map.tillLayer.tileIndex[1].textureName);
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
            Player player = Game1.player;
            Spell spell = spells[spellName];

            // check if player has enough mana
            if (player.Mana < spell.manaCost) { return false; }
            player.Mana -= spell.manaCost;

            // play the cast animiation, pass spell function to perform when animation is finished
            player.flipped = false; // ensure player is not flipped

            if (spell.school == School.Evocation) 
            {
                Game1.instance.ToggleMouseShowing(false);
                player.castingAttackSpell = true; 
            }
            player.sprite.PlayAnimationOnce("cast", spell.onCast);
            player.sprite.DoToolAnimation();

            return true;
        }


        public static void DoNothing()
        {

        }
    }
}
