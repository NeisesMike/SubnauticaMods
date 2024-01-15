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
using LitJson;
using System.Net.NetworkInformation;

namespace FreeLook
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerStartPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        public static bool Prefix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<FreeLookManager>();
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("UpdateRotation")]
        public static bool UpdateRotation(Player __instance)
        {
            //Logger.Log("firing");
            return true;
        }
    }
}