using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using RootMotion.FinalIK;

namespace RollControl
{
	[HarmonyPatch(typeof(ArmsController))]
	class ArmsControllerPatcher
	{
		[HarmonyPostfix]
		[HarmonyPatch("SetPlayerSpeedParameters")]
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
