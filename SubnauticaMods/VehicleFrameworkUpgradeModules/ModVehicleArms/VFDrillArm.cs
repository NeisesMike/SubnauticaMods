﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using VehicleFramework;
using VehicleFramework.UpgradeTypes;

namespace VFDrillArm
{
    public class VFDrillArm : ModVehicleArm
    {
        public override string ClassId => "DrillArmVF";
        public override string DisplayName => "Drill Arm";
        public override List<VehicleFramework.Assets.Ingredient> Recipe => new List<VehicleFramework.Assets.Ingredient>()
                {
                    new VehicleFramework.Assets.Ingredient(TechType.Titanium, 5),
                    new VehicleFramework.Assets.Ingredient(TechType.Lithium, 1),
                    new VehicleFramework.Assets.Ingredient(TechType.Diamond, 4)
                };
        public override string Description => "A drill-arm attachment used for mining large ore veins";
        public override Atlas.Sprite Icon => VehicleFramework.Assets.SpriteHelper.GetSprite("VFDrillArmIcon.png");
        public override IEnumerator GetArmPrefab(IOut<GameObject> arm)
        {
            yield return UWE.CoroutineHost.StartCoroutine(PrawnHelper.EnsurePrawn());
            Exosuit myExo = PrawnHelper.Prawn.GetComponent<Exosuit>();
            GameObject result = null;
            for (int i = 0; i < myExo.armPrefabs.Length; i++)
            {
                if (myExo.armPrefabs[i].techType == TechType.ExosuitDrillArmModule)
                {
                    result = myExo.armPrefabs[i].prefab;
                    break;
                }
            }
            arm.Set(result);
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
            param.arm.GetComponent<Animator>().SetBool("use_tool", true);
            param.arm.GetComponent<ExosuitDrillArm>().drilling = true;
            cooldown = 0;
            return true;
        }
        public override bool OnArmUp(ArmActionParams param, out float cooldown)
        {
            if (!isToggled)
            {
                param.arm.GetComponent<Animator>().SetBool("use_tool", false);
                param.arm.GetComponent<ExosuitDrillArm>().drilling = false;
                param.arm.GetComponent<ExosuitDrillArm>().StopEffects();
                param.arm.GetComponent<VFXController>().Stop();
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
            param.arm.GetComponent<Animator>().SetBool("use_tool", isToggled);
            param.arm.GetComponent<ExosuitDrillArm>().drilling = isToggled;
            return false;
        }
        public override void OnPilotExit(GameObject arm, Vehicle vehicle)
        {
            arm.GetComponent<Animator>().SetBool("use_tool", false);
            arm.GetComponent<ExosuitDrillArm>().drilling = false;
            arm.GetComponent<ExosuitDrillArm>().StopEffects();
            arm.GetComponent<VFXController>().Stop();
        }
        private bool isToggled = false;
    }
}
