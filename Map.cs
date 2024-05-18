﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectPalladium
{
    /* Contains Map data about a given location (tilemaps, terrain, objects */
    public class Map
    {
        private string filename;
        public string name;

        public int scaledTileSize;
        public List<Tilemap> tilemaps = new List<Tilemap>();
        public List<Building> buildings = new List<Building>();

        MapSerializer map;

        public Player player;
        
        public Point tileMapSize = new Point();
        public static int tilesize = 16;

        public Map(string filename) {
            this.filename = filename;
            DeserializeMap();
            tileMapSize = new Point(map.Layers[0].Width, map.Layers[0].Height);

            foreach (Layer layer in map.Layers)
            {
                tilemaps.Add(new Tilemap(layer.Data.Text, tileMapSize));
            }

            this.scaledTileSize = (int) (tilesize * Game1.scale);
        }
        // checks collisions with any collidable objects on the map
        // returns rectangle of intersection of first found collision
        public Rectangle CheckCollisions(Rectangle boundingBox)
        {
            foreach(Building building in buildings)
            {
                if (Rectangle.Intersect(building.bounds, boundingBox) != Rectangle.Empty) return Rectangle.Intersect(building.bounds, boundingBox); 
            }
            return Rectangle.Empty;
        }

        // if the player is behind a building, turn down opacity
        public Building CheckBehindBuilding()
        {
            foreach (Building building in buildings)
            {
                if (Rectangle.Intersect(building.walkBehind, player.boundingBox) != Rectangle.Empty) {
                    building.PlayerBehind = true;
                }
                else
                {
                    building.PlayerBehind = false;
                }
            }
            return null;
        }
        /* Parses TMX file to create map representation */
        public void DeserializeMap()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MapSerializer));

            using (FileStream fs = new FileStream("Content/" + filename, FileMode.Open))
            {
                map = (MapSerializer)serializer.Deserialize(fs);
            }


            ObjectLayer buildingLayer = map.ObjectLayers.First(layer => layer.name.ToLower() == "buildings");

            // for each object in the building layer, add it to the list of buildings
            foreach (TiledObject building in buildingLayer.objects)
            {
                Property[] pList = building.properties.properties; // don't even ask why i have to do this
                string name = pList.First(prop => prop.name.ToLower() == "name").value;
                
                Vector2 pos = new Vector2(building.x / tilesize, building.y / tilesize);
                Debug.WriteLine(pos);
                buildings.Add(new Building(name, pos));
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Building building in buildings) building.Update(gameTime);
            CheckBehindBuilding();
        }
        public void Draw(SpriteBatch b)
        {
            
            foreach(Tilemap tilemap in tilemaps) { tilemap.Draw(b); }
            foreach(Building building in buildings) { building.Draw(b); }
        }

        [XmlRoot("map")]
        public class MapSerializer
        {
            [XmlElement("layer")]
            public Layer[] Layers { get; set; }

            [XmlElement("objectgroup")]
            public ObjectLayer[] ObjectLayers { get; set; }
        }

        public class ObjectLayer
        {
            [XmlAttribute("name")]
            public string name;

            [XmlElement("object")]
            public TiledObject[] objects { get; set; }
        }

        public class Data
        {
            [XmlAttribute("encoding")]
            public string Encoding { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

        public class Layer
        {
            [XmlElement("data")]
            public Data Data { get; set; }

            [XmlAttribute("width")]
            public int Width { get; set; }

            [XmlAttribute("height")]
            public int Height { get; set; }
        }

        public class TiledObject
        {
            [XmlAttribute("id")]
            public int id { get; set; }

            [XmlAttribute("x")]
            public int x { get; set; }

            [XmlAttribute("y")]
            public int y { get; set; }

            [XmlAttribute("height")]
            public int height { get; set; }

            [XmlAttribute("width")]
            public int width { get; set; }

            [XmlElement("properties")]
            public PropertyList properties { get; set; }
        }

        public class PropertyList
        {
            [XmlElement("property")]
            public Property[] properties { get; set; }
        }
        public class Property
        {
            [XmlAttribute("name")]
            public string name { get; set; }

            [XmlAttribute("value")]
            public string value { get; set; }

        }
    }
}
