using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace RollControl
{
    [HarmonyPatch(typeof(ArmsController))]
    class ArmsControllerPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static bool Prefix(ArmsController __instance, Animator ___animator, Player ___player, GUIHand ___guiHand, bool ___reconfigureWorldTarget,
			PlayerTool ___lastTool, bool ___wasBleederAttached, bool ___wasPdaInUse, PDA ___pda)
		{
			bool flag = __instance.IsBleederAttached();
			//__instance.SetPlayerSpeedParameters();
			bool value = ___player.timeGrabbed != 0f && (double)___player.timeGrabbed + 0.4 > (double)Time.time;
			SafeAnimator.SetBool(___animator, "grab", value);
			bool value2 = ___player.timeBashed != 0f && (double)___player.timeBashed + 0.4 > (double)Time.time;
			SafeAnimator.SetBool(___animator, "bash", value2);
			SafeAnimator.SetBool(___animator, "is_underwater", ___player.IsUnderwater() && Player.main.motorMode != Player.MotorMode.Vehicle);
			SafeAnimator.SetBool(___animator, "cinematics_enabled", !GameOptions.GetVrAnimationMode());
			PlayerTool playerTool = ___guiHand.GetTool();
			if (___player.GetPDA().isInUse)
			{
				playerTool = null;
			}
			if (___reconfigureWorldTarget || playerTool != ___lastTool || flag != ___wasBleederAttached || (playerTool != null && playerTool.PollForceConfigureIK()) || ___pda.isActiveAndEnabled != ___wasPdaInUse)
			{
				//__instance.Reconfigure(playerTool);
			}
			___lastTool = playerTool;
			___wasPdaInUse = ___pda.isActiveAndEnabled;
			//__instance.leftAim.Update(__instance.ikToggleTime);
			//__instance.rightAim.Update(__instance.ikToggleTime);
			//__instance.UpdateHandIKWeights();
			Player.main.IsUnderwater();
			if (__instance.bleederAttackTarget.attached)
			{
				SafeAnimator.SetBool(___animator, "using_tool", ___player.GetRightHandHeld());
				if (___player.GetRightHandHeld())
				{
					GoalManager.main.OnCustomGoalEvent("AttackBleeder");
				}
			}
			else
			{
				SafeAnimator.SetBool(___animator, "using_tool", ___guiHand.GetUsingTool());
				SafeAnimator.SetBool(___animator, "using_tool_alt", ___guiHand.GetAltAttacking());
			}
			SafeAnimator.SetBool(___animator, "holding_tool", playerTool != null);
			Inventory.main.GetHeldTool();
			SafeAnimator.SetBool(___animator, "in_seamoth", Player.main.inSeamoth);
			SafeAnimator.SetBool(___animator, "in_exosuit", Player.main.inExosuit);
			SafeAnimator.SetBool(___animator, "cyclops_steering", Player.main.GetMode() == Player.Mode.Piloting);
			SafeAnimator.SetBool(___animator, "bleeder", __instance.bleederAttackTarget.attached);
			SafeAnimator.SetBool(___animator, "jump", ___player.GetPlayFallingAnimation());
			SafeAnimator.SetFloat(___animator, "verticalOffset", MainCameraControl.main.GetImpactBob());
			___wasBleederAttached = flag;
			//__instance.UpdateDiving();



			return false;
        }
    }
}
