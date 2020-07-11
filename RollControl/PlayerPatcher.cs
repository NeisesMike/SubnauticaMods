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
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]

    public class RollControlPatcher
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("com.garyburke.subnautica.rollcontrol.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Initialise();
        }



        public static Options Options = new Options();
        public static void Initialise()
        {
            OptionsPanelHandler.RegisterModOptions(Options);
        }

        /*ref PilotingChair chair,*/
        [HarmonyPrefix]
        public static bool Prefix(Player __instance)
        {
            if (__instance.inSeamoth)
            {
                //do some update stuff

                
                //register input handlers
                

               // return false;
            }
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            // add the scuba mask, for kicks
            __instance.SetScubaMaskActive(__instance.inSeamoth);

            // disable roll stabilization
            var myVehicle = __instance.currentMountedVehicle;
            myVehicle.stabilizeRoll = false;

            // add roll handlers
            float rollSensitivity = 0.3F;
            if (Input.GetKey(Options.rollToPortKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * rollSensitivity, ForceMode.VelocityChange);
            }
            if (Input.GetKey(Options.rollToStarboardKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * -rollSensitivity, ForceMode.VelocityChange);
            }
        }

    }


    internal struct OptionsObject
    {
        public KeyCode RollPortKey { get; set; }
        public KeyCode RollStarboardKey { get; set; }
    }

    public class Options : ModOptions
    {
        public KeyCode rollToPortKey = KeyCode.PageUp;
        public KeyCode rollToStarboardKey = KeyCode.PageUp;
        private string ConfigPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");

        public Options() : base("BurkePatcher")
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
                case "roll_to_port":
                    rollToPortKey = e.Key;
                    break;
                case "roll_to_starboard":
                    rollToStarboardKey = e.Key;
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
                RollStarboardKey = rollToStarboardKey
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

                    if (!data.ContainsKey("RollPortKey") || !data.ContainsKey("RollStarboardKey"))
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
        }
    }

}
