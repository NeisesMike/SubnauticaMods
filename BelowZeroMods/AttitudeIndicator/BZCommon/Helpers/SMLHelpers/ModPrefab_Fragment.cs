using BZCommon.Helpers.Testing;
using HarmonyLib;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UWE;

namespace BZCommon.Helpers.SMLHelpers
{
    public abstract class ModPrefab_Fragment : ModPrefab
    {
        protected readonly string TechTypeName;
        public readonly string VirtualPrefabFilename;
        protected readonly string FriendlyName;
        protected readonly TechType FragmentTemplate;
        protected readonly string PrefabFilePath;
        protected readonly EntitySlot.Type SlotType;
        protected readonly bool PrefabZUp;
        protected readonly LargeWorldEntity.CellLevel CellLevel;
        protected readonly Vector3 LocalScale;                
        public readonly float ScanTime;
        public readonly int TotalFragments;
        public readonly bool DestroyAfterScan;

        public GameObject GameObjectClone { get; set; } = null;
        public Sprite UnlockSprite { get; private set; } = null;

        protected ModPrefab_Fragment(
            string techTypeName,
            string friendlyName,
            TechType template,
            string prefabFilePath,
            EntitySlot.Type slotType,
            bool prefabZUp,
            LargeWorldEntity.CellLevel cellLevel,
            Vector3 localScale,            
            float scanTime = 2,
            int totalFragments = 2,
            bool destroyAfterScan = true
            )
            : base(techTypeName, $"{techTypeName}.Prefab")
        {
            TechTypeName = techTypeName;
            FriendlyName = friendlyName;
            FragmentTemplate = template;
            PrefabFilePath = prefabFilePath;
            SlotType = slotType;
            PrefabZUp = prefabZUp;
            CellLevel = cellLevel;
            LocalScale = localScale;            
            ScanTime = scanTime;
            TotalFragments = totalFragments;
            DestroyAfterScan = destroyAfterScan;
            VirtualPrefabFilename = $"{techTypeName}.Prefab";
        }

        public void Patch()
        {
            TechType = TechTypeHandler.Main.AddTechType(TechTypeName, FriendlyName, string.Empty, null, false);

            CoroutineHost.StartCoroutine(PatchAsync());
        }        

        private IEnumerator PatchAsync()
        {
            while (!SpriteManager.hasInitialized)
            {
                BZLogger.Debug($"{TechTypeName} : Spritemanager is not ready!");
                yield return null;
            }

            BZLogger.Debug($"{TechTypeName} : Async patch started.");

            UnlockSprite = GetUnlockSprite();

            SpriteHandler.Main.RegisterSprite(TechType, UnlockSprite);

            PrefabHandler.Main.RegisterPrefab(this);            

            LootDistributionData.SrcData srcData = new LootDistributionData.SrcData()
            {
                prefabPath = VirtualPrefabFilename,
                distribution = GetBiomeDatas()
            };

            LootDistributionHandler.Main.AddLootDistributionData(ClassID, srcData);

            WorldEntityInfo EntityInfo = new WorldEntityInfo()
            {
                classId = ClassID,
                techType = TechType,
                slotType = SlotType,
                prefabZUp = PrefabZUp,
                cellLevel = CellLevel,
                localScale = LocalScale
            };

            WorldEntityDatabaseHandler.Main.AddCustomInfo(ClassID, EntityInfo);            
        }        

        public CoroutineTask<GameObject> SpawnFragmentAsync(Transform parent, Vector3 position, Quaternion rotation, bool awake)
        {
            BZLogger.Debug($"SpawnFragmentAsync called for: {TechTypeName}");

            TaskResult<GameObject> taskResult = new TaskResult<GameObject>();
            return new CoroutineTask<GameObject>(InstantiateFragmentAsync(taskResult, parent, position, rotation, awake), taskResult);
        }

        public IEnumerator InstantiateFragmentAsync(IOut<GameObject> result, Transform parent, Vector3 position, Quaternion rotation, bool awake)
        {
            if (GameObjectClone != null)
            {
                GameObject fragmentClone1 = UnityEngine.Object.Instantiate(GameObjectClone, parent, position, rotation, awake);

                result.Set(fragmentClone1);

                yield break;
            }            

            TaskResult<GameObject> taskResult = new TaskResult<GameObject>();

            yield return GetGameObjectAsync(taskResult);                       

            GameObject fragmentClone2 = UnityEngine.Object.Instantiate(taskResult.Get(), parent, position, rotation, awake);
            
            result.Set(fragmentClone2);

            yield break;
        }
        
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {            
            if (GameObjectClone != null)
            {
                gameObject.Set(GameObjectClone);
                yield break;
            }

            GameObject result;

            if (FragmentTemplate != TechType.None)
            {
                CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(FragmentTemplate);
                yield return request;

                result = request.GetResult();

                if (result == null)
                {
                    BZLogger.Error($"Cannot instantiate prefab from TechType '{FragmentTemplate}'!");
                    yield break;
                }

                GameObjectClone = UWE.Utils.InstantiateDeactivated(result);
            }
            else if(!string.IsNullOrEmpty(PrefabFileName))
            {
                IPrefabRequest prefabRequest = PrefabDatabase.GetPrefabForFilenameAsync(PrefabFilePath);
                yield return prefabRequest;

                if (prefabRequest.TryGetPrefab(out result))
                {
                    //GameObjectClone = UnityEngine.Object.Instantiate(result);
                    GameObjectClone = UWE.Utils.InstantiateDeactivated(result);
                }
                else
                {
                    BZLogger.Error($"Cannot find prefab in PrefabDatabase at path '{PrefabFilePath}!");
                    yield break;
                }
            }

            GameObjectClone.name = TechTypeName;

            PrefabIdentifier prefabIdentifier = GameObjectClone.GetComponent<PrefabIdentifier>();
            prefabIdentifier.ClassId = TechTypeName;

            TechTag techTag = GameObjectClone.GetComponent<TechTag>();
            techTag.type = TechType;

            ResourceTracker resourceTracker = GameObjectClone.GetComponent<ResourceTracker>();
            resourceTracker.overrideTechType = TechType.Fragment;

            ModifyGameObject();

            AddFragmentTracker(GameObjectClone);

            BZLogger.Debug($"GetGameObjectAsync called for fragment: {TechTypeName}");            

            gameObject.Set(GameObjectClone);            

            yield break;
        }        

        [Conditional("DEBUG")]
        private static void AddFragmentTracker(GameObject gameObject)
        {
            gameObject.AddComponent<FragmentTracker>();
        }

        protected abstract void ModifyGameObject();

        protected abstract List<LootDistributionData.BiomeData> GetBiomeDatas();

        protected abstract Sprite GetUnlockSprite();        
    }
}
