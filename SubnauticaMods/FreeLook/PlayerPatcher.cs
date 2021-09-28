using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
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
    }
}