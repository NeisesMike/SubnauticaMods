using BepInEx.Configuration;
using VehicleFramework.Admin;

namespace AttitudeIndicator
{
    internal static class InstrumentConfig
    {
        internal const string enabledString = "Attitude Indicator Enabled";
        internal const string xString = "Attitude Indicator X Position";
        internal const string yString = "Attitude Indicator Y Position";
        internal const string zString = "Attitude Indicator Z Position";
        internal const string scaleString = "Attitude Indicator Scale";
        internal static void RegisterAll()
        {
            RegisterEnabledOptions();
            RegisterXOptions();
            RegisterYOptions();
            RegisterZOptions();
            RegisterScaleOptions();
        }
        private static void RegisterEnabledOptions()
        {
            const string optionName = enabledString;
            const string optionDescription = "Does the instrument appear?";
            ConfigRegistrar.RegisterForSeamoth<bool>(optionName, new ConfigDescription(optionDescription), true, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForPrawn<bool>(optionName, new ConfigDescription(optionDescription), true, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForCyclops<bool>(optionName, new ConfigDescription(optionDescription), true, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForAllModVehicles<bool>(optionName, new ConfigDescription(optionDescription), true, null, MainPatcher.Instance.Config);
        }
        private static void RegisterXOptions()
        {
            const string optionName = xString;
            const string optionDescription = "Where (left-to-right) does the instrument appear?";
            var valueRange = new AcceptableValueRange<float>(-1.5f, 1.5f);
            ConfigRegistrar.RegisterForSeamoth<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForPrawn<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForCyclops<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForAllModVehicles<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0, null, MainPatcher.Instance.Config);
        }
        private static void RegisterYOptions()
        {
            const string optionName = yString;
            const string optionDescription = "Where (top-to-bottom) does the instrument appear?";
            var valueRange = new AcceptableValueRange<float>(-0.77f, 0.77f);
            ConfigRegistrar.RegisterForSeamoth<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForPrawn<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForCyclops<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForAllModVehicles<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0, null, MainPatcher.Instance.Config);
        }
        private static void RegisterZOptions()
        {
            const string optionName = zString;
            const string optionDescription = "Where (front-to-back) does the instrument appear?";
            var valueRange = new AcceptableValueRange<float>(0, 1.5f);
            ConfigRegistrar.RegisterForSeamoth<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0.5f, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForPrawn<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0.5f, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForCyclops<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0.5f, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForAllModVehicles<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0.5f, null, MainPatcher.Instance.Config);
        }
        private static void RegisterScaleOptions()
        {
            const string optionName = scaleString;
            const string optionDescription = "How large does the instrument appear?";
            var valueRange = new AcceptableValueRange<float>(0, 0.2f);
            ConfigRegistrar.RegisterForSeamoth<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0.1f, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForPrawn<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0.1f, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForCyclops<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0.1f, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForAllModVehicles<float>(optionName, new ConfigDescription(optionDescription, valueRange), 0.1f, null, MainPatcher.Instance.Config);
        }
    }
}
