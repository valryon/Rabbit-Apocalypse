using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lapins.Engine.Particules;
using Microsoft.Xna.Framework;
using Lapins.Engine.Core;

namespace Lapins.Data.Particules
{
    public class Piece : Particule
    {
        public Piece(Vector2 location, Vector2 trajectory, float scale, Color color, bool background)
            : base(location, trajectory, new Rectangle(Application.Random.GetRandomInt(0, 4) * 16, 192, 16, 16), 3f, scale, color, background)
        {

        }

        public override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            trajectory.Y += 50f;

            // Fade out
            alpha -= elapsedTime;
            if (alpha < 0) alpha = 0f;

            base.Update(gameTime);
        }
    }
}
