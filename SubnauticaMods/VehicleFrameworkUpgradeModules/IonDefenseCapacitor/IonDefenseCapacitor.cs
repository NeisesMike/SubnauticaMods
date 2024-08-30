using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using VehicleFramework;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;

namespace IonDefenseCapacitor
{
    public class IonDefenseCapacitor : SelectableUpgrade
    {
        public static float invincibilityDuration = 3f;
        public override string ClassId => "IonDefenseCapacitor";
        public override string DisplayName => "Ion Defense Capacitor";
        public override string Description => "Temporarily envelops the sub in an ion-charged shield, nullifying all damage for a short duration.";
        public override float Cooldown => 45f;
        public override float EnergyCost => 50f;
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.PrecursorIonPowerCell, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Polyaniline, 1),
                    new Ingredient(TechType.EnameledGlass, 2),
                    new Ingredient(TechType.Aerogel, 1)
                }; 

        public override Atlas.Sprite Icon => SpriteHelper.GetSprite("IonDefenseCapacitorIcon.png");
        public override void OnSelected(SelectableActionParams param)
        {
            FMODUWE.PlayOneShot("event:/tools/gravcannon/repulse", param.mv.transform.position, 1f);
            IEnumerator manageInvincibility(ModVehicle thisMV)
            {
                param.mv.GetComponent<LiveMixin>().invincible = true;
                yield return new WaitForSeconds(invincibilityDuration);
                param.mv.GetComponent<LiveMixin>().invincible = false;

            }
            Player.main.StartCoroutine(manageInvincibility(param.mv));
        }
        public override void OnAdded(AddActionParams param)
        {
        }
        public override void OnRemoved(AddActionParams param)
        {
        }
    }
}