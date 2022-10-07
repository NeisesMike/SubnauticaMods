using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using LitJson;
using System.Runtime.CompilerServices;
using System.Collections;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Utility;
using SMLHelper.V2.Patchers;
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
                Logger.Log("Failed to load AssetBundle!");
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
            model.transform.localScale = Vector3.one * AttitudeIndicatorPatcher.SubnauticaConfig.scale;
            model.transform.position = Player.main.transform.position
                + Player.main.transform.forward * AttitudeIndicatorPatcher.SubnauticaConfig.z
                + Player.main.transform.right * AttitudeIndicatorPatcher.SubnauticaConfig.x
                + Player.main.transform.up * AttitudeIndicatorPatcher.SubnauticaConfig.y;
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

