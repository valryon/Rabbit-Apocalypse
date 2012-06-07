using Lapins.Data.Ogmo;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;
using OgmoXNA4.Values;
using Lapins.Engine.Content;

namespace Lapins.Data.Objects
{
    /// <summary>
    /// Moving platform
    /// </summary>
    [OgmoObjectId("platform")]
    public class OPlatform : OgmoMovingObjectEntity
    {
        public OPlatform(Vector2 location)
            : this(location, new Rectangle(0, 0, 64, 16))
        {

        }

        public OPlatform(Vector2 location, Rectangle size)
            : base("oplatform", location)
        {
            dRect.Width = size.Width;
            dRect.Height = size.Height;

            hitbox = new Hitbox(size);
            Floor = new Floor(Rectangle.Empty)
            {
                IsMoving = true,
                IsPassable = true
            };
        }

        public override void OnValueFound(string id, OgmoValue value)
        {
            // Speed
            if (id == "speed")
            {
                Speed = ((OgmoNumberValue)value).Value;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Floor.Rectangle = hitbox.Dimensions;
            Floor.Movement = velocity;
        }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 64, 16); }
        }

        public override Engine.World.Entity Clone()
        {
            var platform = new OPlatform(location);

            platform.Points.AddRange(Points);
            platform.Speed = Speed;

            return platform;
        }
    }

    [OgmoObjectId("small_platform")]
    public class OSmallPlatform : OPlatform
    {
        public OSmallPlatform(Vector2 location)
            : base(location, new Rectangle(0, 0, 32, 16))
        {
        }

        public override Engine.World.Entity Clone()
        {
            var platform = new OSmallPlatform(location);

            platform.Points.AddRange(Points);
            platform.Speed = Speed;

            return platform;
        }
    }

    [OgmoObjectId("big_platform")]
    public class OBigPlatform : OPlatform
    {
        public OBigPlatform(Vector2 location)
            : base(location, new Rectangle(0, 0, 96, 32))
        {
        }

        public override Engine.World.Entity Clone()
        {
            var platform = new OBigPlatform(location);

            platform.Points.AddRange(Points);
            platform.Speed = Speed;

            return platform;
        }
    }
}
