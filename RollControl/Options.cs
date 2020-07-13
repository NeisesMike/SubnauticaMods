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
        public KeyCode RollToggleKey { get; set; }
        public double RollSpeed { get; set; }

    }


    public class Options : ModOptions
    {
        public KeyCode rollToPortKey = KeyCode.Z;
        public KeyCode rollToStarboardKey = KeyCode.C;
        public KeyCode rollToggleKey = KeyCode.RightAlt;
        public double rollSpeed = 0.3F;

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
                case "roll_toggle":
                    rollToggleKey = e.Key;
                    break;
            }
            UpdateJSON();
        }
        private void Options_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "roll_speed":
                    rollSpeed = e.Value;
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
                RollToggleKey = rollToggleKey,
                RollSpeed = Math.Truncate(rollSpeed * 1000d) / 1000d
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
                    rollToggleKey = data.ContainsKey("RollToggleKey") ? options.RollToggleKey : rollToggleKey;
                    rollSpeed = data.ContainsKey("RollSpeed") ? options.RollSpeed : rollSpeed;

                    rollSpeed = Math.Truncate(rollSpeed * 1000d) / 1000d;

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
            AddKeybindOption("roll_toggle", "Roll-Toggle Key", GameInput.GetPrimaryDevice(), rollToggleKey);
            AddSliderOption("roll_speed", "Roll Speed", 0F, 1F, (float)rollSpeed);
        }
    }

}
