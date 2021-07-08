using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System.Collections;
using UnityEngine;
using UWE;

namespace BZCommon.Helpers.SMLHelpers
{
    internal abstract class ModPrefab_Craftable : ModPrefab
    {
        protected readonly string TechTypeName;        
        protected readonly string FriendlyName;
        protected readonly string Description;
        public GameObject GameObjectClone { get; set; } = null;

        protected readonly TechType PrefabTemplate;        
        
        protected readonly TechType RequiredForUnlock;
        protected readonly TechGroup GroupForPDA;
        protected readonly TechCategory CategoryForPDA;
        protected readonly EquipmentType TypeForEquipment;
        protected readonly QuickSlotType TypeForQuickslot;
        protected readonly CraftData.BackgroundType BackgroundType;
        protected readonly Vector2int ItemSize;        
        protected readonly ModPrefab_Fragment _Fragment;

        private bool isEncyExists = false;
        
        protected ModPrefab_Craftable(
            string techTypeName,            
            string friendlyName,
            string description,
            TechType template,                    
            TechType requiredAnalysis,
            TechGroup groupForPDA,
            TechCategory categoryForPDA,
            EquipmentType equipmentType,
            QuickSlotType quickSlotType,
            CraftData.BackgroundType backgroundType,
            Vector2int itemSize,            
            ModPrefab_Fragment fragment
            )
            : base(techTypeName, $"{techTypeName}.Prefab")
        {
            TechTypeName = techTypeName;            
            FriendlyName = friendlyName;
            Description = description;
            PrefabTemplate = template;                        
            RequiredForUnlock = requiredAnalysis;
            GroupForPDA = groupForPDA;
            CategoryForPDA = categoryForPDA;
            TypeForEquipment = equipmentType;
            TypeForQuickslot = quickSlotType;
            BackgroundType = backgroundType;
            ItemSize = itemSize;            
            _Fragment = fragment;

            IngameMenuHandler.Main.RegisterOnQuitEvent(OnQuitEvent);
        }

        private void OnQuitEvent()
        {
            Patch();
        }

        public void Patch()
        {
            TechType = TechTypeHandler.Main.AddTechType(TechTypeName, FriendlyName, Description, null, false);            

            PrePatch();

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

            Sprite sprite = GetItemSprite();
            SpriteHandler.Main.RegisterSprite(TechType, sprite);

            PrefabHandler.Main.RegisterPrefab(this);
            CraftDataHandler.Main.SetTechData(TechType, GetRecipe());            
            CraftDataHandler.Main.SetItemSize(TechType, ItemSize);            
            CraftDataHandler.Main.AddToGroup(GroupForPDA, CategoryForPDA, TechType);            
            CraftDataHandler.Main.SetEquipmentType(TechType, TypeForEquipment);
            CraftDataHandler.Main.SetQuickSlotType(TechType, TypeForQuickslot);
            CraftDataHandler.Main.SetBackgroundType(TechType, BackgroundType);

            EncyData encyData = GetEncyclopediaData();

            if (encyData != null)
            {
                isEncyExists = true;

                PDAEncyclopedia.EntryData entryData = new PDAEncyclopedia.EntryData()
                {
                    key = ClassID,
                    path = EncyHelper.GetEncyPath(encyData.node),
                    nodes = EncyHelper.GetEncyNodes(encyData.node),
                    kind = PDAEncyclopedia.EntryData.Kind.Encyclopedia,
                    unlocked = false,
                    popup = _Fragment != null ? _Fragment.UnlockSprite : sprite,
                    image = encyData.image,
                    audio = null,
                    hidden = false
                };

                PDAEncyclopediaHandler.Main.AddCustomEntry(entryData);

                LanguageHandler.Main.SetLanguageLine($"Ency_{ClassID}", encyData.title);
                LanguageHandler.Main.SetLanguageLine($"EncyDesc_{ClassID}", encyData.description);
            }

            if (RequiredForUnlock == TechType.None && _Fragment != null)
            {
                PDAScanner.EntryData scannerEntryData = new PDAScanner.EntryData()
                {
                    key = _Fragment.TechType,
                    blueprint = TechType,
                    destroyAfterScan = _Fragment.DestroyAfterScan,
                    encyclopedia = isEncyExists ? ClassID : null,
                    isFragment = true,
                    locked = false,
                    scanTime = _Fragment.ScanTime,
                    totalFragments = _Fragment.TotalFragments,
                    unlockStoryGoal = false
                };

                PDAHandler.Main.AddCustomScannerEntry(scannerEntryData);

                KnownTechHandler.Main.SetAnalysisTechEntry(TechType, new TechType[1] { TechType }, _Fragment.UnlockSprite);                
            }
            else
            {
                KnownTechHandler.Main.SetAnalysisTechEntry(RequiredForUnlock, new TechType[1] { TechType }, $"{FriendlyName} blueprint discovered!");
            }

            TabNode NewTabNode = GetTabNodeData();

            if (NewTabNode != null)
            {
                CraftTreeHandler.Main.AddTabNode(NewTabNode.craftTree, NewTabNode.uniqueName, NewTabNode.displayName, NewTabNode.sprite);
            }

            foreach (CraftTreeType craftTreeType in GetCraftTreeTypesData().TreeTypes)
            {
                CraftTreeHandler.Main.AddCraftingNode(craftTreeType.TreeType, TechType, craftTreeType.StepsToTab);
            }

            PostPatch();

            yield break;
        }              

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(PrefabTemplate);
            yield return request;

            GameObject result = request.GetResult();

            if (result == null)
            {
                BZLogger.Warn($"{TechTypeName} : Cannot instantiate prefab from TechType!");
                yield break;
            }

            GameObjectClone = Object.Instantiate(result);            

            ModifyGameObject();

            gameObject.Set(GameObjectClone);

            yield break;
        }       

        protected abstract RecipeData GetRecipe();

        protected abstract EncyData GetEncyclopediaData();

        protected abstract CrafTreeTypesData GetCraftTreeTypesData();

        protected abstract TabNode GetTabNodeData();

        protected abstract void ModifyGameObject();

        protected abstract void PrePatch();

        protected abstract void PostPatch();

        protected abstract Sprite GetItemSprite();
    }    
}
