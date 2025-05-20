using System;
using UnityEngine;
using Nautilus.Assets;
using Nautilus.Utility;
using Nautilus.Assets.Gadgets;

namespace JukeboxLib
{
    public static class JukeboxDiskExtensions
    {
        public static JukeboxDiskPrefab WithAudioClip(this JukeboxDiskPrefab thisDisk, AudioClip clip)
        {
            thisDisk.musicClip = clip;
            return thisDisk;
        }
        public static JukeboxDiskPrefab WithDisplayName(this JukeboxDiskPrefab thisDisk, string name)
        {
            thisDisk.displayName = name;
            return thisDisk;
        }
        public static JukeboxDiskPrefab WithOverrideGameObject(this JukeboxDiskPrefab thisDisk, GameObject diskObject)
        {
            thisDisk.overrideGameObject = diskObject;
            return thisDisk;
        }
        public static JukeboxDiskPrefab WithSpawnLocations(this JukeboxDiskPrefab thisDisk, SpawnLocation[] spawns)
        {
            thisDisk.spawnLocations = spawns;
            return thisDisk;
        }
    }
    public class JukeboxDiskPrefab
    {
        public PrefabInfo Info { get; }
        public TechType TechType { get; }
        public string ClassID { get; }
        public JukeboxDiskPrefab(PrefabInfo info)
        {
            Info = info;
            TechType = info.TechType;
            ClassID = info.ClassID;
        }

        internal AudioClip musicClip = null;
        internal string displayName = string.Empty;
        internal GameObject overrideGameObject = null;
        internal SpawnLocation[] spawnLocations = null;
        public void Register()
        {
            ValidateJukeboxDisk();
            var customPrefab = new CustomPrefab(Info);
            if (overrideGameObject == null)
            {
                customPrefab.SetGameObject(GetGameObject);
            }
            else
            {
                overrideGameObject.AddComponent<JukeboxDisk>();
                customPrefab.SetGameObject(overrideGameObject);
            }
            if (spawnLocations != null && spawnLocations.Length > 0)
            {
                customPrefab.SetSpawns(spawnLocations);
            }
            customPrefab.Register();
            UpdateJukeboxDiskDisplayName();
            JukeboxLibrary.TryAddClip(displayName, musicClip);
        }

        private void ValidateJukeboxDisk()
        {
            const string mainError = "JukeboxLib: Failed to create Jukebox disk. See log for details.";
            if (musicClip == null)
            {
                ErrorMessage.AddError(mainError);
                Logger.Log("JukeboxLib Error: music clip was null! Could not create jukebox disk.");
                throw new ArgumentNullException(nameof(musicClip));
            }
            if (displayName.Equals(string.Empty))
            {
                ErrorMessage.AddError(mainError);
                Logger.Log("JukeboxLib Error: disk display name was null! Could not create jukebox disk.");
                throw new ArgumentNullException(nameof(displayName));
            }
            if (JukeboxDisk.displayNames.ContainsKey(TechType))
            {
                ErrorMessage.AddError(mainError);
                Logger.Log($"JukeboxLib Error: that techtype already exists: {TechType.AsString()}");
                throw new InvalidOperationException();
            }
            if (JukeboxDisk.displayNames.ContainsValue(displayName))
            {
                ErrorMessage.AddError(mainError);
                Logger.Log($"JukeboxLib Error: that display name already exists: {displayName}");
                throw new InvalidOperationException();
            }
        }

        private GameObject GetGameObject()
        {
            var model = UnityEngine.Object.Instantiate(MainPatcher.AssetBundle.LoadAsset<GameObject>("JukeboxDisk.prefab"));
            model.SetActive(false);
            PrefabUtils.AddBasicComponents(model, ClassID, TechType, LargeWorldEntity.CellLevel.Medium);
            MaterialUtils.ApplySNShaders(model);
            model.AddComponent<JukeboxDisk>();
            return model;
        }

        private void UpdateJukeboxDiskDisplayName()
        {
            if (JukeboxDisk.displayNames.ContainsKey(TechType))
            {
                JukeboxDisk.displayNames[TechType] = displayName;
            }
            else
            {
                JukeboxDisk.displayNames.Add(TechType, displayName);
            }
        }
    }
}
