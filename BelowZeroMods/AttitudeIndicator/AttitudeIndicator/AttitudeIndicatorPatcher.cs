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
using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;

namespace AttitudeIndicator
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[AttitudeIndicator] " + message);
        }

#if BELOWZERO
        public static void Output(string msg)
        {
            Hint main = Hint.main;
            if (main == null)
            {
                return;
            }
            uGUI_PopupMessage message = main.message;
            message.ox = 60f;
            message.oy = 0f;
            message.anchor = TextAnchor.MiddleRight;
            message.SetBackgroundColor(new Color(1f, 1f, 1f, 1f));
            string myMessage = msg;
            message.SetText(myMessage, TextAnchor.MiddleRight);
            message.Show(3f, 0f, 0.25f, 0.25f, null);
        }
#endif
    }

    public enum IndicatorStyle
    {
        Standard,
        Blue
    }

#if SUBNAUTICA
    [Menu("Attitude Indicators")]
    public class SubnauticaConfig : ConfigFile
    {
        public enum autoposition
        {
            bottomcenter,
            bottomleft,
            bottomright,
            topleft,
            topright
        }

        [Toggle("Enable Seamoth Attitude Indicator"), OnChange(nameof(ToggleSeamothIndicator))]
        public bool isSeamothAttitudeIndicatorOn = true;

        [Slider("Position X", -1, 1f, Step = 0.0001f)]
        public float x = -0.4039f;
        [Slider("Position Y", -1f, 1f, Step = 0.0001f)]
        public float y = -0.2164f;
        [Slider("Position Z", 0f, 1f, Step = 0.0001f)]
        public float z = 0.6145f;
        [Slider("Scale", 0f, 0.1853f, Step = 0.00001f)]
        public float scale = 0.1102f;

        public void ToggleSeamothIndicator(ToggleChangedEventArgs e)
        {
            if (e.Value && Player.main.isPiloting && Player.main.currentMountedVehicle && !Player.main.currentMountedVehicle.name.Contains("Exosuit"))
            {
                if (AttitudeIndicator.prefab is null)
                {
                    AttitudeIndicator.GetAssets();
                }
                PlayerUpdatePatcher2.indicator = GameObject.Instantiate(AttitudeIndicator.prefab);
                PlayerUpdatePatcher2.indicator.AddComponent<AttitudeIndicator>().model = PlayerUpdatePatcher2.indicator;
            }
            else if (!e.Value)
            {
                if (PlayerUpdatePatcher2.indicator)
                {
                    GameObject.Destroy(PlayerUpdatePatcher2.indicator);
                    PlayerUpdatePatcher2.indicator = null;
                }
            }
        }
        [Toggle("Use Seamoth Auto-position")]
        public bool useSeamothAutoPos = true;

        [Choice("Auto-position"), OnChange(nameof(moveSeamothIndicator))]
        public autoposition seamothIndicatorLocation = autoposition.bottomleft;
        public void moveSeamothIndicator(ChoiceChangedEventArgs e)
        {
            if (!useSeamothAutoPos)
            {
                return;
            }
            switch ((autoposition)e.Index)
            {
                case autoposition.bottomleft:
                    x = -.4097f;
                    y = -.2164f;
                    z = 0.6145f;
                    scale = 0.1102f;
                    break;
                case autoposition.bottomcenter:
                    x = 0f;
                    y = -0.1268f;
                    z = 0.3779f;
                    scale = 0.06f;
                    break;
                case autoposition.bottomright:
                    x = 0.4039f;
                    y = -.2164f;
                    z = 0.6145f;
                    scale = 0.1102f;
                    break;
                case autoposition.topleft:
                    x = -0.5371f;
                    y = 0.3607f;
                    z = 0.6145f;
                    scale = 0.1232f;
                    break;
                case autoposition.topright:
                    x = 0.5371f;
                    y = 0.3607f;
                    z = 0.6145f;
                    scale = 0.1232f;
                    break;
            }
        }



        [Toggle("Enable Cyclops Attitude Indicator"), OnChange(nameof(ToggleCyclopsIndicator))]
        public bool isCyclopsAttitudeIndicatorOn = true;

        [Slider("Cyclops Position X", -1, 1f, Step = 0.0001f)]
        public float Cx = -0.4039f;
        [Slider("Cyclops Position Y", -1f, 1f, Step = 0.0001f)]
        public float Cy = -0.2164f;
        [Slider("Cyclops Position Z", 0f, 1f, Step = 0.0001f)]
        public float Cz = 0.6145f;
        [Slider("Cyclops Scale", 0f, 0.1853f, Step = 0.00001f)]
        public float Cscale = 0.1102f;

        public void ToggleCyclopsIndicator(ToggleChangedEventArgs e)
        {
            if (e.Value && Player.main.isPiloting && Player.main.currentMountedVehicle && !Player.main.currentMountedVehicle.name.Contains("Exosuit"))
            {
                if (AttitudeIndicator.prefab is null)
                {
                    AttitudeIndicator.GetAssets();
                }
                PlayerUpdatePatcher2.indicator = GameObject.Instantiate(AttitudeIndicator.prefab);
                PlayerUpdatePatcher2.indicator.AddComponent<AttitudeIndicator>().model = PlayerUpdatePatcher2.indicator;
            }
            else if (!e.Value)
            {
                if (PlayerUpdatePatcher2.indicator)
                {
                    GameObject.Destroy(PlayerUpdatePatcher2.indicator);
                    PlayerUpdatePatcher2.indicator = null;
                }
            }
        }
        [Toggle("Use Cyclops Auto-position")]
        public bool useCyclopsAutoPos = true;

        [Choice("Auto-position"), OnChange(nameof(moveCyclopsIndicator))]
        public autoposition cyclopsIndicatorLocation = autoposition.bottomleft;
        public void moveCyclopsIndicator(ChoiceChangedEventArgs e)
        {
            if(!useCyclopsAutoPos)
            {
                return;
            }
            switch ((autoposition)e.Index)
            {
                case autoposition.bottomleft:
                    Cx = -.4097f;
                    Cy = -.2164f;
                    Cz = 0.6145f;
                    Cscale = 0.1102f;
                    break;
                case autoposition.bottomcenter:
                    Cx = 0f;
                    Cy = -0.1268f;
                    Cz = 0.3779f;
                    Cscale = 0.06f;
                    break;
                case autoposition.bottomright:
                    Cx = 0.4039f;
                    Cy = -.2164f;
                    Cz = 0.6145f;
                    Cscale = 0.1102f;
                    break;
                case autoposition.topleft:
                    Cx = -0.5371f;
                    Cy = 0.3607f;
                    Cz = 0.6145f;
                    Cscale = 0.1232f;
                    break;
                case autoposition.topright:
                    Cx = 0.5371f;
                    Cy = 0.3607f;
                    Cz = 0.6145f;
                    Cscale = 0.1232f;
                    break;
            }
        }





    }
