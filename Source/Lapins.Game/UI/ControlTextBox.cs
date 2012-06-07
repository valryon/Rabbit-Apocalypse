using System;
using Lapins.Data.Localization;
using Lapins.Engine.Content;
using Lapins.Engine.Core;
using Lapins.Engine.Graphics;
using Microsoft.Xna.Framework;
using Lapins.Data.Utils;
using Lapins.Engine.Input;
using Lapins.Engine.Input.Devices;

namespace Lapins.UI
{
    /// <summary>
    /// Simple HUD modal textbox to display controls to player
    /// </summary>
    public class ControlTextBox : ITextBox
    {
        private Rectangle _srcRect;
        private Vector2 _size, _location;
        private string _skip;
        private Vector2 _skip_size;
        private int _anim = 0;
        private double _time = 0;

        public ControlTextBox()
        {
            _srcRect = new Rectangle(0, 0, 16, 16);
            _size = new Vector2(350, 200);

            _location = new Vector2(Resolution.VirtualWidth / 2 - _size.X / 2, Resolution.VirtualHeight / 2 - _size.Y / 2);
            _skip = LocalizedStrings.GetString("Action");
            _skip_size = Application.MagicContentManager.Font.MeasureString(_skip);
        }

        public void Update(GameTime gametime)
        {
            _time += gametime.ElapsedGameTime.Milliseconds;
            if (_time > 500)
            {
                _time = 0;
                _anim = (_anim == 0) ? 1 : 0;
            }
        }

        private const int MARGIN = 32;
        private const int COLSIZE = 150
            ;
        public void Draw(SpriteBatchProxy spriteBatch)
        {
            Rectangle dst = new Rectangle((int)_location.X - MARGIN, (int)_location.Y - MARGIN, 16, 16);
            _srcRect.X = _srcRect.Y = 0;

            for (int j = 0; j <= Math.Floor((double)((_size.Y + MARGIN * 2) / _srcRect.Height)); j++)
            {
                for (int i = 0; i <= Math.Floor((double)((_size.X + MARGIN * 2) / _srcRect.Width)); i++)
                {
                    spriteBatch.Draw(Application.MagicContentManager.GetTexture("textbox"), dst, _srcRect, Color.White);

                    dst.Offset(16, 0);

                    if (i == 0)
                    {
                        // first so offset for srcRect
                        _srcRect.Offset(16, 0);
                    }
                    else if (i == Math.Floor((double)((_size.X + MARGIN * 2) / _srcRect.Width)) - 1)
                    {
                        //last
                        _srcRect.Offset(16, 0);
                    }
                }

                dst.Offset(-16 * (int)(Math.Floor((double)((_size.X + MARGIN * 2) / _srcRect.Width)) + 1), 16);
                _srcRect.X = 0;

                if (j == 0)
                {
                    // first so offset for srcRect
                    _srcRect.Offset(0, 16);
                }
                else if (j == Math.Floor((double)((_size.Y + MARGIN * 2) / _srcRect.Height)) - 1)
                {
                    //last
                    _srcRect.Offset(0, 16);
                }
            }

            _srcRect.X = _anim * 16;
            _srcRect.Y = 48;

            dst.Offset((int)(_size.X + MARGIN * 2) - (16 + (int)_skip_size.X + 8), -((int)_skip_size.Y + 8));

            spriteBatch.Draw(Application.MagicContentManager.GetTexture("textbox"), dst, _srcRect, Color.White);

            dst.Offset(16, 0);

            spriteBatch.DrawString(Application.MagicContentManager.Font, _skip, dst.Location.ToVector2(), Color.Black);

            // Draw controls
            var devices = Application.InputManager.GetLinkedDevices(LogicalPlayerIndex.One).FindAll(d => d.IsConnected);
            int x = COLSIZE;

            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];

                    Vector2 loc = _location;

                    // Display name
                    string name = LocalizedStrings.GetString("Move");

                    spriteBatch.DrawString(Application.MagicContentManager.Font, name, loc, Color.White);
                    ButtonPrinter.DrawThumbstickLeft(spriteBatch, device, loc + new Vector2(i == 1 ? x - (COLSIZE / 3) : x, i == 1 ? 40 : 0), Color.White);

                    name = LocalizedStrings.GetString("Jump");
                    loc.Y += 40 * devices.Count;

                    spriteBatch.DrawString(Application.MagicContentManager.Font, name, loc, Color.White);
                    ButtonPrinter.Draw(spriteBatch, MappingButtons.A, device, loc + new Vector2(x, 0), Color.White);

                    name = LocalizedStrings.GetString("Walk");
                    loc.Y += 40;

                    spriteBatch.DrawString(Application.MagicContentManager.Font, name, loc, Color.White);
                    ButtonPrinter.Draw(spriteBatch, MappingButtons.X, device, loc + new Vector2(x, 0), Color.White);

                    name = LocalizedStrings.GetString("Action");

                    loc.Y += 40;

                    spriteBatch.DrawString(Application.MagicContentManager.Font, name, loc, Color.White);
                    ButtonPrinter.Draw(spriteBatch, MappingButtons.Y, device, loc + new Vector2(x, 0), Color.White);
                
                x += COLSIZE / 2;
            }
        }
    }
}
