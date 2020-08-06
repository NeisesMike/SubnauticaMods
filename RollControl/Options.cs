using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Harmony;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using LitJson;

namespace RollControl
{
    internal struct OptionsObject
    {
        public KeyCode RollPortKey { get; set; }
        public KeyCode RollStarboardKey { get; set; }
        public KeyCode SeamothRollToggleKey { get; set; }
        public KeyCode ScubaRollToggleKey { get; set; }
        public bool ScubaRollUnlimited { get; set; }

        public double SeamothRollSpeed { get; set; }
        public double ScubaRollSpeed { get; set; }

    }


    public class Options : ModOptions
    {
        public KeyCode rollToPortKey = KeyCode.Z;
        public KeyCode rollToStarboardKey = KeyCode.C;

        public KeyCode seamothRollToggleKey = KeyCode.RightAlt;
        public KeyCode scubaRollToggleKey = KeyCode.RightControl;
        public bool scubaRollUnlimited = false;

        public double seamothRollSpeed = 0.3F;
        public double scubaRollSpeed = 0.45F;

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
            ToggleChanged += Options_ToggleChanged;
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
                case "seamoth_roll_toggle":
                    seamothRollToggleKey = e.Key;
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
                case "seamoth_roll_speed":
                    seamothRollSpeed = e.Value;
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
                SeamothRollToggleKey = seamothRollToggleKey,
                ScubaRollToggleKey = scubaRollToggleKey,
                ScubaRollUnlimited = scubaRollUnlimited,
                SeamothRollSpeed = Math.Truncate(seamothRollSpeed * 1000d) / 1000d,
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
                    seamothRollToggleKey = data.ContainsKey("SeamothRollToggleKey") ? options.SeamothRollToggleKey : seamothRollToggleKey;
                    scubaRollToggleKey = data.ContainsKey("ScubaRollToggleKey") ? options.ScubaRollToggleKey : scubaRollToggleKey;
                   
                    scubaRollUnlimited = data.ContainsKey("ScubaRollUnlimited") ? options.ScubaRollUnlimited : scubaRollUnlimited;

                    seamothRollSpeed = data.ContainsKey("SeamothRollSpeed") ? options.SeamothRollSpeed : seamothRollSpeed;
                    scubaRollSpeed   = data.ContainsKey("ScubaRollSpeed")   ? options.ScubaRollSpeed   : scubaRollSpeed;
                    seamothRollSpeed = Math.Truncate(seamothRollSpeed * 1000d) / 1000d;
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
            AddKeybindOption("seamoth_roll_toggle", "Seamoth Roll-Toggle Key", GameInput.GetPrimaryDevice(), seamothRollToggleKey);
            AddKeybindOption("scuba_roll_toggle",   "Scuba Roll-Toggle Key",   GameInput.GetPrimaryDevice(), scubaRollToggleKey);

            AddToggleOption("scuba_roll_unlimited", "Scuba Roll Unlimited", scubaRollUnlimited);

            AddSliderOption("seamoth_roll_speed", "Seamoth Roll Speed", 0F, 1F, (float)seamothRollSpeed);
            AddSliderOption("scuba_roll_speed",   "Scuba Roll Speed",   0F, 1F, (float)scubaRollSpeed);
        }
    }
}
