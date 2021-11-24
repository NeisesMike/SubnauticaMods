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

namespace CreatureFleeFix
{
	[HarmonyPatch(typeof(FleeOnDamage))]
	[HarmonyPatch("OnTakeDamage")]
	class OnTakeDamagePatcher
	{
		[HarmonyPostfix]
		public static void Postfix(FleeOnDamage __instance, DamageInfo damageInfo)
		{
			if (__instance.accumulatedDamage > __instance.damageThreshold)
			{
				Vector3 DamageDealerPosition = damageInfo.position;
				if(DamageDealerPosition == Vector3.zero)
				{
					// the chance that we actually meant to calculate (0,0,0) is neglible,
					// so let's flee from the player instead,
					DamageDealerPosition = Player.main.transform.position;
				}
				Vector3 MoveVector = Vector3.Normalize(__instance.transform.position - DamageDealerPosition) * (__instance.minFleeDistance + damageInfo.damage / 30f);
				Vector3 DestinationVector = MoveVector + __instance.transform.position;
				Vector3 DestinationVectorOcean = new Vector3(DestinationVector.x, Mathf.Min(DestinationVector.y, Ocean.main.GetOceanLevel()), DestinationVector.z);
				__instance.moveTo = DestinationVectorOcean;
				__instance.timeToFlee = Time.time + __instance.fleeDuration;
			}
		}
	}
}
