using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Utility;
using Nautilus.Patchers;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;

namespace AttitudeIndicator
{
    /* These menu options allow position and size configuration of indicator on a per-vehicle basis
     */
    [Menu("Attitude Indicators")]
    public class SubnauticaConfig : ConfigFile
    {
        /*
        public enum Autoposition
        {
            bottomcenter,
            bottomleft,
            bottomright,
            topleft,
            topright
        }
        */

        private const string updateIntervalTooltip = "This defines the minimum number of seconds the indicator will wait between updates.";
        [Slider("Update Interval", Min = 0, Max = 1, DefaultValue = 0, Step = 0.001f, Tooltip = updateIntervalTooltip)]
        public float updateInterval = 0;

#if SUBNAUTICA
        #region seamoth
        [Toggle("Seamoth: Enable Attitude Indicator")]
        public bool isSeamothAttitudeIndicatorOn = true;

        [Slider("Seamoth: Position X", -1, 1f, Step = 0.0001f)]
        public float x = -0.4039f;
        [Slider("Seamoth: Position Y", -1f, 1f, Step = 0.0001f)]
        public float y = -0.2164f;
        [Slider("Seamoth: Position Z", 0f, 1f, Step = 0.0001f)]
        public float z = 0.6145f;
        [Slider("Seamoth: Scale", 0f, 0.1853f, Step = 0.00001f)]
        public float scale = 0.1102f;
        [Toggle("Seamoth: Use Auto-position")]
        public bool useSeamothAutoPos = true;

        [Choice("Seamoth: Auto-position"), OnChange(nameof(moveSeamothIndicator))]
        public Autoposition seamothIndicatorLocation = Autoposition.bottomleft;
        public void moveSeamothIndicator(ChoiceChangedEventArgs<Autoposition> e)
        {
            if (!useSeamothAutoPos)
            {
                return;
            }
            switch ((Autoposition)e.Index)
            {
                case Autoposition.bottomleft:
                    x = -.4097f;
                    y = -.2164f;
                    z = 0.6145f;
                    scale = 0.1102f;
                    break;
                case Autoposition.bottomcenter:
                    x = 0f;
                    y = -0.1268f;
                    z = 0.3779f;
                    scale = 0.06f;
                    break;
                case Autoposition.bottomright:
                    x = 0.4039f;
                    y = -.2164f;
                    z = 0.6145f;
                    scale = 0.1102f;
                    break;
                case Autoposition.topleft:
                    x = -0.5371f;
                    y = 0.3607f;
                    z = 0.6145f;
                    scale = 0.1232f;
                    break;
                case Autoposition.topright:
                    x = 0.5371f;
                    y = 0.3607f;
                    z = 0.6145f;
                    scale = 0.1232f;
                    break;
            }
        }

        #endregion
        #region cyclops

        [Toggle("Cyclops: Enable Attitude Indicator"), OnChange(nameof(ToggleCyclopsIndicator))]
        public bool isCyclopsAttitudeIndicatorOn = true;

        [Slider("Cyclops: Position X", -1, 1f, Step = 0.0001f)]
        public float Cx = -0.4039f;
        [Slider("Cyclops: Position Y", -1f, 1f, Step = 0.0001f)]
        public float Cy = -0.2164f;
        [Slider("Cyclops: Position Z", 0f, 1f, Step = 0.0001f)]
        public float Cz = 0.6145f;
        [Slider("Cyclops: Scale", 0f, 0.1853f, Step = 0.00001f)]
        public float Cscale = 0.1102f;
        [Toggle("Cyclops: Use Auto-position")]
        public bool useCyclopsAutoPos = true;

        [Choice("Cyclops: Auto-position"), OnChange(nameof(moveCyclopsIndicator))]
        public Autoposition cyclopsIndicatorLocation = Autoposition.bottomleft;
        public void moveCyclopsIndicator(ChoiceChangedEventArgs<Autoposition> e)
        {
            if(!useCyclopsAutoPos)
            {
                return;
            }
            switch ((Autoposition)e.Index)
            {
                case Autoposition.bottomleft:
                    Cx = -.4097f;
                    Cy = -.2164f;
                    Cz = 0.6145f;
                    Cscale = 0.1102f;
                    break;
                case Autoposition.bottomcenter:
                    Cx = 0f;
                    Cy = -0.1268f;
                    Cz = 0.3779f;
                    Cscale = 0.06f;
                    break;
                case Autoposition.bottomright:
                    Cx = 0.4039f;
                    Cy = -.2164f;
                    Cz = 0.6145f;
                    Cscale = 0.1102f;
                    break;
                case Autoposition.topleft:
                    Cx = -0.5371f;
                    Cy = 0.3607f;
                    Cz = 0.6145f;
                    Cscale = 0.1232f;
                    break;
                case Autoposition.topright:
                    Cx = 0.5371f;
                    Cy = 0.3607f;
                    Cz = 0.6145f;
                    Cscale = 0.1232f;
                    break;
            }
        }
        #endregion
#elif BELOWZERO
        #region seatruck
        [Toggle("Seatruck: Enable Attitude Indicator")]
        public bool isSeatruckAttitudeIndicatorOn = true;
        [Slider("Seatruck: Position X", -0.4f, 0.4f, Step = 0.00001f)]
        public float Tx = -0.2655f;
        [Slider("Seatruck: Position Y", -.03f, 0.25f, Step = 0.00001f)]
        public float Ty = -0.03f;
        [Slider("Seatruck: Position Z", 0f, 1f, Step = 0.0001f)]
        public float Tz = 0.2204f;
        [Slider("Seatruck: Scale", 0.01f, 0.2f, Step = 0.00001f)]
        public float Tscale = 0.0415f;
        
