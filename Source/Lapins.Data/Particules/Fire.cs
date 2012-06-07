using Lapins.Engine.Particules;
using Microsoft.Xna.Framework;
using Lapins.Engine.Core;

namespace Lapins.Data.Particules
{
    public class Fire : Particule
    {
        private int _flag;
        private float _drawSize;

        public Fire(Vector2 location, Vector2 trajectory, float scale, bool background)
            : base(
                location,
                trajectory,
                new Rectangle(Application.Random.GetRandomInt(0, 4) * 64, 64, 64, 64),
                0.5f,
                scale,
                Color.White,
                background
            )
        {
            _flag = 0;
            IsAdditive = true;
        }

        public override void Update(GameTime gameTime)
        {
            //MAJ ttl
            base.Update(gameTime);

            // From yellow to red
            float r, g, b;

            src.X = _flag * 64;
            float tsize;

            if (TimeToLive > 0.4f)
            {
                r = 1.0f;
                g = 1.0f;
                b = (TimeToLive - 0.4f) * 10.0f;

                if (TimeToLive > 0.45f)
                    tsize = (0.5f - TimeToLive) * scale * 20.0f;
                else
                    tsize = scale;
            }
            else if (TimeToLive > 0.3f)
            {
                r = 1.0f;
                g = (TimeToLive - 00.0f) * 10.0f;
                b = 0.0f;
                tsize = scale;
            }
            else
            {
                r = TimeToLive * 3.3f;
                g = 0.0f;
                b = 0.0f;
                tsize = (TimeToLive * 0.3f) * scale;
            }

            if (_flag % 2 == 0)
            {
                rotation = (TimeToLive * 7.0f + scale * 20.0f);
            }
            else
            {
                rotation = (-TimeToLive * 11.0f + scale * 20.0f);
            }

            //ndDam : Magic code from a book

            color = new Color(r, g, b);
            _drawSize = tsize;
        }

    }
}
