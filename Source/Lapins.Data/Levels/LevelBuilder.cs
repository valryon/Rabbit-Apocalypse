using System;
using System.Collections.Generic;
using System.Linq;
using Lapins.Data.Objects;
using Lapins.Data.Ogmo;
using Lapins.Engine.Core;
using Lapins.Engine.World;
using Microsoft.Xna.Framework;
using Lapins.Engine.Physics;

namespace Lapins.Data.Levels
{
    /// <summary>
    /// Create a unique and generated level
    /// </summary>
    public static class LevelBuilder
    {
        private static int PartsCount = 25;

        /// <summary>
        /// Generate a new level from all level parts
        /// </summary>
        /// <returns></returns>
        public static Level CreateLevel()
        {
            List<LevelPart> startParts;
            List<LevelPart> endParts;

            List<LevelPart> levelParts = LevelLoader.LoadAllLevelParts(out startParts, out endParts);

            // Select a random start
            var startPart = startParts[Application.Random.GetRandomInt(startParts.Count)];

            // Select parts in order
            //------------------------------------------------------------------------------------------------
            List<LevelPart> selectedParts = SelectAndOrderParts(levelParts, startPart);

            // Finish with a random end tile
            var endPart = endParts[Application.Random.GetRandomInt(endParts.Count)];
            selectedParts.Add(endPart);

            // Generate a level
            //------------------------------------------------------------------------------------------------
            Level newLevel = new Level();
            LevelPart previousPart = null;

            int x = 0;
            int previousZeroLevel = 0;
            Rectangle dimensions = Rectangle.Empty;

            foreach (var part in selectedParts)
            {
                // Find the zero level for a part
                // We just have to take the previous output door location
                int yZeroLevel = 0;

                if (previousPart != null)
                {
                    yZeroLevel = (previousPart.OutputYLoc + previousZeroLevel) - part.InputYLoc;
                }

                previousPart = part;
                previousZeroLevel = yZeroLevel;

                IntegrateCollisions(newLevel, part.CollisionsMask, x, yZeroLevel);
                IntegratePart(newLevel, newLevel.BackgroundLayer, part.BackgroundLayer, x, yZeroLevel);
                IntegratePart(newLevel, newLevel.MiddleGroundLayer, part.MiddleGroundLayer, x, yZeroLevel);
                IntegratePart(newLevel, newLevel.ForegroundLayer, part.ForegroundLayer, x, yZeroLevel);

                x += (int)part.Dimensions.X;

                // Dimensions
                dimensions = Rectangle.Union(dimensions, new Rectangle(x, yZeroLevel, (int)part.Dimensions.X, (int)part.Dimensions.Y));
            }

            dimensions.Inflate(300, 300);
            newLevel.Dimensions = dimensions;

            // Index collisions
            newLevel.IndexFloor(PartsCount * 4);

            // Sort entites by drawing order
            newLevel.BackgroundLayer.Entities.Sort((e1, e2) => { return e1.LayerDepth.CompareTo(e2.LayerDepth); });
            newLevel.MiddleGroundLayer.Entities.Sort((e1, e2) => { return e1.LayerDepth.CompareTo(e2.LayerDepth); });
            newLevel.ForegroundLayer.Entities.Sort((e1, e2) => { return e1.LayerDepth.CompareTo(e2.LayerDepth); });

            PlaceRandomLetters(newLevel);
            SelectKeysSpawn(newLevel);
            LinkTerriers(newLevel);

            return newLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelParts"></param>
        /// <param name="startPart"></param>
        /// <returns></returns>
        private static List<LevelPart> SelectAndOrderParts(List<LevelPart> levelParts, LevelPart startPart)
        {
            // TODO Find the right number
            int partsLeft = PartsCount;
            bool isBlocked = false;
            List<LevelPart> selectedParts = new List<LevelPart>();

            List<int> invalidInputs = new List<int>();
            List<int> invalidOutputs = new List<int>();
            Random rand = new Random(DateTime.Now.Millisecond);

            // From the start part, build a level !
            selectedParts.Add(startPart);

            while (partsLeft > 1 && isBlocked == false)
            {
                // Get the input or the output of the list
                int input = selectedParts.First().InputId;
                int output = selectedParts.Last().OutputId;

                // Draw a random part corresponding to those inputs/outputs
                var validParts = levelParts.Where(p => p.InputId == output || p.OutputId == input).ToList();

                if (validParts.Count == 0)
                {
                    isBlocked = true;
                    continue;
                }

                int n = rand.Next(validParts.Count);
                var part = validParts[n];

                // Try to insert it before or after the current list
                if (part.InputId == output)
                {
                    selectedParts.Add(part);
                }
                else if (part.OutputId == input)
                {
                    selectedParts.Insert(0, part);
                }
                else
                {
                    throw new ArgumentException("WTF?!");
                }

                // Remove the part from the pool
                levelParts.Remove(part);

                partsLeft--;
            }

            return selectedParts;
        }

        /// <summary>
        /// Integrate a part to the final level
        /// </summary>
        /// <param name="yRelativeLocation">The "zero" y of level, determined by the spawn</param>
        private static void IntegrateCollisions(Level level, List<Rectangle> floors, int gridLeft, int yRelativeLocation)
        {
            foreach (Rectangle rect in floors)
            {
                Rectangle floorRect = new Rectangle(rect.X + gridLeft, rect.Y + yRelativeLocation, rect.Width, rect.Height);

                Floor floor = new Floor(floorRect);
                level.TempCollisionsMask.Add(floor);
            }
        }

        /// <summary>
        /// Integrate a part to the final level
        /// </summary>
        /// <param name="yRelativeLocation">The "zero" y of level, determined by the spawn</param>
        private static void IntegratePart(Level level, Layer levelLayer, Layer partLayer, int gridLeft, int yRelativeLocation)
        {
            // Translate everything with the zero level
            Vector2 translation = new Vector2(gridLeft, yRelativeLocation);

            foreach (Entity e in partLayer.Entities)
            {
                e.Location += translation;

                if (e is OSpawn)
                {
                    level.SpawnLocation = e.Location;
                }

                if (e is OgmoMovingObjectEntity)
                {
                    List<Vector2> points = new List<Vector2>();
                    points.AddRange(((OgmoMovingObjectEntity)e).Points);

                    ((OgmoMovingObjectEntity)e).Points.Clear();

                    foreach (Vector2 p in points)
                    {
                        Vector2 p2 = p + translation;
                        ((OgmoMovingObjectEntity)e).Points.Add(p2);
                    }
                }

                levelLayer.Entities.Add(e);
            }

            // Translate and copy tiles
            List<Vector2> cleaning = new List<Vector2>();

            foreach (Vector2 tileLoc in partLayer.Tiles.Tiles.Keys)
            {
                Tile tile = partLayer.Tiles.Tiles[tileLoc];

                cleaning.Add(tileLoc);

                // Translate    
                Vector2 newLoc = new Vector2(gridLeft + (tileLoc.X * (int)levelLayer.Tiles.TileSize.X), yRelativeLocation + (tileLoc.Y * (int)levelLayer.Tiles.TileSize.X));

                // Remove old tile
                if (cleaning.Contains(newLoc)) cleaning.Remove(newLoc);

                // Add a new tile
                levelLayer.Tiles.SetTile(Grid.WorldToGrid(newLoc, levelLayer.Tiles.TileSize), tile);
            }

            // Clean the grid
            foreach (Vector2 v in cleaning)
            {
                partLayer.Tiles.Tiles.Remove(v);
            }
            cleaning.Clear();
        }

        /// <summary>
        /// Link terrier between them
        /// </summary>
        /// <param name="newLevel"></param>
        private static void LinkTerriers(Level newLevel)
        {
            var terrierList = newLevel.MiddleGroundLayer.Entities.Where(e => e is OTerrier).Select(e => e as OTerrier).ToList();
            var terrierNotLinked = new List<OTerrier>();

            terrierNotLinked.AddRange(terrierList);

            if (terrierNotLinked.Count > 1)
            {
                while (terrierNotLinked.Count > 0)
                {
                    OTerrier terrier = terrierNotLinked.First();

                    // Take a random terrier to link with
                    int index = Application.Random.GetRandomInt(terrierList.Count);
                    OTerrier link = terrierList[index];

                    if (link != terrier)
                    {
                        terrier.LinkedTerrier = link;
                        terrierNotLinked.Remove(terrier);
                    }
                }
            }
        }

        /// <summary>
        /// Random key spawn selection
        /// </summary>
        /// <param name="newLevel"></param>
        private static void SelectKeysSpawn(Level newLevel)
        {
            var keysSpawnList = newLevel.MiddleGroundLayer.Entities.Where(e => e is OKeysSpawn).Select(e => e as OKeysSpawn).ToList();

            int index = Application.Random.GetRandomInt(keysSpawnList.Count);
            OKeysSpawn spawn = keysSpawnList[index];

            keysSpawnList.Remove(spawn);

            // Remove other spawn
            newLevel.MiddleGroundLayer.Entities.RemoveAll(e => keysSpawnList.Contains(e));
        }

        private static void PlaceRandomLetters(Level newLevel)
        {
            var lettersSpawnList = newLevel.MiddleGroundLayer.Entities.Where(e => e is OLetterSpawn).Select(e => e as OLetterSpawn).ToList();

            int letterPlaced = 0;

            // Select letters spawn
            for (int i = 0; i < OLetterSpawn.LetterCount; i++)
            {
                if (lettersSpawnList.Count > 0)
                {
                    int index = Application.Random.GetRandomInt(lettersSpawnList.Count);
                    OLetterSpawn spawn = lettersSpawnList[index];
                    lettersSpawnList.Remove(spawn);

                    letterPlaced++;
                }
            }

            if (letterPlaced < OLetterSpawn.LetterCount)
            {
                //Console.WriteLine("WARNING: not enough letter spawn!" + letterPlaced + "/" + OLetterSpawn.LetterCount);
            }

            // Remove others
            newLevel.MiddleGroundLayer.Entities.RemoveAll(e => lettersSpawnList.Contains(e));
        }
    }
}
