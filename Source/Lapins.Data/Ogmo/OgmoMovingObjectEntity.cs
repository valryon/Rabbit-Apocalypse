using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lapins.Engine.Physics;

namespace Lapins.Data.Ogmo
{
    /// <summary>
    /// Moving entity designed with Ogmo
    /// </summary>
    public abstract class OgmoMovingObjectEntity : OgmoObjectEntity
    {
        /// <summary>
        /// Pattern points
        /// </summary>
        public List<Vector2> Points { get; set; }

        /// <summary>
        /// Movement speed
        /// </summary>
        public float Speed { get; set; }
        protected Vector2 velocity;
        private int _index;
        private Vector2 _direction;

        public OgmoMovingObjectEntity(string assetName, Vector2 location)
            : base(assetName, location)
        {
            Points = new List<Vector2>();
            _index = 0;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);

            // Move object along the rail
            if (Points.Count > 0)
            {
                Vector2 currentPoint = Points[_index];

                // Find the direction to move
                _direction = Vector2.Zero;
                Vector2 movementCenter = Location;

                if (movementCenter.X < currentPoint.X)
                {
                    _direction.X = 1;
                }
                else if (movementCenter.X > currentPoint.X)
                {
                    _direction.X = -1;
                }

                if (movementCenter.Y < currentPoint.Y)
                {
                    _direction.Y = 1;
                }
                else if (movementCenter.Y > currentPoint.Y)
                {
                    _direction.Y = -1;
                }

                // Left side touching 
                Rectangle touchingRect = new Rectangle(DstRect.Left - 10, DstRect.Top, 20, DstRect.Height);

                if (touchingRect.Contains((int)currentPoint.X, (int)currentPoint.Y))
                {
                    _index++;
                    if (_index >= Points.Count)
                        _index = 0;
                }

                //Move
                float deplacement = (Speed * elapsedTime);

                velocity = new Vector2(_direction.X * deplacement, _direction.Y * deplacement);
                location += velocity;
            }
        }
    }
}
