using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoXNA44;
using Microsoft.Xna.Framework;
using OgmoXNA4;
using Lapins.Engine.World;
using Lapins.Engine.Core;
using OgmoXNA4.Layers;
using OgmoXNA4.Layers.Settings;
using Lapins.Data.Objects;
using Microsoft.Xna.Framework.Content;
using System.IO;
using OgmoXNA4.Values;
using Lapins.Data.Ogmo;

namespace Lapins.Data.Levels
{
    /// <summary>
    /// Load levels from Ogmo and convert them into usable levels
    /// </summary>
    public static class LevelLoader
    {
        /// <summary>
        /// Game content manager
        /// </summary>
        public static ContentManager Content { get; set; }

        private static Dictionary<string, Type> _ogmoObjectsDictionary;

        static LevelLoader()
        {
            // Find types with an ogmo attribute
            _ogmoObjectsDictionary = (
                                        from t in typeof(LevelLoader).Assembly.GetTypes() // Get all types...
                                        let attributes = t.GetCustomAttributes(true).Where(a => a is OgmoObjectIdAttribute).ToArray()
                                        where attributes != null && attributes.Length > 0
                                        from att in attributes
                                        let objId = att as OgmoObjectIdAttribute
                                        select new
                                        {
                                            Id = objId.OgmoName,
                                            Type = t
                                        }
                                    ).ToDictionary(o => o.Id, o => o.Type);
        }

