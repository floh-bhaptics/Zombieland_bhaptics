using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;

[assembly: MelonInfo(typeof(Zombieland_bhaptics.Zombieland_bhaptics), "Zombieland_bhaptics", "1.1", "Florian Fahrenberger")]
[assembly: MelonGame("XR Games", "zombieland_vr_headshot_fever")]


namespace Zombieland_bhaptics
{
    public class Zombieland_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }


        [HarmonyPatch(typeof(Zombieland.Gameplay.Player.AbstractPlayerGunBehaviour), "HandleAnyGunFired", new Type[] { typeof(Zombieland.Gameplay.Weapons.Ammunition.Ammo) })]
        public class bhaptics_FireGun
        {
            [HarmonyPostfix]
            public static void Postfix(Zombieland.Gameplay.Player.AbstractPlayerGunBehaviour __instance, Zombieland.Gameplay.Weapons.Ammunition.Ammo ammo)
            {
                bool isPrimaryGun = false;
                bool primaryIsRight = __instance.PrimaryHand.IsRightHand;
                string gunType = "Pistol";
                if (ammo.FiringGun == __instance.PrimaryGun)
                {
                    isPrimaryGun = true;
                    if (__instance.PrimaryGun.GunId == Zombieland.Config.GunIDs.HUNTER_Z) { gunType = "Shotgun"; }
                }
                else
                {
                    if (__instance.SecondaryGun.GunId == Zombieland.Config.GunIDs.DOUBLE_MAD) { gunType = "Shotgun"; }
                    if (__instance.SecondaryGun.GunId == Zombieland.Config.GunIDs.VANILLI_M4) { gunType = "Shotgun"; }
                }
                bool isRightHand = !(isPrimaryGun ^ primaryIsRight);
                tactsuitVr.Recoil(gunType, isRightHand);
            }
        }

        [HarmonyPatch(typeof(Zombieland.Gameplay.Services.EndMissionService), "EndMission", new Type[] { typeof(Zombieland.Gameplay.Services.EndMissionReason) })]
        public class bhaptics_EndMission
        {
            [HarmonyPostfix]
            public static void Postfix(Zombieland.Gameplay.Services.EndMissionReason reason)
            {
                if (reason == Zombieland.Gameplay.Services.EndMissionReason.Died) { tactsuitVr.PlaybackHaptics("KilledByZombie"); }
                if (reason == Zombieland.Gameplay.Services.EndMissionReason.Completed) { tactsuitVr.PlaybackHaptics("FinishedLevel"); }
            }
        }

        [HarmonyPatch(typeof(Zombieland.Gameplay.Weapons.Explosives.ExplosionBehaviour), "Explode", new Type[] {  })]
        public class bhaptics_ExplosiveExplode
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("ExplosionBelly");
            }
        }

        [HarmonyPatch(typeof(Zombieland.Gameplay.Weapons.Explosives.GrenadeBehaviour), "Explode", new Type[] { })]
        public class bhaptics_GrenadeExplode
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("ExplosionBelly");
            }
        }

    }
}
