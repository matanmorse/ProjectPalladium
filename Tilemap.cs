using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectPalladium
{
    public class Tilemap
    {
        public AnimatedSprite[,] tiles;
        private int tileSize = 16;
        public int TileSize { get {  return tileSize; } }

        private Point _mapTileSize = new Point(32, 32);

        public Point MapTileSize {  get { return _mapTileSize; } }

        AnimatedSprite sprite;
        Texture2D tileMap;
        public Tilemap(String textureName) {
            tileMap = Game1.contentManager.Load<Texture2D>(textureName);
            tiles = new AnimatedSprite[_mapTileSize.X, _mapTileSize.Y];

            List<Rectangle> tileIndex = ExtractTiles();

            for (int i = 0; i < _mapTileSize.X; i++)
            {
                for (int j = 0; j < _mapTileSize.Y; j++)
                {
                    tiles[i, j] = new AnimatedSprite(tileMap, tileSize, tileSize, tileIndex[ (i + j) % 2]);
                }
            }
        }

        public List<Rectangle> ExtractTiles() {
            List<Rectangle> tiles = new List<Rectangle> ();
            for (int i = 0; i < (tileMap.Width / tileSize) * (tileMap.Height / tileSize); i++)
            {
                Rectangle sourceRect = new Rectangle(tileSize * i % tileMap.Width, i * tileSize / tileMap.Width * tileSize,
                tileSize, tileSize);
                tiles.Add(sourceRect);
            }
            foreach (Rectangle tile in tiles) Debug.Write(tile);
            return tiles;
        }
        public void Draw(SpriteBatch b)
        {
            for (int i = 0; i < _mapTileSize.X; i++)
            {
                for (int j = 0; j < _mapTileSize.Y; j++)
                {
                    Vector2 pos = new Vector2(i * tileSize * Game1.scale, j * tileSize * Game1.scale);
                    
                    tiles[i, j].Draw(b, pos, 0f);
                }
            }
        }

        
    }
}
