using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.IO;

namespace PersistentCreatures
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerStartPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void StartPostfix()
        {
            PersistentCreaturesPatcher.Simulator = Player.main.gameObject.EnsureComponent<PersistentCreatureSimulator>();
        }

        /*
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        public static void UpdatePostfix()
        {
            if(Input.GetKeyDown(KeyCode.Backspace))
            {
                // create and register a shoal in front of the player


            }
        }
        */
    }
}
