using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lapins.Engine.World;
using Microsoft.Xna.Framework;
using OgmoXNA4.Values;

namespace Lapins.Data.Ogmo
{
    /// <summary>
    /// Easy to instanciate Ogmo objects
    /// </summary>
    public abstract class OgmoObjectEntity : Entity
    {
        public OgmoObjectEntity(string assetName, Vector2 location)
            : base(assetName, location, new Rectangle(0, 0, 32, 32), Vector2.One)
        {
            sRect = BaseSrcRect;
            ComputeDstRect();
        }

        /// <summary>
        /// Values from the project file are found
        /// </summary>
        public virtual void OnValueFound(string id, OgmoValue value)
        {
        }

        public abstract Rectangle BaseSrcRect { get; }
    }
}
