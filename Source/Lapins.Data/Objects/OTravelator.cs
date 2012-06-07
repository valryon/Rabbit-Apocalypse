using Lapins.Data.Ogmo;
using Lapins.Engine.Content;
using Lapins.Engine.Graphics;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;
using OgmoXNA4.Values;

namespace Lapins.Data.Objects
{
    /// <summary>
    /// Moving platform
    /// </summary>
    [OgmoObjectId("travelator")]
    [TextureContent(AssetName = "travelator", AssetPath = "gfxs/objects/travelator")]
    public class OTravelator : OgmoObjectEntity
    {

        private float _speed;
        public Vector2 Speed
        {
            get
            {
                return new Vector2(_speed, 0);
            }
            set
            {
                _speed = value.X;
                animation.FrameCooldown = 1000 - _speed;
            }
        }

        public bool IsRight { get; set; }

        public OTravelator(Vector2 location)
            : base("travelator", location)
        {
            hitbox = new Hitbox(BaseSrcRect);

            Floor = new Floor(Rectangle.Empty)
            {
                IsMoving = true
            };

            animation = new SpriteAnimation(sRect, 2, 200f);
        }

        public override void OnValueFound(string id, OgmoValue value)
        {
            if (id == "speed")
            {
                Speed = new Vector2(((OgmoNumberValue)value).Value, 0);
            }
            else if (id == "goRight")
            {
                IsRight = ((OgmoBooleanValue)value).Value;
            }
        }

        public override void Update(GameTime gameTime)
        {
            var finalSpeed = Speed;
            if (!IsRight)
            {
                finalSpeed = -Speed;
            }
            else
            {
                flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            }

            base.Update(gameTime);

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Floor.Rectangle = hitbox.Dimensions;
            Floor.Movement = (finalSpeed * elapsedTime);
        }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 64, 16); }
        }

        public override Engine.World.Entity Clone()
        {
            var travelator = new OTravelator(location);
            travelator.Speed = Speed;
            travelator.IsRight = IsRight;

            return travelator;
        }

    }
}
