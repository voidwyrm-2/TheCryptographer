namespace NuclearPasta.TheCryptographer
{
    public class CryptoGameplay
    {
        static bool GameIsCryptos = Plugin.GameIsCryptos;
        public static void OnEnable()
        {
            On.RoomSpecificScript.SU_A43SuperJumpOnly.Update += RemoveSurvJumpTutorial;
            On.RoomSpecificScript.SU_C04StartUp.Update += RemoveSurvFoodTutorial;
        }

        #region RemoveTutorials
        public static void RemoveSurvFoodTutorial(On.RoomSpecificScript.SU_C04StartUp.orig_Update orig, RoomSpecificScript.SU_C04StartUp self, bool eu)
        {
            if (GameIsCryptos) self.Destroy();
            orig(self, eu);
        }

        public static void RemoveSurvJumpTutorial(On.RoomSpecificScript.SU_A43SuperJumpOnly.orig_Update orig, RoomSpecificScript.SU_A43SuperJumpOnly self, bool eu)
        {
            if (GameIsCryptos) self.Destroy();
            orig(self, eu);
        }
        #endregion
    }
}
