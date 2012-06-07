using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lapins.Engine.Core;
using Microsoft.Xna.Framework;
using Lapins.Engine.Graphics;
using LapinsInput = Lapins.Engine.Input;
using Microsoft.Xna.Framework.Input;
using Lapins.Engine.World;
using Lapins.Engine.Utils;
using Lapins.Data.Levels;
using Lapins.Engine.Input.Devices;
using Lapins.Engine.Content;
using Lapins.Data.Localization;
using Microsoft.Xna.Framework.Graphics;
using Lapins.Data.Particules;
using Lapins.Data.Events;
using Lapins.Data.Entities;
using Lapins.Data.Music;

namespace Lapins.GameStates
{
    /// <summary>
    /// Title screen 
    /// </summary>
    /// 
    [TextureContent(AssetName = "homebg", AssetPath = "gfxs/home/bg")]
    [TextureContent(AssetName = "fore", AssetPath = "gfxs/home/foreground")]
    [TextureContent(AssetName = "title", AssetPath = "gfxs/home/title")]
    [TextureContent(AssetName = "star", AssetPath = "gfxs/home/star")]
    [TextureContent(AssetName = "menu", AssetPath = "gfxs/home/menu")]
    [SoundEffectContent(AssetName = "s_blipmenu", AssetPath = "sfxs/blip")]
    [SoundEffectContent(AssetName = "s_title_explosion", AssetPath = "sfxs/explosion_1")]
    public class HomeState : GameState
    {
        private enum MenuValues
        {
            MenuNewgame = 0,
            MenuHighscores,
            //MenuOptions,
            MenuCredits,
            MenuQuit
        };
        private int MenuValuesSize = Enum.GetValues(typeof(MenuValues)).Length;

        private const int GroundYForTitle = 300;
        private const float TitltForTitle = -0.15f;


        private bool _isInitialized;
        private bool _isMenuDisplayed;
        private Color _bgColor, _generalColor;
        private Rectangle _srcBg;
        private Rectangle _dstBg;

        private Rectangle _srcFore;
        private Rectangle _dstFore;

        private Rectangle _srcTitle;
        private Rectangle _dstTitle;

        private Rectangle _srcStar, _dstStar;

        private Layer _asteroidLayer;
        private double _asteroidtime;

        private bool _isTitleFalling;
        private int _velocity = 1;

        private double _animationTime;

        private string _startStr;
        private Vector2 _startLoc;
        private bool _textDisplayed;
        private double _time;

        private MenuValues _menuCurrent;
        private Rectangle _srcMenu, _dstMenu;

        public HomeState()
            : base()
        {
            _time = 0;
            _textDisplayed = false;
            _bgColor = new Color(57, 74, 130);
            _isInitialized = false;
            _isMenuDisplayed = false;
            _dstBg = new Rectangle(0, 0, Resolution.VirtualWidth, Resolution.VirtualHeight);
            _srcBg = new Rectangle(0, 0, 800, 450);
            _srcFore = new Rectangle(0, 0, 800, 243);
            _dstFore = new Rectangle(0, Resolution.VirtualHeight - 389, Resolution.VirtualWidth, 389);
            _srcTitle = new Rectangle(0, 0, 324, 199);
            _dstTitle = new Rectangle((Resolution.VirtualWidth - 486) / 2, -400, 486, 300);
            _srcStar = new Rectangle(0, 0, 800, 343);
            _dstStar = new Rectangle(0, 0, 1280, 610);

            _dstMenu = new Rectangle(1280 - 254, 300, 254, 64);
            _srcMenu = new Rectangle(0, 0, 127, 32);

            _startStr = LocalizedStrings.GetString("home_pressstart");
            _startLoc = new Vector2((Resolution.VirtualWidth - Application.MagicContentManager.Font.MeasureString(_startStr).X) / 2, 600);
            SceneCamera.Location = new Vector2(Resolution.VirtualWidth / 2, Resolution.VirtualHeight / 2);

            _asteroidLayer = new Layer();
            _generalColor = Color.White;
        }

        protected override void LoadContent()
        {

        }

