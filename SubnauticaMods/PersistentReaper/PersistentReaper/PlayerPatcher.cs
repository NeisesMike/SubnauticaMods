using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.IO;

namespace PersistentReaper
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Awake")]
    public class PlayerAwakePatcher
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            ReaperManager.initReaperManager();
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]
    public class PlayerUpdatePatcher
    {
        private static void leaveScent()
        {
            Int3 __instanceLoc = ReaperManager.getEcoRegion(Player.main.transform.position);
            if (ReaperManager.playerTrailDict.ContainsKey(__instanceLoc) && ReaperManager.playerTrailDict[__instanceLoc] != null)
            {
                ReaperManager.playerTrailDict[__instanceLoc].refreshScent();
            }
            else
            {
                Scent __instanceScent = new Scent();
                ReaperManager.playerTrailDict[__instanceLoc] = __instanceScent;
            }
        }


        [HarmonyPostfix]
        public static void Postfix()
        {
            ReaperManager.updateReapers();
            leaveScent();
        }
    }
}
