using HarmonyLib;

namespace RollControl.Patches
{
    [HarmonyPatch(typeof(Builder))]
    public class BuilderPatcher
    {
        private enum BuilderToolState
        {
            waiting,
            blueprintNormal,
            blueprintBeforeRoll,
            blueprintAfterRoll
        }
        private static BuilderToolState state = BuilderToolState.waiting;

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Builder.Begin))]
        public static bool BeginPrefix()
        {
            if (Player.main.GetComponent<ScubaRollController>().IsActuallyScubaRolling)
            {
                state = BuilderToolState.blueprintBeforeRoll;
                Player.main.GetComponent<ScubaRollController>().isRollEnabled = false;
                Player.main.GetComponent<ScubaRollController>().ResetForEndRoll();
                Player.main.GetComponent<ScubaRollController>().SetupEndingScubaRollOnceAtExit();
            }
            else
            {
                state = BuilderToolState.blueprintNormal;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Builder.End))]
        public static void EndPostfix()
        {
            switch(state)
            {
                case BuilderToolState.blueprintBeforeRoll:
                    state = BuilderToolState.blueprintAfterRoll;
                    break;
                case BuilderToolState.blueprintAfterRoll:
                    state = BuilderToolState.waiting;
                    break;
                default:
                    break;
            }
        }
    }
}
