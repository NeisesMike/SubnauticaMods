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
								TryAddLoot(drillingMV, componentInChildren);
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
		public static void ModVehicleAddToStorage(ItemsContainer destination, Pickupable pickup)
		{
			uGUI_IconNotifier.main.Play(pickup.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);
			pickup.Initialize();
			InventoryItem item = new InventoryItem(pickup);
			destination.UnsafeAdd(item);
			pickup.PlayPickupSound();
		}
		public static void ModVehicleAddToStorage(ModVehicle mv, Pickupable pickup)
		{
			InventoryItem item = new InventoryItem(pickup);
			ItemsContainer destination;
			foreach (var container in mv.InnateStorages?.Select(x => x.Container.GetComponent<InnateStorageContainer>().container))
			{
				if (container.HasRoomFor(pickup))
				{
					destination = container;
					goto load;
				}
			}
			/*
			foreach (var modStorage in mv.ModularStorages)
			{
				if (modStorage.Container.activeSelf)
				{
					ItemsContainer container = modStorage.Container.GetComponent<SeamothStorageContainer>().container;
					if (container.HasRoomFor(pickup))
					{
						destination = container;
						goto load;
					}
				}
			}
			*/
			return;
		load:
			ModVehicleAddToStorage(destination, pickup);
		}
		public static ItemsContainer ModVehicleHasRoomFor(ModVehicle mv, Pickupable pickup)
		{
			foreach (var container in mv.InnateStorages?.Select(x => x.Container.GetComponent<InnateStorageContainer>().container))
			{
				if (container.HasRoomFor(pickup))
				{
					return container;
				}
			}
			/*
			foreach (var modStorage in mv.ModularStorages)
			{
				if(modStorage.Container.activeSelf)
				{
					ItemsContainer container = modStorage.Container.GetComponent<SeamothStorageContainer>().container;
					if(container.HasRoomFor(pickup))
                    {
						return container;
                    }
				}
			}
			*/
			return null;
		}
		public static void TryAddLoot(ModVehicle drillingMV, Pickupable loot)
		{
			ItemsContainer destination = ModVehicleHasRoomFor(drillingMV, loot);
			if (destination == null)
			{
				if (Player.main.GetVehicle() == drillingMV)
				{
					ErrorMessage.AddMessage(Language.main.Get("ContainerCantFit"));
				}
			}
			else
			{
				string arg = Language.main.Get(loot.GetTechName());
				ErrorMessage.AddMessage(Language.main.GetFormat<string>("VehicleAddedToStorage", arg));
				ModVehicleAddToStorage(destination, loot);
			}
		}
	}
}