        protected override void InternalLoad()
        {
            Application.PhysicsManager.IsEnable = true;
            SceneCamera.FadeOut(30f, null);

            _menuCurrent = MenuValues.MenuNewgame;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            animationManager(gameTime);

            if (Application.IsApplicationActive)
            {
                _time += gameTime.ElapsedGameTime.Milliseconds;
                if (_time > 500)
                {
                    _time = 0;
                    _textDisplayed = !_textDisplayed;
                }
                if (!_isInitialized)
                {
                    // Register a gamepad as Player 1 if someone press start
                    // -- Gamepads
                    for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
                    {
                        if (GamePad.GetState(index).Buttons.Start == ButtonState.Pressed)
                        {
                            initializeInput(index);
                            break;
                        }
                    }

                    // -- Keyboards
                    if (Keyboard.GetState().GetPressedKeys().Count() > 0)
                    {
                        initializeInput(PlayerIndex.One);
                    }

                }
                else
                {
                    Application.PhysicsManager.IsEnable = true;
                    manageAsteroids(gameTime);

                    //animation
                    if (_isTitleFalling)
                    {
                        _velocity++;// gameTime.ElapsedGameTime.Milliseconds;
                        _dstTitle.Y += _velocity;//= gameTime.ElapsedGameTime.Milliseconds / 1000;
                        if (_dstTitle.Y >= GroundYForTitle)
                        {
                            _isTitleFalling = false;
                            SpecialEffectsHelper.ShakeScreen(new Vector2(3f, 5f), 0.5f);
                            Application.MagicContentManager.GetSound("s_title_explosion").Play(0.8f, 0f, 0f);
                            _isMenuDisplayed = true;
                            for (int i = 0; i <= 5; i++)
                            {
                                SpecialEffectsHelper.MakeCircularExplosion(new Vector2(_dstTitle.X + (i * (_dstTitle.Width / 5)), GroundYForTitle + _dstTitle.Height * 3 / 4), 150, 1, Color.Beige, false, false, 20);
                            }
                        }
                    }
                    if (_isMenuDisplayed)
                    {
                        foreach (Device device in Application.InputManager.GetLinkedDevices(LapinsInput.LogicalPlayerIndex.One))
                        {
                            if ((device.GetState(LapinsInput.MappingButtons.A).IsReleased) || (device.GetState(LapinsInput.MappingButtons.Start).IsReleased) || (device.GetState(LapinsInput.MappingButtons.Y).IsReleased))
                            {
                                changeState();
                            }
                            else if (device.ThumbStickLeft.Y != 0 && device.ThumbStickLeft.PreviousY == 0)
                            {
                                Application.MagicContentManager.GetSound("s_blipmenu").Play(1f, 0f, 0f);
                                if (device.ThumbStickLeft.Y > 0)
                                {
                                    _menuCurrent++;
                                    if ((int)_menuCurrent >= MenuValuesSize)
                                        _menuCurrent = (MenuValues)0;
                                }
                                else
                                {
                                    _menuCurrent--;
                                    if ((int)_menuCurrent < 0)
                                        _menuCurrent = (MenuValues)(MenuValuesSize - 1);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void initializeInput(PlayerIndex index)
        {
            // Gamepad
            var _gamepadInput = new GamepadDevice(LapinsInput.LogicalPlayerIndex.One, index);

            Application.InputManager.RegisterDevice(_gamepadInput);

            // Keyboard mapping
#if WINDOWS
            var _keyboardInput = new KeyboardDevice(LapinsInput.LogicalPlayerIndex.One);

            // -- RB LB Zoom
            _keyboardInput.MapButton(Keys.PageUp, LapinsInput.MappingButtons.LB);
            _keyboardInput.MapButton(Keys.PageDown, LapinsInput.MappingButtons.RB);
            // -- Back debug mode
            _keyboardInput.MapButton(Keys.Q, LapinsInput.MappingButtons.Back);
            // -- A Jump
            _keyboardInput.MapButton(Keys.Space, LapinsInput.MappingButtons.A);
            // -- B Cancel
            _keyboardInput.MapButton(Keys.Back, LapinsInput.MappingButtons.B);
            // -- X Run
            _keyboardInput.MapButton(Keys.LeftShift, LapinsInput.MappingButtons.X);
            // -- Y Action
            _keyboardInput.MapButton(Keys.Enter, LapinsInput.MappingButtons.Y);
            // -- Start pause
            _keyboardInput.MapButton(Keys.Escape, LapinsInput.MappingButtons.Start);
            // -- ThumbsstickLeft move
            _keyboardInput.MapLeftThumbstick(Keys.Up, Keys.Down, Keys.Left, Keys.Right);

            Application.InputManager.RegisterDevice(_keyboardInput);
#endif

            _isInitialized = true;
            launchAnimation();
        }

        private void launchAnimation()
        {
            SceneCamera.FadeIn(20, () =>
            {
                _bgColor = new Color(168, 37, 20);
                _generalColor = new Color(183, 86, 73);
                SceneCamera.FadeOut(20, () =>
                {
                    _isTitleFalling = true;
                }, Color.White);
            }, Color.White);

            MusicPlayer.PlayHomeMusic();
        }


        private void manageAsteroids(GameTime gametime)
        {
            _asteroidtime -= gametime.ElapsedGameTime.Milliseconds;

            if (_asteroidtime < 0)
            {
                _asteroidtime = Application.Random.GetRandomInt(500, 2000);
                float scale = Application.Random.GetRandomFloat(0.3f, 1);

                Asteroid a = new Asteroid(new Vector2(Application.Random.GetRandomInt(-300, 900), -100),
                            Application.Random.GetRandomVector2(-200, -100, 200, 300),
                            new Vector2(scale, scale),
                            false);
                //a.Color = Color.White * 0.5f;

                _asteroidLayer.Entities.Add(a);
            }

            List<Entity> tmp = new List<Entity>();
            foreach (Entity e in _asteroidLayer.Entities)
            {
                Asteroid a = ((Asteroid)e);
                a.Update(gametime);

                // Make smoke trail
                SpecialEffectsHelper.MakeSmoke(a.Location, new Vector2(a.Speed.X, -a.Speed.Y), a.Scale.X, Color.Wheat * 0.5f, true, 1);

                if (a.Location.Y > 540)
                {
                    tmp.Add(a);
                    SpecialEffectsHelper.MakeCircularExplosion(a.Location, (int)(a.Scale.X * 250), 1, Color.Wheat, true, true);
                }
            }
            foreach (Entity e in tmp)
            {
                _asteroidLayer.Entities.Remove(e);
            }
        }

        private void animationManager(GameTime gameTime)
        {
            _animationTime += gameTime.ElapsedGameTime.Milliseconds;
            if (_animationTime > 1000)
            {
                _srcStar.Y = Application.Random.GetRandomInt(0, 2) * 343;
                _srcFore.Y = Application.Random.GetRandomInt(0, 3) * 243;
                _animationTime = 0;
            }
        }

        public override void Draw(SpriteBatchProxy spriteBatch)
        {
            ParticuleManager.Draw(true);


            spriteBatch.BeginNoCamera();
            spriteBatch.Draw(Application.MagicContentManager.GetTexture("homebg"), _dstBg, _srcBg, _bgColor);
            spriteBatch.End();

            //-----------------------------------------------------------------------------------------------
            spriteBatch.Begin(SceneCamera);

            if (!_isInitialized)
            {
                spriteBatch.Draw(Application.MagicContentManager.GetTexture("star"), _dstStar, _srcStar, Color.White);
            }

            spriteBatch.Draw(Application.MagicContentManager.GetTexture("title"), _dstTitle, _srcTitle, Color.White, TitltForTitle, Vector2.Zero, SpriteEffects.None, 1);

            foreach (Entity e in _asteroidLayer.Entities)
            {
                e.Draw(spriteBatch);
            }

            spriteBatch.End();
            //-----------------------------------------------------------------------------------------------
            ParticuleManager.Draw(false);


            spriteBatch.Begin(SceneCamera);
            spriteBatch.Draw(Application.MagicContentManager.GetTexture("fore"), _dstFore, _srcFore, _generalColor);


            if (_textDisplayed && !_isInitialized)
            {
                spriteBatch.DrawString(Application.MagicContentManager.Font, _startStr, _startLoc, Color.White);
            }

            if (_isMenuDisplayed)
            {
                drawMenu(spriteBatch);
            }

            //-----------------------------------------------------------------------------------------------
            spriteBatch.End();
        }


        private void drawMenu(SpriteBatchProxy spriteBatch)
        {
            int tmp = _dstMenu.Bottom;
            for (int i = 0; i < MenuValuesSize; i++)
            {
                spriteBatch.Draw(Application.MagicContentManager.GetTexture("menu"), _dstMenu, _srcMenu, Color.White);

                Color color = Color.White;

                // Draw a carott
                if (i == (int)_menuCurrent)
                {
                    color = Color.Yellow;
                    spriteBatch.Draw(Application.MagicContentManager.GetTexture("hud"), _dstMenu.Location.ToVector2() + new Vector2(20, 16), new Rectangle(64, 0, 32, 32), Color.White);
                }

                spriteBatch.DrawString(Application.MagicContentManager.Font,
                                        LocalizedStrings.GetString(((MenuValues)i).ToString()),
                                        _dstMenu.Location.ToVector2() + new Vector2(60, 20),
                                        color
                                       );



                _dstMenu.Offset(0, _dstMenu.Height + 20);

            }
            _dstMenu.Offset(0, -(_dstMenu.Bottom - tmp));
        }


        private void changeState()
        {
            UseLoading = false;

            switch (_menuCurrent)
            {
                case MenuValues.MenuNewgame:
                    NextGameState = Application.GameStateManager.GetGameState<PlayState>();
                    UseLoading = true;
                    break;

                //case MenuValues.MenuOptions:
                //ChangeCurrentGameState = false;
                //NextGameState = Application.GameStateManager.GetGameState<PlayState>();
                //break;

                case MenuValues.MenuHighscores:
                    NextGameState = Application.GameStateManager.GetGameState<HighscoreState>();
                    break;

                case MenuValues.MenuCredits:
                    NextGameState = Application.GameStateManager.GetGameState<CreditsState>();
                    break;

                case MenuValues.MenuQuit:
                    ChangeCurrentGameState = false;
                    Application.Quit();
                    break;
            }

            SceneCamera.FadeIn(30f, () =>
            {
                ChangeCurrentGameState = true;
            });
        }

        public override bool ChangeCurrentGameState { get; protected set; }
        public override GameState NextGameState { get; protected set; }
    }
}
