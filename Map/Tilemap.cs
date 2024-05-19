using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ProjectPalladium.Utils;

namespace ProjectPalladium.TileMap
{
    public class Tilemap
    {

        public List<Renderable[,]> layers = new List<Renderable[,]>();

        public List<Renderable> tileIndex;


        private int tileSize = 16;
        public int TileSize { get { return tileSize; } }

        private Point _mapTileSize;

        Renderable[,] layer;

        public Point MapTileSize { get { return _mapTileSize; } }

        Texture2D tileMap;
        public Tilemap(string tileData, Point MapTileSize)
        {
            layer = new Renderable[MapTileSize.X, MapTileSize.Y];
            _mapTileSize = MapTileSize;
            tileIndex = ExtractTiles("tilemap");
            DecodeTileData(tileData);
        }


        /* Create a list of renderable objects from a string of tile ids*/
        public List<Renderable[,]> DecodeTileData(string data)
        {
            // parse the numbers from the layer into a 1d array
            int[] tiles = Array.ConvertAll(data.Split(','), int.Parse);

            for (int i = 0; i < _mapTileSize.X * _mapTileSize.Y; i++)
            {

                // convert the 1d coordinates to 2d based on map size
                int x = i % _mapTileSize.X;
                int y = i / _mapTileSize.X;

                //Debug.WriteLine("tile: " + nums[i] + "at " + x + "," + y);

                // based on the id of the tile, set the tile data in the layer

                layer[x, y] = tileIndex[tiles[i]];
            }
            layers.Add(layer);
            return layers;
        }

        /* Create an index of tiles based on the tilemaps, corresponding to their id's */
        public List<Renderable> ExtractTiles(string fileName)
        {
            tileMap = Game1.contentManager.Load<Texture2D>(fileName);

            List<Renderable> tiles = new List<Renderable>();
            tiles.Add(new Renderable(tileMap, new Rectangle(0, 0, 0, 0))); // in tiled tmx, id=0 is an empty tile, so the first entry is an empty rectangle


            for (int i = 0; i < tileMap.Width / tileSize * (tileMap.Height / tileSize); i++)
            {
                // calculate the source rectangle for the tile based on the index
                Rectangle sourceRect = new Rectangle(tileSize * i % tileMap.Width, i * tileSize / tileMap.Width * tileSize,
                tileSize, tileSize);

                tiles.Add(new Renderable(tileMap, sourceRect));
            }

            // foreach (Rectangle tile in tiles) Debug.Write(tile);
            return tiles;
        }

        /* draw tilemap */
        public void Draw(SpriteBatch b)
        {

            for (int i = 0; i < _mapTileSize.X; i++)
            {
                for (int j = 0; j < _mapTileSize.Y; j++)
                {
                    Vector2 pos = new Vector2(i * tileSize * Game1.scale, j * tileSize * Game1.scale);
                    layer[i, j].Draw(b, pos, layer: Game1.layers.tile);
                    if (DebugParams.showColliders && i == 1 && j == 1) Util.DrawRectangle(new Rectangle((int)pos.X, (int)pos.Y, (int)(tileSize * Game1.scale), (int)(tileSize * Game1.scale)), b);
                }
            }

        }
    }
}
