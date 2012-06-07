using System;
using Lapins.Data.Commands;
using Lapins.Data.Localization;
using Lapins.Data.Ogmo;
using Lapins.Engine.Core;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;
using OgmoXNA4.Values;

namespace Lapins.Data.Objects
{
    /// <summary>
    /// 
    /// </summary>
    [OgmoObjectId("pancarte")]
    public class OPancarte : OgmoObjectEntity
    {
        private string _text;

        public OPancarte(Vector2 location)
            : base("opancarte", location)
        {
            hitbox = new Hitbox(BaseSrcRect);
            _text = "";
            LayerDepth = 75;
        }

        public override void OnValueFound(string id, OgmoValue value)
        {
            if (id == "text")
            {
                _text = LocalizedStrings.GetString(((OgmoStringValue)value).Value);
            }
            else if (id == "isControl")
            {
                if (((OgmoBooleanValue)value).Value)
                {
                    _text = "controls";
                }
            }
        }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 64, 64); }
        }

        public override Engine.World.Entity Clone()
        {
            return new OPancarte(location)
            {
                _text = _text
            };
        }

        public bool IsActivated
        {
            set
            {
                if (value)
                {
                    string textToDisplay = Application.ScriptManager.GetFlag<string>(LapinsScript.TextToDisplay);

                    if (String.IsNullOrEmpty(textToDisplay))
                    {
                        Application.ScriptManager.SetFlag(LapinsScript.TextToDisplay, _text);
                    }
                }
            }
        }
    }
}
