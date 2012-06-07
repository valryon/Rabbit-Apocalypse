using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lapins.Engine.World;
using Microsoft.Xna.Framework;
using Lapins.Engine.Core;
using Lapins.Engine.Content;
using Lapins.Engine.Physics;
using Lapins.Data.Particules;
using Lapins.Data.Levels;

namespace Lapins.Data.Entities
{
    [TextureContent(AssetName = "asteroid", AssetPath = "gfxs/entities/asteroid")]
    public class Asteroid : PhysicsEntity
    {
        public Asteroid(Vector2 location, Vector2 speed, Vector2 scale, bool collidable)
            : base("asteroid", location, new Rectangle(Application.Random.GetRandomInt(0, 2) * 64, 0, 64, 64), speed, scale)
        {
            mass = Application.Random.GetRandomFloat(5f, 50f); //scale and weight could be linked,

            if (collidable)
            {
                int w = DstRect.Width;
                int h = DstRect.Height;
                hitbox = new Hitbox(new Rectangle(-w / 4, -h / 4, DstRect.Width - 2 * w / 4, DstRect.Height - 2 * h / 4));
            }

            FloorCollisionDetected = floorCollisionHandler;

            SetSpriteOriginToMiddle();
        }

        public override void EntityDetected(Entity collider, Microsoft.Xna.Framework.Vector2 depth)
        {
            explode();
        }

        public override void CollisionDetected(Entity collider, Microsoft.Xna.Framework.Vector2 depth)
        {
            if (collider is Player)
            {
                explode();
            }
        }

        private void floorCollisionHandler(Vector2 depth)
        {
            explode();
        }

        private void explode()
        {
            bool onScreen = IsOnScreen();

            IsAlive = false;

            if (onScreen)
            {
                SpecialEffectsHelper.MakeCircularExplosion(DstRect.Center.ToVector2(), 150f * scale.X, scale.X, Color.Gray, false, onScreen);

                // Make the screen shake if the explosion is near the player
                var player = Level.CurrentLevel.Player;
                if (player != null)
                {
                    float distance = (player.Location - location).Length();

                    if (distance <= 256)
                    {
                        SpecialEffectsHelper.ShakeScreen(5f * scale, 0.8f);
                    }
                }
            }
        }

        public override Entity Clone()
        {
            return new Asteroid(location, speed, scale, hitbox != null);
        }
    }
}
