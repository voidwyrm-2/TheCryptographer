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
        static bool GameIsCryptos = Plugin.GameIsCryptos;
        public static void OnEnable()
        {
            On.RoomSpecificScript.SU_A43SuperJumpOnly.Update += RemoveSurvJumpTutorial;
            On.RoomSpecificScript.SU_C04StartUp.Update += RemoveSurvFoodTutorial;
            //On.SporeCloud.Update += SporeCloud_FunnyColors;
            On.SporeCloud.ctor += SporeCloud_FunnyColors;
        }

        //gives the sporecloud FuNnY cOlOrS when it's Crypto's game
        private static void SporeCloud_FunnyColors(On.SporeCloud.orig_ctor orig, SporeCloud self, Vector2 pos, Vector2 vel, Color color, float size, AbstractCreature killTag, int checkInsectsDelay, InsectCoordinator smallInsects)
        {
            if (GameIsCryptos) color = Color.red; self.color = Color.red;
            orig(self, pos, vel, color, size, killTag, checkInsectsDelay, smallInsects);
        }

        /*private static void SporeCloud_FunnyColors(On.SporeCloud.orig_Update orig, SporeCloud self, bool eu)
        {
            if (GameIsCryptos) self.color = Color.red;
            orig(self, eu);
        }*/

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
