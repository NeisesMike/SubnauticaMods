using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using LitJson;
using System.Runtime.CompilerServices;
using System.Collections;

namespace FreeRead
{
    [HarmonyPatch(typeof(PDA))]
    [HarmonyPatch("Close")]
    class PDAClosePatcher
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            Logger.Log("Closing PDA");
            GameInput.SetAutoMove(FreeReadPatcher.isCruising);
            FreeReadPatcher.isCruising = false;
            FreeReadPatcher.isInPDA = false;
            return true;
        }
    }

    [HarmonyPatch(typeof(PDA))]
    [HarmonyPatch("Open")]
    class PDAOpenPatcher
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            FreeReadPatcher.isInPDA = true;
            if (GameInput.GetAutoMove())
            {
               Logger.Log("Starting Cruise!");
               FreeReadPatcher.isCruising = true;
            }
            return true;
        }
    }
}
