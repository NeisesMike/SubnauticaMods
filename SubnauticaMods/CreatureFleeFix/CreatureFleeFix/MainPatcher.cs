using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;

namespace CreatureFleeFix
{
    [BepInPlugin("com.mikjaw.subnautica.creaturefleefix.mod", "CreatureFleeFix", "1.0")][BepInDependency("com.snmodding.nautilus")]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            var harmony = new Harmony("com.mikjaw.subnautica.creaturefleefix.mod");
            harmony.PatchAll();
        }
    }
}
