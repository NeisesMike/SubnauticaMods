using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SelfRepairModule
{
    internal class SelfRepairPermissionChecker : MonoBehaviour, IOnTakeDamage
    {
        private bool isNormal = true;
        private SelfRepairBehavior normalSRB = null;
        private CyclopsSelfRepairBehavior cyclopsSRB = null;
        private bool isToggled = false;
        private float timeLastDamaged = 0f;
        void IOnTakeDamage.OnTakeDamage(DamageInfo _)
        {
            timeLastDamaged = Time.time;
        }
        private void Update()
        {
            if (GameInput.GetButtonDown(MainPatcher.Instance.ToggleKey))
            {
                isToggled = !isToggled;
            }
        }
        internal bool IsRepairAvailable()
        {
            return
                IsToggledOrActive() &&
                IsSufficientlyPowered() &&
                IsNotStunned() &&
                IsNotCrushed();
        }
        private bool IsToggledOrActive()
        {
            switch (MainPatcher.MyConfig.repairType)
            {
                case RepairTypes.Passive:
                    return true;
                case RepairTypes.Toggle:
                    return isToggled;
                default:
                    return false;
            }
        }
        private bool IsSufficientlyPowered()
        {
            if (isNormal)
            {
                return MainPatcher.MyConfig.energyRequirement < GetComponent<Vehicle>().energyInterface.TotalCanProvide(out _);
            }
            else
            {
                return MainPatcher.MyConfig.energyRequirement < GetComponent<PowerRelay>().GetPower();
            }
        }
        private bool IsNotStunned()
        {
            return timeLastDamaged + MainPatcher.MyConfig.stunnedTime < Time.time;
        }
        private bool IsNotCrushed()
        {
            CrushDamage crushDamage = GetComponent<CrushDamage>();
            return MainPatcher.MyConfig.crushDepth
                || !crushDamage.GetCanTakeCrushDamage()
                || crushDamage.GetDepth() < crushDamage.crushDepth;
        }
        internal static SelfRepairPermissionChecker Create(SelfRepairRoot root)
        {
            var checker = root.gameObject.EnsureComponent<SelfRepairPermissionChecker>();
            root.GetComponent<LiveMixin>().damageReceivers = root.GetComponent<LiveMixin>().damageReceivers.Append(checker).ToArray();
            if (root is SelfRepairBehavior)
            {
                checker.normalSRB = root as SelfRepairBehavior;
                checker.isNormal = true;
            }
            else if(root is CyclopsSelfRepairBehavior)
            {
                checker.cyclopsSRB = root as CyclopsSelfRepairBehavior;
                checker.isNormal = false;
            }
            return checker;
        }
    }
}
