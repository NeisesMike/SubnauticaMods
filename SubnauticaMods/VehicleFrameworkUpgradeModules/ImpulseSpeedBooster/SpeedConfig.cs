using VehicleFramework.Admin;
using BepInEx.Configuration;

namespace ImpulseSpeedBooster
{
    public static class SpeedConfig
    {
        internal static void RegisterOptions()
        {
            const string optionName = "Impulse Power";
            const string optionDescription = "How powerful is the fully charge boost from the impulse speed booster";
            ConfigRegistrar.RegisterForSeamoth<float>(optionName, new ConfigDescription(optionDescription, new AcceptableValueRange<float>(0,1)), 1, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForPrawn<float>(optionName, new ConfigDescription(optionDescription, new AcceptableValueRange<float>(0, 1)), 1, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForCyclops<float>(optionName, new ConfigDescription(optionDescription, new AcceptableValueRange<float>(0, 1)), 1, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForAllModVehicles<float>(optionName, new ConfigDescription(optionDescription, new AcceptableValueRange<float>(0, 1)), 1, null, MainPatcher.Instance.Config);
        }
    }
}
