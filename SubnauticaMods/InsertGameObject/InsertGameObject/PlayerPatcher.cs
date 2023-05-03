using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace InsertGameObject
{
    [HarmonyPatch(typeof(Player))]
    class PlayerPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.Start))]
        public static void StartPostfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<GameObjectInserter>();
        }
    }
}
