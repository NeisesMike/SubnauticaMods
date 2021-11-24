using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Utility;

namespace CreatureFleeFix
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[CreatureFleeFix] " + message);
        }
        public static void Output(string msg)
        {
            BasicText message = new BasicText(500, 0);
            message.ShowMessage(msg, 5);
        }
    }
    [QModCore]
    public static class MainPatcher
    {
        [QModPatch]
        public static void Patch()
        {
            var harmony = new Harmony("com.mikjaw.subnautica.creaturefleefix.mod");
            harmony.PatchAll();
        }
    }
}
