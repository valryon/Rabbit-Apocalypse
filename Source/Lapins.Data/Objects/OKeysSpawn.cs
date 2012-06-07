using Lapins.Data.Levels;
using Lapins.Data.Ogmo;
using Lapins.Engine.Content;
using Lapins.Engine.Physics;
using Lapins.Engine.World;
using Microsoft.Xna.Framework;

namespace Lapins.Data.Objects
{
    /// <summary>
    /// Potential keys spawn
    /// </summary>
    [OgmoObjectId("keysSpawn")]
    public class OKeysSpawn : OgmoObjectEntity
    {
        public OKeysSpawn(Vector2 location)
            : base("", location)
        { }

        public override void Update(GameTime gameTime)
        {
            //Spawn keys
            Keys keys = new Keys(location);
            Level.CurrentLevel.AddEntity(Level.CurrentLevel.MiddleGroundLayer, keys, false);

            IsAlive = false;
        }

        public override void Draw(Engine.Graphics.SpriteBatchProxy spriteBatch)
        { }

        public override Rectangle BaseSrcRect
        {
            get { return Rectangle.Empty; }
        }

        public override Engine.World.Entity Clone()
        {
            return new OKeysSpawn(location);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [TextureContent(AssetName = "keys", AssetPath = "gfxs/entities/keys")]
    public class Keys : Entity
    {
        public Keys(Vector2 location)
            : base("keys", location, new Rectangle(0, 0, 32, 32), Vector2.One)
        {
            hitbox = new Hitbox(SrcRect);
        }
        public override Engine.World.Entity Clone()
        {
            return new Keys(location);
        }
    }
}
