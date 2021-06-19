using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using System.Runtime.CompilerServices;

namespace RollControl
{
    [HarmonyPatch(typeof(MouseLook))]
    [HarmonyPatch("Update")]
    class MouseLookPatcher
    {

        [HarmonyPrefix]
        public static bool Prefix(MouseLook __instance, ref float ___rotationY)
		{
			bool flag = __instance.mouseLookEnabled && AvatarInputHandler.main.IsEnabled();
			float num = flag ? Input.GetAxisRaw("Mouse X") : 0f;
			float num2 = flag ? Input.GetAxisRaw("Mouse Y") : 0f;
			if (__instance.invertY)
			{
				num2 *= -1f;
			}
			if (__instance.axes == MouseLook.RotationAxes.MouseXAndY)
			{
				float y = __instance.transform.localEulerAngles.y + num * __instance.sensitivityX;
				___rotationY += num2 * __instance.sensitivityY;
				___rotationY = Mathf.Clamp(___rotationY, __instance.minimumY, __instance.maximumY);
				__instance.transform.localEulerAngles = new Vector3(-___rotationY, y, 0f);
				return false;
			}
			if (__instance.axes == MouseLook.RotationAxes.MouseX)
			{
				__instance.transform.Rotate(0f, num * __instance.sensitivityX, 0f);
				return false;
			}
			___rotationY += num2 * __instance.sensitivityY;
			___rotationY = Mathf.Clamp(___rotationY, __instance.minimumY, __instance.maximumY);
			__instance.transform.localEulerAngles = new Vector3(-___rotationY, __instance.transform.localEulerAngles.y, 0f);

			return false;
		}
	}
}
