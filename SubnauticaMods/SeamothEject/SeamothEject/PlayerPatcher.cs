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
		public static bool findPosition(EjectionPlacement placementToTry, ref Vector3 myPosition, Player thisPlayer, GameObject ignoreObject)
		{
			Vehicle thisSeamoth = thisPlayer.GetVehicle();

			float spawnRadius = 0.5f;
			for (int i = 0; i < 10; i++)
			{
				float f = UnityEngine.Random.value * 2f * 3.1415927f;
				Vector3 vector = new Vector3(thisSeamoth.transform.position.x + Mathf.Cos(f) * spawnRadius,
										     thisSeamoth.transform.position.y,
											 thisSeamoth.transform.position.z + Mathf.Sin(f) * spawnRadius);

				switch (placementToTry)
				{
					case (EjectionPlacement.Behind):
						vector -= 4f * thisSeamoth.transform.forward;
						break;
					case (EjectionPlacement.Above):
						vector += 2.5f * thisSeamoth.transform.up;
						break;
					case (EjectionPlacement.Left):
						vector -= 3.0f * thisSeamoth.transform.right;
						break;
					case (EjectionPlacement.Right):
						vector += 3.0f * thisSeamoth.transform.right;
						break;
					case (EjectionPlacement.Below):
						vector -= 2.5f * thisSeamoth.transform.up;
						break;
					case (EjectionPlacement.Front):
						vector += 4f * thisSeamoth.transform.forward;
						break;
					case (EjectionPlacement.Normal):
						break;
				}
				if (thisPlayer.playerController.WayToPositionClear(vector, ignoreObject, true))
				{
					myPosition = vector;
					return true;
				}
			}
			return false;
		}


		[HarmonyPostfix]
        public static void Postfix(Player __instance, ref bool __result, float spawnRadius, GameObject ignoreObject)
        {
			bool isSeamoth = __instance.GetVehicle().ToString().Contains("SeaMoth");
			if(!isSeamoth)
            {
				return;
            }


			Vector3 position = Vector3.zero;
			bool flag = findPosition(SeamothEjectPatcher.config.myPlacement, ref position, __instance, ignoreObject);


			// ensure we can find a place to exit, if at all possible
			if (!flag)
            {
				flag = findPosition(EjectionPlacement.Behind, ref position, __instance, ignoreObject);
			}
			if (!flag)
			{
				flag = findPosition(EjectionPlacement.Above, ref position, __instance, ignoreObject);
			}
			if (!flag)
			{
				flag = findPosition(EjectionPlacement.Left, ref position, __instance, ignoreObject);
			}
			if (!flag)
			{
				flag = findPosition(EjectionPlacement.Right, ref position, __instance, ignoreObject);
			}
			if (!flag)
			{
				flag = findPosition(EjectionPlacement.Below, ref position, __instance, ignoreObject);
			}
			if (!flag)
			{
				flag = findPosition(EjectionPlacement.Front, ref position, __instance, ignoreObject);
			}
			if (!flag)
			{
				flag = findPosition(EjectionPlacement.Normal, ref position, __instance, ignoreObject);
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
