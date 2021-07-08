using System.Collections.Generic;
using HarmonyLib;

namespace GlowFix
{
    [HarmonyPatch(typeof(uGUI_BuilderMenu))]
    [HarmonyPatch("Start")]
    class uGUI_BuilderMenuPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(uGUI_BuilderMenu __instance, List<TechType>[] ___groupsTechTypes)
        {
            GlowFixPatcher.exteriorModuleTechTypes = ___groupsTechTypes[1];
        }
    }
}
