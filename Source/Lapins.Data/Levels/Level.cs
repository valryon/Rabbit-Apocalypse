using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lapins.Engine.Graphics;
using Lapins.Engine.Core;
using Lapins.Engine.World;
using Lapins.Data.Objects;
using Lapins.Data.Entities;
using Lapins.Engine.Physics;
using Lapins.Engine.Utils;

namespace Lapins.Data.Levels
{
    /// <summary>
    /// A level description.
    /// Grids + objects on ~layers
    /// </summary>
    public class Level
    {
        /// <summary>
        /// Currently played level
        /// </summary>
        public static Level CurrentLevel = null;

        protected Layer _backgroundLayer;
        protected Layer _middleGroundLayer;
        protected Layer _foregroundLayer;
        protected Dictionary<Rectangle, List<Floor>> _collisionsMask;

        public Vector2 SpawnLocation { get; set; }
        public Player Player { get; private set; }
        public Rectangle Dimensions { get; set; }

        public Level()
        {
            _backgroundLayer = new Layer();
            _middleGroundLayer = new Layer();
            _foregroundLayer = new Layer();
            _collisionsMask = new Dictionary<Rectangle, List<Floor>>();
            TempCollisionsMask = new List<Floor>();

            Dimensions = Rectangle.Empty;
        }

        public void Update(GameTime gameTime)
        {
            // Physics
            Application.PhysicsManager.IsEnable = true;
            Application.PhysicsManager.WorldEntities = MiddleGroundLayer.Entities.Where(e => e.IsUpdatable()).ToList();
            Application.PhysicsManager.FloorCollisions = CollisionsMask;
            Application.PhysicsManager.AdditionalFloor.Clear();

            BackgroundLayer.Tiles.Update(gameTime);
            BackgroundLayer.Entities.ForEach(t => updateEntity(gameTime, BackgroundLayer, t));

            MiddleGroundLayer.Tiles.Update(gameTime);
            MiddleGroundLayer.Entities.ForEach(t =>
            {
                updateEntity(gameTime, MiddleGroundLayer, t);

                if ((t.IsAlive) && (t.IsUpdatable()))
                {
                    if (t.Floor != null)
                    {
                        Application.PhysicsManager.AdditionalFloor.Add(t.Floor);
                    }
                }

            });

            ForegroundLayer.Tiles.Update(gameTime);
            ForegroundLayer.Entities.ForEach(t => updateEntity(gameTime, ForegroundLayer, t));
        }

        private void updateEntity(GameTime gameTime, Layer layer, Entity t)
        {
            if (t.IsUpdatable())
            {
                t.Update(gameTime);

                // Destroy everything outside the level
                if (t.Hitbox != null)
                {
                    var box = t.Hitbox.Dimensions;
                    bool alive = Dimensions.Intersects(box) || Dimensions.Contains(box);

                    if ((!alive) && (!t.IsInvincible))
                    {
                        t.IsAlive = false;
                    }
                }

                if ((t.IsAlive == false) && (t.IsRemovable))
                {
                    layer.Entities.Remove(t);
                }
            }
        }

        public void Draw(SpriteBatchProxy spriteBatch)
        {
            BackgroundLayer.Entities.ForEach(t => t.Draw(spriteBatch));
            BackgroundLayer.Tiles.Draw(spriteBatch);

            MiddleGroundLayer.Tiles.Draw(spriteBatch);
            MiddleGroundLayer.Entities.ForEach(t => t.Draw(spriteBatch));

            ForegroundLayer.Entities.ForEach(t => t.Draw(spriteBatch));
            ForegroundLayer.Tiles.Draw(spriteBatch);

            if (Application.IsDebugMode)
            {
                if ((Player != null) && (Player.Hitbox != null))
                {
                    // Draw collisions around player only
                    Rectangle playerHitbox = Player.Hitbox.Dimensions;

                    foreach (Rectangle zone in _collisionsMask.Keys)
                    {
                        if (zone.Intersects(playerHitbox) || zone.Contains(playerHitbox))
                        {
                            foreach (Floor floor in _collisionsMask[zone])
                            {
                                spriteBatch.DrawRectangle(floor.Rectangle, Color.Blue);
                            }
                        }
                    }
                }

                spriteBatch.DrawRectangle(Dimensions, Color.Black * 0.25f);
            }
        }

        public void IndexFloor(int parts)
        {
            // Divide the level in as many parts as we had blocs before
            int dimX = (int)(Dimensions.Width / (float)parts);

            for (int i = 0; i < dimX; i++)
            {
                Rectangle globalRect = new Rectangle(
                    i * dimX,
                    0,
                    dimX,
                    Dimensions.Height
                    );

                List<Floor> floors = new List<Floor>();

                foreach (Floor floor in TempCollisionsMask)
                {
                    if (globalRect.Contains(floor.Rectangle) || globalRect.Intersects(floor.Rectangle))
                    {
                        floors.Add(floor);
                    }
                }

                _collisionsMask.Add(globalRect, floors);
            }

            TempCollisionsMask.Clear();
        }

        #region External access

        /// <summary>
        /// Add entity to the given layer
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="entity"></param>
        public void AddEntity(Layer layer, Entity entity, bool sort)
        {
            if (entity is Player && layer == _middleGroundLayer)
            {
                Player = entity as Player;
            }

            layer.Entities.Add(entity);

            if (sort)
            {
                layer.Entities.Sort((e1, e2) => { return e1.LayerDepth.CompareTo(e2.LayerDepth); });
            }
        }

        /// <summary>
        /// Remove entity from the level
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="entity"></param>
        public bool RemoveEntity(Layer layer, Entity entity)
        {
            return layer.Entities.Remove(entity);
        }

        /// <summary>
        /// Set a tile in the grid on the given layer
        /// </summary>
        /// <param name="layer"></param>
        public void SetTile(Layer layer, Vector2 coords, Tile tile)
        {
            layer.Tiles.SetTile(coords, tile);
        }

        /// <summary>
        /// Set a tile in the grid on the given layer
        /// </summary>
        /// <param name="layer"></param>
        public void ClearTile(Layer layer, Vector2 coords)
        {
            layer.Tiles.ClearTile(coords);
        }

        #endregion

        public Layer BackgroundLayer
        {
            get { return _backgroundLayer; }
        }

        public Layer MiddleGroundLayer
        {
            get { return _middleGroundLayer; }
        }

        public Layer ForegroundLayer
        {
            get { return _foregroundLayer; }
        }

        public List<Floor> TempCollisionsMask
        {
            get;
            private set;
        }

        public Dictionary<Rectangle, List<Floor>> CollisionsMask
        {
            get { return _collisionsMask; }
        }
    }
}
