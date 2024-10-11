using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using VehicleFramework;
using VehicleFramework.UpgradeTypes;
using System;
using System.IO;
using System.Reflection;

namespace VFScannerArm
{
    public class VFScannerArm : ModVehicleArm
    {
        public override string ClassId => "ScannerArmVF";
        public override string DisplayName => "Scanner Arm";
        public override List<VehicleFramework.Assets.Ingredient> Recipe => new List<VehicleFramework.Assets.Ingredient>()
                {
                    new VehicleFramework.Assets.Ingredient(TechType.Titanium, 3),
                    new VehicleFramework.Assets.Ingredient(TechType.Battery, 1)
                };
        public override string Description => "A scanner-arm attachment used for analyzing specimens.";
        public override Atlas.Sprite Icon => VehicleFramework.Assets.SpriteHelper.GetSprite("VFScannerArmIcon.png");
        public override string TabName => "MVCM";
        public override IEnumerator GetArmPrefab(IOut<GameObject> arm)
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string bundlePath = Path.Combine(directoryPath, "scanner");
            var bundle = AssetBundle.LoadFromFile(bundlePath);
            var objectArray = bundle.LoadAllAssets();
            GameObject model = null;
            foreach (System.Object obj in objectArray)
            {
                if (obj.ToString().Contains("ScannerArm"))
                {
                    model = (GameObject)obj;
                }
            }
            model.AddComponent<ScannerArm>();
            Shader shader = Shader.Find("MarmosetUBER");
            foreach (var renderer in model.gameObject.GetComponentsInChildren<Renderer>(true))
            {
                foreach (Material mat in renderer.materials)
                {
                    // give it the marmo shader, no matter what
                    mat.shader = shader;
                }
            }
            arm.Set(model);
            bundle.Unload(false);
            yield break;
        }
        public override void OnAdded(AddActionParams param)
        {
        }
        public override void OnRemoved(AddActionParams param)
        {
        }
        public override bool OnArmDown(ArmActionParams param, out float cooldown)
        {
            param.arm.GetComponent<ScannerArm>().StartUsing();
            cooldown = 0;
            return true;
        }
        public override bool OnArmUp(ArmActionParams param, out float cooldown)
        {
            param.arm.GetComponent<ScannerArm>().StopUsing();
            cooldown = 0;
            return true;
        }
        public override bool OnArmHeld(ArmActionParams param, out float cooldown)
        {
            cooldown = 0;
            return false;
        }
        public override bool OnArmAltUse(ArmActionParams param)
        {
            return false;
        }
        public override void OnPilotExit(GameObject arm, Vehicle vehicle)
        {
            arm.GetComponent<ScannerArm>().StopUsing();
        }
    }
}
