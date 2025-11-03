using HarmonyLib;
using UnityEngine;
using VehicleFramework.Extensions;
using System.Linq;

namespace SonarModule
{
    [HarmonyPatch(typeof(CyclopsHelmHUDManager))]
    public class CyclopsHelmHUDManagerPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(CyclopsHelmHUDManager.UpdateAbilities))]
        public static void CyclopsHelmHUDManagerUpdateAbilitiesPostfix(CyclopsHelmHUDManager __instance)
        {
            bool hasModded = __instance.subRoot.GetCurrentUpgrades().Where(x => x.Contains(CyclopsSonarModule.SonarClassIDCore)).Count() > 0;
            if (hasModded)
            {
                __instance.sonarUpgrade.SetActive(true);
            }
        }
    }
}
