using BepInEx;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Handlers;

namespace MagnetBoots
{
    [BepInPlugin("com.mikjaw.subnautica.magnetboots.mod", "MagnetBoots", "2.0.0")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static MagnetBootsConfig MagnetConfig { get; private set; }
        public void Start()
        {
            VehicleFramework.Admin.UpgradeCompat compat = new VehicleFramework.Admin.UpgradeCompat()
            {
                skipCyclops = true,
                skipExosuit = false,
                skipModVehicle = false,
                skipSeamoth = false
            };
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new MagnetBootsModule(), compat);
            MagnetConfig = OptionsPanelHandler.RegisterModOptions<MagnetBootsConfig>();
        }
    }

    [Menu("Magnet Boots Options")]
    public class MagnetBootsConfig : ConfigFile
    {
        [Slider("Collider Pairs Per Frame (performance)", Tooltip = "When magnet boots are used, they ask for collisions between the vehicle and its host to be ignored. This can be very expensive because some vehicles and hosts have a large number of colliders. Raising this number will make the magnet-vehicle touchable sooner. Lowering this number will reduce stutter on magnet events. Set to zero for no limit.", DefaultValue = 20, Step = 1, Min = 0, Max = 100)]
        public int pairsPerFrame = 20;
    }
}