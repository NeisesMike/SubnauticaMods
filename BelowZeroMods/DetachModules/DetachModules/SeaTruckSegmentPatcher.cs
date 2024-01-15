using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using System.Runtime.CompilerServices;
using System.Collections;
using UWE;
using System.Reflection.Emit;

namespace SeatruckHotkeys
{
    [HarmonyPatch(typeof(SeaTruckSegment))]
    [HarmonyPatch("OnHoverHatch")]
    class SeaTruckSegmentOnHoverHatchPatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckSegment __instance, Player ___player)
        {
            if (___player == null && !__instance.CanEnter())
            {
                return false;
            }
            HandReticle.main.SetIcon(HandReticle.IconType.Interact, 1f);

            string entryString = "EnterSeaTruck";

            if (__instance.isMainCab && SeatruckHotkeysPatcher.Config.isEntryHintingEnabled)
            {
                entryString += "\nEnter-to-Piloting with " + SeatruckHotkeysPatcher.Config.directEntryKey.ToString();
            }

            HandReticle.main.SetText(HandReticle.TextType.Hand, ___player ? "ExitSeaTruck" : entryString, true, GameInput.Button.LeftHand);
            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);

            if (Input.GetKeyDown(SeatruckHotkeysPatcher.Config.directEntryKey) && __instance.isMainCab)
            {
                __instance.motor.StartPiloting();
                __instance.seatruckanimation.currentAnimation = SeaTruckAnimation.Animation.EnterPilot;
                __instance.Enter(Player.main);
                global::Utils.PlayFMODAsset(__instance.enterSound, Player.main.transform, 20f);
            }
            return false;
        }
    }
}