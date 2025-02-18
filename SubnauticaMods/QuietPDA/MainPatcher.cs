namespace QuietPDA
{
    [BepInEx.BepInPlugin("com.mikjaw.subnautica.quietpda.mod", "Quiet PDA", "1.0")]
    public class MainPatcher : BepInEx.BaseUnityPlugin
    {
        public void Start()
        {
            new HarmonyLib.Harmony("com.mikjaw.subnautica.quietpda.mod").PatchAll();
        }
    }
    [HarmonyLib.HarmonyPatch(typeof(SoundQueue))]
    public class SoundQueuePatcher
    {
        [HarmonyLib.HarmonyTargetMethod]
        public static System.Reflection.MethodBase TargetMethod()
        {
            return typeof(SoundQueue).GetMethod("PlayQueued", new System.Type[] { typeof(string), typeof(string) });
        }
        [HarmonyLib.HarmonyPrefix]
        public static bool SoundQueuePlayQueuedHarmonyPrefix()
        {
            return false;
        }
    }
}