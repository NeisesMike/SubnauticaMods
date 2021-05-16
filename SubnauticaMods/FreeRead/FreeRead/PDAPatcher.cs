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

namespace FreeRead
{
    [HarmonyPatch(typeof(PDA))]
    [HarmonyPatch("Close")]
    class PDAClosePatcher
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            FreeReadPatcher.isCruising = false;
            return true;
        }
    }
}
