using Lapins.Data.Ogmo;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;
using Lapins.Engine.Content;
using Lapins.Engine.Utils;
using Lapins.Data.Particules;
using Lapins.Engine.Core;
using Lapins.Data.Entities;

namespace Lapins.Data.Objects
{
    /// <summary>
    /// Bumper
    /// </summary>
    [OgmoObjectId("bumper")]
    public class OBumper : OgmoObjectEntity
    {
        private float _cooldown;
        private bool _isPressed;

        public OBumper(Vector2 location)
            : base("bumper", location)
        {
            hitbox = new Hitbox(new Rectangle(0, 0, 32, 32));
            LayerDepth = 150;
        }

        public override void Update(GameTime gameTime)
        {
            if (_isPressed)
            {
                _cooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_cooldown <= 0)
                {
                    _isPressed = false;
                    sRect.X = 0;

                    // Smoke
                    //SpecialEffectsHelper.MakeSmoke(dRect.Center.ToVector2(), Application.Random.GetRandomVector2(-100f, 100f, 10f, 50f), 0.3f, Color.LightGray, false);
                }
            }

            base.Update(gameTime);
        }

        public virtual Vector2 BounceVector { get { return new Vector2(0, -400); } }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 32, 32); }
        }

        public override Engine.World.Entity Clone()
        {
            return new OBumper(location);
        }

        public bool IsPressed
        {
            get { return _isPressed; }
            set
            {
                _isPressed = value;
                _cooldown = 0.2f;
                sRect.X = 32;
            }
        }
    }

    /// <summary>
    /// Bumper left
    /// </summary>
    [OgmoObjectId("bumper_left")]
    public class OBumperLeft : OBumper
    {
        public OBumperLeft(Vector2 loc)
            : base(loc)
        {
        }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 64, 32, 32); }
        }

        public override Vector2 BounceVector
        {
            get
            {
                return new Vector2(-550, 10);
            }
        }

        public override Engine.World.Entity Clone()
        {
            return new OBumperLeft(location);
        }
    }

    /// <summary>
    /// Bumper right
    /// </summary>
    [OgmoObjectId("bumper_right")]
    public class OBumperRight : OBumper
    {
        public OBumperRight(Vector2 loc)
            : base(loc)
        {
        }

        public override Vector2 BounceVector
        {
            get
            {
                return new Vector2(550, 10);
            }
        }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 32, 32, 32); }
        }

        public override Engine.World.Entity Clone()
        {
            return new OBumperRight(location);
        }
    }

    /// <summary>
    /// Bumper right
    /// </summary>
    [OgmoObjectId("bumper_down")]
    [TextureContent(AssetName = "bumper", AssetPath = "gfxs/objects/bumper")]
    public class OBumperDown : OBumper
    {
        public OBumperDown(Vector2 loc)
            : base(loc)
        {
        }

        public override Vector2 BounceVector
        {
            get
            {
                return new Vector2(0, 350);
            }
        }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 96, 32, 32); }
        }

        public override Engine.World.Entity Clone()
        {
            return new OBumperDown(location);
        }
    }
}
