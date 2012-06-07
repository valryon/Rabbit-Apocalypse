using Lapins.Engine.Core;
using Lapins.Engine.Graphics;
using Lapins.Engine.Input;
using Lapins.Engine.Input.Devices;
using Microsoft.Xna.Framework;
using Lapins.Data.Localization;

namespace Lapins.GameStates
{
    public class CreditsState : GameState
    {
        private Rectangle _srcMenu, _dstMenu;
        private string _paperTeam;
        private Vector2 _paperTeamLoc;

        public CreditsState()
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
            SceneCamera.FadeOut(30, null);

            _paperTeam = LocalizedStrings.GetString("TGPT");
            _paperTeamLoc = new Vector2(
                Resolution.VirtualWidth / 2 - Application.MagicContentManager.Font.MeasureString(_paperTeam).X / 2,
                50
                );
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (Device device in Application.InputManager.GetLinkedDevices(LogicalPlayerIndex.One))
            {
                if ((device.GetState(MappingButtons.A).IsPressed) || (device.GetState(MappingButtons.B).IsPressed) || (device.GetState(MappingButtons.Y).IsPressed) || (device.GetState(MappingButtons.Start).IsPressed))
                {
                    SceneCamera.FadeIn(30, () =>
                    {
                        ChangeCurrentGameState = true;
                        NextGameState = Application.GameStateManager.GetGameState<HomeState>();
                    });
                }
            }
        }

        public override void Draw(SpriteBatchProxy spriteBatch)
        {
            ParticuleManager.Draw(true);

            spriteBatch.BeginNoCamera();
            //-----------------------------

            // Bakcground
            spriteBatch.Draw(Application.MagicContentManager.GetTexture("splash1"), new Rectangle(0, 0, Resolution.VirtualWidth, Resolution.VirtualHeight), Color.White);

            spriteBatch.DrawString(Application.MagicContentManager.Font, _paperTeam, _paperTeamLoc, Color.DarkSlateBlue);

            Vector2 loc = new Vector2(250, 50);
            loc.Y += 50;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "Developpers:", loc, Color.DarkRed);
            loc.Y += 25;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "Damien MAYANCE, Thibault PERSON", loc, Color.Black);

            loc.Y += 50;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "2D Artist:", loc, Color.DarkRed);
            loc.Y += 25;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "Thibault PERSON", loc, Color.Black);

            loc.Y += 50;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "Level designers:", loc, Color.DarkRed);
            loc.Y += 25;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "Gabriel CORBEL, Damien MAYANCE", loc, Color.Black);

            loc.Y += 50;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "Sound designer:", loc, Color.DarkRed);
            loc.Y += 25;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "Thibault PERSON", loc, Color.Black);

            loc.Y += 50;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "Music composer:", loc, Color.DarkRed);
            loc.Y += 25;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "Spintronic", loc, Color.Black);
            loc.Y += 25;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "More songs @ www.chiptunes-headbangers.net/Spintronic", loc, Color.Black);
            

            loc.Y += 100;
            spriteBatch.DrawString(Application.MagicContentManager.Font, LocalizedStrings.GetString("TGPA"), loc, Color.DarkRed);
            loc.Y += 25;
            spriteBatch.DrawString(Application.MagicContentManager.Font, "http://www.thegreatpaperadventure.com", loc, Color.Black);

            //-----------------------------
            // Draw back button
            spriteBatch.Draw(Application.MagicContentManager.GetTexture("menu"), _dstMenu, _srcMenu, Color.White, 0f, Vector2.Zero, Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally, 1.0f);

            spriteBatch.Draw(Application.MagicContentManager.GetTexture("hud"), _dstMenu.Location.ToVector2() + new Vector2(20, 16), new Rectangle(64, 0, 32, 32), Color.White);
            spriteBatch.DrawString(Application.MagicContentManager.Font,
                                    LocalizedStrings.GetString("Back"),
                                    _dstMenu.Location.ToVector2() + new Vector2(60, 20),
                                    Color.Yellow
                                   );


            spriteBatch.End();

            ParticuleManager.Draw(false);

        }

        public override bool ChangeCurrentGameState { get; protected set; }
        public override GameState NextGameState { get; protected set; }
    }
}
