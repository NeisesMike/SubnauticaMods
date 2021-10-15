using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace DebugScene
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerPatcher
    {
        private static bool hasInited = false;
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void Start()
        {
            if (MainMenuPatcher.IsDebugScene)
            {
                // disable Waterscape skybox
                foreach (DisableSkyBox dsb in Transform.FindObjectsOfType<DisableSkyBox>())
                {
                    dsb.enabled = false;
                }
                var sun = new GameObject("DebugSun");
                var sunlight = sun.AddComponent<Light>();
                sunlight.type = LightType.Directional;
                sunlight.transform.eulerAngles = new Vector3(74, 39, 180);

                Player.main.transform.Find("SpawnPlayerSounds/PlayerSounds(Clone)/pda").gameObject.SetActive(false);
                uGUI.main.transform.Find("ScreenCanvas/SubtitlesCanvas/Subtitles").gameObject.SetActive(false);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnLand")]
        public static void OnLandPostfix(Vector3 velocity)
        {
            if (MainMenuPatcher.IsDebugScene && !hasInited)
            {
                // this spawn is in the water below the lifepod
                Player.main.SetPosition(new Vector3(0, -15, 0));

                // this spawn is on top of the lifepod
                //Player.main.SetPosition(new Vector3(Player.main.transform.position.x, Player.main.transform.position.y + 10, Player.main.transform.position.z));

                DevConsole.SendConsoleCommand("item vehiclestoragemodule 6");
                DevConsole.SendConsoleCommand("item exosuitdrillarmmodule 2");
                DevConsole.SendConsoleCommand("item modvehiclestealthmodule1 1");
                DevConsole.SendConsoleCommand("spawn atrama");

                DevConsole.SendConsoleCommand("item VehicleArmorPlating 2");
                DevConsole.SendConsoleCommand("item VehiclePowerUpgradeModule 2");







                hasInited = true;
            }
        }
    }
}
