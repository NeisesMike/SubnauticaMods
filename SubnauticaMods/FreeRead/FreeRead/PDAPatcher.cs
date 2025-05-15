using HarmonyLib;

namespace FreeRead
{
    [HarmonyPatch(typeof(PDA))]
    class PDAClosePatcher
    {
        [HarmonyPatch(nameof(PDA.Close))]
        [HarmonyPrefix]
        public static void Prefix()
        {
            Vehicle thisVehicle = Player.main.currentMountedVehicle;
            if (thisVehicle == null) return;

            thisVehicle.gameObject.EnsureComponent<FreeReadManager>().isFreeReading = false;
        }
    }
}
