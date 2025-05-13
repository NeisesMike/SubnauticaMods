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
                .WithSpawnLocations(new Nautilus.Assets.SpawnLocation[] { new Nautilus.Assets.SpawnLocation(new Vector3(-802.8f, 78.6f, -1049.1f)) })
                .Register();
            new JukeboxDiskPrefab(Nautilus.Assets.PrefabInfo.WithTechType("JukeboxDiskUranus"))
                .WithAudioClip(AssetBundle.LoadAsset<GameObject>("Uranus.prefab").GetComponent<AudioSource>().clip)
                .WithDisplayName("The Planets: Uranus")
                .WithSpawnLocations(new Nautilus.Assets.SpawnLocation[] { new Nautilus.Assets.SpawnLocation(new Vector3(-765.6f, 19.3f, -1116.2f)) })
                .Register();
            new JukeboxDiskPrefab(Nautilus.Assets.PrefabInfo.WithTechType("JukeboxDiskVenus"))
                .WithAudioClip(AssetBundle.LoadAsset<GameObject>("Venus.prefab").GetComponent<AudioSource>().clip)
                .WithDisplayName("The Planets: Venus")
                .WithSpawnLocations(new Nautilus.Assets.SpawnLocation[] { new Nautilus.Assets.SpawnLocation(new Vector3(-716.3f, 76.2f, -1167.1f)) })
                .Register();
            new JukeboxDiskPrefab(Nautilus.Assets.PrefabInfo.WithTechType("JukeboxDiskJupiter"))
                .WithAudioClip(AssetBundle.LoadAsset<GameObject>("Jupiter.prefab").GetComponent<AudioSource>().clip)
                .WithDisplayName("The Planets: Jupiter")
                .WithSpawnLocations(new Nautilus.Assets.SpawnLocation[] { new Nautilus.Assets.SpawnLocation(new Vector3(97.5f, -257.9f, -367.6f)) })
                .Register();
            new JukeboxDiskPrefab(Nautilus.Assets.PrefabInfo.WithTechType("JukeboxDiskMercury"))
                .WithAudioClip(AssetBundle.LoadAsset<GameObject>("Mercury.prefab").GetComponent<AudioSource>().clip)
                .WithDisplayName("The Planets: Mercury")
                .WithSpawnLocations(new Nautilus.Assets.SpawnLocation[] { new Nautilus.Assets.SpawnLocation(new Vector3(-643.6f, -509.3f -941.4f)) })
                .Register();
        }
    }
}
