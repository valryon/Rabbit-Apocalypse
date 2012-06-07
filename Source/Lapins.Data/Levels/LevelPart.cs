using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Lapins.Data.Levels
{
    /// <summary>
    /// Piece of level for the generation
    /// </summary>
    public class LevelPart
    {
        private Layer _backgroundLayer;
        private Layer _middleGroundLayer;
        private Layer _foregroundLayer;
        private List<Rectangle> _collisionsMask;

        public int Id { get; private set; }
        public int InputId { get; set; }
        public int InputYLoc { get; set; }
        public int OutputId { get; set; }
        public int OutputYLoc { get; set; }
        public int MaxRepeat { get; private set; }
        public bool IsStart { get; set; }
        public bool IsEnd { get; set; }
        public Vector2 Dimensions { get; private set; }

        public LevelPart(int id, int maxRepeat, Vector2 dimensions)
        {
            Id = id;
            Dimensions = dimensions;
            MaxRepeat = maxRepeat;

            _backgroundLayer = new Layer();
            _middleGroundLayer = new Layer();
            _foregroundLayer = new Layer();
            _collisionsMask = new List<Rectangle>();
        }

        public Layer BackgroundLayer
        {
            get { return _backgroundLayer; }
            private set { _backgroundLayer = value; }
        }

        public Layer MiddleGroundLayer
        {
            get { return _middleGroundLayer; }
            private set { _middleGroundLayer = value; }
        }

        public Layer ForegroundLayer
        {
            get { return _foregroundLayer; }
            private set { _foregroundLayer = value; }
        }

        public List<Rectangle> CollisionsMask
        {
            get { return _collisionsMask; }
            set { _collisionsMask = value; }
        }

        public override string ToString()
        {
            // DEBUG
            return InputId + " - " + Id + (IsStart ? "Start" : "") + (IsEnd ? "End" : "") + " - " + OutputId;
        }

        internal LevelPart Clone()
        {
            var cloneColList = new List<Rectangle>();
            foreach (Rectangle rect in CollisionsMask)
            {
                cloneColList.Add(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));
            }

            var clone = new LevelPart(Id, MaxRepeat, Dimensions)
            {
                InputId = InputId,
                InputYLoc = InputYLoc,
                OutputId = OutputId,
                OutputYLoc = OutputYLoc,
                IsStart = IsStart,
                IsEnd = IsEnd,
                BackgroundLayer = this.BackgroundLayer.Clone(),
                MiddleGroundLayer = this.MiddleGroundLayer.Clone(),
                ForegroundLayer = this.ForegroundLayer.Clone(),
                CollisionsMask = cloneColList
            };

            return clone;
        }
    }
}
