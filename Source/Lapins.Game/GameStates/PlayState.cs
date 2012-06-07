using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lapins.Engine.Core;
using Microsoft.Xna.Framework;
using Lapins.Engine.Graphics;
using Lapins.Data.Levels;
using Lapins.Engine.Input;
using Lapins.Data.Commands;
using Lapins.Data.Objects;
using Lapins.UI;
using Lapins.Data.Events;
using Lapins.Engine.Content;
using Lapins.Engine.Input.Devices;
using Lapins.Data.Particules;
using Lapins.Engine.Utils;
using Lapins.Data.Utils;
using Lapins.Data.Localization;
using Lapins.Data.Music;

namespace Lapins.GameStates
{
    /// <summary>
    /// Ingame state
    /// </summary>
    public class PlayState : GameState
    {
        /// <summary>
        /// Total time (in seconds)
        /// </summary>
        public static float MaxTime = 600f;

        private Level _level;
        private HUD _hud;
        private Skybox _skybox;
        private Compass _compass;
        private float _elapsedTime;

        private AsteroidGenerator _bgAsteroidGenerator, _asteroidGenerator;

        private bool _isPaused, _isEnded;
        private float _alphaPause, _alphaPauseDelta;

        public PlayState()
        {
            _hud = new HUD();
            _compass = new Compass();
        }

        protected override void LoadContent()
        {

        }

        protected override void InternalLoad()
        {
            _isPaused = false;
            _isEnded = false;
            _alphaPauseDelta = 0f;
            _alphaPause = 0f;
            _elapsedTime = 0f;

            _level = LevelBuilder.CreateLevel();
            Level.CurrentLevel = _level; // Register the level as the current one for data

            SceneCamera.SetBounds(_level.Dimensions);
            SceneCamera.Zoom = 1.6f;

            _bgAsteroidGenerator = new AsteroidGenerator(Level.CurrentLevel.BackgroundLayer, true);
            _asteroidGenerator = new AsteroidGenerator(Level.CurrentLevel.MiddleGroundLayer, false);

            _skybox = new Skybox();
            _compass.Initialize();

            initScript();

            MusicPlayer.PlayGameMusic();

            SceneCamera.FadeOut(45, null);
        }

        private void initScript()
        {
            Application.ScriptManager.SetFlag(LapinsScript.TimeLeft, MaxTime);
            Application.ScriptManager.SetFlag(LapinsScript.TotalTime, MaxTime);
            Application.ScriptManager.SetFlag(LapinsScript.IsPlayerInstanciated, false);
            Application.ScriptManager.SetFlag(LapinsScript.Score, 0);
#if DEBUG
            Application.ScriptManager.SetFlag(LapinsScript.HasKeys, true);
#else
            Application.ScriptManager.SetFlag(LapinsScript.HasKeys, false);
#endif
            Application.ScriptManager.SetFlag(LapinsScript.CarrotsCount, 0);
            Application.ScriptManager.SetFlag(LapinsScript.EscapeState, 0);
            Application.ScriptManager.SetFlag(LapinsScript.PlayerLivesMax, 7);
            Application.ScriptManager.SetFlag(LapinsScript.TextToDisplay, "");
            Application.ScriptManager.SetFlag(LapinsScript.CollectedLetters, new List<Letter>());

            OLetterSpawn.Intialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Application.IsApplicationActive)
            {
                getInput();
            }

            // Stop physics in pause
            Application.PhysicsManager.IsEnable = !_isPaused;

            if (_isPaused == false)
            {
                // Tic, tac, tic, tac, time is running out!
                _elapsedTime += ((float)gameTime.ElapsedGameTime.TotalSeconds);

                float timeLeft = MaxTime - _elapsedTime;
                Application.ScriptManager.SetFlag(LapinsScript.TimeLeft, timeLeft);

                if (timeLeft <= 0)
                {
                    Application.ScriptManager.SetFlag(LapinsScript.EscapeState, 2);
                }

                // Update elements
                _skybox.Update(gameTime);
                _level.Update(gameTime);
                _compass.Update(gameTime);

                if (_level.Player != null)
                {
                    // Generate asteroids if player is instanciated
                    _bgAsteroidGenerator.Update(gameTime);
                    _asteroidGenerator.Update(gameTime);
                }
                else
                {
                    // Center camera on player spawn
                    Application.GameStateManager.CurrentGameState.SceneCamera.Location = _level.SpawnLocation;
                }

                // Back to home if player is dead
                int endState = Application.ScriptManager.GetFlag<int>(LapinsScript.EscapeState);

                if ((endState != 0) && (_isEnded == false))
                {
                    // Launch animation
                    _isEnded = true;

                    // Win
                    if (endState == 1)
                    {
                        Timer.Create(2f, false, t =>
                        {
                            SceneCamera.FadeIn(60f, () =>
                            {
                                SpecialEffectsHelper.ShakeScreen(Vector2.Zero, 1f);

                                ChangeCurrentGameState = true;
                                NextGameState = Application.GameStateManager.GetGameState<GameOverState>();
                            });
                        });
                    }
                    else
                    {
                        Timer.Create(1f, false, t =>
                        {
                            // Lose
                            SceneCamera.FadeIn(30f, () =>
                            {
                                SpecialEffectsHelper.ShakeScreen(Vector2.Zero, 1f);

                                ChangeCurrentGameState = true;
                                NextGameState = Application.GameStateManager.GetGameState<GameOverState>();
                            }, Color.White);
                        });
                    }
                }
                else
                {
                    if (Level.CurrentLevel.Player != null)
                    {
                        // Center camera on player if it's not the end
                        Application.GameStateManager.CurrentGameState.SceneCamera.Location = _level.Player.Location;
                    }
                }

                _hud.Update(gameTime);
            }

