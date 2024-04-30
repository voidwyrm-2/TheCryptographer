namespace NuclearPasta.TheCryptographer
{
    public class CryptoDRUGS
    {
        public static bool IsOnDrugs;

        static readonly SlugcatStats.Name CryptoScug = Plugin.CryptoScug;
        public static void OnEnable()
        {
            On.Player.SwallowObject += Player_IngestPuffDrugs;
            On.Player.SwallowObject += Player_IngestWeed;
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
                    self.mushroomEffect = 3f;
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
                    self.mushroomEffect = 2f;
                }
            }
            orig(self, grasp);
        }
    }
}
