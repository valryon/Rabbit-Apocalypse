using Lapins.Engine.Particules;
using Microsoft.Xna.Framework;
using Lapins.Engine.Core;

namespace Lapins.Data.Particules
{
    /// <summary>
    /// Simple colored smoke
    /// </summary>
    public class Smoke : Particule
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="location"></param>
        /// <param name="trajectory"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        /// <param name="icon">[0,4]</param>
        /// <param name="background"></param>
        public Smoke(Vector2 location, Vector2 trajectory, float scale, Color color, bool background)
            : base(
                location,
                trajectory,
                new Rectangle(Application.Random.GetRandomInt(0, 4) * 64, 0, 64, 64),
                1f,
                scale,
                color,
                background
            )
        {
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Slowly move the smoke
            if (TimeToLive < 500f)
            {
                if (trajectory.Y < -10.0f) trajectory.Y += elapsedTime * 500f;
                if (trajectory.X < -10.0f) trajectory.X += elapsedTime * 150f;
                if (trajectory.Y > 10.0f) trajectory.Y += elapsedTime * 150f;
            }

            alpha -= elapsedTime;
            if (alpha < 0) alpha = 0f;

            base.Update(gameTime);
        }

    }
}
