using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace RollControl
{
    [HarmonyPatch(typeof(SeaMoth))]
    public class SeamothPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void StartPostfix(SeaMoth __instance)
        {
            var src = __instance.gameObject.EnsureComponent<SeamothRollController>();
            src.mySeamoth = __instance;
        }
    }
}
