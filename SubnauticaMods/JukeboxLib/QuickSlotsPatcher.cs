using HarmonyLib;
using UnityEngine;

namespace JukeboxLib
{
    // This patch allows us to use the scroll wheel (or next slot, prev slot)
    // to control volume without actually changing our equipped item
    [HarmonyPatch(typeof(uGUI_QuickSlots))]
    public class uGUI_QuickSlotsPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(uGUI_QuickSlots.HandleInput))]
        public static bool HandleInputPrefix()
        {
            Targeting.GetTarget(Player.main.gameObject, 6f, out GameObject target, out float _);
            if (target?.GetComponentInParent<DesktopJukebox>() != null)
            {
                return false;
            }
            return true;
        }
    }
}
