using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace DebugScene
{
    [HarmonyPatch(typeof(Pickupable))]
    public static class PickupablePatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch("Activate")]
        public static bool ActivatePrefix(Pickupable __instance)
		{
			__instance.gameObject.SetActive(true);
			__instance.isPickupable = true;
			__instance.isValidHandTarget = true;
			PlayerTool component = __instance.GetComponent<PlayerTool>();
			if (component != null && component.mainCollider != null)
			{
				component.mainCollider.isTrigger = false;
			}
			return false;
		}
    }
}
