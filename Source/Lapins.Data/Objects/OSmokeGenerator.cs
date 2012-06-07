using Lapins.Data.Ogmo;
using Lapins.Engine.Physics;
using Microsoft.Xna.Framework;
using OgmoXNA4.Values;
using Lapins.Engine.Core;
using Lapins.Data.Particules;

namespace Lapins.Data.Objects
{
    [OgmoObjectId("smoke_generator")]
    public class OSmokeGenerator : OgmoObjectEntity
    {
        private int _minRangeX;
        private int _minRangeY;
        private int _maxRangeX;
        private int _maxRangeY;
        private float _minSize, _maxSize;
        private bool _isBackground;
        private float _frequency;

        private float _cooldown;

        public OSmokeGenerator(Vector2 location)
            : base("", location)
        {

        }

        public override void OnValueFound(string id, OgmoValue value)
        {
            if (id == "minRangeX")
            {
                _minRangeX = ((OgmoIntegerValue)value).Value;
            }
            else if (id == "minRangeY")
            {
                _minRangeY = ((OgmoIntegerValue)value).Value;
            }
            else if (id == "maxRangeX")
            {
                _maxRangeX = ((OgmoIntegerValue)value).Value;
            }
            else if (id == "maxRangeY")
            {
                _maxRangeY = ((OgmoIntegerValue)value).Value;
            }
            else if (id == "minSize")
            {
                _minSize = ((OgmoNumberValue)value).Value;
            }
            else if (id == "maxSize")
            {
                _maxSize = ((OgmoNumberValue)value).Value;
            }
            else if (id == "isBackground")
            {
                _isBackground = ((OgmoBooleanValue)value).Value;
            }
            else if (id == "frequency")
            {
                _frequency = ((OgmoNumberValue)value).Value;
                _cooldown = Application.Random.GetRandomFloat(0f, _frequency);
            }
        }

        public override void Update(GameTime gameTime)
        {
            _cooldown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_cooldown <= 0)
            {
                _cooldown = _frequency;

                // Smoke
                SpecialEffectsHelper.MakeSmoke(location, 
                    Application.Random.GetRandomVector2(_minRangeX, _maxRangeX, _minRangeY, _maxRangeY), 
                    Application.Random.GetRandomFloat(_minSize,_maxSize), 
                    Color.Gray, _isBackground, 1);
            }

            base.Update(gameTime);
        }

        public override void Draw(Engine.Graphics.SpriteBatchProxy spriteBatch)
        {
        }

        public override Microsoft.Xna.Framework.Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 0, 0); }
        }

        public override Engine.World.Entity Clone()
        {
            return new OSmokeGenerator(location)
            {
                _minRangeX = _minRangeX,
                _minRangeY = _minRangeY,
                _maxRangeX = _maxRangeX,
                _maxRangeY = _maxRangeY,
                _isBackground = _isBackground,
                _minSize = _minSize,
                _maxSize = _maxSize,
                _frequency = _frequency,
                _cooldown = _cooldown
            };
        }

    }
}
