using Lapins.Data.Localization;
using Lapins.Engine.Core;
using Lapins.Engine.Graphics;
using Lapins.Engine.Input;
using Lapins.Engine.Input.Devices;
using Microsoft.Xna.Framework;

namespace Lapins.GameStates
{
    public class HighscoreState : GameState
    {
        public int Rank { get; set; }
        public bool UseRank { get; set; }

        private string _highscoreStr;
        private Vector2 _highscoreLoc;
        private Rectangle _srcMenu, _dstMenu;

        private int _lastHighscore;
        private float _currentAlpha;
        
        public HighscoreState()
            : base()
        {
            _dstMenu = new Rectangle(0, 600, 254, 64);
            _srcMenu = new Rectangle(0, 0, 127, 32);
        }

        protected override void LoadContent()
        {
        }

        protected override void InternalLoad()
        {
            if (UseRank == false)
            {
                Rank = Game.Saver.Data.Highscores.ScoreLines.Length + 1;
            }
            UseRank = false;

            _highscoreStr = LocalizedStrings.GetString("Highscores");
            _highscoreLoc = new Vector2(
                Resolution.VirtualWidth / 2 - Application.MagicContentManager.Font.MeasureString(_highscoreStr).X / 2,
                50
                );

            _lastHighscore = 0;
            _currentAlpha = 0f;

            SceneCamera.FadeOut(30f, null);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_lastHighscore < Game.Saver.Data.Highscores.ScoreLines.Length)
            {
                _currentAlpha += 0.05f;

                if (_currentAlpha >= 1f)
                {
                    _currentAlpha = 0f;
                    _lastHighscore++;
                }
            }
            else
            {
                foreach (Device device in Application.InputManager.GetLinkedDevices(LogicalPlayerIndex.One))
                {
                    if ((device.GetState(MappingButtons.A).IsReleased) || (device.GetState(MappingButtons.B).IsReleased) || (device.GetState(MappingButtons.Start).IsReleased) || (device.GetState(MappingButtons.X).IsReleased))
                    {
                        SceneCamera.FadeIn(30, () =>
                        {
                            ChangeCurrentGameState = true;
                            NextGameState = Application.GameStateManager.GetGameState<HomeState>();
                        });
                    }
                }
            }
        }

        public override void Draw(SpriteBatchProxy spriteBatch)
        {
            ParticuleManager.Draw(true);

            spriteBatch.BeginNoCamera();

            // Bakcground
            spriteBatch.Draw(Application.MagicContentManager.GetTexture("splash1"), new Rectangle(0, 0, Resolution.VirtualWidth, Resolution.VirtualHeight), Color.White);

            // Highscores
            spriteBatch.DrawString(Application.MagicContentManager.Font, _highscoreStr, _highscoreLoc, Color.Black);

            var scoreLines = Game.Saver.Data.Highscores.ScoreLines;

            int y = (int)_highscoreLoc.Y + 50;
            for (int i = 0; i < scoreLines.Length; i++)
            {
                if (i > _lastHighscore) continue;

                Color color = Color.Black;
                Color carotColor = Color.White;

                if (Rank == i)
                {
                    color = Color.Orange;
                }
                else if (i == _lastHighscore)
                {
                    color *= _currentAlpha;
                    carotColor *= _currentAlpha;
                }

                // Little carrot for each line
                spriteBatch.Draw(Application.MagicContentManager.GetTexture("hud"), new Vector2(_highscoreLoc.X - 115, y - 8), new Rectangle(64, 0, 32, 32), carotColor);

                spriteBatch.DrawString(Application.MagicContentManager.Font, scoreLines[i].ToString(), new Vector2(_highscoreLoc.X - 75, y), color);
                y += 45;
            }
            spriteBatch.End();
            //-----------------------------

            // Draw back button
            if (_lastHighscore >= Game.Saver.Data.Highscores.ScoreLines.Length)
            {
                spriteBatch.BeginNoCamera();
                spriteBatch.Draw(Application.MagicContentManager.GetTexture("menu"), _dstMenu, _srcMenu, Color.White, 0f, Vector2.Zero, Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally, 1.0f);

                spriteBatch.Draw(Application.MagicContentManager.GetTexture("hud"), _dstMenu.Location.ToVector2() + new Vector2(20, 16), new Rectangle(64, 0, 32, 32), Color.White);
                spriteBatch.DrawString(Application.MagicContentManager.Font,
                                        LocalizedStrings.GetString("Back"),
                                        _dstMenu.Location.ToVector2() + new Vector2(60, 20),
                                        Color.Yellow
                                       );

                spriteBatch.End();
            }
            ParticuleManager.Draw(false);
        }

        public override bool ChangeCurrentGameState { get; protected set; }
        public override GameState NextGameState { get; protected set; }
    }
}
