using Lapins.Engine.Content;
using Lapins.Engine.Core;
using Lapins.Engine.Graphics;
using Lapins.Engine.Input;
using Lapins.Engine.Input.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lapins.Data.Utils
{
    /// <summary>
    /// Display controller buttons icons on the screen
    /// </summary>
    [TextureContent(AssetName = "buttons", AssetPath = "gfxs/misc/buttons")]
    public static class ButtonPrinter
    {
        /// <summary>
        /// Translate a button to a picture, depending on the device 
        /// </summary>
        /// <returns></returns>
        private static Rectangle GetButtonRect(MappingButtons button, DeviceType deviceType)
        {
            Rectangle dst = Rectangle.Empty;

            if (deviceType == DeviceType.KeyboardMouse)
            {
                return new Rectangle(0, 0, 72, 68);
            }
            else if (deviceType == DeviceType.Gamepad)
            {
                switch (button)
                {
                    case MappingButtons.Start:
                        dst = new Rectangle(713, 415, 81, 93);
                        break;

                    case MappingButtons.Back:
                        dst = new Rectangle(491, 425, 80, 82);
                        break;

                    case MappingButtons.A:
                        dst = new Rectangle(878, 437, 76, 83);
                        break;

                    case MappingButtons.B:
                        dst = new Rectangle(82, 625, 76, 82);
                        break;

                    case MappingButtons.Y:
                        dst = new Rectangle(2, 625, 76, 82);
                        break;

                    case MappingButtons.X:
                        dst = new Rectangle(798, 437, 76, 83);
                        break;

                    default:
                        dst = Rectangle.Empty;
                        break;
                }
            }

            return dst;
        }

        private static Rectangle GetThumbstickRect()
        {
        return new Rectangle(1,403,75,85);
        }

        /// <summary>
        /// Display a button on the screen
        /// </summary>
        public static void Draw(SpriteBatchProxy spriteBatch, MappingButtons button, Device device, Vector2 loc)
        {
            Draw(spriteBatch, button, device, loc, Color.White);
        }

        /// <summary>
        /// Display a button on the screen
        /// </summary>
        /// <param name="color">Special color</param>
        public static void Draw(SpriteBatchProxy spriteBatch, MappingButtons button, Device device, Vector2 location, Color color)
        {
            if (device.Type == DeviceType.KeyboardMouse)
            {
                var keyboard = (KeyboardDevice)device;
                var key = keyboard.GetMapping(button);

                if (key != null)
                {
                    // Draw binded letter
                    spriteBatch.DrawString(Application.MagicContentManager.Font, key.ToString(), location, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                }
            }
            else
            {
                Rectangle src = GetButtonRect(button, device.Type);
                Rectangle dst = src;
                dst.X = (int)location.X;
                dst.Y = (int)location.Y;
                dst.Width /= 3;
                dst.Height /= 3;

                spriteBatch.Draw(Application.MagicContentManager.GetTexture("buttons"), dst, src, color);
            }
        }

        /// <summary>
        /// Display a left stick on the screen
        /// </summary>
        /// <param name="color">Special color</param>
        public static void DrawThumbstickLeft(SpriteBatchProxy spriteBatch, Device device, Vector2 location, Color color)
        {
            if (device.Type == DeviceType.KeyboardMouse)
            {
                var keyboard = (KeyboardDevice)device;
                var keys = keyboard.GetLeftThumbstickMapping();

                // Draw binded letters
                spriteBatch.DrawString(Application.MagicContentManager.Font, keys[0] + "/" + keys[1] + "/" + keys[2] + "/" + keys[3], location, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                Rectangle src = GetThumbstickRect();
                Rectangle dst = src;
                dst.X = (int)location.X;
                dst.Y = (int)location.Y;
                dst.Width /= 3;
                dst.Height /= 3;

                spriteBatch.Draw(Application.MagicContentManager.GetTexture("buttons"), dst, src, color);
                spriteBatch.DrawString(Application.MagicContentManager.Font, Localization.LocalizedStrings.GetString("LeftStick"),location + new Vector2(dst.Width,0),color);
            }
        }

        /// <summary>
        /// Display a right on the screen
        /// </summary>
        /// <param name="color">Special color</param>
        public static void DrawThumbstickRight(SpriteBatchProxy spriteBatch, Device device, Vector2 location, Color color)
        {
            if (device.Type == DeviceType.KeyboardMouse)
            {
                var keyboard = (KeyboardDevice)device;
                var keys = keyboard.GetRightThumbstickMapping();

                // Draw binded letters
                spriteBatch.DrawString(Application.MagicContentManager.Font, keys[0] + "/" + keys[1] + "/" + keys[2] + "/" + keys[3], location, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                Rectangle src = GetThumbstickRect();
                Rectangle dst = src;
                dst.X = (int)location.X;
                dst.Y = (int)location.Y;
                dst.Width /= 3;
                dst.Height /= 3;

                spriteBatch.Draw(Application.MagicContentManager.GetTexture("buttons"), dst, src, color);
                spriteBatch.DrawString(Application.MagicContentManager.Font, Localization.LocalizedStrings.GetString("RightStick"), location + new Vector2(dst.Width, 0), color);
            }
        }
    }
}
