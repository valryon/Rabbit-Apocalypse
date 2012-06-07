using System;
using System.Collections.Generic;
using Lapins.Data.Commands;
using Lapins.Data.Levels;
using Lapins.Data.Objects;
using Lapins.Data.Particules;
using Lapins.Data.Utils;
using Lapins.Engine.Content;
using Lapins.Engine.Core;
using Lapins.Engine.Graphics;
using Lapins.Engine.Input;
using Lapins.Engine.Input.Devices;
using Lapins.Engine.Physics;
using Lapins.Engine.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Lapins.Data.Entities
{
    /// <summary>
    /// Rabiit controlled by the player
    /// </summary>
    [TextureContent(AssetName = "player", AssetPath = "gfxs/entities/player_sprite")]
    [SoundEffectContent(AssetName = "s_jump1", AssetPath = "sfxs/jump1")]
    [SoundEffectContent(AssetName = "s_jump2", AssetPath = "sfxs/jump2")]
    [SoundEffectContent(AssetName = "s_jump3", AssetPath = "sfxs/jump3")]
    [SoundEffectContent(AssetName = "s_bump", AssetPath = "sfxs/bump")]
    [SoundEffectContent(AssetName = "s_teleport", AssetPath = "sfxs/teleport")]
    [SoundEffectContent(AssetName = "s_flotch", AssetPath = "sfxs/flotch")]
    [SoundEffectContent(AssetName = "s_pancarte", AssetPath = "sfxs/page-flip")]
    [SoundEffectContent(AssetName = "s_eat1", AssetPath = "sfxs/eat1")]
    [SoundEffectContent(AssetName = "s_eat2", AssetPath = "sfxs/eat2")]
    [SoundEffectContent(AssetName = "s_eat3", AssetPath = "sfxs/eat3")]
    [SoundEffectContent(AssetName = "s_eat4", AssetPath = "sfxs/eat4")]
    [SoundEffectContent(AssetName = "s_eat5", AssetPath = "sfxs/eat5")]
    [SoundEffectContent(AssetName = "s_keys", AssetPath = "sfxs/keys")]
    [SoundEffectContent(AssetName = "s_letter", AssetPath = "sfxs/letter")]
    [SoundEffectContent(AssetName = "s_life", AssetPath = "sfxs/life")]

    public class Player : PhysicsEntity
    {
        /// <summary>
        /// Enum for animation
        /// </summary>
        private enum PlayerState
        {
            Idle = 0,
            Moving,
            Sleeping,
            WakingUp,
            Jumping,
            ChangingDir,
            Falling,
            Landing,
            Dead,
            Boxed,
            BoxedJumping,
            BoxedGoingUp,
            BoxedFalling,
            BoxedLanding
        };

        #region Constants

        private const int DefaultLives = 5;
        private const float BunnyMass = 1f;

        /// <summary>
        /// Time before our little rabbit fell asleep, in milliseconds
        /// </summary>
        private const float TimeBeforeAsleep = 3000f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -100;
        private const uint MaxJumpAllowed = 3;
        private const float TimebetweenJump = 250;
        private const float BunnyJumpCoef = 35f;

        private const float RunningSpeed = 350f;
        private const float WalkingSpeed = 200f;

        private const float DefaultRespawnTime = 1.5f; // 1 sec of animation & death before respawn
        private const float DefaultInvincibleTime = 1500f;

        #endregion

        private float _elapsedFrameTime;

        // Life & Death 
        private int _lives;
        private bool _isDead;
        //private Hitbox _deadHitbox;
        private bool _respawnInProgress;
        private float _respawnTime;
        private int _boxlife;

        // Jumping state
        private bool _isJumping;
        private bool _wasJumping;
        private float _jumpTime;
        private uint _currentJump;
        private double _bunnyJumpTime;
        private Vector2 _bonusSpeed;

        // Animations
        private PlayerState _currentState;
        private double _idleTime;
        private SpriteAnimation[] _animator;

        // Input
        private bool _inputDetected;
        private bool _pressedAction;
        private bool _isRunning;
        private bool _isReading;
        private float _showInputAlpha;
        private MappingButtons _inputHelp;

        // Teleport
        private bool _canTeleport;
        private float _lastTeleport;

        // End
        private bool _isNotVisible;

        //private static List<SoundEffect> _eatSounds;

        public Player()
            : base("player", Vector2.Zero, new Rectangle(0, 0, 50, 55), new Vector2(WalkingSpeed, 0), Vector2.One)
        {
            mass = BunnyMass;
            hitbox = new Hitbox(new Rectangle(5, 29, 36, 26));
            awarenessbox = new Hitbox(new Rectangle(0, 10, 50, 54));

            IsRemovable = false;
            OnDeath += new Action(onDeathAction);
            LayerDepth = 110;
        }

        public override void Initialize(Vector2 location)
        {
            // Booleans
            _canTeleport = true;
            _isReading = false;
            _respawnInProgress = false;

            // Jump values
            _currentJump = 0;
            _bunnyJumpTime = 0;

            _boxlife = 0;
            // Lives
            _lives = DefaultLives;
            Application.ScriptManager.SetFlag(LapinsScript.PlayerLives, _lives);

            invincibleTime = DefaultInvincibleTime;

            // Animations settings
            _currentState = PlayerState.Idle;
            _idleTime = 0;
            _animator = new SpriteAnimation[Enum.GetValues(typeof(PlayerState)).Length];

            // Animators initialization
            Rectangle tmp = base.SrcRect;
            _animator[(int)PlayerState.Idle] = new SpriteAnimation(tmp, 5, 300, -1);
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.Moving] = new SpriteAnimation(tmp, 5, 150, -1);
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.Sleeping] = new SpriteAnimation(tmp, 5, 300, -1);
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.WakingUp] = new SpriteAnimation(tmp, 5, 200, 1);
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.Jumping] = new SpriteAnimation(tmp, 4, 100, 1) { KeepLastFrame = true };
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.ChangingDir] = new SpriteAnimation(tmp, 3, 150, 1);
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.Falling] = new SpriteAnimation(tmp, 2, 200, -1);
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.Landing] = new SpriteAnimation(tmp, 5, 100, 1);
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.Dead] = new SpriteAnimation(tmp, 4, 200, 1) { KeepLastFrame = true };
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.Boxed] = new SpriteAnimation(tmp, 2, 250, -1);
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.BoxedJumping] = new SpriteAnimation(tmp, 4, 150, 1);
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.BoxedGoingUp] = new SpriteAnimation(tmp, 1, 150, 1) { KeepLastFrame = true };
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.BoxedFalling] = new SpriteAnimation(tmp, 3, 250, 1) { KeepLastFrame = true };
            tmp.Offset(0, 55);
            _animator[(int)PlayerState.BoxedLanding] = new SpriteAnimation(tmp, 5, 150, 1);

            // Send default animator to base
            animation = _animator[(int)_currentState];

            base.Initialize(location);

            Application.ScriptManager.SetFlag(LapinsScript.IsPlayerInstanciated, true);
        }

        public override void Update(GameTime gameTime)
        {
            // Time spent
            _elapsedFrameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Reset booleans
            _pressedAction = false;
            _inputDetected = false;
            _isJumping = false;
            _inputDetected = false;
            _isRunning = true;

            string textToDisplay = Application.ScriptManager.GetFlag<string>(LapinsScript.TextToDisplay);

            _isReading = !String.IsNullOrEmpty(textToDisplay);

            // Player is dead : cancel inputs & physics
            if (_isDead)
            {
                // Stay invincible to avoid further events trigger
                invincibleTime = DefaultInvincibleTime;

                _respawnTime -= _elapsedFrameTime;
                if (_respawnTime <= 0)
                {
                    if (!_respawnInProgress)
                    {
                        _respawnInProgress = true;

                        // Fade in/out & teleport
                        Application.GameStateManager.CurrentGameState.SceneCamera.FadeIn(10f, () =>
                        {
                            TeleportToNearestCheckpoint();
                            mass = BunnyMass;

                            if (_lives >= 0)
                            {
                                Application.GameStateManager.CurrentGameState.SceneCamera.FadeOut(10f, () => { _currentState = PlayerState.Idle; _respawnInProgress = false; _isDead = false; });
                            }
                        });
                    }
                }
            }
            else
            {
                // Restore teleport capability
                if (_canTeleport == false)
                {
                    _lastTeleport += _elapsedFrameTime;

                    if (_lastTeleport >= 1.5f)
                    {
                        _lastTeleport = 0f;
                        _canTeleport = true;
                    }
                }

                // Bunny jump
                if (IsOnGround)
                {
                    _bonusSpeed = Vector2.Zero;

                    _bunnyJumpTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (_bunnyJumpTime > TimebetweenJump)
                    {
                        _currentJump = 0;
                    }
                }

                // Get input
                foreach (Device device in Application.InputManager.GetLinkedDevices(LogicalPlayerIndex.One))
                {
                    GetInput(gameTime, device);
                }
            }

            // Update max speed
            if (_isRunning)
            {
                speed.X = RunningSpeed;
            }
            else
            {
                speed.X = WalkingSpeed;
            }

            // Bad idea
            //speed += _bonusSpeed;

            // Update jump
            velocity.Y = DoJump(velocity.Y, gameTime);

            updateAnimation(gameTime);

            // Reset some animations if player pressed something
            if (_inputDetected)
            {
                _idleTime = 0;
                _respawnTime = 0;
            }

            if (_showInputAlpha > 0f)
            {
                _showInputAlpha -= 0.1f;
            }

            if (_isDead)
            {
                speed.X = velocity.X = 0;
                velocity.Y = 200;

            }
            base.Update(gameTime);


        }

        /// <summary>
        /// Manage input for player for a given device
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="device"></param>
        private void GetInput(GameTime gameTime, Device device)
        {
            if (_isNotVisible) return;

            if (_isReading)
            {
                if ((device.GetState(MappingButtons.A).IsReleased) || (device.GetState(MappingButtons.X).IsReleased) || (device.GetState(MappingButtons.Y).IsPressed))
                {
                    Application.ScriptManager.SetFlag(LapinsScript.TextToDisplay, "");
                }
            }

            else
            {
                // Move left & right
                float leftright = device.ThumbStickLeft.X;
                if (leftright < -0.3f || leftright > 0.3f)
                {
                    velocity.X += ((_boxlife > 0) ? 25f : 50f) * leftright;
                    _inputDetected = true;

                    if (IsStuckLeft && velocity.X < 0)
                        velocity.X = 0f;

                    if (IsStuckRight && velocity.X > 0)
                        velocity.X = 0f;
                }

                if (device.GetState(MappingButtons.Y).IsPressed)
                {
                    _pressedAction = true;
                }
                if (device.GetState(MappingButtons.X).IsDown)
                {
                    _isRunning = false;
                }

                // Jump
                if (device.GetState(MappingButtons.A).IsDown)
                {
                    if (IsOnGround && !_wasJumping) // take off the ground
                    {
                        _isJumping = true;
                        _currentJump++;

                        if (_currentJump > MaxJumpAllowed) _currentJump = 1;
                        _bunnyJumpTime = 0;

                        // Add 
                        if (MovingFloorMovement != Vector2.Zero)
                        {
                            if ((velocity.X < 0) && (MovingFloorMovement.X < 0) || (velocity.X > 0) && (MovingFloorMovement.X > 0))
                            {
                                _bonusSpeed.X = Math.Abs(MovingFloorMovement.X);
                            }
                            if ((velocity.Y < 0) && (MovingFloorMovement.Y < 0) || (velocity.Y > 0) && (MovingFloorMovement.Y > 0))
                            {
                                _bonusSpeed.Y = Math.Abs(MovingFloorMovement.Y);
                            }

                            _bonusSpeed *= 100;
                        }
                    }
                    else
                    {
                        _isJumping = true;
                    }
                }
            }
        }

        private void updateAnimation(GameTime gameTime)
        {
            PlayerState next = _currentState;

            if (!_isDead)
            {
                //for the future me: add 3 phase for mid air 1°) jump start 2°) accent phase and after falling phase
                // State animation
                if (_currentState == PlayerState.Sleeping && (Velocity.X != 0 || Velocity.Y != 0))
                {
                    _animator[(int)PlayerState.WakingUp].Reset();
                    next = PlayerState.WakingUp;
                }

                if (Velocity.Y < 0 && _currentState != PlayerState.Jumping) //he is jumping, ascendant 
                {
                    _animator[(int)PlayerState.Jumping].Reset();
                    next = PlayerState.Jumping;
                }

                if (Velocity.Y > 0)
                {
                    if (_currentState == PlayerState.Jumping)
                    {
                        _animator[(int)PlayerState.ChangingDir].Reset();
                        next = PlayerState.ChangingDir;
                    }
                    else if (_currentState == PlayerState.ChangingDir && animation.Over)
                    {
                        _animator[(int)PlayerState.Falling].Reset();
                        next = PlayerState.Falling;
                    }
                    else
                    {
                        _animator[(int)PlayerState.Falling].Reset();
                        next = PlayerState.Falling;
                    }

                }

                if (Velocity.Y == 0)
                {
                    if (_currentState == PlayerState.Falling)
                    {
                        _animator[(int)PlayerState.Landing].Reset();
                        next = PlayerState.Landing;
                    }
                    if (_currentState == PlayerState.Landing && animation.Over)
                    {
                        _animator[(int)PlayerState.Idle].Reset();
                        next = PlayerState.Idle;
                    }



                    if (_currentState == PlayerState.WakingUp && animation.Over)
                    {
                        _animator[(int)PlayerState.Idle].Reset();
                        next = PlayerState.Idle;
                    }

                    // Asleep
                    if (_currentState == PlayerState.Idle)
                    {
                        _idleTime += gameTime.ElapsedGameTime.Milliseconds;
                        if (_idleTime >= TimeBeforeAsleep)
                        {
                            _animator[(int)PlayerState.Sleeping].Reset();
                            next = PlayerState.Sleeping;
                        }
                    }
                    if (Velocity.X != 0 && _currentState != PlayerState.Moving)
                    {
                        _animator[(int)PlayerState.Moving].Reset();
                        next = PlayerState.Moving;

                    }
                    else if (velocity.X == 0 && _currentState == PlayerState.Moving)
                    {
                        _animator[(int)PlayerState.Idle].Reset();
                        next = PlayerState.Idle;
                    }



                }
                if (_boxlife > 0)
                {
                    next = PlayerState.Boxed;
                    if (Velocity.Y < 0)
                    {
                        if (_currentState != PlayerState.BoxedGoingUp)
                        {
                            next = PlayerState.BoxedJumping;
                        }
                        else if (_currentState == PlayerState.BoxedJumping && animation.Over)
                        {
                            next = PlayerState.BoxedGoingUp;
                        }
                        else if (Velocity.Y > 0)
                        {
                            next = PlayerState.BoxedFalling;
                        }
                        else
                        {
                            if (_currentState == PlayerState.BoxedFalling)
                            {
                                next = PlayerState.BoxedLanding;
                            }
                            else if (_currentState == PlayerState.BoxedLanding && _animator[(int)PlayerState.BoxedLanding].Over)
                            {
                                next = PlayerState.Boxed;
                            }
                        }
                    }

                }

            }
            _currentState = next;
            animation = _animator[(int)_currentState];
        }

        public override void Draw(Engine.Graphics.SpriteBatchProxy spriteBatch)
        {
            if (_isNotVisible) return;

#if DEBUG
            spriteBatch.DrawString(Application.MagicContentManager.Font, string.Format("current state={0}", _currentState), location
                - Vector2.UnitY * 10, Color.Green);
            spriteBatch.DrawString(Application.MagicContentManager.Font, string.Format("Velocity Y={0}", Velocity.Y), location
                - Vector2.UnitY * 25, Color.Green);
            spriteBatch.DrawString(Application.MagicContentManager.Font, string.Format("Jump={0}", _currentJump), location
                - Vector2.UnitY * 50, Color.Green);
            spriteBatch.DrawString(Application.MagicContentManager.Font, string.Format("bunny={0}", (_bunnyJumpTime < TimebetweenJump)), location
                - Vector2.UnitY * 75, Color.Green);
#endif

            if (IsInvincible)
            {
                Color = Color.White * 0.75f;
            }
            else
            {
                Color = Color.White;
            }

            base.Draw(spriteBatch);

            // helper ?
            var devices = Application.InputManager.GetLinkedDevices(LogicalPlayerIndex.One).FindAll(d => d.IsConnected);
            int x = 5;

            if (devices.Count > 1)
            {
                x = -10;
                spriteBatch.DrawString(Application.MagicContentManager.Font, "/", location + new Vector2(20, -40), Color.White * _showInputAlpha);
            }

            foreach (Device device in Application.InputManager.GetLinkedDevices(LogicalPlayerIndex.One))
            {
                if (device.IsConnected)
                {
                    if (_showInputAlpha > 0)
                    {
                        ButtonPrinter.Draw(spriteBatch, _inputHelp, device, location + new Vector2(x, -40), Color.White * _showInputAlpha);
                    }
                }

                if (devices.Count > 1)
                {
                    x += 50;
                }
            }


        }

        /// <summary>
        /// Positive box collision
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="depth"></param>
        public override void EntityDetected(Entity collider, Vector2 depth)
        {

            if (!_isDead)
            {
                if (collider is OCoin)
                {
                    collider.IsAlive = false;
                    LapinsScript.AddToScore(100);

                    int carrotCounter = Application.ScriptManager.GetFlag<int>(LapinsScript.CarrotsCount);
                    carrotCounter++;
                    Application.ScriptManager.SetFlag(LapinsScript.CarrotsCount, carrotCounter);

                    // Sound
                    playEatSound();

                    if (carrotCounter % 250 == 0)
                    {
                        addLife();
                    }
                }
                else if (collider is OLife)
                {
                    collider.IsAlive = false;

                    addLife();
                }
                else if (collider is Keys)
                {
                    collider.IsAlive = false;

                    LapinsScript.AddToScore(5000);

                    Application.ScriptManager.SetFlag(LapinsScript.HasKeys, true);

                    // Sound
                    Application.MagicContentManager.GetSound("s_keys").Play(0.5f, 0, 0);
                }
                else if (collider is Letter)
                {
                    collider.IsAlive = false;

                    LapinsScript.AddToScore(2000);

                    List<Letter> collectedLetters = Application.ScriptManager.GetFlag<List<Letter>>(LapinsScript.CollectedLetters);
                    collectedLetters.Add((Letter)collider);

                    // Sort by order in the initial list
                    collectedLetters.Sort();

                    if (collectedLetters.Count == OLetterSpawn.LetterCount)
                    {
                        LapinsScript.AddToScore(10000);
                    }

                    Application.ScriptManager.SetFlag(LapinsScript.CollectedLetters, collectedLetters);

                    // Sound
                    Application.MagicContentManager.GetSound("s_letter").Play(0.5f, 0, 0);

                }
                else if (collider is OShip)
                {
                    // End game ?
                    bool hasKeys = Application.ScriptManager.GetFlag<bool>(LapinsScript.HasKeys);

                    if (hasKeys)
                    {
                        if (_isNotVisible == false)
                        {
                            LapinsScript.AddToScore(5000);
                            Application.ScriptManager.SetFlag(LapinsScript.EscapeState, 1);

                            _isNotVisible = true;
                        }
                    }
                    else
                    {
                        ((OShip)collider).Question();
                    }
                }
                else if (collider is OPancarte)
                {
                    // Display input to use
                    _showInputAlpha = 1.0f;
                    _inputHelp = MappingButtons.Y;

                    // Display help ?
                    if (_pressedAction)
                    {
                        // Sound
                        Application.MagicContentManager.GetSound("s_pancarte").Play(0.5f, 0, 0);

                        ((OPancarte)collider).IsActivated = true;
                    }
                }
            }
        }

        private void addLife()
        {
            LapinsScript.AddToScore(500);

            // Win life
            _lives++;
            Application.ScriptManager.SetFlag(LapinsScript.PlayerLives, _lives);

            // Sound
            Application.MagicContentManager.GetSound("s_life").Play(0.5f, 0, 0);
        }

        /// <summary>
        /// Player hitbox collision
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="depth"></param>
        public override void CollisionDetected(Entity collider, Vector2 depth)
        {
            if (!_isDead)
            {
                if (collider is OSpikes)
                {
                    LoseLife();
                    TriggerDeathAnimationAndTeleport();
                }
                else if (collider is Asteroid)
                {
                    LoseLife();
                }
                else if (collider is OWolfTrap)
                {
                    var wolfTrap = collider as OWolfTrap;
                    wolfTrap.Activate();

                    LoseLife();
                    TriggerDeathAnimationAndTeleport();
                }
                else if (collider is OTerrier)
                {
                    // Display input to use
                    _showInputAlpha = 1.0f;
                    _inputHelp = MappingButtons.Y;

                    // Teleport !
                    if (_canTeleport)
                    {
                        if (_pressedAction)
                        {
                            var terrier = collider as OTerrier;
                            if (terrier.LinkedTerrier != null)
                            {
                                _canTeleport = false;

                                // Sound
                                Application.MagicContentManager.GetSound("s_teleport").Play(0.5f, 0, 0);

                                // Fade in/out & teleport
                                Application.GameStateManager.CurrentGameState.SceneCamera.FadeIn(10f, () =>
                                {
                                    location = terrier.LinkedTerrier.Location + new Vector2(70, 120) - (new Vector2(SrcRect.Width / 2, SrcRect.Height));
                                    Application.GameStateManager.CurrentGameState.SceneCamera.FadeOut(10f, null);
                                });
                            }

                        }
                    }
                }
                else if (collider is OBumper)
                {
                    // Bumper will eject bunny in the air
                    if (location.Y < collider.Location.Y)
                    {
                        var bumper = ((OBumper)collider);
                        bumper.IsPressed = true;

                        velocity = bumper.BounceVector;

                        _isJumping = true;
                        _jumpTime += _elapsedFrameTime;

                        // Sound
                        Application.MagicContentManager.GetSound("s_bump").Play(0.5f, 0, 0);
                    }
                }
                else if (collider is OBox)
                {
                    var box = (OBox)collider;
                    if (!box.Activated)
                    {
                        box.Activated = true;
                    }
                    if (box.Trapped)
                    {
                        _boxlife = 5;
                        box.IsAlive = false;
                    }
                }
            }
        }

        private void TriggerDeathAnimationAndTeleport()
        {
            invincibleTime = 0f;
            _isDead = true;
            _boxlife = 0;
            _currentState = PlayerState.Dead;
            _animator[(int)PlayerState.Dead].Reset();
            _respawnInProgress = false;
            _respawnTime = DefaultRespawnTime;
        }

        private void TeleportToNearestCheckpoint()
        {
            // Find the nearest checkpoint
            float minDistance = float.MaxValue;
            OCheckpoint cp = null;

            Level.CurrentLevel.MiddleGroundLayer.Entities.ForEach(e =>
            {
                if (e is OCheckpoint)
                {
                    float distance = (e.Location - location).Length();
                    if (distance < minDistance)
                    {
                        cp = e as OCheckpoint;
                        minDistance = distance;
                    }
                }

            });

            if (cp != null)
            {
                location = cp.Location;
                invincibleTime = DefaultInvincibleTime;

                _currentState = PlayerState.Idle;
            }
        }

        private void LoseLife()
        {
            // Update lives count
            _lives--;

            // Sound
            Application.MagicContentManager.GetSound("s_flotch").Play(0.5f, 0, 0);

            Application.ScriptManager.SetFlag(LapinsScript.PlayerLives, _lives);
            LapinsScript.AddToScore(1);

            // BLOOOOOOD
            SpecialEffectsHelper.MakeBlood(dRect.Center.ToVector2(), 200f * scale.X, scale.Y, false, 16);

            invincibleTime = DefaultInvincibleTime;

            if (_lives < 0)
            {
                TriggerDeathAnimationAndTeleport();

                Application.ScriptManager.SetFlag(LapinsScript.EscapeState, 2);
            }
        }

        /// <summary>
        /// onDeathAction called when the player has no more lives or goes out of the level
        /// </summary>
        private void onDeathAction()
        {
            if (!_isDead)
            {
                LoseLife();
                TriggerDeathAnimationAndTeleport();


                // Cancel the isAlive = false
                if (_lives >= 0)
                {
                    IsAlive = true;
                }
            }
        }



        private void decreaseBoxLife()
        {
            if (_boxlife > 0)
            {
                _boxlife--;
                if (_boxlife == 0)
                {
                    //lets destroy this bloody crate ><
                    SpecialEffectsHelper.MakeExplosion(DstRect.Center.ToVector2(), Application.Random.GetRandomFloat(scale.X / 4, scale.X), Color.Beige, false);
                }
            }
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// from the XNA platformer kit
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (_isJumping)
            {
                bool first = false;
                float jumpVelocity = JumpLaunchVelocity + (_isRunning ? (JumpLaunchVelocity / 2) : 0f);

                // Begin or continue a jump
                if ((!_wasJumping && IsOnGround) || _jumpTime > 0.0f)
                {

                    if (_jumpTime == 0.0f)
                    {
                        if (_currentJump > 0)
                        {
                            string sound = "s_jump" + _currentJump;
                            Application.MagicContentManager.GetSound("s_jump1").Play(0.5f, 0, 0);
                        }

                        first = true;
                    }

                    _jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //sprite.PlayAnimation(jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < _jumpTime && _jumpTime <= MaxJumpTime)
                {
                    if (first)
                    {
                        velocityY += jumpVelocity * ((_boxlife > 0) ? 1f : 1) - ((_boxlife > 0) ? 0 : _currentJump * BunnyJumpCoef);
                        decreaseBoxLife();
                    }
                    else
                    {
                        velocityY += (jumpVelocity - _jumpTime * 50f) / 20;
                    }
                }
                else
                {
                    // Reached the apex of the jump
                    _jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                _jumpTime = 0.0f;
            }
            _wasJumping = _isJumping;

            return velocityY;
        }

        private void playEatSound()
        {
            /* if (_eatSounds == null)
             {
                 _eatSounds = new List<SoundEffect>()
                 {
                     Application.MagicContentManager.GetSound("s_eat1"),
                     Application.MagicContentManager.GetSound("s_eat2"),
                     Application.MagicContentManager.GetSound("s_eat3"),
                     Application.MagicContentManager.GetSound("s_eat4"),
                     Application.MagicContentManager.GetSound("s_eat5")
                 };
             }

             _eatSounds.GetRandomElement<SoundEffect>().Play(0.5f, 0, 0);*/
            Application.MagicContentManager.GetSound("s_eat1").Play(0.5f, 0, 0);
        }

        public override Entity Clone()
        {
            return new Player();
        }
    }
}
