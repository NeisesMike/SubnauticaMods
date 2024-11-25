using HarmonyLib;
using UnityEngine;
using VehicleFramework;

namespace SonarModule
{
    [HarmonyPatch(typeof(SonarScreenFX))]
    public class SonarScreenFXPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SonarScreenFX.Update))]
        public static bool SonarScreenFXUpdatePrefix(SonarScreenFX __instance)
        {
            if (
                Player.main.IsInCyclops()
                || Player.main.GetVehicle() == null
                || !Player.main.GetVehicle().GetCurrentUpgrades().Contains("SonarModule(Clone)")
              )
            {
                return true;
            }
            __instance.pingDistance += Time.deltaTime / MainPatcher.MyConfig.duration;
            if (__instance.pingDistance > 1f)
            {
                __instance.enabled = false;
            }
            Shader.SetGlobalFloat(__instance.pingDistanceShaderID, __instance.pingDistance);
            return false;
        }
    }
}
