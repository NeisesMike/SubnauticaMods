using HarmonyLib;
using UnityEngine;

namespace JukeboxLib
{
    [HarmonyPatch(typeof(uGUI_QuickSlots))]
    public class uGUI_QuickSlotsPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(uGUI_QuickSlots.HandleInput))]
        public static bool HandleInputPrefix()
        {
            Targeting.GetTarget(Player.main.gameObject, 6f, out GameObject target, out float _);
            if (target?.GetComponentInParent<Jukebox>() != null)
            {
                return false;
            }
            return true;
        }
    }
}
