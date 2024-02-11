using HarmonyLib;

namespace GlowFix
{
    [HarmonyPatch(typeof(SkyApplier))]
    [HarmonyPatch(nameof(SkyApplier.Start))]
    class SkyApplierPatcher
    {
		[HarmonyPrefix]
        public static bool Prefix(SkyApplier __instance, ref mset.Sky ___environmentSky, ref bool ___initialized)
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

			if (LargeWorld.main == null || !LargeWorld.main.IsMounted())
			{
				return false;
			}
			if (GlowFixPatcher.exteriorModuleTechTypes == null)
			{
				return false;
			}

			bool isThisAnExteriorModule = false;
			foreach (TechType myTT in GlowFixPatcher.exteriorModuleTechTypes)
			{
				if (myCon.techType == myTT)
				{
					isThisAnExteriorModule = true;
					break;
				}
			}

			if (!isThisAnExteriorModule)
			{
				return true;
			}

			__instance.OnEnvironmentChanged(null);
			return false;

		}
	}

}
