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

using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;

namespace GroundedItems
{
    [BepInPlugin("com.mikjaw.subnautica.groundeditems.mod", "Grounded Items", "1.0")]
    public class MainPatcher : BaseUnityPlugin
    {
        public static ManualLogSource logger;
        public void Start()
        {
            logger = base.Logger;
            var harmony = new Harmony("com.mikjaw.subnautica.groundeditems.mod");
            harmony.PatchAll();
            DepthManager.depth_dictionary = DepthManager.GetDepthDictionary();
        }
    }
}
