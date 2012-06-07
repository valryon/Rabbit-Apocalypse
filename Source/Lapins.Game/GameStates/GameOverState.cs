using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lapins.Engine.Core;
using Lapins.Engine.Graphics;
using Microsoft.Xna.Framework;
using Lapins.Engine.Input;
using Lapins.Data.Commands;
using Lapins.Engine.Score;
using Lapins.Engine.Input.Devices;
using Lapins.Data.Localization;
using Lapins.Data.Objects;
using Lapins.Data.Music;
using Lapins.Engine.Content;

namespace Lapins.GameStates
{
    /// <summary>
    /// Display a nice game over screen with Replay / Quit and Highscores
    /// </summary>
    /// 
    [TextureContent(AssetName = "end_sad", AssetPath = "gfxs/misc/end_sad")]
    [TextureContent(AssetName = "end_happy", AssetPath = "gfxs/misc/end_happy")]
    public class GameOverState : GameState
    {
        private const int ScoreLines = 7;

        private int _lives;
        private int _score;
        private TimeSpan _timeLeft;
        private bool _hasKeys;
        private int _carrotes;
        private bool _lose;
        private int _rank;
        private List<Letter> _letters;
        private string _gameoverStr;
        private Vector2 _gameoverLoc;
        private Rectangle _srcMenu, _dstMenu;

        private int _lastItem;
        private float _currentAlpha;

        public GameOverState()
        {
            _dstMenu = new Rectangle(0, 600, 254, 64);
            _srcMenu = new Rectangle(0, 0, 127, 32);
        }

        protected override void LoadContent()
        {

        }

        protected override void InternalLoad()
        {
            _gameoverStr = LocalizedStrings.GetString("Game Over");
            _gameoverLoc = new Vector2(
                Resolution.VirtualWidth / 2 - Application.MagicContentManager.Font.MeasureString(_gameoverStr).X / 2,
                50
                );

            _lives = Application.ScriptManager.GetFlag<int>(LapinsScript.PlayerLives);
            _lives = _lives < 0 ? 0 : _lives;

            _score = Application.ScriptManager.GetFlag<int>(LapinsScript.Score);
            var timeLeft = Application.ScriptManager.GetFlag<float>(LapinsScript.TimeLeft);
            _timeLeft = TimeSpan.FromSeconds(timeLeft);

            _hasKeys = Application.ScriptManager.GetFlag<bool>(LapinsScript.HasKeys);
            _carrotes = Application.ScriptManager.GetFlag<int>(LapinsScript.CarrotsCount);

            _letters = Application.ScriptManager.GetFlag<List<Letter>>(LapinsScript.CollectedLetters);

            int escapeState = Application.ScriptManager.GetFlag<int>(LapinsScript.EscapeState);

            _lose = escapeState == 2;

            // Save score
            _rank = Game.Saver.Data.Highscores.AddScore(new ScoreLine("TGS2011", _score));
            Game.Saver.SaveAsync();

            _lastItem = 0;
            _currentAlpha = 0f;

            MusicPlayer.PlayGameOver(!_lose);

            SceneCamera.FadeOut(30f, null);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_lastItem < ScoreLines)
            {
                _currentAlpha += 0.015f;

                if (_currentAlpha >= 1f)
                {
                    _currentAlpha = 0f;
                    _lastItem++;
                }
            }
            else
            {
                // New highscore ?

                foreach (Device device in Application.InputManager.GetLinkedDevices(LogicalPlayerIndex.One))
                {
                    if ((device.GetState(MappingButtons.A).IsPressed) || (device.GetState(MappingButtons.B).IsPressed) || (device.GetState(MappingButtons.Y).IsPressed) || (device.GetState(MappingButtons.Start).IsPressed))
                    {
                        ChangeCurrentGameState = true;
                        NextGameState = Application.GameStateManager.GetGameState<HighscoreState>();
                        ((HighscoreState)NextGameState).Rank = _rank;
                        ((HighscoreState)NextGameState).UseRank = true;
                    }
                }
            }
        }

