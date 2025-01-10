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
        [HarmonyPatch("Begin")]
        public static bool BeginPrefix()
        {
            if (ScubaRollController.IsActuallyScubaRolling)
            {
                state = BuilderToolState.blueprintBeforeRoll;
                ScubaRollController.ResetForEndRoll();
            }
            else
            {
                state = BuilderToolState.blueprintNormal;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("End")]
        public static void EndPostfix()
        {
            switch(state)
            {
                case BuilderToolState.blueprintBeforeRoll:
                    state = BuilderToolState.blueprintAfterRoll;
                    break;
                case BuilderToolState.blueprintAfterRoll:
                    ScubaRollController.ResetForStartRoll(null); // TODO maybe we should treat this like AfterCinematic
                    state = BuilderToolState.waiting;
                    break;
                default:
                    break;
            }
        }
    }
}
