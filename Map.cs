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

        public List<Tilemap> tilemaps = new List<Tilemap>();
        public List<Building> buildings = new List<Building>();

        MapSerializer map;

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
        /* Parses TMX file to create map representation */
        public void DeserializeMap()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MapSerializer));

            using (FileStream fs = new FileStream("Content/" + filename, FileMode.Open))
            {
                map = (MapSerializer)serializer.Deserialize(fs);
            }
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
    }
}
