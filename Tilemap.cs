﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Utils;

namespace ProjectPalladium
{
    public class Tilemap
    {
        private string subFolder = "tilemaps/";
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

        public Texture2D tileMap;
        public Tilemap(string tileData, Point MapTileSize, string name, bool collideLayer=false, bool isTillLayer=false, string imageName="tilemap")
        {
            layer = new Renderable[MapTileSize.X, MapTileSize.Y];
            this.name = name;
            _mapTileSize = MapTileSize;
            isCollideLayer = collideLayer;
            this.isTillLayer = isTillLayer;
            tileIndex = ExtractTiles(subFolder + imageName);
            DecodeTileData(tileData);
        }


        /* Create a list of renderable objects from a string of tile ids*/
        public void DecodeTileData(string data)
        {
            // parse the numbers from the layer into a 1d array
            // Debug.WriteLine(data);
            int[] tiles = Array.ConvertAll(data.Split(','), int.Parse);
            for (int i = 0; i < _mapTileSize.X * _mapTileSize.Y; i++)
            {

                // convert the 1d coordinates to 2d based on map size
                int x = i % _mapTileSize.X;
                int y = i / _mapTileSize.X;


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
            if (fileName == "TilledDirt") { Debug.WriteLine("dirt"); }
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
                if (Rectangle.Intersect(collider, boundingBox) != Rectangle.Empty) return collider;
            }
            return Rectangle.Empty;
        }

        public void SetTileData(Point location, Renderable tile)
        {
            layer[location.X, location.Y] = tile;
        }

        public string GetSerializedTileData()
        {
            string result = "";
            for (int i = 0; i < _mapTileSize.Y; i++)
            {
                for (int j = 0; j < _mapTileSize.X; j++)
                {
                    result += Util.FindTileIDFromRect(Layer[j, i].getSourceRect, tileMap) + ",";
                }
            }
            result = result.Remove(result.Length - 1, 1);
            return result;
        }

        /* draw tilemap */
        public void Draw(SpriteBatch b, float layerDepth = Game1.layers.tile)
        {
            showColliders = DebugParams.showTileColliders;
            
            if (showColliders) { foreach (Rectangle r in colliders) { if ( (Game1.player.pos - r.Location.ToVector2()).Length() < 2500) Util.DrawRectangle(r, b); } }

            for (int i = 0; i < _mapTileSize.X; i++)
            {
                for (int j = 0; j < _mapTileSize.Y; j++)
                {
                    Vector2 pos = new Vector2(i * tileSize * Game1.scale, j * tileSize * Game1.scale);

                    // debug, draws tiles around player
                    if (DebugParams.showFootTile) if (Util.IsTileWithinOneTileOfPlayer(new Point(i, j))) Util.DrawRectangle(new Rectangle(pos.ToPoint(), new Point(Map.scaledTileSize, Map.scaledTileSize)), b);

                    layer[i, j].Draw(b, pos, layer: layerDepth);
                }
            }

        }
    }
}
