using System.Reflection;
using Lapins.Data.Levels;
using Lapins.Engine.Content;
using Lapins.Engine.Core;
using Lapins.Engine.Storage;
using Lapins.GameStates;
using Lapins.Save;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lapins.Data.Music;

namespace Lapins
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    #region Basic content declaration
    [TextureContent(AssetName = "particules", AssetPath = "gfxs/misc/particules")]
    [TextureContent(AssetName = "null", AssetPath = "gfxs/misc/1x1", LoadOnStartup = false)]
    [FontContent(AssetName = "font", AssetPath = "fonts/spriteFont", IsDefaultFont = true)]
    #endregion
    public sealed class Game : Application
    {
        public static Saver<LapinsSaveData> Saver;
        private MusicPlayer _musicPlayer;

        private bool _reloadConfiguration;

        public Game()
            : base("Rabbit Apocalypse!", "Content", "0.5")
        {
            Saver = new Saver<LapinsSaveData>(this, "lapins.lps", "Lapins");

            // Uncomment this to force a language
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            LevelLoader.Content = this.Content;
        }

        protected override void Initialize()
        {
            _reloadConfiguration = false;

            base.Initialize();

            // Game states
            GameStateManager.RegisterGameState(new SplashscreensState());
            GameStateManager.RegisterGameState(new HomeState());
            GameStateManager.RegisterGameState(new PlayState());
            GameStateManager.RegisterGameState(new GameOverState());
            GameStateManager.RegisterGameState(new CreditsState());
            GameStateManager.RegisterGameState(new HighscoreState());
            GameStateManager.RegisterGameState(new LoadingState());

            // Let's load the savegame and wait until it's not
            Saver.ForceLoadAsync();
            Saver.LoadCompleted += Saver_LoadCompleted;

            _musicPlayer = new MusicPlayer();

            // Load the first scene
            GameStateManager.LoadGameState(GameStateManager.GetGameState<SplashscreensState>());
        }

        void Saver_LoadCompleted(LapinsSaveData data)
        {
            Saver.LoadCompleted -= Saver_LoadCompleted;

            if (data == null)
            {
                Saver.Initialize(new LapinsSaveData());
            }

            _reloadConfiguration = true;
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_reloadConfiguration)
            {
                _reloadConfiguration = false;

                // Try to change resolution using the savec configuration
                Resolution.SetResolution(Saver.Data.ResolutionWidth, Saver.Data.ResolutionHeight, Saver.Data.IsFullscreen);
            }

            _musicPlayer.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _musicPlayer.Draw(SpriteBatch);

#if DEBUG
            SpriteBatch.BeginNoCamera();
            Application.FpsCounter.Draw(SpriteBatch, new Vector2(GameResolutionWidth - 100, GameResolutionHeight - 30));
            SpriteBatch.End();
#endif
        }

        protected override int GameResolutionWidth { get { return 1280; } }

        protected override int GameResolutionHeight { get { return 720; } }

        protected override int ScreenResolutionWidth { get { return 1280; } }

        protected override int ScreenResolutionHeight { get { return 720; } }

        protected override bool IsFullscreen { get { return false; } }

        protected override System.Reflection.Assembly[] GameAssemblies
        {
            get { return new Assembly[] { GetType().Assembly, typeof(Level).Assembly }; }
        }
    }
}
