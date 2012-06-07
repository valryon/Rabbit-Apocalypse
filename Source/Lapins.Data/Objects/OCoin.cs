using Lapins.Data.Ogmo;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;

namespace Lapins.Data.Objects
{
    [OgmoObjectId("coin")]
    public class OCoin : OgmoObjectEntity
    {
        public OCoin(Vector2 location)
            : base("ocoin", location)
        {
            hitbox = new Hitbox(new Rectangle(0, 0, 16, 16));
        }

        public override Engine.World.Entity Clone()
        {
            return new OCoin(location);
        }

        public override Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 16, 16); }
        }
    }
}
