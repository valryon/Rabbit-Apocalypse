using System;
using Lapins.Data.Commands;
using Lapins.Engine.Content;
using Lapins.Engine.Core;
using Lapins.Engine.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Lapins.Data.Objects;

namespace Lapins.UI
{
    /// <summary>
    /// Display player's information
    /// </summary>
    [TextureContent(AssetName = "hud", AssetPath = "gfxs/misc/HUD")]
    public class HUD
    {
        private const int yHudLocation = 24;

        private ITextBox _textbox;

        public HUD()
        {
        }

        public void Initialize()
        {

        }

        public void Update(GameTime gametime)
        {
            string textToDisplay = Application.ScriptManager.GetFlag<string>(LapinsScript.TextToDisplay);

            if (String.IsNullOrEmpty(textToDisplay))
            {
                _textbox = null;
            }
            else
            {
                if (_textbox == null)
                {
                    if (textToDisplay == "controls")
                    {
                        _textbox = new ControlTextBox();
                    }
                    else
                    {
                        _textbox = new TextBox(textToDisplay);
                    }
                }
                _textbox.Update(gametime);
            }

        }

        private static int HudLetterSize = 32;
        public void Draw(SpriteBatchProxy spriteBatch)
        {
            bool playerExists = Application.ScriptManager.GetFlag<bool>(LapinsScript.IsPlayerInstanciated);

            if (playerExists)
            {
                int lives = Application.ScriptManager.GetFlag<int>(LapinsScript.PlayerLives);
                int score = Application.ScriptManager.GetFlag<int>(LapinsScript.Score);
                float timeLeft = Application.ScriptManager.GetFlag<float>(LapinsScript.TimeLeft);
                bool hasKeys = Application.ScriptManager.GetFlag<bool>(LapinsScript.HasKeys);
                int carrotes = Application.ScriptManager.GetFlag<int>(LapinsScript.CarrotsCount); ;
                List<Letter> letters = Application.ScriptManager.GetFlag<List<Letter>>(LapinsScript.CollectedLetters); ;

                var span = TimeSpan.FromSeconds(timeLeft);

                spriteBatch.BeginNoCamera();

                spriteBatch.Draw(Application.MagicContentManager.GetTexture("hud"), new Vector2(110, yHudLocation), new Rectangle(0, 110, 64, 16), Color.White);
                DrawNumberAsSprite(spriteBatch, new Vector2(180, yHudLocation), ":" + score.ToString("000000000"));
                drawLifeBar(spriteBatch, lives, Vector2.Zero);

                if (hasKeys)
                {
                    spriteBatch.Draw(Application.MagicContentManager.GetTexture("keys"), new Vector2(500, yHudLocation), new Rectangle(0, 0, 32, 32), Color.White);
                }

                // Carrottes
                spriteBatch.Draw(Application.MagicContentManager.GetTexture("hud"), new Vector2(1280 - 6 * 16 - 80, yHudLocation - 8), new Rectangle(64, 0, 32, 32), Color.White);
                DrawNumberAsSprite(spriteBatch, new Vector2(1280 - 6 * 16 - 40, yHudLocation), "X" + carrotes.ToString("00000"));

                //time
                DrawNumberAsSprite(spriteBatch, new Vector2(600, yHudLocation), span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00"));


                // Letters
                for (int i = 0; i < letters.Count; i++)
                {
                    letters[i].DstRect = new Rectangle(840 + HudLetterSize * OLetterSpawn.Letters.IndexOf(letters[i].TheLetter), yHudLocation, HudLetterSize, HudLetterSize);
                    letters[i].Draw(spriteBatch);
                }

                if (_textbox != null)
                {
                    _textbox.Draw(spriteBatch);
                }

                spriteBatch.End();



            }
        }

        private const int HorizontalPadding = 0;
        private const int VerticalPadding = 0;

        private void drawLifeBar(SpriteBatchProxy spritebatch, int life, Vector2 loc)
        {
            int maxLives = Application.ScriptManager.GetFlag<int>(LapinsScript.PlayerLivesMax);
            Rectangle src = new Rectangle(0, 0, 32, 32);
            Rectangle dst = new Rectangle(24, yHudLocation, 48, 48);

            for (int i = 0; i < maxLives; i++)
            {
                if (i < life)
                {
                    spritebatch.Draw(Application.MagicContentManager.GetTexture("hud"), dst, src, Color.White);
                }
                else
                {
                    //spritebatch.Draw(Application.MagicContentManager.GetTexture("hud"), dst, src, Color.White);
                }
                //dst.Offset(location.Width + HorizontalPadding, 0);
                dst.Offset(0, dst.Height + VerticalPadding);
            }

        }



        private void DrawNumberAsSprite(SpriteBatchProxy spritebatch, Vector2 loc, string str)
        {

            var src = new Rectangle(0, 92, 16, 16);
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (char.IsDigit(str, i))
                {
                    src.X = (str[i] - '0') * 16;
                }
                else if (str[i] == 'X')
                {
                    src.X = 160;
                }
                else if (str[i] == ':')
                {
                    src.X = 176;
                }
                else
                {
                    continue;
                }
                spritebatch.Draw(Application.MagicContentManager.GetTexture("hud"), loc, src, Color.White);
                loc.X += 16;
            }
        }
    }



}
