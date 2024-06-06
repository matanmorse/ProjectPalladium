﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Buildings;
using ProjectPalladium.Plants;
using ProjectPalladium.Triggers;
using ProjectPalladium.Utils;
using Trigger = ProjectPalladium.Utils.Trigger;


namespace ProjectPalladium
{
    /* Contains Map data about a given location (tilemaps, terrain, objects */
    public class Map
    {
        private string filename;
        public string name;

        public static int scaledTileSize;
        public List<Tilemap> tilemaps = new List<Tilemap>();
        public List<Tilemap> collidingTilemaps = new List<Tilemap>();

        public Tilemap tillLayer;

        public List<Building> buildings = new List<Building>();
        public List<GameObject> gameObjects = new List<GameObject>();

        MapSerializer map;

        public Player player;

        public Point tileMapSize = new Point();
        private static int tilesize = 16;

        private List<Trigger> triggers = new List<Trigger>();


        public Vector2 spawnLocation;
        

        public Map(string filename)
        {
            this.spawnLocation = new Vector2(50,100);
            this.filename = filename;
            DeserializeMap();
            tileMapSize = new Point(map.Layers[0].Width, map.Layers[0].Height);

            // populate the layers list
            foreach (Layer layer in map.Layers)
            {
                // check if the layer has appropriate properties  and its value
                bool isCollideLayer = false;
                bool isTillLayer = false;
                if (layer.properties != null)
                {
                    Property isCollider = layer.properties.properties.FirstOrDefault(prop => prop.name == "iscollider", null);
                    Property isTill = layer.properties.properties.FirstOrDefault(prop => prop.name == "isTillLayer", null);
                    isCollideLayer = isCollider == null || isCollider.value == "false" ? false : true;
                    isTillLayer = isTill == null || isTill.value == "false" ? false : true;
                }
                

                Tilemap tmap = new Tilemap(layer.Data.Text, tileMapSize, layer.Name, isCollideLayer, isTillLayer);
                tilemaps.Add(tmap);
                if (tmap.isCollideLayer) collidingTilemaps.Add(tmap);
                if (tmap.isTillLayer) this.tillLayer = tmap;
            }

            scaledTileSize = (int)(tilesize * Game1.scale);
        }

        // checks collisions with any collidable objects on the map
        // returns rectangle of intersection of first found collision
        public List<Rectangle> CheckCollisions(Rectangle boundingBox)
        {
            List<Rectangle> intersections = new List<Rectangle>();
            Rectangle totalInstersection = Rectangle.Empty;
            foreach (Building building in buildings)
            {
                Rectangle intersection = Rectangle.Intersect(building.bounds, boundingBox);
                if (intersection != Rectangle.Empty) intersections.Add(intersection);
            }

            // check tilemaps for collision
            foreach (Tilemap tilemap in collidingTilemaps)
            {
                foreach (Rectangle collider in tilemap.colliders)
                {
                    Rectangle intersection = Rectangle.Intersect(collider, boundingBox);
                    if (intersection != Rectangle.Empty) intersections.Add(intersection);

                }
            }
            return intersections;
        }

        // if the player is behind a building, turn down opacity
        public void CheckBehindBuilding()
        {
            foreach (Building building in buildings)
            {
                building.PlayerBehind = Rectangle.Intersect(building.walkBehind, player.boundingBox) != Rectangle.Empty;
            }
        }

