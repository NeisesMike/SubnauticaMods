using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Harmony;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using LitJson;
using System.Runtime.CompilerServices;


namespace RollControl
{

	[HarmonyPatch(typeof(UnderwaterMotor))]
	[HarmonyPatch("OnCollisionStay")]
	class PlayerCollisionPatcher
	{
		[HarmonyPrefix]
		public static bool Prefix(Collision collision, UnderwaterMotor __instance)
		{
			Rigidbody ogRb = Traverse.Create(__instance).Field("rb").GetValue<Rigidbody>();
			ogRb.freezeRotation = true;
			Traverse.Create(__instance).Field("rb").SetValue(ogRb);

			return true;

			if (PlayerAwakePatcher.myRollMan.isRollToggled)
			{
				/*
				var m_Material = GetComponent<Renderer>().material;
				m_Material
				*/

				__instance.grounded = false;
				return false;
			}
			return true;

			/*
			Vector3 vector = default(Vector3);
			int num = 0;
			for (int i = 0; i < collision.contacts.Length; i++)
			{
				ContactPoint contactPoint = collision.contacts[i];
				if (num == 0)
				{
					vector = contactPoint.normal;
				}
				else
				{
					vector += contactPoint.normal;
				}
				num++;
			}
			if (num > 0)
			{
				vector /= (float)num;
				__instance.grounded = true;
				if (vector.y > 0.5f)
				{
					__instance.grounded = true;
				}
				___surfaceNormal = vector;
				return false;
			}
			___surfaceNormal = new Vector3(0f, 1f, 0f);
			return false;
			*/
		}
	}

	[HarmonyPatch(typeof(UnderwaterMotor))]
	[HarmonyPatch("Update")]
	class PlayerCollisionPatcher3
	{
		[HarmonyPrefix]
		public static bool Prefix(UnderwaterMotor __instance)
		{
			Rigidbody ogRb = Traverse.Create(__instance).Field("rb").GetValue<Rigidbody>();
			ogRb.freezeRotation = true;
			Traverse.Create(__instance).Field("rb").SetValue(ogRb);
			return true;
		}
	}
}
