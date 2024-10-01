using BepInEx;
using System.Collections;
using UnityEngine;

namespace VFScannerArm
{
    [BepInPlugin("com.mikjaw.subnautica.vfscannerarm.mod", "VFScannerArm", "1.0")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, MinimumDependencyVersion: "1.3.0")]
    public class MainPatcher : BaseUnityPlugin
    {
        public static GameObject originalScannerToolPrefab = null;
        public static ScannerTool originalScannerTool = null;
        public void Start()
        {
            UWE.CoroutineHost.StartCoroutine(GetOriginalScannerTool());
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new VFScannerArm());
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
