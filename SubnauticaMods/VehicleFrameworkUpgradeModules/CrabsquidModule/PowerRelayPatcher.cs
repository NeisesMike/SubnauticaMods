using System.Collections;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using VehicleFramework;

namespace CrabsquidModule
{
    [HarmonyPatch(typeof(PowerRelay))]
    public static class PowerRelayPatcher
    {
        public static float energyCost = 5f;
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PowerRelay.DisableElectronicsForTime))]
        public static bool DisableElectronicsForTimePrefix(PowerRelay __instance)
        {
            SubRoot subroot = __instance.gameObject.GetComponent<SubRoot>();
            if(Player.main.IsInCyclops() && Player.main.currentSub == subroot && subroot.GetCurrentUpgrades().Where(x => x.Contains("CrabsquidModule")).Count() > 0)
            {
                UWE.CoroutineHost.StartCoroutine(MaybeConsumeEnergy(__instance));
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool mutex = false;
        public static IEnumerator MaybeConsumeEnergy(PowerRelay eint)
        {
            if(mutex)
            {
                yield break;
            }
            mutex = true;
            eint.ConsumeEnergy(energyCost, out _);
            yield return new WaitForSeconds(1.0f);
            mutex = false;
        }
    }
}
