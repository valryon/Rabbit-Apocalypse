using Lapins.Data.Entities;
using Lapins.Data.Levels;
using Lapins.Data.Ogmo;
using Lapins.Engine.Graphics;
using Lapins.Engine.World;
using Microsoft.Xna.Framework;
using Lapins.Data.Particules;
using Lapins.Engine.Core;

namespace Lapins.Data.Objects
{
    /// <summary>
    /// Player spawn
    /// </summary>
    [OgmoObjectId("spawn")]
    public class OSpawn : OgmoObjectEntity
    {
        private const float SpawnCooldownValue = 2f;
        private float spawnCooldown;

        public OSpawn(Vector2 location)
            : base("", location)
        {
            spawnCooldown = SpawnCooldownValue;
        }

        public override void Update(GameTime gameTime)
        {
            spawnCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spawnCooldown <= 0)
            {
                // Make a player spawn
                var player = new Player();
                player.Initialize(location - new Vector2(0, player.DstRect.Height));
                Level.CurrentLevel.AddEntity(Level.CurrentLevel.MiddleGroundLayer, player, true);

                SpecialEffectsHelper.ShakeScreen(new Vector2(4f, 3f), 0.50f);

                // Smoke effect
                SpecialEffectsHelper.MakeCircularExplosion(location, 100f, 2f, Color.White, false, true);

                IsAlive = false;
            }
        }

        public override void Draw(SpriteBatchProxy spriteBatch)
        {
            // Do not draw spawn
        }

        public override Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 0, 0); }
        }

        public override Entity Clone()
        {
            return new OSpawn(location);
        }
    }
}
