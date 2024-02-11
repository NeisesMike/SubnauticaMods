﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using Nautilus.Options;
using Nautilus.Handlers;
using System.Runtime.CompilerServices;
using System.Collections;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Utility;
using Nautilus.Patchers;
using System.Threading;
using System.Collections.Concurrent;

namespace AttitudeIndicator
{
    public enum VehicleType
    {
        Seamoth,
        Cyclops,
        Seatruck,
        Snowfox,
        None
    }

    public class AttitudeIndicator : MonoBehaviour
    {
        public static GameObject prefab = null;
        public VehicleType type = VehicleType.None;
        public GameObject model;
        public GameObject aircraft;
        public GameObject chassi;
        public GameObject frame;
        public GameObject globe;
        public GameObject ring;
        public static void GetAssets()
        {
            // load the asset bundle
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, "assets/attitudeindicator"));
            if (myLoadedAssetBundle == null)
            {
                AILogger.Log("Failed to load AssetBundle!");
                return;
            }

            System.Object[] arr = myLoadedAssetBundle.LoadAllAssets();
            foreach (System.Object obj in arr)
            {
                if (obj.ToString().Contains("attitudeindicator"))
                {
                    prefab = (GameObject)obj;
                }
            }
        }

        public void Start()
        {
            aircraft = model.transform.Find("Aircraft").gameObject;
            chassi = model.transform.Find("Chassi").gameObject;
            frame = model.transform.Find("Frame").gameObject;
            globe = model.transform.Find("Globe").gameObject;
            ring = model.transform.Find("Ring").gameObject;
        }
        public void Update()
        {
            UpdatePosition();
            UpdateRotations();
        }

        public void UpdatePosition()
        {
#if SUBNAUTICA
            void PositionSeamothAI()
            {
                model.transform.localScale = Vector3.one * AttitudeIndicatorPatcher.SubnauticaConfig.scale;
                model.transform.position = Player.main.transform.position
                    + Player.main.transform.forward * AttitudeIndicatorPatcher.SubnauticaConfig.z
                    + Player.main.transform.right * AttitudeIndicatorPatcher.SubnauticaConfig.x
                    + Player.main.transform.up * AttitudeIndicatorPatcher.SubnauticaConfig.y;
            }
            void PositionCyclopsAI()
            {
                model.transform.localScale = Vector3.one * AttitudeIndicatorPatcher.SubnauticaConfig.Cscale;
                model.transform.position = Player.main.transform.position
                    + Player.main.transform.forward * AttitudeIndicatorPatcher.SubnauticaConfig.Cz
                    + Player.main.transform.right * AttitudeIndicatorPatcher.SubnauticaConfig.Cx
                    + Player.main.transform.up * AttitudeIndicatorPatcher.SubnauticaConfig.Cy;
            }
#elif BELOWZERO
            void PositionSnowfoxAI()
            {
                model.transform.localScale = Vector3.one * AttitudeIndicatorPatcher.SubnauticaConfig.Fscale;
                model.transform.position = Player.main.transform.position
                    + Player.main.transform.forward * AttitudeIndicatorPatcher.SubnauticaConfig.Fz
                    + Player.main.transform.right * AttitudeIndicatorPatcher.SubnauticaConfig.Fx
                    + Player.main.transform.up * AttitudeIndicatorPatcher.SubnauticaConfig.Fy;
            }
            void PositionSeatruckAI()
            {
                model.transform.localScale = Vector3.one * AttitudeIndicatorPatcher.SubnauticaConfig.Tscale;
                model.transform.position = Player.main.transform.position
                    + Player.main.transform.forward * AttitudeIndicatorPatcher.SubnauticaConfig.Tz
                    + Player.main.transform.right * AttitudeIndicatorPatcher.SubnauticaConfig.Tx
                    + Player.main.transform.up * AttitudeIndicatorPatcher.SubnauticaConfig.Ty;
            }
#endif
            switch (type)
            {
#if SUBNAUTICA
                case VehicleType.Seamoth:
                    PositionSeamothAI();
                    break;
                case VehicleType.Cyclops:
                    PositionCyclopsAI();
                    break;
#elif BELOWZERO
                case VehicleType.Snowfox:
                    PositionSnowfoxAI();
                    break;
                case VehicleType.Seatruck:
                    PositionSeatruckAI();
                    break;
#endif
                default:
                    AILogger.Log("ERROR: this AI had no vehicle type");
                    break;
            }

        }

        public void UpdateRotations()
        {
            float playerPitch = MainCameraControl.main.transform.eulerAngles.x;
            float playerRoll = MainCameraControl.main.transform.eulerAngles.z;

            model.transform.LookAt(MainCamera.camera.transform, MainCamera.camera.transform.up);

            var but = ring.transform.localEulerAngles;
            but.z = playerRoll - 90;
            ring.transform.localEulerAngles = but;

            var fart = globe.transform.localEulerAngles;
            fart.z = playerPitch - 90;
            globe.transform.localEulerAngles = fart;
        }

    }
}

