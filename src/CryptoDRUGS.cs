namespace NuclearPasta.TheCryptographer
{
    public class CryptoDRUGS
    {
        public static bool IsOnDrugs;

        static readonly SlugcatStats.Name CryptoScug = Plugin.CryptoScug;
        public static void OnEnable()
        {
            //On.Player.Update += Player_DruggieMode; //gives Crypto stat boosts when under the effect of mushrooms
            On.Player.SwallowObject += Player_IngestPuffDrugs;
            On.Player.SwallowObject += Player_IngestWeed;
            On.Player.ThrownSpear += Player_ThrownSpear;
            On.Player.Update += Player_IsOnDrugs;
            On.Player.Stun += Player_Stun;
        }

        private static void Player_IsOnDrugs(On.Player.orig_Update orig, Player self, bool eu) {
            IsOnDrugs = self.mushroomEffect != 0f;
            orig(self, eu);
        }

        private static void Player_Stun(On.Player.orig_Stun orig, Player self, int st) {
            if (self.SlugCatClass != CryptoScug && self.mushroomEffect != 0f) {
                orig.Invoke(self, st);
            } else {
                self.stun = 0;
                orig.Invoke(self, st);
            }
        }

        private static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            if (self.SlugCatClass == CryptoScug && self.mushroomEffect != 0f)
            {
                spear.spearDamageBonus += 4f;
                spear.gravity = 0.1f;
                spear.bodyChunks[0].vel.y += spear.bodyChunks[0].vel.y + spear.bodyChunks[0].vel.y;
            }
            
            orig(self, spear);
        }

        #region funkymushrooms
        static bool resetDrugs = false;
        //static bool giveFood = false;
        private static void Player_DruggieMode(On.Player.orig_Update orig, Player self, bool eu)
        {
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
                    self.dynamicRunSpeed[0] = 3f;
                    self.dynamicRunSpeed[1] = 3.5f;
                    //self.slugcatStats.runspeedFac = 3.3f;
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
                    //self.slugcatStats.runspeedFac = 1f; //3.6f; // runspeedFac is a MULTIPLIER; IT IS NOT THE RUNSPEED ITSELF; SET runspeedFac BACK TO 1 ALWAYS
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
                if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.PuffBall && self.FoodInStomach != self.MaxFoodInStomach)
                {
                    //self.objectInStomach.type = AbstractPhysicalObject.AbstractObjectType.Rock;
                    self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.Rock, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                    self.AddFood(1);
                    self.mushroomCounter += 400;
                }
            }
            orig(self, grasp);
        }
        #endregion

        private static void Player_IngestWeed(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            if (self.SlugCatClass == CryptoScug)
            {
                if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.FlyLure && self.FoodInStomach != self.MaxFoodInStomach)
                {
                    //self.objectInStomach.Destroy();
                    self.objectInStomach = new AbstractConsumable(self.room.world, null, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                    self.AddQuarterFood();
                    self.mushroomCounter += 230;
                }
            }
            orig(self, grasp);
        }
    }
}
