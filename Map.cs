using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectPalladium.Buildings;
using ProjectPalladium.Items;
using ProjectPalladium.Plants;
using ProjectPalladium.Triggers;
using ProjectPalladium.Utils;
using ProjectPalladium.Animation;
using ProjectPalladium.Characters;
using Circle = ProjectPalladium.Utils.Util.Circle;
using Trigger = ProjectPalladium.Utils.Trigger;
using System.Buffers;
using System.Xml.Linq;
using ProjectPalladium.Stations;


namespace ProjectPalladium
{
    /* Contains Map data about a given location (tilemaps, terrain, objects) */
    public class Map
    {
        public string filename;

        public static int scaledTileSize;
        public List<Tilemap> tilemaps = new List<Tilemap>();
        public List<Tilemap> collidingTilemaps = new List<Tilemap>();

        public Tilemap tillLayer;

        public List<Building> buildings = new List<Building>();
        public List<GameObject> gameObjects = new List<GameObject>();
        private Stack<GameObject> objectsToRemove = new Stack<GameObject>();
        private Stack<Character> charactersToRemove = new Stack<Character>();

        MapSerializer map;
        XmlSerializer serializer;

        public Player player;

        public Point tileMapSize = new Point();
        public static int tilesize = 16;

        protected List<Trigger> triggers = new List<Trigger>();


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
                Tilemap tmap;

                if (isTillLayer)
                {
                    tmap = new Tilemap(layer.Data.Text, tileMapSize, layer.Name, isCollideLayer, isTillLayer, imageName:"TilledDirt");
                }
                else
                {
                    tmap = new Tilemap(layer.Data.Text, tileMapSize, layer.Name, isCollideLayer, isTillLayer);
                }

                tilemaps.Add(tmap);
                if (tmap.isCollideLayer) collidingTilemaps.Add(tmap);
                if (tmap.isTillLayer) this.tillLayer = tmap;
            }

