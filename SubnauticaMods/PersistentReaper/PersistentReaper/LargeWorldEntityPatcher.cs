using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace PersistentReaper
{
    [HarmonyPatch(typeof(LargeWorldEntity))]
    [HarmonyPatch("Start")]
    public class LargeWorldEntityPatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(LargeWorldEntity __instance)
        {
            // don't register Percy
            if (!ReaperManager.reaperDict.ContainsKey(__instance.gameObject))
            {
                return false;
            }
            return true;
        }
    }
}
