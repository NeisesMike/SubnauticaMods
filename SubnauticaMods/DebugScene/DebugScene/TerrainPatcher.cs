using HarmonyLib;
using UWE;

namespace DebugScene
{
    [HarmonyPatch(typeof(PAXTerrainController))]
    public class TerrainPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch("Initialize")]
        public static bool InitSkipper(PAXTerrainController __instance, WorldStreaming.WorldStreamer ___streamerV2)
        {
            if(MainMenuPatcher.IsDebugScene)
            {
                //__instance.isWorking = false;
                bool temp = __instance.isWorking;
                ___streamerV2.isLoading = false;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("isWorking", MethodType.Getter)]
        public static bool GetterOverride()
        {
            FreezeTime.End("WaitScreen");
            FreezeTime.End("IngameMenu");
            FreezeTime.End("FeedbackPanel");
            FreezeTime.End("HardcoreGameOver");
            return true;
        }

        /*
        [HarmonyPrefix]
        [HarmonyPatch("isWorking", MethodType.Getter)]
        public static void GetterOverridePrefix(PAXTerrainController __instance, ref bool __result)
        {
            Logger.Log("bing: " + __instance.isWorking.ToString());
            __result = !__result;
            Logger.Log("bong: " + __instance.isWorking.ToString());
            Logger.Log("bang: " + __result.ToString());
        }

        [HarmonyPostfix]
        [HarmonyPatch("isWorking", MethodType.Getter)]
        public static void GetterOverride(PAXTerrainController __instance, ref bool __result)
        {
            Logger.Log("bing: " + __result.ToString());
        }
        */
    }
}
