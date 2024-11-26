using HarmonyLib;
using UnityEngine;
using VehicleFramework;
using System.Linq;

namespace SonarModule
{
    [HarmonyPatch(typeof(SonarScreenFX))]
    public class SonarScreenFXPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SonarScreenFX.Update))]
        public static bool SonarScreenFXUpdatePrefix(SonarScreenFX __instance)
        {
            bool cyclopsReady = Player.main.IsInCyclops() && Player.main.currentSub.GetCurrentUpgrades().Where(x => x.Contains(CyclopsSonarModule.SonarClassIDCore)).Count() > 0;
            bool vehicleReady = Player.main.GetVehicle() != null && Player.main.GetVehicle().GetCurrentUpgrades().Where(x => x.Contains(SonarModule.SonarClassIDCore)).Count() > 0;
            if(cyclopsReady || vehicleReady)
            {
                __instance.pingDistance += Time.deltaTime / MainPatcher.MyConfig.duration;
                if (__instance.pingDistance > 1f)
                {
                    __instance.enabled = false;
                }
                Shader.SetGlobalFloat(__instance.pingDistanceShaderID, __instance.pingDistance);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
