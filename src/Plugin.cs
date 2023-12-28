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
            CryptoCrafting.OnEnable();
            On.Player.Update += Player_DruggieMode_Mushrooms;
            On.Player.Update += Player_IsScugsGame;
            On.SporeCloud.Update += SporeCloud_FunnyColors;
            On.Player.SwallowObject += Player_SwallowObject;
        }

        private void SporeCloud_FunnyColors(On.SporeCloud.orig_Update orig, SporeCloud self, bool eu)
        {
            //self.color 
            orig(self, eu);
        }

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

        //drug-like things in RW: mushrooms, sporepuffs
        #region drugs
        #region funkymushrooms
        bool resetDrugs = false;
        private void Player_DruggieMode_Mushrooms(On.Player.orig_Update orig, Player self, bool eu)
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
                    if(resetDrugs)
                    {
                        self.mushroomEffect = 1f;
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
        #region ingestion
        private void Player_SwallowObject(On.Player.orig_SwallowObject orig, Player self, int grasp)
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
        #endregion
        #endregion



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

        #region CryptoCrafting
        #endregion



        // Implement MeanLizards
        /*private void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            if(MeanLizards.TryGet(world.game, out float meanness) && MeanLizards.TryGet(world.game, out float meanness))
            {
                self.spawnDataEvil = Mathf.Min(self.spawnDataEvil, meanness);
            }
        }


        // Implement SuperJump
        private void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);

            if (SuperJump.TryGet(self, out var power))
            {
                self.jumpBoost *= 1f + power;
            }
        }

        // Implement ExlodeOnDeath
        private void Player_Die(On.Player.orig_Die orig, Player self)
        {
            bool wasDead = self.dead;

            orig(self);

            if(!wasDead && self.dead && ExplodeOnDeath.TryGet(self, out bool explode) && explode)
            {
                // Adapted from ScavengerBomb.Explode
                var room = self.room;
                var pos = self.mainBodyChunk.pos;
                var color = self.ShortCutColor();
                room.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, self, 0.7f, 160f, 1f));
                room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
                room.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));

                room.ScreenMovement(pos, default, 1.3f);
                room.PlaySound(SoundID.Bomb_Explode, pos);
                room.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));
            }
        }*/
    }
}