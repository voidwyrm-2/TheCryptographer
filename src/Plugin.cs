using System;
using BepInEx;
using UnityEngine;
using ImprovedInput;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace NuclearPasta.TheCryptographer
{
    //[BepInEx.BepInDependency("fisobs", BepInEx.BepInDependency.DependencyFlags.HardDependency)]
    [BepInEx.BepInDependency("improved-input-config", BepInEx.BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("nc.TheCryptographer", "The Cryptographer", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        /* from Pocky(Pocky is great <-- NO POCKY IS *NOT* GREAT, HE'S BRITISH):
        Basically
        check if grasp 0 has a rock and grasp 1 has a rock and return true if that happens
        And then hook to SpitUpCraftedObject
        and run the same grasp check again
        If it's true
        Spawn a spear and make the scug grab it
        then call return if you did this before orig so it doesn't try any more crafts
        */

        //public static readonly GameFeature<float> ScaredCentis = GameFloat("cryptocarto/scared_centis");



        public static readonly SlugcatStats.Name CryptoScug = new SlugcatStats.Name("NC.Cryptographer");
        public static readonly int CryptoMaxFood = 10;

        public static bool GameIsCryptos;
        public static bool ScugIsCrypto;

        public static bool AtePuff = false;
        public static bool AteWeed = false;


        public static PlayerKeybind CraftSpear;

        public static bool ImprovedInputEnabled;

        public static bool isInit;

        public static float CentiScaredness = 1f;

        public void BIXLog(string info) => Logger.LogDebug(info);

        public void OnEnable()
        {
            try
            {
                CraftSpear = PlayerKeybind.Register("cryptocarto:craftkey", "The Cartographer", "Craft Spear", KeyCode.C, KeyCode.JoystickButton3);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }

            CryptoGameplay.OnEnable();
            CryptoCrafting.OnEnable();
            CryptoDRUGS.OnEnable();
            
            On.Player.Update += Player_IsScugsGame; // checks if it is actually a story game session and if that story game session is Crypto's campaign
            On.Player.Update += Player_IsScug; // checks if the current player instance is Crypto

            On.Player.Update += Player_LogStuff; // logs certain values

            On.RainWorld.PostModsInit += RainWorld_PostModsInit;
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;

            IL.Menu.IntroRoll.ctor += IntroRoll_ctor; // allows for multiple(or at least two) title cards, courtesy of OlayColay(Vinki's creator), thanks OlayColay!

            On.CentipedeAI.IUseARelationshipTracker_UpdateDynamicRelationship += CentipedeAI_IUseARelationshipTracker_UpdateDynamicRelationship; // since Crypto reeks of drugs(sporepuffs among them), he repels centipedes slightly
            
            On.Player.ThrownSpear += Player_ThrownSpear; // gives spears that Crypto throws a boost

            On.Player.Update += Player_DruggieMode; // gives Crypto stat boosts when under the effect of mushrooms
        }

        private CreatureTemplate.Relationship CentipedeAI_IUseARelationshipTracker_UpdateDynamicRelationship(On.CentipedeAI.orig_IUseARelationshipTracker_UpdateDynamicRelationship orig, CentipedeAI self, RelationshipTracker.DynamicRelationship dRelation)
        {
            //if (ScugIsCrypto) dRelation.currentRelationship.type = CreatureTemplate.Relationship.Type.Uncomfortable; dRelation.currentRelationship.intensity = CentiScaredness;
            if (ScugIsCrypto) dRelation.currentRelationship.type = CreatureTemplate.Relationship.Type.Afraid; dRelation.currentRelationship.intensity = CentiScaredness;

            return orig(self, dRelation);
        }

        private static void IntroRoll_ctor(ILContext il)
        {
            var cursor = new ILCursor(il);

            if (cursor.TryGotoNext(i => i.MatchLdstr("Intro_Roll_C_"))
                && cursor.TryGotoNext(MoveType.After, i => i.MatchCallOrCallvirt<string>(nameof(string.Concat))))
            {
                cursor.Emit(OpCodes.Ldloc_3);
                cursor.EmitDelegate<Func<string, string[], string>>((titleImage, oldTitleImages) =>
                {
                    titleImage = (UnityEngine.Random.value < 1f) ? "TitleCard_CryptoCarto" : "TitleCard_CryptoCarto-S";
                    return titleImage;
                });
            }
        }

        private static readonly bool logSpearThrown = false;
        private void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            if (self.SlugCatClass == CryptoScug && self.mushroomEffect != 0f)
            {
                if (logSpearThrown)
                {
                    Logger.LogDebug("Spear was thrown by CryptoSlug with mushroom effect!");
                    Logger.LogDebug($"Before the boost its stats were: spearDamageBonus={spear.spearDamageBonus}, gravity={spear.gravity}, bodyChunks[0].vel.y={spear.bodyChunks[0].vel.y}");
                }

                if (self.mushroomEffect > 7.25f) spear.spearDamageBonus += 9f;
                else spear.spearDamageBonus += 5f;
                spear.gravity = 0.1f;
                spear.bodyChunks[0].vel.y *= 2.5f;

                if (logSpearThrown) Logger.LogDebug($"Now its stats are: spearDamageBonus={spear.spearDamageBonus}, gravity={spear.gravity}, bodyChunks[0].vel.y={spear.bodyChunks[0].vel.y}");
            }

            orig(self, spear);
        }

        #region funkymushrooms
        private static readonly bool logPlayerStats = true;
        private static bool canLogPlayerStatsOtherwise = false;
        private void Player_DruggieMode(On.Player.orig_Update orig, Player self, bool eu)
        {
            //float[] runSpeed = { 10f };
            if (self.SlugCatClass == CryptoScug)
            {
                if (self.mushroomCounter != 0)
                {
                    if (logPlayerStats) Logger.LogDebug($"CryptoSlug is under the influence of drugs, stats: " +
                        $"glowing={self.glowing}, " +
                        $"slugcatStats.throwingSkill={self.slugcatStats.throwingSkill}, " +
                        $"dynamicRunSpeed[0]=0;{self.dynamicRunSpeed[0]} 1;{self.dynamicRunSpeed[1]}, " +
                        $"playerState.permanentDamageTracking={self.playerState.permanentDamageTracking}, " +
                        $"Malnourished={self.Malnourished}"
                        ); canLogPlayerStatsOtherwise = true;
                    self.Blink(5);
                    self.glowing = true;
                    self.slugcatStats.throwingSkill = 4;
                    self.dynamicRunSpeed[0] = 3f;
                    self.dynamicRunSpeed[1] = 3.5f;
                    //self.mushroomEffect = 0.5f;
                    self.Regurgitate();
                    self.playerState.permanentDamageTracking = 0.0;
                    if (self.Malnourished)
                    {
                        self.SetMalnourished(false);
                    }
                }
                else
                {
                    self.glowing = false;
                    self.slugcatStats.throwingSkill = 1;
                    if (canLogPlayerStatsOtherwise && logPlayerStats) Logger.LogDebug($"CryptoSlug is no longer under the influence of drugs, stats: " +
                        $"glowing={self.glowing}, " +
                        $"slugcatStats.throwingSkill={self.slugcatStats.throwingSkill}, " +
                        $"dynamicRunSpeed[0]=0;{self.dynamicRunSpeed[0]} 1;{self.dynamicRunSpeed[1]}, " +
                        $"playerState.permanentDamageTracking={self.playerState.permanentDamageTracking}, " +
                        $"Malnourished={self.Malnourished}"
                        ); canLogPlayerStatsOtherwise = false;
                }
            }
            orig(self, eu);
        }
        #endregion

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            try
            {
                if (!isInit)
                {
                    isInit = true;
                    CraftSpear.Description = "The key held to craft a spear.";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            finally
            {
                orig.Invoke(self);
            }
        }

        private void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            ImprovedInputEnabled = ModManager.ActiveMods.Exists((ModManager.Mod mod) => mod.id == "improved-input-config");
            orig(self);
        }


        private static readonly bool LogMushrooms = false;
        private static readonly bool LogKeys = true;
        private static readonly bool LogDrugs = false;
        private void Player_LogStuff(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (LogMushrooms)
            { 
                Logger.LogDebug("mushroomEffect:" + self.mushroomEffect.ToString());
                Logger.LogDebug("mushroomCounter:" + self.mushroomCounter.ToString());
            }
            if (LogKeys)
            {
                if(CryptoCrafting.IsCraftSpearCustomInput(self)) Logger.LogDebug("CraftSpearkey:" + Convert.ToString(self.IsPressed(CraftSpear)));
            }
            if (LogDrugs)
            {
                Logger.LogDebug("AtePuff:" + AtePuff.ToString());
                Logger.LogDebug("AteWeed:" + AteWeed.ToString());
                //if (AtePuff) AtePuff = false;
                //if (AteWeed) AteWeed = false;
            }
            orig(self, eu);
        }

        private void Player_IsScug(On.Player.orig_Update orig, Player self, bool eu){
            if (self.SlugCatClass == CryptoScug) ScugIsCrypto = true;
            else ScugIsCrypto = false;
            orig(self, eu);
        }

        //checks if it is actually a story game session and if that story game session is Crypto's campaign
        private void Player_IsScugsGame(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self.room != null)
            {

                //if ((self.room.game.session as StoryGameSession).saveStateNumber == CryptoScug && self.room.game.session is StoryGameSession) GameIsCryptos = true;
                if (self.room.game.session is StoryGameSession) { if ((self.room.game.session as StoryGameSession).saveStateNumber == CryptoScug && self.abstractCreature.world.game.StoryCharacter is not null) GameIsCryptos = true; }
                else GameIsCryptos = false;
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