        /// <summary>
        /// Load all level parts from the content folder
        /// </summary>
        /// <returns></returns>
        public static List<LevelPart> LoadAllLevelParts(out List<LevelPart> startParts, out List<LevelPart> endParts)
        {
            List<LevelPart> levelParts = new List<LevelPart>();
            startParts = new List<LevelPart>();
            endParts = new List<LevelPart>();

            // Load all parts on disk
            //------------------------------------------------------------------------------------------------
            string levelDir = Path.Combine(Path.Combine(Content.RootDirectory, "data"), "levels");
            var dir = new DirectoryInfo(levelDir);

            foreach (FileInfo fi in dir.GetFiles())
            {
                var levelPartPath = Path.Combine("data/levels", Path.GetFileNameWithoutExtension(fi.FullName));
                LevelPart levelPart = LoadLevelPart(levelPartPath);

                if (levelPart.IsStart)
                {
                    startParts.Add(levelPart);
                }
                else if (levelPart.IsEnd)
                {
                    endParts.Add(levelPart);
                }
                else
                {
                    levelParts.Add(levelPart);

                    for (int i = 1; i < levelPart.MaxRepeat; i++)
                    {
                        levelParts.Add(levelPart.Clone());
                    }
                }
            }

            if (startParts.Count == 0) throw new ArgumentException("Missing start part");
            if (endParts.Count == 0) throw new ArgumentException("Missing end part");

            return levelParts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Ex: @"data/levels/test"</param>
        /// <returns></returns>
        private static LevelPart LoadLevelPart(string path)
        {
            return LevelLoader.Convert(Content.Load<OgmoLevel>(path));
        }

        /// <summary>
        /// Convert an OgmoLevel in a usable level
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ogmoLevel"></param>
        /// <returns></returns>
        private static LevelPart Convert(OgmoLevel ogmoLevel)
        {
            // Get level dimensions
            Vector2 dimensions = new Vector2(ogmoLevel.Width, ogmoLevel.Height);

            // Get level infos
            int id = ogmoLevel.GetValue<OgmoIntegerValue>("id").Value;
            int maxRepeat = ogmoLevel.GetValue<OgmoIntegerValue>("max_repeat").Value;

            int input = -1;
            int output = -1;
            int inputY = -1;
            int outputY = -1;
            bool isStart = false;
            bool isEnd = false;

            LevelPart levelPart = new LevelPart(id, maxRepeat, dimensions);

            // Add gfx resources
            foreach (OgmoTileset tileSet in ogmoLevel.Project.Tilesets)
            {
                Application.MagicContentManager.AddTexture(tileSet.Name, tileSet.Texture);
            }
            foreach (OgmoObjectTemplate objTemplate in ogmoLevel.Project.ObjectTemplates)
            {
                Application.MagicContentManager.AddTexture(System.IO.Path.GetFileNameWithoutExtension(objTemplate.TextureFile), objTemplate.Texture);
            }

            //Layers
            for (int i = 0; i < ogmoLevel.Layers.Length; i++)
            {
                var olayer = ogmoLevel.Layers[i];
                var olayersettings = ogmoLevel.Project.LayerSettings.Where(ls => ls.Name == olayer.Name).First();

                // Background tiles
                if (olayer.Name == "background")
                {
                    ExtractTileLayer(levelPart.BackgroundLayer, olayer, olayersettings);
                }
                // Background objects
                else if (olayer.Name == "background_objects")
                {
                    ExtractObjectLayer(levelPart.BackgroundLayer, olayer, olayersettings);
                }
                //Collisions
                else if (olayer.Name == "collision")
                {
                    levelPart.CollisionsMask = ExtractGridRectLayer(olayer, olayersettings);
                }
                // Middleground tiles
                else if (olayer.Name == "floor")
                {
                    ExtractTileLayer(levelPart.MiddleGroundLayer, olayer, olayersettings);
                }
                // Game objects (static)
                else if (olayer.Name == "static_objects")
                {
                    ExtractObjectLayer(levelPart.MiddleGroundLayer, olayer, olayersettings);
                }
                // Game objects 
                else if (olayer.Name == "objects")
                {
                    ExtractObjectLayer(levelPart.MiddleGroundLayer, olayer, olayersettings);
                }
                // Middleground tiles
                else if (olayer.Name == "foreground")
                {
                    ExtractTileLayer(levelPart.ForegroundLayer, olayer, olayersettings);
                }
            }

            // Get input, output, start & end objects
            foreach (OgmoObjectEntity oent in levelPart.MiddleGroundLayer.Entities)
            {
                if (oent is OInput)
                {
                    input = ((OInput)oent).Id;
                    inputY = (int)((OInput)oent).Location.Y;
                }
                else if (oent is OOutput)
                {
                    output = ((OOutput)oent).Id;
                    outputY = (int)((OOutput)oent).Location.Y;
                }
                else if (oent is OShip)
                {
                    isEnd = true;
                }
                else if (oent is OSpawn)
                {
                    isStart = true;
                }
            }

            if (input == -1) throw new ArgumentException("Missing input door!");
            if (output == -1) throw new ArgumentException("Missing output door!");

            levelPart.InputId = input;
            levelPart.InputYLoc = inputY;
            levelPart.OutputId = output;
            levelPart.OutputYLoc = outputY;
            levelPart.IsStart = isStart;
            levelPart.IsEnd = isEnd;

            return levelPart;
        }

        private static List<Rectangle> ExtractGridRectLayer(OgmoLayer olayer, OgmoLayerSettings olayersettings)
        {
            List<Rectangle> floors = new List<Rectangle>();

            if (olayer is OgmoGridLayer)
            {
                var gridlayer = olayer as OgmoGridLayer;
                var gridlayersettings = olayersettings as OgmoGridLayerSettings;

                foreach (Rectangle rect in gridlayer.RectangleData)
                {
                    floors.Add(rect);
                }
            }

            return floors;
        }

        private static void ExtractTileLayer(Layer layer, OgmoLayer olayer, OgmoLayerSettings olayersettings)
        {
            if (olayer is OgmoTileLayer)
            {
                var tilelayer = olayer as OgmoTileLayer;
                var tilelayersettings = olayersettings as OgmoTileLayerSettings;

                if (tilelayersettings.ExportTileIDs == true) throw new ApplicationException("Unsupported option : exportIds=true");

                // Create a grid with right options
                var tileDim = new Vector2(tilelayersettings.GridSize, tilelayersettings.GridSize);

                // We need to know the grid dimension before creating it, but we have to compute it from all tiles...
                List<KeyValuePair<Vector2, Tile>> tiles = new List<KeyValuePair<Vector2, Tile>>();

                foreach (OgmoTile otile in tilelayer.Tiles)
                {
                    Rectangle source = new Rectangle(otile.TextureOffset.X, otile.TextureOffset.Y, otile.Tileset.TileWidth, otile.Tileset.TileHeight);

                    if (otile.RectSize != Vector2.Zero)
                    {
                        // Create multiple tiles from this one
                        var tileSize = Grid.WorldToGrid(otile.RectSize, tileDim);

                        for (int x = 0; x < tileSize.X; x++)
                        {
                            for (int y = 0; y < tileSize.Y; y++)
                            {
                                Vector2 tileGridLocation = Grid.WorldToGrid(otile.Position, tileDim);
                                tileGridLocation += new Vector2(x, y);

                                var newTile = new Tile(otile.Tileset.Name, source);

                                tiles.Add(new KeyValuePair<Vector2, Tile>(tileGridLocation, newTile));
                            }
                        }
                    }
                    else
                    {
                        Vector2 tileGridLocation = Grid.WorldToGrid(otile.Position, tileDim);
                        var newTile = new Tile(otile.Tileset.Name, source);

                        tiles.Add(new KeyValuePair<Vector2, Tile>(tileGridLocation, newTile));
                    }


                }

                foreach (KeyValuePair<Vector2, Tile> tile in tiles)
                {
                    layer.Tiles.SetTile(tile.Key, tile.Value);
                }

            }
        }

        private static void ExtractObjectLayer(Layer layer, OgmoLayer olayer, OgmoLayerSettings olayersettings)
        {
            if (olayer is OgmoObjectLayer)
            {
                var objlayer = olayer as OgmoObjectLayer;
                var objlayersettings = olayersettings as OgmoObjectLayerSettings;

                foreach (OgmoObject oo in objlayer.Objects)
                {
                    OgmoObjectEntity newObject = null;

                    if (oo.IsTiled)
                    {
                        throw new ArgumentException("Tiled object are not supported !");
                        ////Find template
                        //foreach (OgmoObjectTemplate template in ogmoLevel.Project.ObjectTemplates)
                        //{
                        //    if (template.Name == oo.Name)
                        //    {
                        //        newSprite = context.SpriteFactory.CreateSprite(context, oo.Name, oo.Position, oo.Source, new Vector2(template.Width, template.Height));
                        //        break;
                        //    }
                        //}
                    }
                    else
                    {
                        newObject = InstanciateOgmoObject(oo);
                        //newObject = InstanciateOgmoObject(oo.Name, oo.Position, oo.Source);
                    }


                    layer.Entities.Add(newObject);
                }


            }
        }

        /// <summary>
        /// From an Ogmo object, instanciate it
        /// </summary>
        /// <returns></returns>
        public static OgmoObjectEntity InstanciateOgmoObject(OgmoObject oobject)
        {
            Type type = null;

            if (_ogmoObjectsDictionary.TryGetValue(oobject.Name, out type))
            {
                var constructor = type.GetConstructor(new Type[] { typeof(Vector2) });
                var obj = constructor.Invoke(new object[] { oobject.Position });

                var ogmoObject = obj as OgmoObjectEntity;

                foreach (var value in oobject.Values)
                {
                    ogmoObject.OnValueFound(value.Name, value);
                }

                if (obj is OgmoMovingObjectEntity)
                {
                    var movObj = obj as OgmoMovingObjectEntity;

                    // Add nodes
                    foreach (OgmoNode node in oobject.Nodes)
                    {
                        movObj.Points.Add(node.Position);
                    }

                    // Add current location
                    movObj.Points.Add(oobject.Position);

                }

                return ogmoObject;
            }

            throw new ArgumentException("Invalid object ID: " + oobject.Name);
        }

        /// <summary>
        /// External Ogmo object instanciation from name and location
        /// </summary>
        /// <returns></returns>
        public static OgmoObjectEntity InstanciateOgmoObjectFromId(string name, Vector2 location)
        {
            Type type = null;

            if (_ogmoObjectsDictionary.TryGetValue(name, out type))
            {
                var constructor = type.GetConstructor(new Type[] { typeof(Vector2) });
                var obj = constructor.Invoke(new object[] { location });

                var ogmoObject = obj as OgmoObjectEntity;
                return ogmoObject;
            }

            throw new ArgumentException("Invalid object ID: " + name);
        }
    }
}
