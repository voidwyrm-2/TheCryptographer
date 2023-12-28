using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCryptographer
{
    public class CryptoDRUGS
    {
        static readonly SlugcatStats.Name CryptoScug = NuclearPasta.TheCryptographer.Plugin.CryptoScug;
        public static void OnEnable()
        {
            On.Player.Update += Player_DruggieMode_Mushrooms; //gives Crypto stat boosts when under the effect of mushrooms
            On.Player.SwallowObject += Player_IngestPuffDrugs;
        }

        #region funkymushrooms
        static bool resetDrugs = false;
        static bool giveFood = false;
        private static void Player_DruggieMode_Mushrooms(On.Player.orig_Update orig, Player self, bool eu)
        {
            //Logger.LogInfo("mushroomEffect:" + self.mushroomEffect.ToString());
            //Logger.LogInfo("mushroomCounter:" + self.mushroomCounter.ToString());
            //float[] runSpeed = { 10f };
            if (self.SlugCatClass == CryptoScug)
            {
                //if (self.mushroomEffect != 0f && self.mushroomCounter != 0)
                if (self.mushroomCounter != 0)
                {
                    self.Blink(5);
                    //self.airInLungs = 3.8f; // DO NOT UNCOMMENT, CAUSES THE GAME TO BORK
                    self.glowing = true;
                    //self.gravity = 0.3f;
                    self.slugcatStats.throwingSkill = 4;
                    //self.dynamicRunSpeed = runSpeed;
                    self.slugcatStats.runspeedFac = 3.3f;
                    self.mushroomEffect = 0.3f;
                    resetDrugs = true;
                    self.Regurgitate();
                    self.playerState.permanentDamageTracking = 0.0;
                    if (self.Malnourished)
                    {
                        self.SetMalnourished(false);
                    }
                }
                else
                {
                    if (resetDrugs)
                    {
                        self.mushroomEffect = 1f; //doing this causes a interesting flash effect when the mushroom effect runs out
                        resetDrugs = false;
                    }
                    self.glowing = false;
                    //self.gravity = 1f;
                    self.slugcatStats.throwingSkill = 1;
                    self.slugcatStats.runspeedFac = 1f;//3.6f; // runspeedFac is a MULTIPLIER; IT IS NOT THE RUNSPEED ITSELF; SET runspeedFac BACK TO 1 ALWAYS
                }
            }
            orig(self, eu);
        }
        #endregion

        #region pesticide
        private static void Player_IngestPuffDrugs(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            if (self.SlugCatClass == CryptoScug)
            {
                if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.PuffBall)
                {
                    self.objectInStomach.type = AbstractPhysicalObject.AbstractObjectType.Rock;
                    self.AddFood(1);
                    self.AddQuarterFood();
                    self.AddQuarterFood();
                }
            }
            orig(self, grasp);
        }
        #endregion
    }
}
