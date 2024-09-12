using BepInEx;
using HarmonyLib;

namespace VFDrillArm
{
    [BepInPlugin("com.mikjaw.subnautica.vfdrillarm.mod", "VFDrillArm", "1.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod", MinimumDependencyVersion: "1.3.2")]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new VFDrillArm());
            var harmony = new Harmony("com.mikjaw.subnautica.vfdrillarm.mod");
            harmony.PatchAll();
        }
    }
}
