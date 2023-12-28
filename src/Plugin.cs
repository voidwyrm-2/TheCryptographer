using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using On;
using static MonoMod.InlineRT.MonoModRule;
using IL.MoreSlugcats;
using IL;
using System.Diagnostics.Eventing.Reader;

namespace NuclearPasta.TheCryptographer
{
    //[BepInEx.BepInDependency("rwmodding.coreorg.pom", BepInEx.BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("nc.TheCryptographer", "The Cryptographer", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        /* from Pocky(Pocky is great):
        Basically
        check if grasp 0 has a rock and grasp 1 has a rock and return true if that happens
        And then hook to SpitUpCraftedObject
        and run the same grasp check again
        If it's true
        Spawn a spear and make the scug grab it
        then call return if you did this before orig so it doesn't try any more crafts
        */

        //public static readonly PlayerFeature<float> SuperJump = PlayerFloat("slugcoin/super_jump");
        //public static readonly PlayerFeature<bool> ExplodeOnDeath = PlayerBool("slugcoin/explode_on_death");
        //public static readonly GameFeature<float> MeanLizards = GameFloat("slugcoin/mean_lizards");
        //public static readonly PlayerFeature<bool> ExplosiveSpear = PlayerBool("slugcoin/explosive_spear");



        public static readonly SlugcatStats.Name CryptoScug = new SlugcatStats.Name("NC.Cryptographer");
        public static readonly int CryptoMaxFood = 10;

        public static bool CryptosGame;

        public void OnEnable()
        {

            //On.Player.Jump += Player_Jump;
            //On.Player.Die += Player_Die;
            //On.Lizard.ctor += Lizard_ctor;
            CryptoGameplay.OnEnable();
            CryptoCrafting.OnEnable();
            
            On.Player.Update += Player_IsScugsGame;
        }

        //checks if it is actually a story game session and if that story game session is Crypto's campaign
        private void Player_IsScugsGame(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self.room != null)
            {

                //if ((self.room.game.session as StoryGameSession).saveStateNumber == CryptoScug && self.room.game.session is StoryGameSession) CryptosGame = true;
                if (self.room.game.session is StoryGameSession) { if ((self.room.game.session as StoryGameSession).saveStateNumber == CryptoScug && self.abstractCreature.world.game.StoryCharacter is not null) CryptosGame = true; }
                else CryptosGame = false;
            }
            orig(self, eu);
        }

        /*#region AllowGourmCrafting
        public static bool Player_AllowGourmCrafting(On.Player.orig_GraspsCanBeCrafted orig, Player self)
        {
            
            bool result;
            if (self.SlugCatClass == CryptoScug && self.input[0].y == 1)
            {
                if (self.CraftingResults() != null)
                {
                    bool flag3 = self.FoodInStomach >= self.MaxFoodInStomach;
                    result = !flag3;
                }
                else
                {
                    result = (self.CraftingResults() != null);
                }
            }
            else
            {
                result = orig.Invoke(self);
            }
            return result;
        }
        #endregion*/
    }
}