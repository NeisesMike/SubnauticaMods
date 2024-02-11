using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using Nautilus.Options;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.Net.NetworkInformation;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace FreeLook
{
    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("Update")]
    public class SeaTruckMotorUpdatePatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckMotor __instance, float ___afterBurnerTime, bool ___waitForDocking, GameObject ___inputStackDummy, float ___animAccel, FMOD.Studio.PARAMETER_ID ___velocityParamIndex,
                FMOD.Studio.PARAMETER_ID ___depthParamIndex, FMOD.Studio.PARAMETER_ID ___rpmParamIndex, FMOD.Studio.PARAMETER_ID ___turnParamIndex, FMOD.Studio.PARAMETER_ID ___upgradeParamIndex,
                FMOD.Studio.PARAMETER_ID ___damagedParamIndex, Animator ___animator)
        {
            return true; // TODO: what ?
            void UpdateDrag()
            {
                if (__instance.useRigidbody)
                {
                    if (__instance.transform.root.position.y < 0f)
                    {
                        if (__instance.IsPiloted())
                        {
                            __instance.useRigidbody.drag = __instance.pilotingDrag;
                            return;
                        }
                        __instance.useRigidbody.drag = __instance.underWaterDrag;
                        return;
                    }
                    else
                    {
                        __instance.useRigidbody.drag = __instance.aboveWaterDrag;
                    }
                }
            }

            if (!FreeLookPatcher.isFreeLooking)
            {
                return true;
            }
            else
            {
                if (__instance.afterBurnerActive && Time.time > ___afterBurnerTime)
                {
                    __instance.afterBurnerActive = false;
                }
                UpdateDrag();
                if (__instance.piloting && __instance.useRigidbody != null && !__instance.IsBusyAnimating() && !___waitForDocking)
                {
                    if ((__instance.truckSegment.isMainCab ? (AvatarInputHandler.main.IsEnabled() && !Player.main.GetPDA().isInUse) : ___inputStackDummy.activeInHierarchy) && GameInput.GetButtonDown(GameInput.Button.Exit))
                    {
                        __instance.StopPiloting(false, false, false);
                    }
                    else if (!__instance.truckSegment.isMainCab && GameInput.GetButtonDown(GameInput.Button.PDA))
                    {
                        __instance.StopPiloting(false, false, false);
                        __instance.OpenPDADelayed(0.7f);
                    }
                    else if (!__instance.truckSegment.isMainCab && Player.main.transform.position.y > -1.5f)
                    {
                        __instance.StopPiloting(false, false, false);
                        float d = 5f;
                        __instance.useRigidbody.AddForce(-Vector3.up * d, ForceMode.VelocityChange);
                    }
                    else if (!__instance.truckSegment.underCreatureAttack && __instance.IsPowered())
                    {
                        if (__instance.CanTurn())
                        {
                            if (___animator)
                            {
                                ___animAccel = Mathf.Lerp(___animAccel, (float)__instance.leverDirection.y, Time.deltaTime * 3f);
                                ___animator.SetFloat("move_speed_z", ___animAccel);
                            }
                        }
                        if (__instance.upgrades && GameInput.GetButtonDown(GameInput.Button.Sprint))
                        {
                            __instance.upgrades.TryActivateAfterBurner();
                        }
                    }
                    if (___inputStackDummy.activeInHierarchy && IngameMenu.main != null)
                    {
                        if (GameInput.GetButtonDown(GameInput.Button.UIMenu))
                        {
                            IngameMenu.main.Open();
                        }
                        else if (!IngameMenu.main.gameObject.activeInHierarchy)
                        {
                            UWE.Utils.lockCursor = true;
                        }
                    }
                }
                if (__instance.engineSound)
                {
                    if (__instance.piloting && __instance.IsPowered())
                    {
                        __instance.engineSound.Play();
                        __instance.engineSound.SetParameterValue(___velocityParamIndex, __instance.useRigidbody.velocity.magnitude);
                        __instance.engineSound.SetParameterValue(___depthParamIndex, __instance.transform.root.position.y);
                        __instance.engineSound.SetParameterValue(___rpmParamIndex, (GameInput.GetMoveDirection().z + 1f) * 0.5f);
                        __instance.engineSound.SetParameterValue(___turnParamIndex, Mathf.Clamp(GameInput.GetLookDelta().x * 0.3f, -1f, 1f));
                        __instance.engineSound.SetParameterValue(___upgradeParamIndex, (float)(((__instance.powerEfficiencyFactor < 1f) ? 1 : 0) + (__instance.horsePowerUpgrade ? 2 : 0)));
                        if (__instance.liveMixin)
                        {
                            __instance.engineSound.SetParameterValue(___damagedParamIndex, 1f - __instance.liveMixin.GetHealthFraction());
                        }
                    }
                    else
                    {
                        __instance.engineSound.Stop();
                    }
                }
                if (___waitForDocking && !__instance.truckSegment.IsDocking())
                {
                    ___waitForDocking = false;
                    Player.main.ExitLockedMode(false, false, null);
                }
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("FixedUpdate")]
    public class SeaTruckMotorFixedUpdatePatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckMotor __instance, bool ____piloting, GameObject ___inputStackDummy)
        {
            if (!FreeLookPatcher.isFreeLooking)
            {
                return true;
            }

            void StabilizeRoll()
            {
                float num = Mathf.Abs(__instance.transform.root.eulerAngles.z - 180f);
                if (num <= 180f)
                {
                    float d = Mathf.Clamp01(1f - num / 180f) * 8f;
                    __instance.useRigidbody.AddTorque(__instance.transform.root.forward * d * Time.deltaTime * Mathf.Sign(__instance.transform.root.eulerAngles.z - 180f), ForceMode.VelocityChange);
                }
            }

            // Token: 0x060034C1 RID: 13505 RVA: 0x00125A30 File Offset: 0x00123C30
            void StabilizePitch()
            {
                float num = Mathf.Abs(__instance.transform.root.eulerAngles.x - 180f);
                if (180f - num > 0f)
                {
                    float d = Mathf.Clamp01(1f - num / 180f) * 8f;
                    __instance.useRigidbody.AddTorque(__instance.transform.root.right * d * Time.deltaTime * Mathf.Sign(__instance.transform.root.eulerAngles.x - 180f), ForceMode.VelocityChange);
                }
            }

            bool myIsPowered = !__instance.requiresPower || (__instance.relay && __instance.relay.IsPowered());
            bool myIsBusyAnimating = __instance.waitForAnimation && __instance.seatruckanimation != null && __instance.seatruckanimation.currentAnimation > SeaTruckAnimation.Animation.Idle;
            float myGetWeight = __instance.truckSegment.GetWeight() + __instance.truckSegment.GetAttachedWeight() * (__instance.horsePowerUpgrade ? 0.65f : 0.8f);

            if (__instance.transform.position.y < Ocean.GetOceanLevel() && __instance.useRigidbody != null && myIsPowered && !myIsBusyAnimating)
            {
                if (____piloting)
                {
                    Vector3 vector = (AvatarInputHandler.main.IsEnabled() || ___inputStackDummy.activeInHierarchy) ? GameInput.GetMoveDirection() : Vector3.zero;
                    if (__instance.afterBurnerActive)
                    {
                        vector.z = 1f;
                    }
                    vector = vector.normalized;
                    Vector3 a = __instance.transform.rotation * vector;
                    float num = 1f / Mathf.Max(1f, myGetWeight * 0.35f) * __instance.acceleration;
                    if (__instance.afterBurnerActive)
                    {
                        num += 7f;
                    }
                    __instance.useRigidbody.AddForce(num * a, ForceMode.Acceleration);
                    if (__instance.relay && vector != Vector3.zero)
                    {
                        float num2;
                        __instance.relay.ConsumeEnergy(Time.deltaTime * __instance.powerEfficiencyFactor * 0.12f, out num2);
                    }
                }
                StabilizeRoll();
            }
            if (__instance.truckSegment.IsFront() && (!myIsPowered || __instance.truckSegment.ReachingOutOfWater() || (__instance.seatruckanimation && __instance.seatruckanimation.currentAnimation == SeaTruckAnimation.Animation.Enter)))
            {
                StabilizePitch();
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("CanTurn")]
    public class SeaTruckMotorCanTurnPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckMotor __instance, ref bool __result)
        {
            __result = !FreeLookPatcher.isFreeLooking && (!__instance.truckSegment.isMainCab || __instance.truckSegment.transform.position.y < 0f);
        }
    }
}
