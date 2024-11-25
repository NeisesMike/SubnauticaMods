using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace PersistentReaper
{
    public class ReaperBehavior
    {
        public Int3 currentRegion = Int3.zero;
        public bool isLockedOntoPlayer = false;

        public static bool IsValidTargetForPercy(GameObject Percy)
        {
            // Percy only hunts humans
            GameObject target = Player.main.gameObject;
            Vehicle thisPossibleVehicle = Player.main.currentMountedVehicle;

            //=============================================
            // Determine whether Percy can see or hear us.
            // His eyes are sharper than normal,
            // but his hearing is superb.
            //=============================================

            // First check if Percy can hear us.
            // Sound travels quite far underwater,
            // and doesn't require any specific facing.
            // TODO: configure this value
            // 90 is just a guess, based off of Stealth Module intel
            if (90 < Vector3.Distance(target.transform.position, Percy.transform.position))
            {
                // Too far away even to hear us.
                return false;
            }

            // determine what kind of sound the player is making
            // seamoth noise
            // cyclops noise
            // prawn noise
            // TODO: tool noise
            // TODO: for now, only vehicles make sufficient noise for Percy to hear
            // TODO: configure these constants
            bool isWithinEarshot = !Physics.Linecast(Percy.transform.position, target.transform.position, Voxeland.GetTerrainLayerMask());
            bool isPlayerLoud;

            if (thisPossibleVehicle)
            {
                if (Player.main.inSeamoth)
                {
                    isPlayerLoud = 6 < Player.main.GetComponent<Rigidbody>().velocity.magnitude;
                }
                else if (Player.main.inExosuit)
                {
                    isPlayerLoud = 2 < Player.main.GetComponent<Rigidbody>().velocity.magnitude;
                }
                else if (Player.main.currentSub)
                {
                    isPlayerLoud = 1 < Player.main.GetComponent<Rigidbody>().velocity.magnitude && !Player.main.currentSub.silentRunning;
                }
                else
                {
                    isPlayerLoud = false;
                }
            }
            else
            {
                isPlayerLoud = false;
            }

            if (isWithinEarshot && isPlayerLoud)
            {
                // Percy has heard us.
                return true;
            }

            // If Percy couldn't hear us,
            // there's still a chance he could see us,
            // and Percy can see extra far.
            // TODO: configure this value
            // 75 is just a guess, based off of Stealth Module intel
            if (75 < Vector3.Distance(target.transform.position, Percy.transform.position))
            {
                // Too far away to see us.
                return false;
            }
            if (Percy.GetComponent<Creature>().GetCanSeeObject(target))
            {
                // Percy has seen us.
                return true;
            }

            // If Persistent Percy failed to perceive us,
            // the target is invalid.
            return false;
        }
    }
}
