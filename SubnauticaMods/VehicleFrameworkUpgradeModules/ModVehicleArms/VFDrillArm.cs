using System.Collections.Generic;
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
        public override string TabName => "MVCM";
        public override IEnumerator GetArmPrefab(IOut<GameObject> arm)
        {
            yield return UWE.CoroutineHost.StartCoroutine(PrawnHelper.EnsurePrawn());
            arm.Set(PrawnHelper.Prawn.GetComponent<Exosuit>().GetArmPrefab(TechType.ExosuitDrillArmModule));
        }
        public override void OnAdded(AddActionParams param)
        {
        }
        public override void OnRemoved(AddActionParams param)
        {
        }
        public override void OnSelected(SelectableActionParams param)
        {
        }
        public override void OnArmSelected(ArmActionParams param)
        {
            if (param.arm.GetComponent<Animator>().GetBool("use_tool"))
            {
                param.arm.GetComponent<Animator>().SetBool("use_tool", false);
                param.arm.GetComponent<ExosuitDrillArm>().drilling = false;
                param.arm.GetComponent<ExosuitDrillArm>().StopEffects();
                param.arm.GetComponent<VFXController>().Stop();
            }
            else
            {
                param.arm.GetComponent<Animator>().SetBool("use_tool", true);
                param.arm.GetComponent<ExosuitDrillArm>().drilling = true;
            }
        }
    }
}
