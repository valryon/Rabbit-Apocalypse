using System.Collections.Generic;
using System.Linq;
using Lapins.Data.Entities;
using Lapins.Data.Levels;
using Lapins.Data.Objects;
using Lapins.Engine.Content;
using Lapins.Engine.Core;
using Lapins.Engine.Graphics;
using Lapins.Engine.World;
using Microsoft.Xna.Framework;

namespace Lapins.UI
{
    #region Compass Elements

    public abstract class CompassElement
    {
        public CompassElement(Vector2 location)
        {
            Location = location;
        }

        public Vector2 Location { get; set; }
        public abstract Rectangle SrcRect { get; }
        public Rectangle DstRect { get; protected set; }

        public bool IsRevealed { get; set; }

        public virtual void Draw(SpriteBatchProxy spriteBatch)
        {
            DstRect = new Rectangle((int)Location.X, (int)Location.Y, SrcRect.Width, SrcRect.Height);

            if (IsRevealed)
            {
                spriteBatch.Draw(Application.MagicContentManager.GetTexture("compass"), DstRect, SrcRect, Color.White);
            }
        }
    }

    public class PlayerCompassElement : CompassElement
    {
        public PlayerCompassElement(Vector2 location)
            : base(location)
        {
            IsRevealed = true;
        }

        public override Rectangle SrcRect { get { return new Rectangle(0, 0, 16, 16); } }
    }

    public class TerrierCompassElement : CompassElement
    {
        public TerrierCompassElement(Vector2 location)
            : base(location)
        {
            //IsColouredIn = false;
            //IsColouredOut = false;
        }

        //public override void Draw(SpriteBatchProxy spriteBatch)
        //{
        //    if (IsRevealed)
        //    {
        //        Rectangle src = SrcRect;

        //        // Mid dst
        //        var midLeftSrc = new Rectangle(src.X, src.Y, src.Width / 2, src.Height);
        //        var midRightSrc = new Rectangle(src.X + src.Width / 2, src.Y, src.Width / 2, src.Height);

        //        DstRect = new Rectangle((int)Location.X, (int)Location.Y, SrcRect.Width/2, SrcRect.Height);
        //        spriteBatch.Draw(Application.MagicContentManager.GetTexture("compass"), DstRect, midLeftSrc, ColorIn);

        //        DstRect = new Rectangle((int)Location.X + SrcRect.Width / 2, (int)Location.Y, SrcRect.Width / 2, SrcRect.Height);
        //        spriteBatch.Draw(Application.MagicContentManager.GetTexture("compass"), DstRect, midRightSrc, ColorOut);
        //    }
        //}

        public override Rectangle SrcRect { get { return new Rectangle(16, 0, 16, 16); } }
        //public Color ColorIn { get; set; }
        //public Color ColorOut { get; set; }
        //public bool IsColouredIn { get; set; }
        //public bool IsColouredOut { get; set; }
    }

    public class KeysCompassElement : CompassElement
    {
        public KeysCompassElement(Vector2 location)
            : base(location)
        {
        }

        public override Rectangle SrcRect { get { return new Rectangle(32, 0, 16, 16); } }
    }

    public class ShipCompassElement : CompassElement
    {
        public ShipCompassElement(Vector2 location)
            : base(location)
        {
            IsRevealed = true;
        }

        public override Rectangle SrcRect { get { return new Rectangle(48, 0, 16, 16); } }
    }

    #endregion

    /// <summary>
    /// Get information from the scene (such as keys, ship, player, item location) and display them on a compass
    /// </summary>
    [TextureContent(AssetName = "compass", AssetPath = "gfxs/misc/compass")]
    public class Compass
    {
        private Dictionary<Entity, CompassElement> _elements;

        private Rectangle sRect, dRect;
        //private List<Color> _availableColors = new List<Color>() {
        //    Color.Red,
        //    Color.Pink,
        //    Color.Blue,
        //    Color.Gray,
        //    Color.Green,
        //    Color.Violet,
        //    Color.Orange,
        //    Color.Yellow,
        //    Color.Turquoise
        //};

