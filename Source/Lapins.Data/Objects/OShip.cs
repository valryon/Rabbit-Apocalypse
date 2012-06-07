using Lapins.Data.Ogmo;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;
using Lapins.Data.Particules;
using Lapins.Engine.Core;
using Lapins.Engine.Content;
using Lapins.Engine.Graphics;
using System;

namespace Lapins.Data.Objects
{
    [OgmoObjectId("ship")]
    [TextureContent(AssetName = "ship", AssetPath = "gfxs/objects/ship")]
    public class OShip : OgmoObjectEntity
    {
        private double _question;

        public OShip(Vector2 location)
            : base("ship", location)
        {
            hitbox = new Hitbox(BaseSrcRect);
            LayerDepth = 50;
            _question = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_question > 0)
            {
                _question -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        public override void Draw(SpriteBatchProxy spritebatch)
        {
            base.Draw(spritebatch);

            if (_question > 0)
            {
                spritebatch.Draw(Application.MagicContentManager.GetTexture("ship"), new Rectangle((int)Location.X+ 68, (int)Location.Y + 111, 64, 64), new Rectangle(0, 192, 64, 64), Color.White);
            }
        }


        public void Question()
        {
            _question = 5000;
        }

        public override Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 192, 192); }
        }

        public override Engine.World.Entity Clone()
        {
            return new OShip(location);
        }



    }
}
