using Lapins.Data.Ogmo;
using Lapins.Engine.Content;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;

namespace Lapins.Data.Objects
{
    /// <summary>
    /// Trap for wolf... or rabbits !
    /// </summary>
    [OgmoObjectId("wolftrap")]
    [TextureContent(AssetName = "wolftrap", AssetPath = "gfxs/entities/wolftrap")]
    public class OWolfTrap : OgmoObjectEntity
    {
        public OWolfTrap(Vector2 location)
            : base("wolftrap", location)
        {
            hitbox = new Hitbox(new Rectangle(0, 20, 32, 12));
            LayerDepth = 110;
        }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 32, 32); }
        }

        public override Engine.World.Entity Clone()
        {
            return new OWolfTrap(location);
        }

        internal void Activate()
        {
            hitbox = null;
            sRect.X = 32;
        }
    }
}
