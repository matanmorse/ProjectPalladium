using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using ProjectPalladium;
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
        private CastingUI castingUI;
        private const int SPELL_LENGTH_LIMIT = 10;

        private List<Point> visitedPoints = new List<Point>(SPELL_LENGTH_LIMIT);

        public string spellPath = "";
        public Wand(int id, string name, string textureName, string description)
            :base(id, name, textureName, description)
        {
            castingUI = ui.castingUI;
        }

        public override void Update()
        {
            if (Input.GetLeftMouseClicked() && !casting) { casting = true; }
            if (casting && !Input.GetLeftMouseDown()) { casting = false; }
            if (casting) { castingDirection = FindDirection(); }
        }
        
        public Point FindDirection()
        {
            // if we don't have a starting point, set it to the current mouse poisition
            Vector2 mousePos = Util.PointToVector2(Input.nativeMousePos);
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

                if (castingUI == null) { castingUI = ui.castingUI; }

                if (CheckIntersections(dir))
                {
                    castingUI.Reset();
                }
                if (spellPath.Length > SPELL_LENGTH_LIMIT)
                {
                    castingUI.Reset();   
                }

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

        // checks if the spell intersects with itself, given a direction from the last point
        public bool CheckIntersections(Point dir)
        {
            if (visitedPoints.Count == 0) { visitedPoints.Add(dir); return false; }

            Point curPoint = visitedPoints.Last() + dir;
            if (visitedPoints.Contains(curPoint)) { return true; }

            visitedPoints.Add(curPoint);
            return false;

        }
        public void ResetSpell()
        {
            if (storedSpell != Spell.none) storedSpell.Cast();

            visitedPoints = new List<Point>(SPELL_LENGTH_LIMIT);
            casting = false;
            storedSpell = Spell.none;
            startingPos = Vector2.Zero;
            spellPath = "";
        }
    }
}
