using Fisobs.Core;
using UnityEngine;

namespace NuclearPasta.TheCryptographer.Custom.ReinforcedSpear
{
    public class ReinforcedSpearIcon : Icon
    {
        // In vanilla, you only have one int value to store custom data.
        // In this example, that is the hue of the object, which is scaled by 1000
        // For example, 0 is red, 70 is orange
        public override int Data(AbstractPhysicalObject apo)
        {
            return apo is ReinforcedSpearAbstract spear ? (int)(spear.hue * 1000f) : 0;
        }

        public override Color SpriteColor(int data)
        {
            return RWCustom.Custom.HSL2RGB(data / 1000f, 0.65f, 0.4f);
        }

        public override string SpriteName(int data)
        {
            return "assets/icon_ReinforcedSpear";
        }
    }
}