            if (_alphaPauseDelta != 0)
            {
                _alphaPause += _alphaPauseDelta;
                if (_alphaPause > 0.75f)
                {
                    _alphaPause = 0.75f;
                    _alphaPauseDelta = 0f;
                }
                else if (_alphaPause < 0f)
                {
                    _alphaPause = 0f;
                    _alphaPauseDelta = 0f;
                }
            }
        }

        private void getInput()
        {
            foreach (Device device in Application.InputManager.GetLinkedDevices(LogicalPlayerIndex.One))
            {
                // Zoom
                if (device.GetState(MappingButtons.LB).IsDown)
                {
                    SceneCamera.Zoom += 0.025f;
                    //Console.WriteLine(SceneCamera.Zoom);
                }
                else if (device.GetState(MappingButtons.RB).IsDown)
                {
                    SceneCamera.Zoom -= 0.025f;
                    //Console.WriteLine(SceneCamera.Zoom);
                }

#if DEBUG
                if (device.GetState(MappingButtons.Back).IsPressed)
                {
                    Application.IsDebugMode = !Application.IsDebugMode;
                }
#endif

                // Pause game
                if (device.GetState(MappingButtons.Start).IsPressed)
                {
                    _isPaused = !_isPaused;

                    if (_isPaused)
                    {
                        _alphaPauseDelta = 0.025f;
                    }
                    else
                    {
                        _alphaPauseDelta = -0.025f;
                    }
                }

                // Quit game
                if (device.GetState(MappingButtons.Back).IsPressed)
                {
                    if (_isPaused)
                    {
                        ChangeCurrentGameState = true;
                        NextGameState = Application.GameStateManager.GetGameState<HomeState>();
                    }
                }
            }
        }

        public override void Draw(SpriteBatchProxy spriteBatch)
        {
            // Draw the skybox
            _skybox.Draw(spriteBatch);

            ParticuleManager.Draw(true);

            spriteBatch.Begin(SceneCamera);

            _level.Draw(spriteBatch);

            if (Application.IsDebugMode)
            {
                // World base axis
                spriteBatch.DrawRectangle(new Rectangle(-1, -1, 2, _level.Dimensions.Height), Color.Red);
                spriteBatch.DrawRectangle(new Rectangle(-1, -1, _level.Dimensions.Width, 2), Color.Green);

                Application.PhysicsManager.Draw(spriteBatch);
            }

            spriteBatch.End();

            ParticuleManager.Draw(false);

            // Draw HUD and Compass out of the camera
            if (_isEnded == false)
            {
                _hud.Draw(spriteBatch);
                _compass.Draw(spriteBatch);
            }

            spriteBatch.BeginNoCamera();
            spriteBatch.DrawRectangle(new Rectangle(0, 0, Resolution.VirtualWidth, Resolution.VirtualHeight), Color.Black * _alphaPause);

            if (_isPaused)
            {
                // Draw "PAUSE" 
                Vector2 _pauseLoc = new Vector2(Resolution.VirtualWidth / 2 - Application.MagicContentManager.Font.MeasureString("PAUSE").X / 2, 300);

                spriteBatch.DrawString(Application.MagicContentManager.Font, "PAUSE", _pauseLoc, Color.White * _alphaPause);

                // Draw controls
                var devices = Application.InputManager.GetLinkedDevices(LogicalPlayerIndex.One).FindAll(d => d.IsConnected);

                Vector2 buttonLoc = new Vector2((int)_pauseLoc.X - 150, _pauseLoc.Y + 60);

                for (int i = 0; i < devices.Count; i++)
                {
                    ButtonPrinter.Draw(spriteBatch, MappingButtons.Start, devices[i], buttonLoc, Color.White * _alphaPause);
                    buttonLoc.X += 50;
                }

                Vector2 textLoc = buttonLoc + new Vector2(100,0);
                spriteBatch.DrawString(Application.MagicContentManager.Font, LocalizedStrings.GetString("BackToGame"), textLoc, Color.White * _alphaPause);

                buttonLoc = new Vector2((int)_pauseLoc.X - 150, _pauseLoc.Y + 100);

                for (int i = 0; i < devices.Count; i++)
                {
                    ButtonPrinter.Draw(spriteBatch, MappingButtons.Back, devices[i], buttonLoc, Color.White * _alphaPause);
                    buttonLoc.X += 50;
                }
                textLoc = buttonLoc + new Vector2(100, 0);
                spriteBatch.DrawString(Application.MagicContentManager.Font, LocalizedStrings.GetString("BackToHome"), textLoc, Color.White * _alphaPause);
            }
            spriteBatch.End();
        }

        public override bool ChangeCurrentGameState
        {
            get;
            protected set;
        }

        public override GameState NextGameState
        {
            get;
            protected set;
        }
    }
}
