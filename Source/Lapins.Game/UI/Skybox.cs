using Lapins.Engine.Content;
using Lapins.Engine.Core;
using Lapins.Engine.Graphics;
using Lapins.GameStates;
using Microsoft.Xna.Framework;

namespace Lapins.UI
{
    /// <summary>
    /// Display a nice colored sky depending on the timeleft
    /// </summary>
    [TextureContent(AssetName = "skybox", AssetPath = "gfxs/backgrounds/skybox")]
    public class Skybox
    {
        private Color _color;
        private int r, g, b;
        private float step, cooldown;
        private int stepCount;

        public Skybox()
        {
            // Cornflowerblue values (see MSDoc)
            _color = Color.CornflowerBlue;
            r = 100;
            g = 149;
            b = 237;

            step = PlayState.MaxTime / 300; // We need ~250 steps to have the final sky

            cooldown = step;
        }

        public void Update(GameTime gameTime)
        {
            cooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // MAGIC NUMBERZ PARTY !!!!!!!!!!!!
            if (cooldown <= 0)
            {
                cooldown = step;

                stepCount++;

                r++;
                r = (int)MathHelper.Clamp(r, 0, 255);

                if ((r < 200) || (r == 255))
                {
                    b--;
                    b = (int)MathHelper.Clamp(b, 30, 255);
                }

                if (r >= 230)
                {
                    g--;
                    if (b == 30)
                    {
                        g = (int)MathHelper.Clamp(g, 30, 255);
                    }
                    else
                    {
                        g = (int)MathHelper.Clamp(g, 70, 255);
                    }
                }

                _color = new Color(r, g, b);
            }
        }

        public void Draw(SpriteBatchProxy spriteBatch)
        {
            spriteBatch.BeginNoCamera();
            spriteBatch.Draw(Application.MagicContentManager.GetTexture("skybox"), new Rectangle(0, 0, Resolution.VirtualWidth, Resolution.VirtualHeight), _color);
            spriteBatch.End();
        }
    }
}
