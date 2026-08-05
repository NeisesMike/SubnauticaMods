using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;

namespace SonarModule
{
    public class SonarModule : ToggleableUpgrade
    {
        public const string SonarClassIDCore = "SonarModule";
        public override string ClassId => SonarClassIDCore;
        public override string DisplayName => Language.main.Get("SonarModule");
        public override string Description => Language.main.Get("Tooltip_SonarModule");
        public override float RepeatRate => MainPatcher.MyConfig.repeatRate;
        public override float TimeToFirstActivation => 0f;
        public override float EnergyCostPerActivation => MainPatcher.MyConfig.powerConsumption;
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.Magnetite, 2),
                    new Ingredient(TechType.Titanium, 1),
                    new Ingredient(TechType.Glass, 1)
                };

        public override UnityEngine.Sprite Icon => SpriteHelper.GetSprite("SonarModuleIcon.png");

        public override void OnAdded(AddActionParams param)
        {
        }
        public override void OnRemoved(AddActionParams param)
        {
        }

        public override void OnRepeat(ToggleActionParams param)
        {
            SNCameraRoot.main.SonarPing();
            FMODUWE.PlayOneShot("event:/sub/seamoth/sonar_loop", param.vehicle.transform.position, 1f);
        }
    }
}