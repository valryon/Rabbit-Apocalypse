using Lapins.Data.Ogmo;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;
using Lapins.Engine.Content;
using Lapins.Engine.Graphics;
using Lapins.Data.Particules;
using Lapins.Engine.Core;
using OgmoXNA4.Values;

namespace Lapins.Data.Objects
{
    /// <summary>
    /// MGS box
    /// </summary>
    [OgmoObjectId("box")]
    [TextureContent(AssetName = "box", AssetPath = "gfxs/objects/box")]
    public class OBox : OgmoObjectEntity
    {
        private bool _activated;
        public bool Activated
        {
            get
            {
                return _activated;
            }
            set
            {
                _activated = value;
                if (value && animation == null)
                {
                    animation = new SpriteAnimation(SrcRect, 3, 200, 1);
                }
            }
        }

        public bool Trapped { get; set; }


        public OBox(Vector2 location)
            : base("box", location)
        {
            hitbox = new Hitbox(new Rectangle(10, 35, 40, 30));
            Activated = false;
            LayerDepth = 130;
        }

        public override void OnValueFound(string id, OgmoValue value)
        {
            if (id == "flip")
            {
                if (((OgmoBooleanValue)value).Value)
                {
                    flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                }
            }
        }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 64, 64); }
        }

        public override void Update(GameTime gameTime)
        {
            if (Activated)
            {
                animation.Update(gameTime);
                if (animation.Over)
                {
                    Activated = false;
                    Trapped = true;

                    //Smoke just to hide rabbit sprite transition (protips)
                    SpecialEffectsHelper.MakeSmoke(dRect.Center.ToVector2(),
                        Application.Random.GetRandomVector2(0, 50, 0, 50),
                        Application.Random.GetRandomFloat((double)(scale.X / 4), (double)scale.X),
                        Color.Beige, false, 6);
                }
            }
            base.Update(gameTime);
        }


        public override Engine.World.Entity Clone()
        {
            return new OBox(location)
            {
                Flip = flip
            };
        }


    }
}
