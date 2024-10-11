using BepInEx;
using System.Collections;
using UnityEngine;
using HarmonyLib;

namespace VFScannerArm
{
    [BepInPlugin("com.mikjaw.subnautica.vfscannerarm.mod", "VFScannerArm", "1.1")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public static GameObject originalScannerToolPrefab = null;
        public static ScannerTool originalScannerTool = null;
        public void Start()
        {
            UWE.CoroutineHost.StartCoroutine(GetOriginalScannerTool());
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new VFScannerArm());
            var harmony = new Harmony("com.mikjaw.subnautica.vfscannerarm.mod");
            harmony.PatchAll();
        }
        public IEnumerator GetOriginalScannerTool()
        {
            TaskResult<GameObject> result = new TaskResult<GameObject>();
            yield return CraftData.GetPrefabForTechTypeAsync(TechType.Scanner, false, result);
            originalScannerToolPrefab = result.Get();
            originalScannerTool = originalScannerToolPrefab.GetComponent<ScannerTool>();
        }
    }
}
