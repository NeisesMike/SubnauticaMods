using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using VehicleFramework;
using VehicleFramework.VehicleTypes;

namespace VFScannerArm
{
    [HarmonyPatch(typeof(GUIHand))]
    public class GUIHandPatcher
    {
        // This patch allows PDAScanner to UpdateTarget when the player is in a vehicle.
        // GUIHand.OnUpdate will do almost nothing if the player is in a vehicle
        // because Player.main.IsFreeToInteract is false.
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GUIHand.OnUpdate))]
        public static void GUIHandOnUpdatePostfix()
        {
            ModVehicle mv = Player.main?.GetVehicle() as ModVehicle;
            Exosuit exo = Player.main?.GetVehicle() as Exosuit;
            ScannerArm scannerArm = null;
            if (mv != null && mv.IsPlayerControlling())
            {
                var armsMan = mv.GetComponent<VehicleFramework.VehicleRootComponents.VFArmsManager>();
                scannerArm = armsMan?.leftArm?.GetComponent<ScannerArm>();
                if (scannerArm == null)
                {
                    scannerArm = armsMan?.rightArm?.GetComponent<ScannerArm>();
                }
            }
            if (exo != null && exo.GetPilotingMode())
            {
                scannerArm = exo.leftArm.GetGameObject().GetComponent<ScannerArm>();
                if (scannerArm == null)
                {
                    scannerArm = exo.rightArm.GetGameObject().GetComponent<ScannerArm>();
                }
            }
            if (scannerArm == null)
            {
                return;
            }
            PDAScanner.UpdateTarget(8f, false);
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            if (scanTarget.isValid && PDAScanner.CanScan(PDAScanner.scanTarget) == PDAScanner.Result.Scan)
            {
                uGUI_ScannerIcon.main.Show();
            }
        }
    }
}
