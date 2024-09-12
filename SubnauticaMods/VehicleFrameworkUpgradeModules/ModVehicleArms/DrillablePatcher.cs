using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using VehicleFramework;

namespace VFDrillArm
{
	[HarmonyPatch(typeof(Drillable))]
	public class DrillablePatcher
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(Drillable.ManagedUpdate))]
		public static void ManagedUpdatePostfix(Drillable __instance)
		{
			ModVehicle drillingMV = ExosuitDrillArmPatcher.drillingModVehicle;
			if (__instance.lootPinataObjects.Count > 0 && drillingMV)
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject gameObject in __instance.lootPinataObjects)
				{
					if (gameObject == null)
					{
						list.Add(gameObject);
					}
					else
					{
						Vector3 b = drillingMV.transform.position + new Vector3(0f, 0.8f, 0f);
						gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, b, Time.deltaTime * 5f);
						if (Vector3.Distance(gameObject.transform.position, b) < 3f)
						{
							Pickupable componentInChildren = gameObject.GetComponentInChildren<Pickupable>();
							if (componentInChildren)
							{
								drillingMV.AddToStorage(componentInChildren);
								list.Add(gameObject);
							}
						}
					}
				}
				if (list.Count > 0)
				{
					foreach (GameObject item2 in list)
					{
						__instance.lootPinataObjects.Remove(item2);
					}
				}
			}
		}
	}
}
