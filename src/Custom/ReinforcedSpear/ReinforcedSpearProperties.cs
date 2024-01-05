using Fisobs.Properties;

namespace NuclearPasta.TheCryptographer.Custom.ReinforcedSpear
{
    public class ReinforcedSpearProperties : ItemProperties
    {
        public override void Throwable(Player player, ref bool throwable) => throwable = false;

        // The player should only be able to grab one Crate at a time
        public override void Grabability(Player player, ref Player.ObjectGrabability grabability) => grabability = Player.ObjectGrabability.OneHand;
    }
}
