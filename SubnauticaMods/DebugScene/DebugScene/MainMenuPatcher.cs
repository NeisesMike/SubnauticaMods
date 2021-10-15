using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.IO;
using System.Reflection;

using UnityEngine.U2D;
using UnityEngine;
using UnityEngine.SceneManagement;
using UWE;

namespace DebugScene
{
    [HarmonyPatch(typeof(uGUI_MainMenu))]
    public class MainMenuPatcher
    {
        public static SceneController sc;
        public static bool IsDebugScene = false;

        public static void StartGame()
        {
            IsDebugScene = true;
            GameObject scObj = new GameObject("Debug Scene Controller");
            sc = scObj.AddComponent<SceneController>();
            CoroutineHost.StartCoroutine(sc.StartNewGame(GameMode.Creative));
        }

        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        public static void GoToScene()
        {
            if (MainPatcher.Config.StraightToScene)
            {
                StartGame();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("OnButtonFreedom")]
        public static bool ClickFreedom()
        {
            StartGame();
            return false;
        }
    }
}
