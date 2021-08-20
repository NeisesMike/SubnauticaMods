using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace PersistentCreatures
{
    [HarmonyPatch(typeof(LargeWorldEntity))]
    [HarmonyPatch("Start")]
    public class LargeWorldEntityPatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(LargeWorldEntity __instance)
        {
            if (__instance.gameObject.GetComponent<PersistentCreatureBehavior>() == null)
            {
                return true;
            }
            return false;
        }
    }
}
