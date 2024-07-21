using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Spells;
using ProjectPalladium.Tools;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ProjectPalladium;

namespace ProjectPalladium.UI
{

 
    public struct Directions {
        public static Point none = Point.Zero;
        public static Point left = new Point(-1, 0);
        public static Point right = new Point(1,0);
        public static Point up = new Point(0, -1);
        public static Point down = new Point(0, 1);
    }


    public class CastingUI : UIElement
    {
        private Wand wand;
        Point SPELL_MARKER_OFFSET;
        private static UIManager ui = Game1.UIManager;
        private Vector2 SPELL_DISPLAY_POS = Util.PointToVector2(UIManager.toolbar.globalPos - new Point(0, 8) * new Point((int)Game1.scale));
        private TextRenderer _storedSpellDisplay;
        private Dictionary<string, Spell> spells = Spell.spells;
        private UIElement aimElement = new UIElement("aim indicator", "aim", 0,0, UIManager.rootElement, originType:OriginType.center);

        public Dictionary<Point, float> directions = new Dictionary<Point, float>()
        {
            { Directions.up, 0f },
            { Directions.right, MathHelper.ToRadians(90) },
            { Directions.down, MathHelper.ToRadians(180) },
            { Directions.left, MathHelper.ToRadians(270) },
        };

        public static Dictionary<Point, char> directionChars = new Dictionary<Point, char>()
        {
            { Directions.up, 'U' },
            { Directions.right, 'R' },
            { Directions.down, 'D' },
            { Directions.left, 'L' },
        };

        public CastingUI(UIElement root) : 
            base("Casting UI", "", Game1.UINativeResolution.X / 2, Game1.UINativeResolution.Y / 2, root, OriginType.center, isBox: true) { 
            SPELL_MARKER_OFFSET = new Point(15, 15) * new Point((int) scale, (int) scale);
            _storedSpellDisplay = new TextRenderer(SPELL_DISPLAY_POS, originType:TextRenderer.Origin.center);
            showing = false;

            aimElement.opacity = 0.7f;

            AddToRoot();
         }

        public override void Draw(SpriteBatch b)
        {
            base.Draw(b);
            aimElement.Draw(b);

            if (!showing) return;

            _storedSpellDisplay.Draw(b, wand.storedSpell.name);
        }

        public override void Update(GameTime gameTime)
        {
            aimElement.showing = Game1.player.castingAttackSpell;
            if (aimElement.showing) { aimElement.LocalPos = Input.nativeMousePos; }

            if (UIManager.inventoryUI.showing) { return; }

            base.Update(gameTime);
            if (ui.Player == null) { return; }
            if (!(ui.Player.ActiveItem is Wand)) { return; }
          
            wand = ui.Player.ActiveItem as Wand;

            if (showing && !wand.casting) { Reset(); }

            if (wand.casting && !showing) {
                this.LocalPos = Input.nativeMousePos; }

            showing = wand.casting;
            if (wand.casting) {
                if (wand.castingDirection != Directions.none) { AddChild(wand.castingDirection); }
            }
        }

        public void AddChild(Point direction)
        {

            Point startingPos = children.Count() == 0 ? Point.Zero : children.Last().LocalPos;
            //if (children.Count != 0) { wand.startingPos = Util.PointToVector2(children.Last().globalPos); }

            Point pos = startingPos + (SPELL_MARKER_OFFSET * direction);
            base.AddChild(new UIElement("spellmarker", "spellmarker", pos.X, pos.Y, this, OriginType.center, rotation: directions[direction]));

            
        }

        public void Reset()
        {
            
            this.children = new List<UIElement>();
            Wand wand = ui.Player.ActiveItem as Wand;
            wand.ResetSpell();
        }


    }
}
