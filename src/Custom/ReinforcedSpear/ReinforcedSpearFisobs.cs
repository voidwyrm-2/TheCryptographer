using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;
using System;
using UnityEngine;

namespace NuclearPasta.TheCryptographer.Custom.ReinforcedSpear
{
    public class ReinforcedSpearFisobs : Fisob
    {
        public static readonly AbstractPhysicalObject.AbstractObjectType AbstractRSpear = new("NC.ReinforcedSpear", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID SURSpear = new("NC.ReinforcedSpear", true);

        public ReinforcedSpearFisobs() : base(AbstractRSpear)
        {
            /*try
            { 
                Icon = new ReinforcedSpearIcon();
            }
            catch (Exception e)
            {
                
            }*/
            //Icon = new ReinforcedSpearIcon();

            SandboxPerformanceCost = new(linear: 0.2f, 0f);

            RegisterUnlock(SURSpear, parent: MultiplayerUnlocks.SandboxUnlockID.Slugcat, data: 0);
        }

        public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
        {
            // Crate data is just floats separated by ; characters.
            string[] p = saveData.CustomData.Split(';');

            if (p.Length < 5)
            {
                p = new string[5];
            }

            var result = new ReinforcedSpearAbstract(world, saveData.Pos, saveData.ID)
            {
                hue = float.TryParse(p[0], out var h) ? h : 0,
                saturation = float.TryParse(p[1], out var s) ? s : 1,
                scaleX = float.TryParse(p[2], out var x) ? x : 1,
                scaleY = float.TryParse(p[3], out var y) ? y : 1,
            };

            // If this is coming from a sandbox unlock, the hue and size should depend on the data value (see CrateIcon below).
            if (unlock is SandboxUnlock u)
            {
                result.hue = u.Data / 1000f;

                if (u.Data == 0)
                {
                    result.scaleX += 0.2f;
                    result.scaleY += 0.2f;
                }
            }

            return result;
        }

        private static readonly ReinforcedSpearProperties properties = new();

        public override ItemProperties Properties(PhysicalObject forObject)
        {
            // If you need to use the forObject parameter, pass it to your ItemProperties class's constructor.
            // The Mosquitoes example from the Fisobs github demonstrates this.
            return properties;
        }
    }
}
