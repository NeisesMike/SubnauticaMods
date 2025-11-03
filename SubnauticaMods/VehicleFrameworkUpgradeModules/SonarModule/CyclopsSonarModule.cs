using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;

namespace SonarModule
{
    public class CyclopsSonarModule : ModVehicleUpgrade
    {
        public const string SonarClassIDCore = "VFSonarModule";
        public override string ClassId => SonarClassIDCore;
        public override string DisplayName => "Sonar Module";
        public override string Description => "A dedicated system for detecting and displaying topographical data on the HUD.";
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
        public override void OnCyclops(AddActionParams param)
        {
        }
    }
}