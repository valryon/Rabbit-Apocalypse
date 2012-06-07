using Lapins.Data.Ogmo;
using Lapins.Engine.Physics;
using Lapins.Engine.World;
using Microsoft.Xna.Framework;

namespace Lapins.Data.Objects
{
    public abstract class OSpikes : OgmoObjectEntity
    {
        public OSpikes(string asset, Vector2 location)
            : base(asset, location)
        {
            LayerDepth = 90;
            IsRemovable = false;
        }

        public override Rectangle BaseSrcRect
        {
            get { return new Rectangle(0, 0, 32, 32); }
        }
    }

    [OgmoObjectId("spikes_floor")]
    public class SpikesFloor : OSpikes
    {
        public SpikesFloor(Vector2 location)
            : base("ospikesFloor", location)
        {
            hitbox = new Hitbox(new Rectangle(0, 10, 32, 22));
        }

        public override Entity Clone()
        {
            return new SpikesFloor(location);
        }
    }

    [OgmoObjectId("spikes_ceiling")]
    public class SpikesCeiling : OSpikes
    {
        public SpikesCeiling(Vector2 location)
            : base("ospikesCeiling", location)
        {
            hitbox = new Hitbox(new Rectangle(0, 0, 32, 22));
        }

        public override Entity Clone()
        {
            return new SpikesCeiling(location);
        }
    }

    [OgmoObjectId("spikes_left")]
    public class SpikesLeft : OSpikes
    {
        public SpikesLeft(Vector2 location)
            : base("ospikesLeft", location)
        {
            hitbox = new Hitbox(new Rectangle(0, 0, 22, 32));
        }

        public override Entity Clone()
        {
            return new SpikesLeft(location);
        }
    }

    [OgmoObjectId("spikes_right")]
    public class SpikesRight : OSpikes
    {
        public SpikesRight(Vector2 location)
            : base("ospikesRight", location)
        {
            hitbox = new Hitbox(new Rectangle(10, 0, 22, 32));
        }

        public override Entity Clone()
        {
            return new SpikesRight(location);
        }
    }
}