            scaledTileSize = (int)(tilesize * Game1.scale);
        }

        // return first object a circle collides with on the map
        public Object checkCollisions(Circle c)
        {
            foreach (Tilemap t in collidingTilemaps)
            {
                foreach (Rectangle r in t.colliders) { if (c.Intersects(r)) return r; }
            }
            foreach (Character character in SceneManager.CurScene.Characters)
            {
                if (c.Intersects(character.boundingBox)) return character;
            }
            foreach (Building b in buildings)
            {
                if (c.Intersects(b.bounds)) return b;
            }
            foreach (GameObject g in gameObjects)
            {
                if (c.Intersects(g.bounds)) return g;
            }
            return null; // null means no intersections
        }

        // check collisions for projectiles
        public Object checkCollisions(Projectile p)
        {
            foreach (Tilemap t in collidingTilemaps)
            {
                foreach (Rectangle r in t.colliders) 
                {
                    if (p.hitbox.Intersects(r, includeTotalIntersection: false)) return r; 
                }
            }
            foreach (Character character in SceneManager.CurScene.Characters)
            {
                if (character == p.owner) continue;
                if (p.hitbox.Intersects(character.boundingBox, includeTotalIntersection: false)) return character;
            }
            foreach (Building b in buildings)
            {
                if (b == p.owner) continue;
                if (p.hitbox.Intersects(b.bounds, includeTotalIntersection: false)) return b;
            }
            foreach (GameObject g in gameObjects)
            {
                if (g == p.owner) continue;
                if (p.hitbox.Intersects(g.bounds, includeTotalIntersection: false)) return g;
            }
            return null; // null means no intersections
        }

        // checks collisions with any collidable objects on the map
        // returns rectangle of intersection of first found collision
        public List<Rectangle> CheckCollisions(Rectangle boundingBox)
        {
            List<Rectangle> intersections = new List<Rectangle>();
            Rectangle totalInstersection = Rectangle.Empty;
            foreach (Building building in buildings)
            {
                CheckSingleCollision(building.bounds, boundingBox, intersections);
            }

            foreach(GameObject gameObject in gameObjects)
            {
                if (gameObject.bounds == null || gameObject.bounds == Rectangle.Empty) continue;
                CheckSingleCollision(gameObject.bounds, boundingBox, intersections);
            }
            // check tilemaps for collision
            foreach (Tilemap tilemap in collidingTilemaps)
            {
                foreach (Rectangle collider in tilemap.colliders)
                {
                   CheckSingleCollision(collider, boundingBox, intersections);
                }
            }

            foreach (Character c in SceneManager.CurScene.Characters)
            {
                if (c.boundingBox == boundingBox) continue; // dont collide with ourself
                CheckSingleCollision(c.boundingBox, boundingBox, intersections);
            }
            if (boundingBox != player.boundingBox)
            {
                CheckSingleCollision(boundingBox, player.boundingBox, intersections);
            }
            return intersections;
        }

        public void CheckSingleCollision(Rectangle r1,  Rectangle r2, List<Rectangle> intersections)
        {
            Rectangle intersection = Rectangle.Intersect(r1, r2);
            if (intersection != Rectangle.Empty) intersections.Add(intersection);
        }

        // if the player is behind a building, turn down opacity
        public void CheckBehindBuilding()
        {
            foreach (Building building in buildings)
            {
                building.PlayerBehind = Rectangle.Intersect(building.walkBehind, player.boundingBox) != Rectangle.Empty;
            }
        }

        /* Update the "behind objects" property of buildings and gameobjects */
        public void CheckBehindObjects()
        {
            foreach (GameObject obj in gameObjects)
            {
                if (obj is Plant)
                {
                    obj.PlayerBehind = obj.walkBehind.Contains(new Point(player.boundingBox.Center.X, player.boundingBox.Bottom));
                }
                else
                {
                    obj.PlayerBehind = obj.walkBehind.Intersects(player.boundingBox);
                }
            }
        }

        /* Parses TMX file to create map representation */
        public void DeserializeMap()
        {
            serializer = new XmlSerializer(typeof(MapSerializer));

            using (FileStream fs = new FileStream("Content/maps/" + filename, FileMode.Open))
            {
                map = (MapSerializer)serializer.Deserialize(fs);
            }

            GetBuildings();
            GetTriggers();
            GetSpawnPoint();
            LoadGameObjects();
        }

        /* Helper function to load all GameObjects from a tmx file */
        public void LoadGameObjects()
        {
            if (map.ObjectLayers == null) return;

            ObjectLayer gameObjectLayer = map.ObjectLayers.FirstOrDefault(p => p.name == "gameobjects", null);
            if (gameObjectLayer == null) return;

            TiledObject[] objects = gameObjectLayer.objects;
            if (objects == null) return;

            foreach (TiledObject obj in objects)
            {
                // this object is a plant
                if(obj.GetType().Name == "PlantSerialized") 
                {
                    PlantSerialized plantSerialized = obj as PlantSerialized;
                    gameObjects.Add(new Plant(plantSerialized));
                }
            }
            
        }

        // everything that updates when a gametime tick occurs goes here
        public void UpdateOnGameTime()
        {
            foreach (GameObject g in gameObjects)
            {
                if (g is Plant)
                {
                    Plant p = g as Plant;
                    p.UpdateOnGameTime();
                }
                if (g is Station)
                {
                    Station st = g as Station;
                    st.UpdateOnGameTime();
                }
            }
        }
        
        /* Loads triggers from tmx file */
        public void GetTriggers()
        {
            if (map.ObjectLayers == null) return;
            ObjectLayer triggersLayer = map.ObjectLayers.FirstOrDefault(layer => layer.name.ToLower() == "triggers", null);
            if (triggersLayer == null || triggersLayer.objects == null) return;

            foreach (TiledObject trigger in triggersLayer.objects)
            {
                Point location = new Point(trigger.x, trigger.y) * new Point((int)Game1.scale);
                Point size = new Point(trigger.width, trigger.height) * new Point((int)Game1.scale);

                Rectangle bounds = new Rectangle(location, size);
                Property[] pList = trigger.properties.properties; // don't even ask why i have to do the

                string name = pList.First(prop => prop.name.ToLower() == "name").value;
                if (name == "exit")
                {
                    string linkID;
                    Property link = pList.FirstOrDefault(prop => prop.name.ToLower() == "link", null);
                    if (link != null)
                    {
                        linkID = pList.FirstOrDefault(prop => prop.name.ToLower() == "link", null).value;
                    }
                    else
                    {
                        linkID = "";
                    }

                    string goToScene = pList.FirstOrDefault(prop => prop.name.ToLower() == "map", null).value;
                    triggers.Add(new ChangeSceneTrigger(goToScene, bounds, goToScene, linkID));
                }
            }

        }

        /* Loads Spawn point from TMX file */
        public void GetSpawnPoint()
        {
            if (map.ObjectLayers == null) return;
            ObjectLayer spawnPoints = map.ObjectLayers.FirstOrDefault(layer => layer.name.ToLower() == "spawn", null);
            if (spawnPoints == null || spawnPoints.objects == null) {
                Debug.Write("no default spawn pos");
                return;
                //throw new Exception("no spawn point found");
            }

            foreach (TiledObject spawnPointObj in spawnPoints.objects)
            {
                if (spawnPointObj.properties == null) continue;
                Property name = spawnPointObj.properties.properties.FirstOrDefault(x => x.name == "name", null);
                if (name == null || name.value != "spawnpoint") continue;
                Vector2 spawnPoint = new Vector2(spawnPointObj.x, spawnPointObj.y);
                spawnLocation = spawnPoint;
            }
        }

        /* Loads buildings from TMX file */
        public void GetBuildings()
        {
            if (map.ObjectLayers == null) return;
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

        public void LoadNPCs()
        {
            AddNPC("mage");
        }

        public void AddNPC(string name)
        {
            Debug.WriteLine("adding NPC");
            SceneManager.CurScene.Characters.Add(new Villager(name));
        }

        /* Called when the map is loaded via the scenemanager */
        public void OnLoad()
        {
            LoadNPCs();
        }
        public void Update(GameTime gameTime)
        {
            Enemy.UpdateStaticItems();

            foreach (Building building in buildings) building.Update(gameTime);
            foreach (GameObject obj in gameObjects) obj.Update(gameTime);
            foreach (Trigger t in triggers) t.CheckEnter();
            CheckBehindBuilding();
            CheckBehindObjects();

            DoStagedChanges();

        }

        public void Draw(SpriteBatch b)
        {
           float layer = 0.01f; 
           foreach (Tilemap tilemap in tilemaps) { tilemap.Draw(b, layer+=0.01f); }
           foreach (Building building in buildings) { building.Draw(b); }
           foreach (GameObject obj in gameObjects) {
                obj.Draw(b); 
            
            }
        }

    

        public bool AddEnemy(string name, Vector2 pos)
        {
           

            SceneManager.CurScene.Characters.Add(
                new Enemy(
                    new AnimatedSprite(16, 16, "enemies/" + name + "anims", name),
                    pos,
                    name,
                    SceneManager.CurScene.Map,
                    new Vector2(-5, -5) * Game1.scale,
                    new Vector2(10, 10) * Game1.scale
                ));

            return true;
        }
        /* Interface to add new plant to map */
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

        public bool AddGameObject(string itemName, Vector2 tile, Type worldObjectType)
        {
            // create placeable object of dynamic type
            // depending on the type, the constructor will need to have different parameters
            string textureName = itemName.Replace(" ", "").ToLower() + "placed";
            object[] parameters;
            if (worldObjectType == typeof(PlaceableGameObject))
            {
                parameters = new object[] { itemName, tile, textureName };
            }
            else
            {
                parameters = new object[] { itemName, tile };
            }
            var newObj = Activator.CreateInstance(worldObjectType, parameters) as PlaceableGameObject;

            if (FindGameObjectAtTile(tile.ToPoint()) != null) return false;
            if (tile.ToPoint() == player.feet) return false; // shouldn't be able to place buildings under us
            if (CheckCollisions(newObj.bounds).Count != 0)
            {
                Debug.WriteLine("stopped from placing");
                return false;
            }
           

            gameObjects.Add(newObj);

            // add this to the list of enemy dangers
            Enemy.AddDanger(Enemy.DecayType.InverseSquare, 0.1f, newObj.bounds.Center.ToVector2(),
                (Math.Max(newObj.bounds.Width, newObj.bounds.Height) + (5 * Game1.scale)) / Game1.scale);

            return true;
        }

        public bool RemoveGameObject(GameObject obj)
        {
            if (obj == null) return false;
            foreach (GameObject x in gameObjects)
            {
                if (obj == x)
                {
                    objectsToRemove.Push(x);
                    return true;
                }
            }
            return false; // the provided object does not exist on the map
        }

        public bool RemoveCharacter(Character c)
        {
            if (c == null) return false;
            charactersToRemove.Push(c); return true;
            
        }
        private void DoStagedChanges()
        {
            if (objectsToRemove.Count > 0)
            {
                foreach (GameObject obj in objectsToRemove)
                {
                    gameObjects.Remove(obj);
                    Enemy.RemoveDanger(obj);
                }
                objectsToRemove.Clear();
            }
            if (charactersToRemove.Count > 0)
            {
                foreach(Character x in charactersToRemove)
                {
                    SceneManager.CurScene.Characters.Remove(x);
                }
                charactersToRemove.Clear();
            }
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

        /* Helper function for tilemap xml serialization */
        private void SaveTilemaps()
        {
            Layer[] layers = new Layer[tilemaps.Count];

            // update data of mapserializer
            for (int i = 0; i < tilemaps.Count; i++)
            {
                Layer curLayer = new Layer();
                string tileData = tilemaps[i].GetSerializedTileData();

                Data curData = new Data();
                curData.Text = tileData;
                curData.Encoding = "csv";

                curLayer.Data = curData;
                
                curLayer.Width = tilemaps[i].MapTileSize.X;
                curLayer.Height = tilemaps[i].MapTileSize.Y;
                curLayer.Name = tilemaps[i].name;
                curLayer.properties = map.Layers[i].properties; // properties are static, so just copy them from the original file.

                layers[i] = curLayer;
            }

            map.Layers = layers; // update the new layer
        }

        /* Helper function for GameObject xml serialization */
        private void SaveObjects()
        {
            TiledObject[] gObjList = new TiledObject[gameObjects.Count];
            int i = 0;
            foreach (GameObject obj in gameObjects)
            {

                if (obj is Plant)
                {
                    Plant pobj = (Plant)obj;

                    PlantSerialized p = new PlantSerialized();
                    p.growthStage = pobj.GrowthStage;
                    p.timeSinceLastGrowth = pobj.TimeSinceLastGrowth;
                    p.name = pobj.name.ToLower().Replace(" ", "") + "plant";

                    p.x = (int)pobj.globalPos.X;
                    p.y = (int)pobj.globalPos.Y;

                    p.id = i;

                    gObjList[i] = p;
                }
                i++;
            }
            ObjectLayer gObjLayer = new ObjectLayer();
            gObjLayer.name = "gameobjects";
            gObjLayer.objects = gObjList;

            map.ObjectLayers[1] = gObjLayer;
        }

        /* Save map to TMX file */
        public void Save(string fileName)
        {

            SaveTilemaps();

            SaveObjects();

            fileName = "Content\\" + fileName;
            // Combine with current directory to get the full path
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            using (TextWriter writer = new StreamWriter(fullPath))
            {
                serializer.Serialize(writer, map);
            }           
        }

        // get the spawnPos for a trigger based on the map it is going to's tmx file.
        public static Vector2 GetAssociatedSpawnPosFromLink(string mapName, string linkID)
        {
            // partially deserialize map to access properties
            MapSerializer map;
            Vector2 spawnPos = Vector2.Zero;

            XmlSerializer serializer = new XmlSerializer(typeof(MapSerializer));

            if (mapName == "hollow") mapName = "hollowdefault"; // since triggers are unchanging, just use the default version for simplicity

            using (FileStream fs = new FileStream("Content/maps/" + mapName + ".tmx", FileMode.Open))
            {
                map = (MapSerializer)serializer.Deserialize(fs);

                ObjectLayer triggersLayer = map.ObjectLayers.FirstOrDefault(x => x.name.ToLower() == "spawn", null);

                foreach (TiledObject spawnObject in triggersLayer.objects)
                {
                    if (spawnObject.properties == null) continue;
                    Property[] pList = spawnObject.properties.properties;
                    string thisLinkID;

                    
                        Property p = pList.FirstOrDefault(prop => prop.name.ToLower() == "link");
                        if (p == null) continue;
                        thisLinkID = pList.FirstOrDefault(prop => prop.name.ToLower() == "link").value;
                    
                    
                    if (thisLinkID == linkID) // it is the associated spawn object for this cst
                    {
                        spawnPos = new Vector2(spawnObject.x, spawnObject.y);
                    }
                }
            }

            return spawnPos;
        }
        public override string ToString()
        {
            return filename;
        }

        [Serializable]
        public class Test
        {
            public string Name { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            // Add other properties as needed
        }

        [XmlRoot("map")]
        [Serializable]
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

        [XmlInclude(typeof(PlantSerialized))]
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

        public class PlantSerialized : TiledObject
        {
            [XmlAttribute("name")]
            public string name { get; set; }

            [XmlAttribute("growthstage")]
            public int growthStage { get; set; }

            [XmlAttribute("timesincelastgrowth")]
            public int timeSinceLastGrowth { get; set; }
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
