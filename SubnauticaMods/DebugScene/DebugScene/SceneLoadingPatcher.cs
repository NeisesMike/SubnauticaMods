using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace DebugScene
{
    [HarmonyPatch(typeof(uGUI_SceneLoading))]
    public class SceneLoadingPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("Begin")]
        public static void BeginInspector(uGUI_SceneLoading __instance, bool withoutFadeIn)
        {
            if (MainMenuPatcher.IsDebugScene)
            {
                __instance.End(false);
            }
        }
    }
}
