using System.Collections;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using VehicleFramework.Extensions;

namespace CrabsquidModule
{
    [HarmonyPatch(typeof(EnergyInterface))]
    public static class EnergyInterfacePatcher
    {
        public static float energyCost = 5f;
        [HarmonyPrefix]
        [HarmonyPatch(nameof(EnergyInterface.DisableElectronicsForTime))]
        public static bool DisableElectronicsForTimePrefix(EnergyInterface __instance)
        {
            Vehicle mv = __instance.gameObject.GetComponent<Vehicle>();
            if (mv == null || Player.main.currentMountedVehicle != mv || mv.GetCurrentUpgrades().Where(x => x.Contains("CrabsquidModule")).Count() == 0)
            {
                return true;
            }
            UWE.CoroutineHost.StartCoroutine(MaybeConsumeEnergy(__instance));
            return false;
        }

        private static bool mutex = false;
        public static IEnumerator MaybeConsumeEnergy(EnergyInterface eint)
        {
            if(mutex)
            {
                yield break;
            }
            mutex = true;
            eint.ConsumeEnergy(energyCost);
            yield return new WaitForSeconds(1.0f);
            mutex = false;
        }
    }
}
