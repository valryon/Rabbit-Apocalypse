using Lapins.Data.Ogmo;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;
using Lapins.Engine.Content;
using Lapins.Engine.Graphics;

namespace Lapins.Data.Objects
{
    /// <summary>
    /// Life !
    /// </summary>
    [OgmoObjectId("life")]
    [TextureContent(AssetName = "life", AssetPath = "gfxs/objects/life")]
    public class OLife : OgmoObjectEntity
    {
        public OLife(Vector2 location)
            : base("life", location)
        {
            hitbox = new Hitbox(BaseSrcRect);
            animation = new SpriteAnimation(sRect, 2, 500f);
        }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 16, 16); }
        }



        public override Engine.World.Entity Clone()
        {
            return new OLife(location);
        }

    }
}
