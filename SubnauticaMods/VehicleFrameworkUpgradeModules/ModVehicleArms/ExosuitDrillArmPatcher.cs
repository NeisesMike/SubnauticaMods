using HarmonyLib;
using VehicleFramework;
using UnityEngine;

namespace VFDrillArm
{
	[HarmonyPatch(typeof(ExosuitDrillArm))]
	public static class ExosuitDrillArmPatcher
	{
		public static ModVehicle drillingModVehicle = null;
		public static bool TraceTargetPosition(GameObject ignoreObj, Vector3 origin, float maxDist, ref GameObject closestObj, ref Vector3 position)
		{
			bool result = false; ;
			int num = UWE.Utils.RaycastIntoSharedBuffer(new Ray(origin, ignoreObj.transform.forward), maxDist, -2097153, QueryTriggerInteraction.UseGlobal);
			if (num == 0)
			{
				num = UWE.Utils.SpherecastIntoSharedBuffer(origin, 0.7f, ignoreObj.transform.forward, maxDist, -2097153, QueryTriggerInteraction.UseGlobal);
			}
			closestObj = null;
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = UWE.Utils.sharedHitBuffer[i];
				if ((!(ignoreObj != null) || !UWE.Utils.IsAncestorOf(ignoreObj, raycastHit.collider.gameObject)) && (!raycastHit.collider || !raycastHit.collider.isTrigger || raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Useable")) && (closestObj == null || raycastHit.distance < num2))
				{
					closestObj = raycastHit.collider.gameObject;
					num2 = raycastHit.distance;
					position = raycastHit.point;
					result = true;
				}
			}
			return result;
		}
		[HarmonyPrefix]
		[HarmonyPatch(nameof(ExosuitDrillArm.OnHit))]
		public static bool OnHitPrefix(ExosuitDrillArm __instance)
		{
			drillingModVehicle = null;
			ModVehicle mv = UWE.Utils.GetComponentInHierarchy<ModVehicle>(__instance.gameObject);
			if (mv == null)
			{
				return true;
			}
			else
			{
				Vector3 zero = Vector3.zero;
				GameObject gameObject = null;
				__instance.drillTarget = null;
				Vector3 origin = mv.gameObject.transform.position;
				if (mv.Arms.leftArmPlacement != null && mv.Arms.rightArmPlacement != null)
				{
					origin = Vector3.Lerp(mv.Arms.leftArmPlacement.position, mv.Arms.rightArmPlacement.position, 0.5f);
				}
				TraceTargetPosition(mv.gameObject, origin, 5f, ref gameObject, ref zero);
				if (gameObject && __instance.drilling)
				{
					Drillable drillable = gameObject.FindAncestor<Drillable>();
					__instance.loopHit.Play();
					if (!drillable)
					{
						LiveMixin liveMixin = gameObject.FindAncestor<LiveMixin>();
						if (liveMixin)
						{
							liveMixin.IsAlive();
							liveMixin.TakeDamage(4f, zero, DamageType.Drill, null);
							__instance.drillTarget = gameObject;
						}
						VFXSurface component2 = gameObject.GetComponent<VFXSurface>();
						if (__instance.drillFXinstance == null)
						{
							__instance.drillFXinstance = VFXSurfaceTypeManager.main.Play(component2, __instance.vfxEventType, __instance.fxSpawnPoint.position, __instance.fxSpawnPoint.rotation, __instance.fxSpawnPoint);
						}
						else if (component2 != null && __instance.prevSurfaceType != component2.surfaceType)
						{
							__instance.drillFXinstance.GetComponent<VFXLateTimeParticles>().Stop();
							UnityEngine.Object.Destroy(__instance.drillFXinstance.gameObject, 1.6f);
							__instance.drillFXinstance = VFXSurfaceTypeManager.main.Play(component2, __instance.vfxEventType, __instance.fxSpawnPoint.position, __instance.fxSpawnPoint.rotation, __instance.fxSpawnPoint);
							__instance.prevSurfaceType = component2.surfaceType;
						}
						gameObject.SendMessage("BashHit", __instance, SendMessageOptions.DontRequireReceiver);
						return false;
					}
					GameObject gameObject2;
					drillingModVehicle = mv;
					drillable.OnDrill(__instance.fxSpawnPoint.position, null, out gameObject2);
					__instance.drillTarget = gameObject2;
					if (__instance.fxControl.emitters[0].fxPS != null && !__instance.fxControl.emitters[0].fxPS.emission.enabled)
					{
						__instance.fxControl.Play(0);
						return false;
					}
				}
				else
				{
					__instance.StopEffects();
				}
				return false;
			}
		}
	}
}