        /*
                [Toggle("Seatruck: Use Auto-position")]
                public bool useSeatruckAutoPosition = true;

                [Choice("Seatruck: Auto-position"), OnChange(nameof(moveSeatruckIndicator))]
                public Autoposition seatruckIndicatorLocation = Autoposition.bottomleft;
                public void moveSeatruckIndicator<T>(ChoiceChangedEventArgs<T> e)
                {
                    if (!useSeatruckAutoPosition)
                    {
                        return;
                    }
                    switch ((Autoposition)e.Index)
                    {
                        case Autoposition.bottomleft:
                            Tx = -.4097f;
                            Ty = -.2164f;
                            Tz = 0.6145f;
                            Tscale = 0.1102f;
                            break;
                        case Autoposition.bottomcenter:
                            Tx = 0f;
                            Ty = -0.1268f;
                            Tz = 0.3779f;
                            Tscale = 0.06f;
                            break;
                        case Autoposition.bottomright:
                            Tx = 0.4039f;
                            Ty = -.2164f;
                            Tz = 0.6145f;
                            Tscale = 0.1102f;
                            break;
                        case Autoposition.topleft:
                            Tx = -0.5371f;
                            Ty = 0.3607f;
                            Tz = 0.6145f;
                            Tscale = 0.1232f;
                            break;
                        case Autoposition.topright:
                            Tx = 0.5371f;
                            Ty = 0.3607f;
                            Tz = 0.6145f;
                            Tscale = 0.1232f;
                            break;
                    }
                }
        */
        #endregion
        #region snowfox
        [Toggle("Snowfox: Enable Attitude Indicator")]
        public bool isSnowfoxAttitudeIndicatorOn = true;
        [Slider("Snowfox: Position X", -0.16f, 0.16f, Step = 0.00001f)]
        public float Fx = -0.16f;
        [Slider("Snowfox: Position Y", -0.06f, 0.16f, Step = 0.00001f)]
        public float Fy = -0.007f;
        [Slider("Snowfox: Position Z", 0f, 1f, Step = 0.0001f)]
        public float Fz = 0.16f;
        [Slider("Snowfox: Scale", 0.01f, 0.2f, Step = 0.00001f)]
        public float Fscale = 0.06f;
        
        /*
        [Toggle("Snowfox: Use Auto-position")]
        public bool useSnowfoxAutoPosition = true;
        [Choice("Snowfox: Auto-position"), OnChange(nameof(moveSnowfoxIndicator))]
        public Autoposition snowfoxIndicatorLocation = Autoposition.bottomleft;
        public void moveSnowfoxIndicator(ChoiceChangedEventArgs<Autoposition> e)
        {
            if (!useSnowfoxAutoPosition)
            {
                return;
            }
            switch ((Autoposition)e.Index)
            {
                case Autoposition.bottomleft:
                    Fx = -.4097f;
                    Fy = -.2164f;
                    Fz = 0.6145f;
                    Fscale = 0.1102f;
                    break;
                case Autoposition.bottomcenter:
                    Fx = 0f;
                    Fy = -0.1268f;
                    Fz = 0.3779f;
                    Fscale = 0.06f;
                    break;
                case Autoposition.bottomright:
                    Fx = 0.4039f;
                    Fy = -.2164f;
                    Fz = 0.6145f;
                    Fscale = 0.1102f;
                    break;
                case Autoposition.topleft:
                    Fx = -0.5371f;
                    Fy = 0.3607f;
                    Fz = 0.6145f;
                    Fscale = 0.1232f;
                    break;
                case Autoposition.topright:
                    Fx = 0.5371f;
                    Fy = 0.3607f;
                    Fz = 0.6145f;
                    Fscale = 0.1232f;
                    break;
            }
        }
        */
        #endregion
#endif
    }
}
