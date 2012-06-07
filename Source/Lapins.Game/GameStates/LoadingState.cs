using System;
using Lapins.Data.Localization;
using Lapins.Engine.Core;
using Lapins.Engine.Graphics;
using Lapins.Engine.Input;
using Lapins.Engine.Input.Devices;
using Microsoft.Xna.Framework;

namespace Lapins.GameStates
{
    public class LoadingState : LoadingGameState
    {
        private float _alpha, _alphaDelta;
        private string _titleStr;
        private Vector2 _titleLoc;

        protected override void InternalLoad()
        {
            _alpha = 0f;
            _alphaDelta = 0.025f;

            _titleStr = LocalizedStrings.GetString("GameTitle");
            _titleLoc = centerString(_titleStr, 50);

            SceneCamera.FadeOut(30f, null);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _alpha += _alphaDelta;
            if (_alpha > 1)
            {
                _alpha = 1;
                _alphaDelta = -_alphaDelta;
            }
            else if (_alpha < 0)
            {
                _alpha = 0;
                _alphaDelta = -_alphaDelta;
            }

            if (IsNextGameStateLoadingComplete)
            {
                foreach (Device device in Application.InputManager.GetLinkedDevices(LogicalPlayerIndex.One))
                {
                    if ((device.GetState(MappingButtons.A).IsPressed) || (device.GetState(MappingButtons.B).IsPressed) || (device.GetState(MappingButtons.Y).IsPressed) || (device.GetState(MappingButtons.Start).IsPressed))
                    {
                        SceneCamera.FadeIn(30, () =>
                        {
                            ChangeCurrentGameState = true;
                        });
                    }
                }
            }
        }

        public override void Draw(SpriteBatchProxy spriteBatch)
        {
            spriteBatch.BeginNoCamera();

            // Bakcground
            spriteBatch.Draw(Application.MagicContentManager.GetTexture("splash1"), new Rectangle(0, 0, Resolution.VirtualWidth, Resolution.VirtualHeight), Color.White);

            // Draw story
            spriteBatch.DrawString(Application.MagicContentManager.Font, _titleStr, _titleLoc, Color.Black);

            Vector2 storyLoc = new Vector2(350, 100);
            spriteBatch.DrawString(Application.MagicContentManager.Font, LocalizedStrings.GetString("Story1"), storyLoc, Color.Green);

            storyLoc.Y += 50;
            storyLoc.X = 120;
            spriteBatch.DrawString(Application.MagicContentManager.Font, LocalizedStrings.GetString("Story2"), storyLoc, Color.Black);
            storyLoc.Y += 30;
            spriteBatch.DrawString(Application.MagicContentManager.Font, LocalizedStrings.GetString("Story3"), storyLoc, Color.Black);
            storyLoc.Y += 30;
            spriteBatch.DrawString(Application.MagicContentManager.Font, LocalizedStrings.GetString("Story4"), storyLoc, Color.Black);
            storyLoc.Y += 30;
            spriteBatch.DrawString(Application.MagicContentManager.Font, LocalizedStrings.GetString("Story5"), storyLoc, Color.Black);
            storyLoc.Y += 30;
            spriteBatch.DrawString(Application.MagicContentManager.Font, LocalizedStrings.GetString("Story6"), storyLoc, Color.Black);


            String str = LocalizedStrings.GetString("Story7");
            storyLoc = centerString(str, storyLoc.Y + 50);
            spriteBatch.DrawString(Application.MagicContentManager.Font, str, storyLoc, Color.Green);

            str = LocalizedStrings.GetString("Story8");
            storyLoc = centerString(str, storyLoc.Y + 30);
            spriteBatch.DrawString(Application.MagicContentManager.Font, str, storyLoc, Color.Black);

            // Draw wait 
            str = LocalizedStrings.GetString("home_pressstart");

            if (IsNextGameStateLoadingComplete == false)
            {
                str = LocalizedStrings.GetString("Loading");
            }

            Vector2 loc = centerString(str, 525);
            spriteBatch.DrawString(Application.MagicContentManager.Font, str, loc, Color.Blue * _alpha);

            spriteBatch.End();
        }

        private Vector2 centerString(string str, float y)
        {
            return new Vector2((Resolution.VirtualWidth - Application.MagicContentManager.Font.MeasureString(str).X) / 2, y); ;
        }
    }
}