        public void CheckBehindObjects()
        {
            foreach (GameObject obj in gameObjects)
            {
                obj.PlayerBehind = obj.walkBehind.Contains(new Point(player.boundingBox.Center.X, player.boundingBox.Bottom ));
            }
        }
        /* Parses TMX file to create map representation */
        public void DeserializeMap()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MapSerializer));

            using (FileStream fs = new FileStream("Content/" + filename, FileMode.Open))
            {
                map = (MapSerializer)serializer.Deserialize(fs);
            }

            GetBuildings();
            GetTriggers();
            GetSpawnPoint();

        }

        public void GetTriggers()
        {
            ObjectLayer triggersLayer = map.ObjectLayers.FirstOrDefault(layer => layer.name.ToLower() == "triggers", null);
            if (triggersLayer == null || triggersLayer.objects == null) return;

            foreach (TiledObject trigger in triggersLayer.objects)
            {
                Point location = new Point(trigger.x, trigger.y) * new Point((int)Game1.scale);
                Point size = new Point(trigger.width, trigger.height) * new Point((int)Game1.scale);

                Rectangle bounds = new Rectangle(location, size);
                Property[] pList = trigger.properties.properties; // don't even ask why i have to do this
                string name = pList.First(prop => prop.name.ToLower() == "name").value;
                
                if (name == "exit")
                { 
                    string goToScene = pList.FirstOrDefault(prop => prop.name.ToLower() == "map", null).value;
                    triggers.Add(new ChangeSceneTrigger(goToScene, bounds, goToScene));
                }
            }

        }

        public void GetSpawnPoint()
        {
            ObjectLayer spawnPoints = map.ObjectLayers.FirstOrDefault(layer => layer.name.ToLower() == "spawn", null);
            if (spawnPoints == null || spawnPoints.objects == null) { spawnLocation = new Vector2(100, 100); return; }

            foreach (TiledObject spawnPointObj in spawnPoints.objects)
            {
                Vector2 spawnPoint = new Vector2(spawnPointObj.x, spawnPointObj.y);
                spawnLocation = spawnPoint;
            }
        }

        public void GetBuildings()
        {
            ObjectLayer buildingLayer = map.ObjectLayers.FirstOrDefault(layer => layer.name.ToLower() == "buildings", null);
            //Checks if there is a building layer
            if (buildingLayer == null || buildingLayer.objects == null) { return; }

            // for each object in the building layer, add it to the list of buildings
            foreach (TiledObject building in buildingLayer.objects)
            {
                Property[] pList = building.properties.properties; // don't even ask why i have to do this
                string name = pList.First(prop => prop.name.ToLower() == "name").value;

                Vector2 pos = new Vector2(building.x / tilesize, building.y / tilesize);
                buildings.Add(new Building(name, pos));
            }
        }
        public void Update(GameTime gameTime)
        {
            foreach (Building building in buildings) building.Update(gameTime);
            foreach (GameObject obj in gameObjects) obj.Update(gameTime);
            foreach (Trigger t in triggers) t.CheckEnter();
            CheckBehindBuilding();
            CheckBehindObjects();
        }

        public void Draw(SpriteBatch b)
        {
           float layer = 0.01f; 
           foreach (Tilemap tilemap in tilemaps) { tilemap.Draw(b, layer+=0.01f); }
           foreach (Building building in buildings) { building.Draw(b); }
           foreach (GameObject obj in gameObjects) { obj.Draw(b); }
        }

        public bool AddPlant(string plantName, Vector2 tile)
        {
            // check if the tile is tilled
            if (tillLayer.Layer[(int)tile.X, (int)tile.Y] == Renderable.empty) return false;

            //check if there is already something there
            if (FindGameObjectAtTile(tile.ToPoint()) != null) return false;

            // add the plant to the map
            gameObjects.Add(new Plant(plantName, tile));
            return true;
        }

        public void RemovePlant(Vector2 tile)
        {
            GameObject obj = FindGameObjectAtTile(tile.ToPoint());
            if ( obj == null || !(obj is Plant)) return;
            gameObjects.Remove(obj);
        }
        public GameObject FindGameObjectAtTile(Point tile)
        {

            return gameObjects.FirstOrDefault(i => i.LocalPos == tile.ToVector2(), null);
        }

        public override string ToString()
        {
            return filename;
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


            [XmlElement("properties")]
            public PropertyList properties { get; set; }

            [XmlAttribute("name")]
            public string Name { get; set; }

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
