using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuclearPasta.TheCryptographer
{
    public class CryptoCrafting
    {
        static readonly SlugcatStats.Name CryptoScug = NuclearPasta.TheCryptographer.Plugin.CryptoScug;
        public static void OnEnable()
        {
            On.Player.GraspsCanBeCrafted += Player_GraspsCanBeCrafted;
            On.Player.CraftingResults += Player_CraftingResults;
            //On.Player.GraspsCanBeCrafted += Player_AllowGourmCrafting;
            On.RainWorld.PostModsInit += RainWorld_CraftingHooks;
        }

        #region PointlessStuff
        private static void RainWorld_CraftingHooks(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            On.MoreSlugcats.GourmandCombos.CraftingResults += GourmandCombos_CraftingResults;
            orig(self);
        }

        private static bool Player_GraspsCanBeCrafted(On.Player.orig_GraspsCanBeCrafted orig, Player self)
        {
            if (self.SlugCatClass == CryptoScug)
                return self.input[0].y == 1 && self.CraftingResults() != null;

            return orig(self);
        }

        private static AbstractPhysicalObject.AbstractObjectType Player_CraftingResults(On.Player.orig_CraftingResults orig, Player self)
        {
            if (self.grasps.Length < 2 || self.SlugCatClass != CryptoScug) //We need to be holding at least two things
                return orig(self);

            var craftingResult = CryptoCraft(self, self.grasps[0], self.grasps[1]);

            return craftingResult?.type;
        }

        private static AbstractPhysicalObject GourmandCombos_CraftingResults(On.MoreSlugcats.GourmandCombos.orig_CraftingResults orig, PhysicalObject crafter, Creature.Grasp graspA, Creature.Grasp graspB)
        {
            if ((crafter as Player).SlugCatClass == CryptoScug)
                return CryptoCraft(crafter as Player, graspA, graspB);

            return orig(crafter, graspA, graspB);
        }
        #endregion

        public static AbstractPhysicalObject CryptoCraft(Player player, Creature.Grasp graspA, Creature.Grasp graspB)
        {
            //ExplosiveSpear.TryGet(player, out bool shouldBeExplosive);
            if (player == null || graspA?.grabbed == null || graspB?.grabbed == null) return null;

            //Check grasps here
            if (player.SlugCatClass == CryptoScug)
            {
                AbstractPhysicalObject.AbstractObjectType grabbedObjectTypeA = graspA.grabbed.abstractPhysicalObject.type;
                AbstractPhysicalObject.AbstractObjectType grabbedObjectTypeB = graspB.grabbed.abstractPhysicalObject.type;
                #region Crafting
                #region Spears
                #region Normal
                if (grabbedObjectTypeA == AbstractPhysicalObject.AbstractObjectType.Rock && grabbedObjectTypeB == AbstractPhysicalObject.AbstractObjectType.Rock) return new AbstractSpear(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID(), false, false);
                #endregion

                #region Explosive
                else if ((grabbedObjectTypeA == AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant && grabbedObjectTypeB == AbstractPhysicalObject.AbstractObjectType.Spear) || (grabbedObjectTypeA == AbstractPhysicalObject.AbstractObjectType.Spear && grabbedObjectTypeB == AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant)) return new AbstractSpear(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID(), true, false);
                #endregion

                #region Electric
                else if ((grabbedObjectTypeA == AbstractPhysicalObject.AbstractObjectType.Rock && grabbedObjectTypeB == AbstractPhysicalObject.AbstractObjectType.Spear) || (grabbedObjectTypeA == AbstractPhysicalObject.AbstractObjectType.Spear && grabbedObjectTypeB == AbstractPhysicalObject.AbstractObjectType.Rock)) { var electricSpear = new AbstractSpear(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID(), false, true); electricSpear.electricCharge = 0; return electricSpear; }
                #endregion

                /*#region Fire
                else if (grabbedObjectTypeA == AbstractPhysicalObject.AbstractObjectType.Rock && grabbedObjectTypeB == AbstractPhysicalObject.AbstractObjectType.Rock) return new AbstractSpear(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID(), false, false);
                #endregion*/
                #endregion
                #endregion
            }

            return null;
        }
    }
}