#elif BELOWZERO
    [Menu("Attitude Indicators")]
    public class BelowZeroConfig : ConfigFile
    {
        List<GameObject> seatruckOptions = new List<GameObject>();
        List<GameObject> snowfoxOptions = new List<GameObject>();

        [Choice("Indicator Style"), OnChange(nameof(changeIndicatorStyle))]
        public IndicatorStyle chosenStyle = IndicatorStyle.Standard;

        private const string updateIntervalTooltip = "This defines the minimum number of seconds the indicator will wait between updates.";
        [Slider("Update Interval", Min = 0, Max = 1, DefaultValue = 0, Step = 0.001f, Tooltip = updateIntervalTooltip)]
        public float updateInterval = 0;

        [Toggle("Enable Seatruck Attitude Indicator"), OnChange(nameof(setVisibilitySeatruck)), OnGameObjectCreated(nameof(setToggleActiveSeatruck))]
        public bool TisAttitudeIndicatorOn = true;
        [Slider("Position X", -0.3f, 0.3f, Step = 0.00001f), OnGameObjectCreated(nameof(grabGameObjectSeatruck))]
        public float Tx = -0.2273f;
        [Slider("Position Y", -.03f, 0.25f, Step = 0.00001f), OnGameObjectCreated(nameof(grabGameObjectSeatruck))]
        public float Ty = 0f;
        [Slider("Scale", 0.005f, 0.02f, Step = 0.000001f), OnGameObjectCreated(nameof(grabGameObjectSeatruck))]
        public float Tscale = 0.0093f;

        [Toggle("Enable Snowfox Attitude Indicator"), OnChange(nameof(setVisibilitySnowfox)), OnGameObjectCreated(nameof(setToggleActiveSnowfox))]
        public bool FisAttitudeIndicatorOn = true;
        [Slider("Position X", -0.16f, 0.16f, Step = 0.00001f), OnGameObjectCreated(nameof(grabGameObjectSnowfox))]
        public float Fx = -0.0766f;
        [Slider("Position Y", -0.06f, 0.16f, Step = 0.00001f), OnGameObjectCreated(nameof(grabGameObjectSnowfox))]
        public float Fy = 0f;
        [Slider("Scale", 0.0016f, 0.009f, Step = 0.000001f), OnGameObjectCreated(nameof(grabGameObjectSnowfox))]
        public float Fscale = 0.0047f;

        public void setVisibilitySeatruck(ToggleChangedEventArgs e)
        {
            seatruckOptions.RemoveAll(x => !x);
            foreach (GameObject option in seatruckOptions)
            {
                option.SetActive(e.Value);
            }
        }
        public void setVisibilitySnowfox(ToggleChangedEventArgs e)
        {
            snowfoxOptions.RemoveAll(x => !x);
            foreach (GameObject option in snowfoxOptions)
            {
                option.SetActive(e.Value);
            }
        }

        public void setToggleActiveSeatruck(GameObjectCreatedEventArgs e)
        {
            bool correctVehicle = AttitudeIndicatorPatcher.currentVehicle == VehicleType.Seatruck;
            if (correctVehicle)
            {
                seatruckOptions.RemoveAll(x => !x);
                foreach (GameObject option in seatruckOptions)
                {
                    option.SetActive(TisAttitudeIndicatorOn);
                }
            }
            else
            {
                e.GameObject.SetActive(false);
                seatruckOptions.RemoveAll(x => !x);
                foreach (GameObject option in seatruckOptions)
                {
                    option.SetActive(false);
                }
            }
        }
        public void setToggleActiveSnowfox(GameObjectCreatedEventArgs e)
        {
            bool correctVehicle = AttitudeIndicatorPatcher.currentVehicle == VehicleType.Snowfox;
            if (correctVehicle)
            {
                snowfoxOptions.RemoveAll(x => !x);
                foreach (GameObject option in snowfoxOptions)
                {
                    option.SetActive(FisAttitudeIndicatorOn);
                }
            }
            else
            {
                e.GameObject.SetActive(false);
                snowfoxOptions.RemoveAll(x => !x);
                foreach (GameObject option in snowfoxOptions)
                {
                    option.SetActive(false);
                }
            }
        }

        public void grabGameObjectSeatruck(GameObjectCreatedEventArgs e)
        {
            seatruckOptions.Add(e.GameObject);
            e.GameObject.SetActive(AttitudeIndicatorPatcher.currentVehicle == VehicleType.Seatruck && TisAttitudeIndicatorOn);
        }
        public void grabGameObjectSnowfox(GameObjectCreatedEventArgs e)
        {
            snowfoxOptions.Add(e.GameObject);
            e.GameObject.SetActive(AttitudeIndicatorPatcher.currentVehicle == VehicleType.Snowfox && FisAttitudeIndicatorOn);
        }

        public void changeIndicatorStyle(ChoiceChangedEventArgs e)
        {
            AttitudeIndicator.killAttitudeIndicator();
            AttitudeIndicator.initAttitudeIndicatorSprites((IndicatorStyle)e.Index);
        }
    }
#endif

    [BepInPlugin("com.mikjaw.subnautica.attitudeindicator.mod", "AttitudeIndicator", "2.0")]
    public class AttitudeIndicatorPatcher : BaseUnityPlugin
    {
#if SUBNAUTICA
        internal static SubnauticaConfig SubnauticaConfig { get; private set; }
#elif BELOWZERO
        internal static BelowZeroConfig BelowZeroConfig { get; private set; }
#endif
        internal static VehicleType currentVehicle;

        public void Start()
        {
#if SUBNAUTICA
            SubnauticaConfig = OptionsPanelHandler.Main.RegisterModOptions<SubnauticaConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.attitudeindicator.mod");
#elif BELOWZERO
            BelowZeroConfig = OptionsPanelHandler.Main.RegisterModOptions<BelowZeroConfig>();
            var harmony = new Harmony("com.mikjaw.belowzero.attitudeindicator.mod");
            AttitudeIndicator.initAttitudeIndicatorSprites(BelowZeroConfig.chosenStyle);
#endif
            harmony.PatchAll();
        }
    }
}

