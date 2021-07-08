using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace PersistentReaper
{
    [HarmonyPatch(typeof(EcoRegionManager))]
    [HarmonyPatch("EcoUpdate")]
    public class EcoRegionManagerPatcher
    {
        public static bool isDictionaryGrabbed = false;

        [HarmonyPostfix]
        public static void Postfix(Dictionary<Int3, EcoRegion> ___regionMap)
        {
            // grab the region dictionary and store it in ReaperManager
            if(!isDictionaryGrabbed)
            {
                ReaperManager.ecoRegionDict = ___regionMap;
                isDictionaryGrabbed = true;
            }
        }
    }
}
