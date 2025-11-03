using BepInEx;

namespace IonDefenseCapacitor
{
    [BepInPlugin("com.mikjaw.subnautica.iondefensecapacitor.mod", "IonDefenseCapacitor", "2.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new IonDefenseCapacitor());
        }
    }
}
