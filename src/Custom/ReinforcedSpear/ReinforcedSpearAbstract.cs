using Fisobs.Core;

namespace NuclearPasta.TheCryptographer.Custom.ReinforcedSpear
{
    public class ReinforcedSpearAbstract : AbstractPhysicalObject
    {
        public ReinforcedSpearAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, ReinforcedSpearFisobs.AbstractRSpear, null, pos, ID)
        {
            scaleX = 1;
            scaleY = 1;
            saturation = 0.5f;
            hue = 1f;
        }

        public override void Realize()
        {
            base.Realize();
            if (realizedObject == null)
                realizedObject = new ReinforcedSpear(this);
        }

        public float hue;
        public float saturation;
        public float scaleX;
        public float scaleY;

        public override string ToString()
        {
            return this.SaveToString($"{hue};{saturation};{scaleX};{scaleY}");
        }
    }
}
