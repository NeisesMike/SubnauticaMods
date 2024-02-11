using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AttitudeIndicator
{
    /* The IndicatorManager has control passed to it via a postfix patch on Player.Update
     * The IndicatorManager controls the creation and destruction of the instrument.
     */
    public static class IndicatorManager
    {
        public static AttitudeIndicator main;
        public static GameObject mainGO;
        public static VehicleType TypeLastFrame = VehicleType.None;
        public static void Die()
        {
            if(mainGO != null)
            {
                GameObject.Destroy(mainGO);
            }
            if(main != null)
            {
                Component.Destroy(main);
            }
            mainGO = null;
            main = null;
        }
        public static bool CheckShouldDie()
        {
#if SUBNAUTICA
            if (AttitudeIndicatorPatcher.SubnauticaConfig.isCyclopsAttitudeIndicatorOn || AttitudeIndicatorPatcher.SubnauticaConfig.isSeamothAttitudeIndicatorOn)
            {
                return false;
            }
#elif BELOWZERO
            if (AttitudeIndicatorPatcher.SubnauticaConfig.isSeatruckAttitudeIndicatorOn || AttitudeIndicatorPatcher.SubnauticaConfig.isSnowfoxAttitudeIndicatorOn)
            {
                return false;
            }
#endif
            return true;
        }
        public static VehicleType GetCurrentVehicleType()
        {
#if SUBNAUTICA
            if(Player.main.inSeamoth)
            {
                return VehicleType.Seamoth;
            }
            if(Player.main.isPiloting && Player.main.IsInSubmarine)
            {
                return VehicleType.Cyclops;
            }
#elif BELOWZERO
            if (Player.main.inHovercraft)
            {
                return VehicleType.Snowfox;
            }
            if (Player.main.inSeatruckPilotingChair)
            {
                return VehicleType.Seatruck;
            }
#endif
            if(Player.main.isPiloting && Player.main.currentMountedVehicle && !Player.main.currentMountedVehicle.name.Contains("Exosuit"))
            {
                // This should catch ModVehicles from Vehicle Framework.
                return VehicleType.Seamoth;
            }
            return VehicleType.None;
        }
        public static void EnsureAI(VehicleType type)
        {
            if (main == null && mainGO == null)
            {
                mainGO = GameObject.Instantiate(AttitudeIndicator.prefab);
                mainGO.SetActive(false);
                main = mainGO.AddComponent<AttitudeIndicator>();
                main.model = mainGO;
                main.type = type;
                mainGO.SetActive(true);
            }
        }
        public static void DoUpdate()
        {
            if (AttitudeIndicator.prefab == null)
            {
                AttitudeIndicator.GetAssets();
            }
            if (CheckShouldDie())
            {
                Die();
                return;
            }
            VehicleType type = GetCurrentVehicleType();
            switch (type)
            {
#if SUBNAUTICA
                case VehicleType.Seamoth:
                    if (AttitudeIndicatorPatcher.SubnauticaConfig.isSeamothAttitudeIndicatorOn)
                    {
                        EnsureAI(VehicleType.Seamoth);
                    }
                    else
                    {
                        Die();
                    }
                    break;
                case VehicleType.Cyclops:
                    if (AttitudeIndicatorPatcher.SubnauticaConfig.isCyclopsAttitudeIndicatorOn)
                    {
                        EnsureAI(VehicleType.Cyclops);
                    }
                    else
                    {
                        Die();
                    }
                    break;
#elif BELOWZERO
                case VehicleType.Snowfox:
                    if (AttitudeIndicatorPatcher.SubnauticaConfig.isSnowfoxAttitudeIndicatorOn)
                    {
                        EnsureAI(VehicleType.Snowfox);
                    }
                    else
                    {
                        Die();
                    }
                    break;
                case VehicleType.Seatruck:
                    if (AttitudeIndicatorPatcher.SubnauticaConfig.isSeatruckAttitudeIndicatorOn)
                    {
                        EnsureAI(VehicleType.Seatruck);
                    }
                    else
                    {
                        Die();
                    }
                    break;
#endif
                default:
                    Die();
                    break;
            }
        }
    }
}
