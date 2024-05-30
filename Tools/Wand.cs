using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Tutorial;
using ProjectPalladium.UI;
using Microsoft.Xna.Framework.Input;
using ProjectPalladium.Spells;

namespace ProjectPalladium.Tools
{
    public class Wand : Tool
    {
        public Spell storedSpell = Spell.none;
        public Point castingDirection = Directions.none;
        public int SPELLMARKER_SIZE = 32;
        public bool casting = false;
        public UIManager ui = Game1.UIManager;
        public Vector2 startingPos = Vector2.Zero;
        private Dictionary<Point, char> directionChars = CastingUI.directionChars;
        private Dictionary<string, Spell> spells = Spell.spells;

        public string spellPath = "";
        public Wand(int id, string name, string textureName, string description)
            :base(id, name, textureName, description)
        {

        }

        public override void Update()
        {
            casting = Input.GetLeftMouseDown();
            if (casting) { castingDirection = FindDirection(); }
        }
        
        public Point FindDirection()
        {
            // if we don't have a starting point, set it to the current mouse poisition
            Vector2 mousePos = Util.PointToVector2(Input.mousePos);
            Point dir = Directions.none;
            if (startingPos == Vector2.Zero) { startingPos = mousePos; return Directions.none; }

            // calculate the direction vector
            Vector2 difference = mousePos - startingPos;

            if (difference.X > SPELLMARKER_SIZE) dir = Directions.right;
            if (difference.X < -SPELLMARKER_SIZE) dir = Directions.left;
            if (difference.Y < -SPELLMARKER_SIZE) dir = Directions.up;
            if (difference.Y > SPELLMARKER_SIZE) dir = Directions.down;

            if (dir != Directions.none)
            {
                MoveStartingPoint(dir);
                spellPath += directionChars[dir];
                CheckSpellPaths();
            }
            return dir;
        }

        public void CheckSpellPaths()
        {
            Spell curSpell = Spell.none;
            foreach (string key in spells.Keys)
            {
                if (spellPath == spells[key].spellPath) { curSpell = spells[key]; break; }
            }
            storedSpell = curSpell;
        }
        public void MoveStartingPoint(Point dir) { startingPos += Util.PointToVector2(new Point(SPELLMARKER_SIZE, SPELLMARKER_SIZE) * dir); }

        public void ResetSpell()
        {
            if (storedSpell != Spell.none) storedSpell.Cast();
            storedSpell = Spell.none;
            startingPos = Vector2.Zero;
            spellPath = "";
        }
    }
}
