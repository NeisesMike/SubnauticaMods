using BepInEx;
using UnityEngine;
using Nautilus.Utility;
using System.Reflection;
using JukeboxLib;

namespace ThePlanets
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.mikjaw.subnautica.theplanets.mod";
        public const string PLUGIN_NAME = "The Planets Jukebox Disks";
        public const string PLUGIN_VERSION = "1.0.0";
        internal static readonly Vector3 marsLocation = new Vector3(-803.2f, 77.1f, -1047.6f);
        internal static readonly Vector3 uranusLocation = new Vector3(-764.6f, 17.9f, -1116.2f);
        internal static readonly Vector3 venusLocation = new Vector3(-716.8f, 75.6f, -1167.8f);
        internal static readonly Vector3 jupiterLocation = new Vector3(97.35f, -258.66f, -367.2f);
        internal static readonly Vector3 mercuryLocation = new Vector3(-644.5f, -509.4f, -940.6f);
        public void Start()
        {
            AssetBundle AssetBundle = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly.GetExecutingAssembly(), "theplanets");
            if (AssetBundle == null)
            {
                ErrorMessage.AddError("JukeboxLib: Failed to fetch asset bundle. See log for details.");
                throw new System.Exception("JukeboxLib: Failed to fetch asset bundle. Double check existence of jukeboxDisk_assets.asset_bundle!");
            }
            new JukeboxDiskPrefab(Nautilus.Assets.PrefabInfo.WithTechType("JukeboxDiskMars"))
                .WithAudioClip(AssetBundle.LoadAsset<GameObject>("Mars.prefab").GetComponent<AudioSource>().clip)
                .WithDisplayName("The Planets: Mars")
                .WithSpawnLocations(new Nautilus.Assets.SpawnLocation[] { new Nautilus.Assets.SpawnLocation(marsLocation) })
                .Register();
            new JukeboxDiskPrefab(Nautilus.Assets.PrefabInfo.WithTechType("JukeboxDiskUranus"))
                .WithAudioClip(AssetBundle.LoadAsset<GameObject>("Uranus.prefab").GetComponent<AudioSource>().clip)
                .WithDisplayName("The Planets: Uranus")
                .WithSpawnLocations(new Nautilus.Assets.SpawnLocation[] { new Nautilus.Assets.SpawnLocation(uranusLocation) })
                .Register();
            new JukeboxDiskPrefab(Nautilus.Assets.PrefabInfo.WithTechType("JukeboxDiskVenus"))
                .WithAudioClip(AssetBundle.LoadAsset<GameObject>("Venus.prefab").GetComponent<AudioSource>().clip)
                .WithDisplayName("The Planets: Venus")
                .WithSpawnLocations(new Nautilus.Assets.SpawnLocation[] { new Nautilus.Assets.SpawnLocation(venusLocation) })
                .Register();
            new JukeboxDiskPrefab(Nautilus.Assets.PrefabInfo.WithTechType("JukeboxDiskJupiter"))
                .WithAudioClip(AssetBundle.LoadAsset<GameObject>("Jupiter.prefab").GetComponent<AudioSource>().clip)
                .WithDisplayName("The Planets: Jupiter")
                .WithSpawnLocations(new Nautilus.Assets.SpawnLocation[] { new Nautilus.Assets.SpawnLocation(jupiterLocation) })
                .Register();
            new JukeboxDiskPrefab(Nautilus.Assets.PrefabInfo.WithTechType("JukeboxDiskMercury"))
                .WithAudioClip(AssetBundle.LoadAsset<GameObject>("Mercury.prefab").GetComponent<AudioSource>().clip)
                .WithDisplayName("The Planets: Mercury")
                .WithSpawnLocations(new Nautilus.Assets.SpawnLocation[] { new Nautilus.Assets.SpawnLocation(mercuryLocation) })
                .Register();

            new HarmonyLib.Harmony(PLUGIN_GUID).PatchAll(typeof(PlayerPatcher));
        }
    }
}
