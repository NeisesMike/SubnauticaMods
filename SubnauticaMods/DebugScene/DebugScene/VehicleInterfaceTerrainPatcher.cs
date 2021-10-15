using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace DebugScene
{
    [HarmonyPatch(typeof(VehicleInterface_Terrain))]
    public class VehicleInterfaceTerrainPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static bool Update(VehicleInterface_Terrain __instance)
        {
            return !MainMenuPatcher.IsDebugScene;
        }
        [HarmonyPrefix]
        [HarmonyPatch("OnEnable")]
        public static bool OnEnable(VehicleInterface_Terrain __instance)
        {
            return !MainMenuPatcher.IsDebugScene;
        }
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        public static bool Start(VehicleInterface_Terrain __instance)
        {
            return !MainMenuPatcher.IsDebugScene;
        }
    }



    
}
