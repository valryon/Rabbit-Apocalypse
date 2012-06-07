using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lapins.Engine.Core;
using Lapins.Engine.Graphics;
using Microsoft.Xna.Framework;
using Lapins.Engine.Content;
using Lapins.Engine.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace Lapins.GameStates
{
    /// <summary>
    /// Display splashscreens
    /// </summary>
    [TextureContent(AssetName = "splash1", AssetPath = "gfxs/splashscreens/splash1_tgt")]
    [TextureContent(AssetName = "team", AssetPath = "gfxs/splashscreens/teamname")]
    [SoundEffectContent(AssetName = "s_gameboy", AssetPath = "sfxs/gameboy")]
    public class SplashscreensState : GameState
    {
#if DEBUG
        private static bool ShowSplashScreen = false;
#else
        private static bool ShowSplashScreen = true;
#endif
        private float _currentAlpha;
        private double _time;
        private Rectangle _src, _dst;

        protected override void LoadContent()
        {
        }

        protected override void InternalLoad()
        {
            if (ShowSplashScreen)
            {
                _currentAlpha = 0;
                _time = 0;
                _src = new Rectangle(0, 0, 375, 106);
                _dst = new Rectangle((int)(Resolution.VirtualWidth - _src.Width) / 2, (Resolution.VirtualHeight - _src.Height) / 2, _src.Width, _src.Height);

                SceneCamera.FadeOut(70f, () =>
                {
                    // GB sound
                    Application.MagicContentManager.GetSound("s_gameboy").Play(0.5f, 0f, 0f);

                    Timer.Create(3f, false, (t =>
                    {
                        SceneCamera.FadeIn(120f, () =>
                        {

                            ChangeCurrentGameState = true;
                            NextGameState = Application.GameStateManager.GetGameState<HomeState>();
                            SceneCamera.FadeOut(80f, null);

                        });
                    }));
                });
            }
            else
            {
                ChangeCurrentGameState = true;
                NextGameState = Application.GameStateManager.GetGameState<HomeState>();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_currentAlpha < 1)
            {
                _time += gameTime.ElapsedGameTime.Milliseconds;
                if (_time > 20)
                {
                    _time = 0;
                    _currentAlpha += 0.01f;

                }
            }
            base.Update(gameTime);
        }



        public override void Draw(SpriteBatchProxy spriteBatch)
        {
            ParticuleManager.Draw(true);

            spriteBatch.BeginNoCamera();

            spriteBatch.Draw(Application.MagicContentManager.GetTexture("splash1"), new Rectangle(0, 0, Resolution.VirtualWidth, Resolution.VirtualHeight), Color.White);
            spriteBatch.Draw(Application.MagicContentManager.GetTexture("team"), _dst, _src, new Color(1, 1, 1, _currentAlpha));

            spriteBatch.End();

            ParticuleManager.Draw(false);
        }

        public override bool ChangeCurrentGameState { get; protected set; }

        public override GameState NextGameState { get; protected set; }
    }
}
