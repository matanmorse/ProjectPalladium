using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPalladium.Stations
{
    public class ReturnLectern : Station
    {
        public ReturnLectern(Vector2 pos) : base("lectern", pos, "lectern", "lecternplaced", nonAnimated:true)
        {
            
            button.onRightClick = ReturnToHollow;
            this.button.SetBounds(new Rectangle(this.globalPos.ToPoint(), this.sprite.size * new Point((int)Game1.scale)));
        }

        public void ReturnToHollow()
        {
            if (Vector2.Distance(Game1.player.pos, globalPos) > Map.scaledTileSize) return;
            SceneManager.ChangeScene("hollow", Vector2.Zero); // don't need to specify a return position, will use default hollow spawnpos
        }
        public override void Draw(SpriteBatch b)
        {
            base.Draw(b);
        }
    }
}
