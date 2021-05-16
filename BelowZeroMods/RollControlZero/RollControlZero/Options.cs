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

using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;

namespace RollControlZero
{
    /*
    internal struct OptionsObject
    {
        public KeyCode RollPortKey { get; set; }
        public KeyCode RollStarboardKey { get; set; }
        public KeyCode SeatruckRollToggleKey { get; set; }
        public KeyCode ScubaRollToggleKey { get; set; }
        public bool ScubaRollUnlimited { get; set; }

        public double SeatruckRollSpeed { get; set; }
        public double ScubaRollSpeed { get; set; }

    }

    public class Options : ModOptions
    {
        public KeyCode rollToPortKey = KeyCode.Z;
        public KeyCode rollToStarboardKey = KeyCode.C;

        public KeyCode seatruckRollToggleKey = KeyCode.RightAlt;
        //public KeyCode scubaRollToggleKey = KeyCode.RightControl;
        //public bool scubaRollUnlimited = false;

        public double seatruckRollSpeed = 0.15F;
        //public double scubaRollSpeed = 0.45F;

        private string ConfigPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");

        public Options() : base("Roll Controls")
        {
            InitEvents();
            LoadDefaults();
        }

        private void InitEvents()
        {
            KeybindChanged += Options_KeybindChanged;
            SliderChanged += Options_SliderChanged;
            //ToggleChanged += Options_ToggleChanged;
        }

        private void Options_KeybindChanged(object sender, KeybindChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "roll_to_port":
                    rollToPortKey = e.Key;
                    break;
                case "roll_to_starboard":
                    rollToStarboardKey = e.Key;
                    break;
                case "seatruck_roll_toggle":
                    seatruckRollToggleKey = e.Key;
                    break;
                case "scuba_roll_toggle":
                    scubaRollToggleKey = e.Key;
                    break;
            }
            UpdateJSON();
        }
        private void Options_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "seatruck_roll_speed":
                    seatruckRollSpeed = e.Value;
                    break;
                case "scuba_roll_speed":
                    scubaRollSpeed = e.Value;
                    break;
            }
            UpdateJSON();
        }
        private void Options_ToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "scuba_roll_unlimited":
                    scubaRollUnlimited = e.Value;
                    break;
            }
            UpdateJSON();
        }

        private void LoadDefaults()
        {
            if (!File.Exists(ConfigPath))
            {
                UpdateJSON();
            }
            else
            {
                ReadOptionsFromJSON();
            }
        }

        private void UpdateJSON()
        {
            OptionsObject options = new OptionsObject
            {
                RollPortKey = rollToPortKey,
                RollStarboardKey = rollToStarboardKey,
                SeatruckRollToggleKey = seatruckRollToggleKey,
                SeatruckRollSpeed = Math.Truncate(seatruckRollSpeed * 1000d) / 1000d,

                ScubaRollToggleKey = scubaRollToggleKey,
                ScubaRollUnlimited = scubaRollUnlimited,
                ScubaRollSpeed = Math.Truncate(scubaRollSpeed * 1000d) / 1000d
            };

            var stringBuilder = new StringBuilder();
            var jsonWriter = new JsonWriter(stringBuilder)
            {
                PrettyPrint = true
            };
            JsonMapper.ToJson(options, jsonWriter);

            string optionsJSON = stringBuilder.ToString();
            File.WriteAllText(ConfigPath, optionsJSON);
        }

        private void ReadOptionsFromJSON()
        {
            if (File.Exists(ConfigPath))
            {   // Parse and load options from the new config.json
                try
                {
                    string optionsJSON = File.ReadAllText(ConfigPath);
                    var options = JsonMapper.ToObject<OptionsObject>(optionsJSON);
                    var data = JsonMapper.ToObject(optionsJSON);

                    rollToPortKey = data.ContainsKey("RollPortKey") ? options.RollPortKey : rollToPortKey;
                    rollToStarboardKey = data.ContainsKey("RollStarboardKey") ? options.RollStarboardKey : rollToStarboardKey;
                    seatruckRollToggleKey = data.ContainsKey("seatruckRollToggleKey") ? options.SeatruckRollToggleKey : seatruckRollToggleKey;
                    seatruckRollSpeed = Math.Truncate(seatruckRollSpeed * 1000d) / 1000d;

                    scubaRollToggleKey = data.ContainsKey("ScubaRollToggleKey") ? options.ScubaRollToggleKey : scubaRollToggleKey;
                   
                    scubaRollUnlimited = data.ContainsKey("ScubaRollUnlimited") ? options.ScubaRollUnlimited : scubaRollUnlimited;

                    seatruckRollSpeed = data.ContainsKey("SeatruckRollSpeed") ? options.SeatruckRollSpeed : seatruckRollSpeed;
                    scubaRollSpeed   = data.ContainsKey("ScubaRollSpeed")   ? options.ScubaRollSpeed   : scubaRollSpeed;
                    scubaRollSpeed   = Math.Truncate(scubaRollSpeed   * 1000d) / 1000d;


                    //if (!data.ContainsKey("RollPortKey") || !data.ContainsKey("RollStarboardKey") || !data.ContainsKey("RollSpeed"))
                    {
                        UpdateJSON();
                    }
                }
                catch (Exception)
                {   // JSON was invalid, create default values
                    UpdateJSON();
                }
            }
            else
            {   // Create the config.json with default values
                UpdateJSON();
            }
        }

        public override void BuildModOptions()
        {
            AddKeybindOption("roll_to_port", "Roll-to-Port Key", GameInput.GetPrimaryDevice(), rollToPortKey);
            AddKeybindOption("roll_to_starboard", "Roll-to-Starboard Key", GameInput.GetPrimaryDevice(), rollToStarboardKey);
            AddKeybindOption("seatruck_roll_toggle", "Seatruck Roll-Toggle Key", GameInput.GetPrimaryDevice(), seatruckRollToggleKey);
            AddSliderOption("seatruck_roll_speed", "Seatruck Roll Speed", (float)0, (float)1, (float)seatruckRollSpeed);

            //AddKeybindOption("scuba_roll_toggle",   "Scuba Roll-Toggle Key",   GameInput.GetPrimaryDevice(), scubaRollToggleKey);
            //AddToggleOption("scuba_roll_unlimited", "Scuba Roll Unlimited", scubaRollUnlimited);
            //AddSliderOption("scuba_roll_speed",   "Scuba Roll Speed",   0F, 1F, (float)scubaRollSpeed);
        }
    }
    */
}
