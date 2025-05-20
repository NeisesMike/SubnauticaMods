using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace SimpleMainMenu
{
    [HarmonyPatch(typeof(uGUI_MainMenu))]
    public class uGUI_MainMenuPatcher
    {
        private static float rememberedX = 0f;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(uGUI_MainMenu.Awake))]
        public static void uGUI_MainMenuAwakePostfix(uGUI_MainMenu __instance)
        {
            rememberedX = GetPrimaryOptions(__instance).localPosition.x;
            if (MainPatcher.SimpleMainMenuConfig.EnableRightSide.Value)
            {
                ResetPrimaryOptionsPlacement(GetPrimaryOptions(__instance));
            }
            else
            {
                UWE.CoroutineHost.StartCoroutine(DisableHome());
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(uGUI_MainMenu.OnRightSideOpened))]
        public static bool uGUI_MainMenuOnRightSideOpenedPrefix(uGUI_MainMenu __instance, GameObject root)
        {
            if (MainPatcher.SimpleMainMenuConfig.EnableRightSide.Value)
            {
                ResetPrimaryOptionsPlacement(GetPrimaryOptions(__instance));
                return true;
            }
            else
            {
                if (root.gameObject.name == "Home")
                {
                    root.SetActive(false);
                    AdjustPrimaryOptionsPlacement(GetPrimaryOptions(__instance));
                    return false;
                }
                else
                {
                    ResetPrimaryOptionsPlacement(GetPrimaryOptions(__instance));
                    return true;
                }
            }
        }

        private static void ResetPrimaryOptionsPlacement(Transform rightSide)
        {
            rightSide.localPosition = new Vector3(
                rememberedX,
                rightSide.localPosition.y,
                rightSide.localPosition.z);
        }
        private static void AdjustPrimaryOptionsPlacement(Transform rightSide)
        {
            rightSide.localPosition = new Vector3(
                0,
                rightSide.localPosition.y,
                rightSide.localPosition.z);
        }
        private static Transform GetPrimaryOptions(uGUI_MainMenu menu)
        {
            return menu.transform.Find("Panel/MainMenu/PrimaryOptions");
        }
        private static Transform GetRightSide(uGUI_MainMenu menu)
        {
            return menu.transform.Find("Panel/MainMenu/RightSide");
        }
        private static IEnumerator DisableHome()
        {
            yield return new WaitUntil(() => MainMenuRightSide.main!= null);
            yield return new WaitUntil(() => MainMenuRightSide.main.homeGroup!= null);
            yield return new WaitUntil(() => MainMenuRightSide.main.homeGroup.gameObject != null);
            yield return new WaitUntil(() => MainMenuRightSide.main.homeGroup.gameObject.activeInHierarchy);
            MainMenuRightSide.main.homeGroup.gameObject.SetActive(false);
        }
    }
}
