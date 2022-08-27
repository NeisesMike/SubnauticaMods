using HarmonyLib;
using System.Collections.Generic;

namespace StealthModule
{
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("Awake")]
    class SMAwakePatcher
    {
        [HarmonyPostfix]
        public static void Postfix(SeaMoth __instance)
        {
            __instance.gameObject.EnsureComponent<StealthModule>();
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    class OnUpgradeModuleChangeSMPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(SeaMoth __instance)
        {
            __instance.gameObject.EnsureComponent<StealthModule>();

            // Dictionary of TechTypes and their stealth additions.
            Dictionary<TechType, StealthQuality> dictionary = new Dictionary<TechType, StealthQuality>
            {
                {
                    StealthModulePatcher.seamothStealthModule1.TechType,
                    StealthQuality.Low
                },
                {
                    StealthModulePatcher.seamothStealthModule2.TechType,
                    StealthQuality.Medium
                },
                {
                    StealthModulePatcher.seamothStealthModule3.TechType,
                    StealthQuality.High
                }
            };

            // Stealth upgrade to add.
            StealthQuality stealthUpgrade = StealthQuality.None;

            // Loop through available stealth module upgrades
            foreach (KeyValuePair<TechType, StealthQuality> entry in dictionary)
            {
                TechType stealthTechType = entry.Key;
                StealthQuality stealthValue = entry.Value;

                int count = __instance.modules.GetCount(stealthTechType);

                // If you have at least 1 such depth module
                if (count > 0)
                {
                    if (stealthValue > stealthUpgrade)
                    {
                        stealthUpgrade = stealthValue;
                    }
                }
            }

            // Configure the component.
            __instance.gameObject.GetComponent<StealthModule>().quality = stealthUpgrade;
        }
    }
}
