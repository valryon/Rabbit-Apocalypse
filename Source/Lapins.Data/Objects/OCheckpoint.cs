using Lapins.Data.Ogmo;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;
using Lapins.Engine.Core;

namespace Lapins.Data.Objects
{
    [OgmoObjectId("checkpoint")]
    public class OCheckpoint : OgmoObjectEntity
    {
        public OCheckpoint(Vector2 location)
            : base("", location)
        {
           
        }
        public override void Draw(Engine.Graphics.SpriteBatchProxy spriteBatch)
        {
            if (Application.IsDebugMode)
            {
                spriteBatch.DrawRectangle(dRect, Color.Violet);
            }
        }

        public override Engine.World.Entity Clone()
        {
            return new OCheckpoint(location);
        }

        public override Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 64, 64); }
        }
    }
}
