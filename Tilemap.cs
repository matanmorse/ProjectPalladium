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

namespace ProjectPalladium
{
    public class Tilemap
    {
        // is this layer for colliders or not
        public bool isCollideLayer;

        public bool isTillLayer;

        public string name;

        public List<Rectangle> colliders = new List<Rectangle>();

        public List<Renderable[,]> layers = new List<Renderable[,]>();

        
        public List<Renderable> tileIndex;

        private bool showColliders;

        private int tileSize = 16;
        public int TileSize { get { return tileSize; } }

        private Point _mapTileSize;

        private Renderable[,] layer;

        public Renderable[,] Layer { get { return layer; } }

        public Point MapTileSize { get { return _mapTileSize; } }

        Texture2D tileMap;
        public Tilemap(string tileData, Point MapTileSize, string name, bool collideLayer=false, bool isTillLayer=false)
        {
            layer = new Renderable[MapTileSize.X, MapTileSize.Y];
            this.name = name;
            _mapTileSize = MapTileSize;
            isCollideLayer = collideLayer;
            this.isTillLayer = isTillLayer;
            tileIndex = ExtractTiles("tilemap");
            DecodeTileData(tileData);
        }


        /* Create a list of renderable objects from a string of tile ids*/
        public void DecodeTileData(string data)
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

                // if this layer contains colliders, add collider rects over all tiles in this layer
                if (isCollideLayer && tiles[i] != 0)
                {
                    Vector2 pos = new Vector2(x * tileSize * Game1.scale, y * tileSize * Game1.scale);
                    colliders.Add(new Rectangle((int)pos.X, (int)pos.Y, (int)(tileSize * Game1.scale), (int)(tileSize * Game1.scale)));
                }
            }
        }

        /* Create an index of tiles based on the tilemaps, corresponding to their id's */
        public List<Renderable> ExtractTiles(string fileName)
        {
            tileMap = Game1.contentManager.Load<Texture2D>(fileName);

            List<Renderable> tiles = new List<Renderable>();
            tiles.Add(Renderable.empty); // in tiled tmx, id=0 is an empty tile, so the first entry is an empty rectangle


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

        public Rectangle checkCollisions(Rectangle boundingBox)
        {
            foreach (Rectangle collider in colliders)
            {
                if (Rectangle.Intersect(collider, boundingBox) != Rectangle.Empty) return Rectangle.Intersect(collider, boundingBox);
            }
            return Rectangle.Empty;
        }

        public void SetTileData(Point location, Renderable tile)
        {
            layer[location.X, location.Y] = tile;
        }

        /* draw tilemap */
        public void Draw(SpriteBatch b, float layerDepth = Game1.layers.tile)
        {
            showColliders = DebugParams.showColliders;
            if (showColliders) { foreach (Rectangle r in colliders) { Util.DrawRectangle(r, b); } }

            for (int i = 0; i < _mapTileSize.X; i++)
            {
                for (int j = 0; j < _mapTileSize.Y; j++)
                {
                    Vector2 pos = new Vector2(i * tileSize * Game1.scale, j * tileSize * Game1.scale);
                    layer[i, j].Draw(b, pos, layer: layerDepth);
                }
            }

        }
    }
}
