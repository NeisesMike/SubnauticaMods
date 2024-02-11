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
using System.Runtime.CompilerServices;
using System.Collections;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Utility;

namespace AttitudeIndicator
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]
    public class PlayerUpdatePatcher
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            IndicatorManager.DoUpdate();
        }
    }
}

