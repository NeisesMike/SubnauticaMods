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
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.TitaniumIngot, 1),
                    new Ingredient(TechType.PowerCell, 1)
                };
        public override string Description => "A vehicle-mounted arm version of the spectroscope scanner used to acquire technology blueprints and data on living organisms.";
        public override Sprite Icon => VehicleFramework.Assets.SpriteHelper.GetSprite("VFScannerArmIcon.png");
        public override TechType UnlockWith => TechType.Fragment;
        public override bool UnlockAtStart => false;
        public override Sprite UnlockedSprite => VehicleFramework.Assets.SpriteHelper.GetSprite("ScannerArmPopUp.png");
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
            isToggled = false;
            param.arm.GetComponent<ScannerArm>().StartUsing();
            cooldown = 0;
            return true;
        }
        public override bool OnArmUp(ArmActionParams param, out float cooldown)
        {
            if (!isToggled)
            {
                param.arm.GetComponent<ScannerArm>().StopUsing();
            }
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
            isToggled = !isToggled;
            if(isToggled)
            {
                param.arm.GetComponent<ScannerArm>().StartUsing();
            }
            else
            {
                param.arm.GetComponent<ScannerArm>().StopUsing();
            }
            return false;
        }
        public override void OnPilotExit(GameObject arm, Vehicle vehicle)
        {
            arm.GetComponent<ScannerArm>().StopUsing();
        }
        private bool isToggled = false;
    }
}
