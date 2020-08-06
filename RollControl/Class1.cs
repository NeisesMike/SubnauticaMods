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
	[HarmonyPatch(typeof(PlayerMotor))]
	[HarmonyPatch("Update")]
	class PlayerMotorPatcher
	{
		private static float dynamicFriction = 1;
		private static float staticFriction = 1;
		private static float bounciness = 1;
		private static bool isFirst = true;


		[HarmonyPrefix]
		public static bool Prefix(PlayerMotor __instance)
		{
			return true;
			var coll = __instance.GetComponent<Collider>();
			if ( isFirst )
            {
				dynamicFriction = coll.material.dynamicFriction;
				staticFriction = coll.material.staticFriction;
				bounciness = coll.material.bounciness;
				isFirst = false;
			}

			if (PlayerAwakePatcher.myRollMan.isRollToggled)
			{
				coll.material.dynamicFriction = 0;
				coll.material.staticFriction = 0;
				coll.material.bounciness = 1;
				coll.material.frictionCombine = PhysicMaterialCombine.Minimum;

			}

			coll.material.dynamicFriction = dynamicFriction;
			coll.material.staticFriction = staticFriction;
			coll.material.bounciness = bounciness;

			return true;
		}
	}
}