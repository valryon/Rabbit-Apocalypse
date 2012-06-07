using System.Collections.Generic;
using Lapins.Engine.World;

namespace Lapins.Data.Levels
{
    /// <summary>
    /// Level Part layer
    /// </summary>
    public class Layer
    {
        private Grid _grid;
        private List<Entity> _entities;

        public Layer()
        {
            _grid = new Grid();
            _entities = new List<Entity>();
        }

        public Grid Tiles
        {
            get { return _grid; }
            set { _grid = value; }
        }
        public List<Entity> Entities
        {
            get { return _entities; }
        }

        internal Layer Clone()
        {
            var clone = new Layer()
            {
                _grid = this.Tiles.Clone()
            };

            foreach (var ent in Entities)
            {
                clone.Entities.Add(ent.Clone());
            }

            return clone;
        }
    }
}
