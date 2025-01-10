using HarmonyLib;
using UnityEngine;

namespace RollControl
{
	[HarmonyPatch(typeof(ArmsController))]
	class ArmsControllerPatcher
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(ArmsController.SetPlayerSpeedParameters))]
		public static void SetPlayerSpeedParametersPostfix(Animator ___animator)
		{
			// this odd line fixes the body getting in the way sometimes when
			// swimming down while roll is enabled
			if (ScubaRollController.IsActuallyScubaRolling)
			{
				SafeAnimator.SetFloat(___animator, "view_pitch", 0);
			}
		}
	}
}
