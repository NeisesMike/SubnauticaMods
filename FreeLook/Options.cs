

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

namespace FreeLook
{
    internal struct OptionsObject
    {
        public KeyCode FreeLookKey { get; set; }
    }


    public class Options : ModOptions
    {
        public KeyCode freeLookKey = KeyCode.PageUp;
        private string ConfigPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");

        public Options() : base("Free Look")
        {
            InitEvents();
            LoadDefaults();
        }

        private void InitEvents()
        {
            KeybindChanged += Options_KeybindChanged;
        }

        private void Options_KeybindChanged(object sender, KeybindChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "free_look":
                    freeLookKey = e.Key;
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
                FreeLookKey = freeLookKey
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

                    freeLookKey = data.ContainsKey("FreeLookKey") ? options.FreeLookKey : freeLookKey;
                }
                catch (Exception)
                {
                }
            }
            UpdateJSON();
        }

        public override void BuildModOptions()
        {
            AddKeybindOption("free_look", "Free-Look Key", GameInput.GetPrimaryDevice(), freeLookKey);
        }
    }

}
