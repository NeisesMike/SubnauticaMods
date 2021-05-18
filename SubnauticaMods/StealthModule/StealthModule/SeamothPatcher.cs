using HarmonyLib;
using System.Collections.Generic;

namespace StealthModule
{
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnPilotModeBegin")]
    class SeaMothOnPilotModeBeginPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(SeaMoth __instance)
        {
            // Dictionary of TechTypes and their stealth additions.
            Dictionary<TechType, StealthQuality> dictionary = new Dictionary<TechType, StealthQuality>
            {
                {
                    StealthModulePatcher.stealthModule1.TechType,
                    StealthQuality.Low
                },
                {
                    StealthModulePatcher.stealthModule2.TechType,
                    StealthQuality.Medium
                },
                {
                    StealthModulePatcher.stealthModule3.TechType,
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

            // Configure the upgrade.
            StealthModulePatcher.stealthQuality = stealthUpgrade;
        }
    }


    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnPilotModeEnd")]
    class SeaMothOnPilotModeEndPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(SeaMoth __instance)
        {
            // Remove stealth.
            StealthModulePatcher.stealthQuality = StealthQuality.None;
        }
    }


}
