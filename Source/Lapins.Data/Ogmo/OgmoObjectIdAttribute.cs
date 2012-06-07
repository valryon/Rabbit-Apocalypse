using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lapins.Data.Ogmo
{
    /// <summary>
    /// Tells the engine the entity name in Ogmo editor
    /// </summary>
    public class OgmoObjectIdAttribute : Attribute
    {
        public string OgmoName { get; set; }

        public OgmoObjectIdAttribute(string name)
        {
            OgmoName = name;
        }
    }
}
