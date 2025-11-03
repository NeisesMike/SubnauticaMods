using BepInEx;
using HarmonyLib;

namespace CrabsquidModule
{
    [BepInPlugin("com.mikjaw.subnautica.crabsquidmodule.mod", "CrabsquidModule", "2.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new CrabsquidModule());
            var harmony = new Harmony("com.mikjaw.subnautica.crabsquidmodule.mod");
            harmony.PatchAll();
        }
    }
}