using Lapins.Data.Ogmo;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;
using OgmoXNA4.Values;
using Lapins.Engine.Core;

namespace Lapins.Data.Objects
{
    [OgmoObjectId("input")]
    public class OInput : OgmoObjectEntity
    {
        public int Id { get; set; }

        public OInput(Vector2 location)
            : base("oinput", location)
        {

        }

        public override void OnValueFound(string id, OgmoValue value)
        {
            if (id == "id")
            {
                Id = ((OgmoIntegerValue)value).Value;
            }
        }

        public override void Draw(Engine.Graphics.SpriteBatchProxy spriteBatch)
        {
            if (Application.IsDebugMode)
            {
                base.Draw(spriteBatch);
                spriteBatch.DrawString(Application.MagicContentManager.Font, Id.ToString(), location, Color.Red);
            }
        }

        public override Engine.World.Entity Clone()
        {
            return new OInput(location)
            {
                Id = Id
            };
        }

        public override Rectangle BaseSrcRect
        {
            get { return new Rectangle(0,0,32,32); }
        }
    }

    [OgmoObjectId("output")]
    public class OOutput : OgmoObjectEntity
    {
        public int Id { get; set; }

        public OOutput(Vector2 location)
            : base("ooutput", location)
        {

        }

        public override void OnValueFound(string id, OgmoValue value)
        {
            if (id == "id")
            {
                Id = ((OgmoIntegerValue)value).Value;
            }
        }

        public override void Draw(Engine.Graphics.SpriteBatchProxy spriteBatch)
        {
            if (Application.IsDebugMode)
            {
                base.Draw(spriteBatch);
                spriteBatch.DrawString(Application.MagicContentManager.Font, Id.ToString(), location, Color.Red);
            }
        }

        public override Engine.World.Entity Clone()
        {
            return new OOutput(location)
            {
                Id = Id
            };
        }

        public override Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 32, 32); }
        }
    }
}