        public Compass()
        {
            _elements = new Dictionary<Entity, CompassElement>();

            sRect = new Rectangle(0, 16, 128, 16);

            // Screen position
            dRect.Width = 500;
            dRect.Height = 16;
            dRect.X = Resolution.VirtualWidth / 2 - dRect.Width / 2;
            dRect.Y = Resolution.VirtualHeight - dRect.Height ;
        }

        public void Initialize()
        {
            _elements.Clear();
        }

        public void Update(GameTime gameTime)
        {
            // Scan entities and update the list
            foreach (Entity ent in Level.CurrentLevel.MiddleGroundLayer.Entities)
            {
                if (ent is Player)
                {
                    updatePlayers(ent);
                }
                else if (ent is OShip)
                {
                    updateShips(ent);
                }
                else if (ent is OTerrier)
                {
                    updateTerriers(ent, true);
                }
                else if (ent is Keys)
                {
                    updateKeys(ent);
                }
            }
        }

        private void updateKeys(Entity ent)
        {
            if (_elements.Keys.Contains(ent) == false)
            {
                _elements.Add(ent, new KeysCompassElement(computeCompassLocation(ent.Location)));
            }
            else
            {
                if (ent.IsOnScreen())
                {
                    _elements[ent].IsRevealed = true;
                }
            }
        }

        private void updatePlayers(Entity ent)
        {
            if (_elements.Keys.Contains(ent) == false)
            {
                _elements.Add(ent, new PlayerCompassElement(computeCompassLocation(ent.Location)));
            }
            else
            {
                _elements[ent].Location = computeCompassLocation(ent.Location);
            }
        }

        private void updateShips(Entity ent)
        {
            if (_elements.Keys.Contains(ent) == false)
            {
                _elements.Add(ent, new ShipCompassElement(computeCompassLocation(ent.Location)));
            }
        }

        private void updateTerriers(Entity ent, bool color)
        {
            if (_elements.Keys.Contains(ent) == false)
            {
                _elements.Add(ent, new TerrierCompassElement(computeCompassLocation(ent.Location)));
            }
            else
            {
                if (ent.IsOnScreen())
                {
                    _elements[ent].IsRevealed = true;

                    // COloration algo, not working if outputs can be the same for multiple terriers
                    //var terrierCompass = _elements[ent] as TerrierCompassElement;

                    //if (terrierCompass.IsColouredIn == false)
                    //{
                    //    // Autocolor linked terriers
                    //    // Choose a color
                    //    Color randColor = _availableColors[Application.Random.GetRandomInt(_availableColors.Count)];
                    //    _availableColors.Remove(randColor);

                    //    terrierCompass.ColorIn = randColor;
                    //    terrierCompass.IsColouredIn = true;

                    //    // Other terrier color
                    //    var linkedTerrier = ((OTerrier)ent).LinkedTerrier;
                    //    if (linkedTerrier != null)
                    //    {
                    //        updateTerriers(linkedTerrier, false);

                    //        var linkedTerrierCompass = _elements[linkedTerrier] as TerrierCompassElement;
                    //        linkedTerrierCompass.IsColouredOut = true;
                    //        linkedTerrierCompass.ColorOut = randColor;
                    //    }
                    //}
                }
            }
        }

        public void Draw(SpriteBatchProxy spriteBatch)
        {
            spriteBatch.BeginNoCamera();

            // Draw compass
            spriteBatch.Draw(Application.MagicContentManager.GetTexture("compass"), dRect, sRect, Color.White);

            foreach (CompassElement element in _elements.Values)
            {
                element.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        private Vector2 computeCompassLocation(Vector2 entityLocation)
        {
            Vector2 loc = new Vector2();

            // Get the relative position (%) of the entity on the level
            Rectangle levelDimensions = Level.CurrentLevel.Dimensions;
            float purcent = (entityLocation.X * 100) / levelDimensions.Width;

            loc.X = dRect.X + (purcent * dRect.Width) / 100;
            loc.Y = dRect.Y - 8;

            return loc;
        }
    }
}
