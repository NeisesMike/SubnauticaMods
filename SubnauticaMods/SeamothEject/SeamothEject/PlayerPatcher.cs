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

namespace SeamothEject
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("SpawnNearby")]
    class PlayerSpawnNearbyPatcher
	{
        [HarmonyPostfix]
        public static void Postfix(Player __instance, ref bool __result, float spawnRadius, GameObject ignoreObject)
        {
			Vector3 position = Vector3.zero;
			bool flag = false;
			for (int i = 0; i < 10; i++)
			{
				float f = UnityEngine.Random.value * 2f * 3.1415927f;
				Vector3 vector = new Vector3(__instance.transform.root.position.x + Mathf.Cos(f) * spawnRadius, __instance.transform.root.position.y, __instance.transform.root.position.z + Mathf.Sin(f) * spawnRadius);
				switch(SeamothEjectPatcher.Config.myPlacement)
                {
					case (EjectionPlacement.Behind):
						vector -= 6.5f * __instance.transform.root.forward;
						break;
					case (EjectionPlacement.Above):
						vector += 6.5f * __instance.transform.root.up;
						break;
					case (EjectionPlacement.Left):
						vector -= 6.5f * __instance.transform.root.right;
						break;
					case (EjectionPlacement.Right):
						vector += 6.5f * __instance.transform.root.right;
						break;
					case (EjectionPlacement.Bottom):
						vector -= 6.5f * __instance.transform.root.up;
						break;
					case (EjectionPlacement.Front):
						vector += 6.5f * __instance.transform.root.forward;
						break;
					case (EjectionPlacement.Normal):
						break;
				}
				if (__instance.playerController.WayToPositionClear(vector, ignoreObject, true))
				{
					position = vector;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				__instance.transform.root.parent = null;
				__instance.SetPosition(position);
			}
			else
			{
				ErrorMessage.AddError(Language.main.Get("SpawnNearbyFailed"));
			}
			__result = flag;
		}
    }
}