        public override void Draw(SpriteBatchProxy spriteBatch)
        {
            ParticuleManager.Draw(true);

            spriteBatch.BeginNoCamera();
            //-----------------------------

            // Bakcground
            spriteBatch.Draw(Application.MagicContentManager.GetTexture((_lose) ? "end_sad" : "end_happy"), new Rectangle(0, 0, Resolution.VirtualWidth, Resolution.VirtualHeight), Color.White);
            
            // Title
            spriteBatch.DrawString(Application.MagicContentManager.Font, _gameoverStr, _gameoverLoc, Color.White);

            Color color = Color.White;

            if (_lastItem >= 1)
            {
                string sEnd;

                if (_lose)
                {
                    color = Color.Red;
                    sEnd = LocalizedStrings.GetString("Defeat");
                }
                else
                {
                    color = Color.Green;
                    sEnd = LocalizedStrings.GetString("Victory");
                }

                if (_lastItem == 0) color *= _currentAlpha;

                var loc = new Vector2(
                    Resolution.VirtualWidth / 2 - Application.MagicContentManager.Font.MeasureString(sEnd).X / 2,
                    100
                    );

                spriteBatch.DrawString(Application.MagicContentManager.Font, sEnd, loc, color);

            }

            color = Color.White;
            int x = 200;
            int colSize = 300;
            int lineSize = 30;
            int y = 160;

            if (_lastItem >= 2)
            {
                if (_lastItem == 1) color *= _currentAlpha;

                var livesLoc = new Vector2(x + colSize, y);

                // Stats
                spriteBatch.DrawString(Application.MagicContentManager.Font, "Remaining lives: ", new Vector2(x, y), color);

                for (int i = 0; i < _lives; i++)
                {
                    spriteBatch.Draw(Application.MagicContentManager.GetTexture("hud"), livesLoc, new Rectangle(0, 0, 32, 32), Color.White);
                    livesLoc.X += 32;
                }
            }
            y += lineSize;

            if (_lastItem >= 3)
            {
                if (_lastItem == 1) color *= _currentAlpha;

                string sCarot = _carrotes.ToString("0000000");

                spriteBatch.DrawString(Application.MagicContentManager.Font, "Carrottes: ", new Vector2(x, y), color);
                spriteBatch.DrawString(Application.MagicContentManager.Font, _carrotes.ToString("0000000"), new Vector2(x + colSize, y), color);

                Vector2 carotLoc = new Vector2(x + Application.MagicContentManager.Font.MeasureString(sCarot).X + colSize + 16, y - 4);
                spriteBatch.Draw(Application.MagicContentManager.GetTexture("hud"), carotLoc, new Rectangle(64, 0, 32, 32), Color.White);
            }
            y += lineSize;

            if (_lastItem >= 4)
            {
                if (_lastItem == 1) color *= _currentAlpha;

                spriteBatch.DrawString(Application.MagicContentManager.Font, "Time left: ", new Vector2(x, y), color);
                spriteBatch.DrawString(Application.MagicContentManager.Font, _timeLeft.Minutes.ToString("00") + ":" + _timeLeft.Seconds.ToString("00"), new Vector2(x + colSize, y), color);
            }
            y += lineSize;

            if (_lastItem >= 5)
            {
                if (_lastItem == 1) color *= _currentAlpha;

                spriteBatch.DrawString(Application.MagicContentManager.Font, "Keys: ", new Vector2(x, y), color);

                if (_hasKeys)
                {
                    spriteBatch.Draw(Application.MagicContentManager.GetTexture("keys"), new Vector2(x + colSize, y) + new Vector2(0, -8), new Rectangle(0, 0, 32, 32), Color.White);
                }
            }
            y += lineSize;

            if (_lastItem >= 6)
            {
                if (_lastItem == 1) color *= _currentAlpha;

                spriteBatch.DrawString(Application.MagicContentManager.Font, "Letters: " + _letters.Count + "/" + OLetterSpawn.LetterCount, new Vector2(x, y), color);

                int letterScore = 2000 * _letters.Count + (_letters.Count == OLetterSpawn.LetterCount ? 10000 : 0);
                spriteBatch.DrawString(Application.MagicContentManager.Font, letterScore.ToString("000000000"), new Vector2(x + colSize, y), color);
            }
            y += lineSize * 2;

            if (_lastItem >= 7)
            {
                color = Color.Yellow;
                if (_lastItem == 1) color *= _currentAlpha;

                spriteBatch.DrawString(Application.MagicContentManager.Font, "Score: ", new Vector2(x, y), color);
                spriteBatch.DrawString(Application.MagicContentManager.Font, _score.ToString("000000000"), new Vector2(x + colSize, y), color);
            }

            //-----------------------------

            // Draw highscores button
            if (_lastItem >= ScoreLines)
            {
                spriteBatch.Draw(Application.MagicContentManager.GetTexture("menu"), _dstMenu, _srcMenu, Color.White, 0f, Vector2.Zero, Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally, 1.0f);

                spriteBatch.Draw(Application.MagicContentManager.GetTexture("hud"), _dstMenu.Location.ToVector2() + new Vector2(20, 16), new Rectangle(64, 0, 32, 32), Color.White);
                spriteBatch.DrawString(Application.MagicContentManager.Font,
                                        LocalizedStrings.GetString("Highscores"),
                                        _dstMenu.Location.ToVector2() + new Vector2(60, 20),
                                        Color.Yellow
                                       );

            }

            spriteBatch.End();

            ParticuleManager.Draw(false);
        }

        public override bool ChangeCurrentGameState { get; protected set; }
        public override GameState NextGameState { get; protected set; }
    }
}
