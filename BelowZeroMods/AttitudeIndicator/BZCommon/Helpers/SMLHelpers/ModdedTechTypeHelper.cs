using System;
using System.Collections.Generic;

namespace BZCommon.Helpers.SMLHelpers
{
    public class ModdedTechTypeHelper
    {
        public readonly Dictionary<TechType, EquipmentType> TypeDefCache = new Dictionary<TechType, EquipmentType>();

        public readonly Dictionary<string, TechType> FoundModdedTechTypes = new Dictionary<string, TechType>();

        public ModdedTechTypeHelper()
        {
            SearchForModdedTechTypes();            
        }
        
        private void SearchForModdedTechTypes()
        {
            int i = 0;

            int[] techTypeArray = (int[])Enum.GetValues(typeof(TechType));

            for (int j = 0; j < techTypeArray.Length; j++)
            {
                if (techTypeArray[j] >= 11000)
                {
                    TechType techType = (TechType)techTypeArray[j];
                    string techName = TechTypeExtensions.AsString(techType);
                                       
                    EquipmentType equipmentType = TechData.GetEquipmentType(techType);
                    FoundModdedTechTypes.Add(techName, techType);
                    TypeDefCache.Add(techType, equipmentType);
                    BZLogger.Log($"Modded techtype found! Name: [{techName}], ID: [{(int)techType}], Type: [{equipmentType}]");
                    i++;
                }
            }

            BZLogger.Log($"Found [{i}] modded TechType(s).");
        }

        public bool IsModdedTechType(string techName)
        {
            if(FoundModdedTechTypes.ContainsKey(techName))
            {
                return true;
            }

            return false;
        }

        public bool IsModdedTechType(TechType techType)
        {
            if (FoundModdedTechTypes.ContainsValue(techType))
            {
                return true;
            }

            return false;
        }        

    }    
}
