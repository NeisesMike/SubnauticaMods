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
    [HarmonyPatch("Update")]
    public class PlayerUpdatePatcher
    {
        private static void leaveScent()
        {
            Int3 thisLoc = ReaperManager.getEcoRegion(Player.main.transform.position);
            // if this isn't a valid location for reapers, skip
            if(!ReaperManager.checkRegionLegality(thisLoc))
            {
                return;
            }
            if (ReaperManager.playerTrailDict.ContainsKey(thisLoc) && ReaperManager.playerTrailDict[thisLoc] != null)
            {
                ReaperManager.playerTrailDict[thisLoc].refreshScent();
            }
            else
            {
                Scent thisScent = new Scent();
                ReaperManager.playerTrailDict[thisLoc] = thisScent;
            }
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (PersistentReaperPatcher.Config.areReapersActive)
            {
                ReaperManager.updateReapers();
                leaveScent();
            }
        }
    }
}
