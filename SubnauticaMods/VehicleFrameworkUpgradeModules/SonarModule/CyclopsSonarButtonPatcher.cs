using HarmonyLib;
using UnityEngine;
using VehicleFramework;
using System.Linq;

namespace SonarModule
{
    [HarmonyPatch(typeof(CyclopsSonarButton))]
    public class CyclopsSonarButtonPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CyclopsSonarButton.TurnOnSonar))]
        public static bool CyclopsSonarButtonTurnOnSonarPrefix(CyclopsSonarButton __instance)
        {
            bool isModded = __instance.subRoot.GetCurrentUpgrades().Where(x => x.Contains(CyclopsSonarModule.SonarClassIDCore)).Count() > 0;
            if (isModded)
            {
                __instance.sonarActive = true;
                __instance.InvokeRepeating("SonarPing", 0f, MainPatcher.MyConfig.repeatRate);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
