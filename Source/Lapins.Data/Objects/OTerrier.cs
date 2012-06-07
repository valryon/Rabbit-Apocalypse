using Lapins.Data.Ogmo;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;

namespace Lapins.Data.Objects
{
    /// <summary>
    /// Terrier teleporter
    /// </summary>
    [OgmoObjectId("terrier")]
    public class OTerrier : OgmoObjectEntity
    {
        /// <summary>
        /// Linked terrier for teleportation
        /// </summary>
        public OTerrier LinkedTerrier { get; set; }

        public OTerrier(Vector2 location)
            : base("oterrier", location)
        {
            hitbox = new Hitbox(new Rectangle(40, 50, 65, 80));
            LayerDepth = 50;
        }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 128, 128); }
        }

        public override Engine.World.Entity Clone()
        {
            return new OTerrier(location);
        }

    }
}
