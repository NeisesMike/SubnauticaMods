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
			// do we still need this?
			SafeAnimator.SetFloat(___animator, "view_pitch", 0);
		}
	}
}
