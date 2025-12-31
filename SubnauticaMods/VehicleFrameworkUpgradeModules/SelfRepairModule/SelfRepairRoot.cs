using System.Collections;
using UnityEngine;
using Nautilus.Extensions;

namespace SelfRepairModule
{
    internal abstract class SelfRepairRoot : MonoBehaviour
    {
        internal SelfRepairPermissionChecker checker;
        internal FMODASRPlayer weldSound;
        private bool lastIsRepairing = false;
        internal abstract float GetRepairAmount();
        internal abstract float GetRepairEnergyCost();
        internal abstract bool IsReady();
        internal abstract void SpendEnergy(float energyCost);
        internal abstract void DoRepair(float repairAmount);
        internal abstract float GetAffordableRepair(float potentialRepair);
        internal float ConvertPowerToRepair(float power)
        {
            return Mathf.Min(GetRepairAmount(), GetRepairAmount() * (power / GetRepairEnergyCost()));
        }
        internal float ConvertRepairToPower(float repairValue)
        {
            return Mathf.Min(GetRepairEnergyCost(), GetRepairEnergyCost() * (repairValue / GetRepairAmount()));
        }
        internal virtual void Awake()
        {
            checker = SelfRepairPermissionChecker.Create(this);
        }
        private IEnumerator Start()
        {
            yield return GetWelderSound(gameObject, this);
        }
        private void Update()
        {
            if (checker == null) return;
            if (IsReady() && checker.IsRepairAvailable())
            {
                float amountToRepair = GetAffordableRepair(GetRepairAmount()) * Time.deltaTime;
                SpendEnergy(ConvertRepairToPower(amountToRepair));
                DoRepair(amountToRepair);
                if (!lastIsRepairing && MainPatcher.MyConfig.repairSound)
                {
                    weldSound?.Play();
                }
                lastIsRepairing = true;
            }
            else
            {
                if (lastIsRepairing && MainPatcher.MyConfig.repairSound)
                {
                    weldSound?.Stop();
                }
                lastIsRepairing = false;
            }
        }
        private static IEnumerator GetWelderSound(GameObject parent, SelfRepairRoot repair)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.Welder);
            yield return task;
            GameObject gameObjectPrefab = task.GetResult();
            FMODASRPlayer orgWeldSound = gameObjectPrefab.GetComponent<Welder>().weldSound;

            repair.weldSound = parent.AddComponent<FMODASRPlayer>();
            FMOD_StudioEventEmitter startSound = parent.AddComponent<FMOD_StudioEventEmitter>();
            startSound.CopyComponent(orgWeldSound.startLoopSound);

            repair.weldSound.startLoopSound = startSound;

            yield return null;
            if (repair.lastIsRepairing) // make sure it plays even on first-add
            {
                repair.weldSound.Play();
            }
        }
        internal void SetEnabled(bool enable)
        {
            if (enable == enabled) return;
            if (!enable)
            {
                lastIsRepairing = false;
                weldSound?.Stop();
            }
            enabled = enable;
        }
    }
}
