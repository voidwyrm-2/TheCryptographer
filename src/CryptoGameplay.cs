using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NuclearPasta.TheCryptographer
{
    public class CryptoGameplay
    {
        static bool CryptosGame = NuclearPasta.TheCryptographer.Plugin.CryptosGame;
        public static void OnEnable()
        {
            On.RoomSpecificScript.SU_A43SuperJumpOnly.Update += RemoveSurvJumpTutorial;
            On.RoomSpecificScript.SU_C04StartUp.Update += RemoveSurvFoodTutorial;
            On.SporeCloud.Update += SporeCloud_FunnyColors;
        }


        //gives the sporecloud FuNnY cOlOrS when it's Crypto's game
        private static void SporeCloud_FunnyColors(On.SporeCloud.orig_Update orig, SporeCloud self, bool eu)
        {
            if (CryptosGame) self.color = Color.red;
            orig(self, eu);
        }

        #region RemoveTutorials
        public static void RemoveSurvFoodTutorial(On.RoomSpecificScript.SU_C04StartUp.orig_Update orig, RoomSpecificScript.SU_C04StartUp self, bool eu)
        {
            if (CryptosGame) self.Destroy();
            orig(self, eu);
        }

        public static void RemoveSurvJumpTutorial(On.RoomSpecificScript.SU_A43SuperJumpOnly.orig_Update orig, RoomSpecificScript.SU_A43SuperJumpOnly self, bool eu)
        {
            if (CryptosGame) self.Destroy();
            orig(self, eu);
        }
        #endregion
    }
}
