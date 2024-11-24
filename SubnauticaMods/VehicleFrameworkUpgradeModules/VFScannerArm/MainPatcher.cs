using BepInEx;
using System.Collections;
using UnityEngine;
using HarmonyLib;
using VehicleFramework.Admin;

namespace VFScannerArm
{
    [BepInPlugin("com.mikjaw.subnautica.vfscannerarm.mod", "VFScannerArm", "1.3")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public static GameObject originalScannerToolPrefab = null;
        public static ScannerTool originalScannerTool = null;
        public void Start()
        {
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
