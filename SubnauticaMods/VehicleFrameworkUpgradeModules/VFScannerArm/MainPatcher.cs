using System.Collections;
using BepInEx;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;
using HarmonyLib;
using VehicleFramework.Admin;

namespace VFScannerArm
{
    [BepInPlugin("com.mikjaw.subnautica.vfscannerarm.mod", "VFScannerArm", "2.0")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public static GameObject originalScannerToolPrefab = null;
        public static ScannerTool originalScannerTool = null;
        internal static ScannerArmConfig ScannerConfig { get; private set; }
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();
            ScannerConfig = OptionsPanelHandler.RegisterModOptions<ScannerArmConfig>();
            UWE.CoroutineHost.StartCoroutine(DoRegistrations());
            var harmony = new Harmony("com.mikjaw.subnautica.vfscannerarm.mod");
            harmony.PatchAll();
        }
        public IEnumerator DoRegistrations()
        {
            UWE.CoroutineHost.StartCoroutine(GetOriginalScannerTool());
            var vfscannerarm = new VFScannerArm();
            UpgradeTechTypes scannerArmTT = UpgradeRegistrar.RegisterUpgrade(vfscannerarm);
            TaskResult<GameObject> armRequest = new TaskResult<GameObject>();
            yield return UWE.CoroutineHost.StartCoroutine(vfscannerarm.GetArmPrefab(armRequest));
            GameObject armPrefab = armRequest.Get();
            FragmentUtils.RegisterScannerArmFragment(scannerArmTT.forModVehicle, armPrefab);
            if (ScannerConfig.vanillaFabricator)
            {
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.SeamothUpgrades,
                    vfscannerarm.TechTypes.forExosuit,
                    new string[] { "ExosuitModules" }
                );
            }
        }
        public IEnumerator GetOriginalScannerTool()
        {
            TaskResult<GameObject> result = new TaskResult<GameObject>();
            yield return CraftData.GetPrefabForTechTypeAsync(TechType.Scanner, false, result);
            originalScannerToolPrefab = result.Get();
            originalScannerTool = originalScannerToolPrefab.GetComponent<ScannerTool>();
        }
    }

    [Menu("Scanner Arm Options")]
    public class ScannerArmConfig : ConfigFile
    {
        [Toggle("Can be crafted in vanilla fabricator", Tooltip = "Allow the module to be crafted in the vehicle upgrades console. Restart required.")]
        public bool vanillaFabricator = false;
    }
}
