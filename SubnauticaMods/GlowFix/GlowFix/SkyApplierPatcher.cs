using HarmonyLib;
using System.Threading.Tasks;
using UnityEngine;

namespace GlowFix
{
	[HarmonyPatch(typeof(SkyApplier))]
	[HarmonyPatch("Start")]
	class SkyApplierPatcher
	{
		static async void myStart(SkyApplier mySA)
        {
			while (GlowFixPatcher.exteriorModuleTechTypes == null)
			{
				await Task.Delay(1000);
			}

			Constructable myCon = mySA.gameObject.GetComponent<Constructable>();
			bool isAnExteriorModule = false;
			foreach (TechType myTT in GlowFixPatcher.exteriorModuleTechTypes)
			{
				Logger.Log(myTT.ToString());
				if (myCon.techType == myTT)
				{
					isAnExteriorModule = true;
					break;
				}
			}

			if (mySA.anchorSky == Skies.Custom && mySA.customSkyPrefab != null)
			{
				mySA.environmentSky = MarmoSkies.main.GetSky(mySA.customSkyPrefab);
			}
			if (isAnExteriorModule)
			{
				if (mySA.applySky == null)
				{
					mySA.GetAndApplySkybox(null);
				}
			}
            else
            {
				if (mySA.applySky == null)
				{
					GameObject environment = SkyApplier.GetEnvironment(mySA.transform.root.gameObject, mySA.anchorSky);
					mySA.GetAndApplySkybox(environment);
				}
			}
		}

		[HarmonyPrefix]
		public static bool Prefix(SkyApplier __instance)
		{
			if (!__instance.gameObject)
			{
				return true;
			}
			Constructable myCon = __instance.gameObject.GetComponent<Constructable>();
			if (!myCon)
			{
				return true;
			}

			myStart(__instance);
			return false;
		}
	}

}
