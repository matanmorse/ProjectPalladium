using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectPalladium
{
    public class Tilemap
    {
        private XDocument tmxDoc;

        public List<Renderable[,]> layers = new List<Renderable[,]>();

        public List<Renderable> tileIndex;

        // public AnimatedSprite[,] layer;
        
        private int tileSize = 16;
        public int TileSize { get {  return tileSize; } }

        private Point _mapTileSize = new Point(64, 32);

        public Point MapTileSize {  get { return _mapTileSize; } }

        Texture2D tileMap;
        public Tilemap(String fileName) {
            tileIndex = ExtractTiles("tilemap");

            TMXParser(fileName);
        }

        // populate the layers based on given tmx file
        // returns list of 2d array of sprites representing each layer
        public void TMXParser(String fileName)
        {
            
            // load the tmx file
            fileName = "content/" + fileName;
            tmxDoc = new XDocument();
            tmxDoc = XDocument.Load(fileName);

            DecodeTMX();            
        }

        public List<Renderable[,]> DecodeTMX ()
        {
            IEnumerable<XElement> layerData = tmxDoc.Descendants("layer");

            foreach (XElement elem in layerData) // for each layer
            {
                String line = elem.Value.Trim();

                // parse the numbers from the layer into a 1d array
                int[] tiles = Array.ConvertAll(line.Split(','), int.Parse);

                Renderable[,] layer = new Renderable[_mapTileSize.X, _mapTileSize.Y];
                for (int i = 0; i < (_mapTileSize.X * _mapTileSize.Y); i++)
                {

                    // convert the 1d coordinates to 2d based on map size
                    int x = i % _mapTileSize.X;
                    int y = i / _mapTileSize.X;

                    //Debug.WriteLine("tile: " + nums[i] + "at " + x + "," + y);

                    // based on the id of the tile, set the tile data in the layer
                    layer[x, y] = tileIndex[tiles[i]];
                }
                layers.Add(layer);
            }
            return layers;
        }
        public List<Renderable> ExtractTiles(string fileName)
        {
            tileMap = Game1.contentManager.Load<Texture2D>(fileName);

            List<Renderable> tiles = new List<Renderable>();
            tiles.Add(new Renderable(tileMap, new Rectangle(0, 0, 0, 0))); // in tiled tmx, id=0 is an empty tile, so the first entry is an empty rectangle

            
            for (int i = 0; i < (tileMap.Width / tileSize) * (tileMap.Height / tileSize); i++)
            {
                // calculate the source rectangle for the tile based on the index
                Rectangle sourceRect = new Rectangle(tileSize * i % tileMap.Width, i * tileSize / tileMap.Width * tileSize,
                tileSize, tileSize);

                tiles.Add(new Renderable(tileMap, sourceRect));
            }

            // foreach (Rectangle tile in tiles) Debug.Write(tile);
            return tiles;
        }

        public void Draw(SpriteBatch b)
        {

            
            
            foreach (Renderable[,] layer in layers)
            {
                for (int i = 0; i < _mapTileSize.X; i++)
                {
                    for (int j = 0; j < _mapTileSize.Y; j++)
                    {
                        Vector2 pos = new Vector2(i * tileSize * Game1.scale, j * tileSize * Game1.scale);
                        layer[i, j].Draw(b, pos);
                    }
                }
            }
            
        }

        
    }
}
