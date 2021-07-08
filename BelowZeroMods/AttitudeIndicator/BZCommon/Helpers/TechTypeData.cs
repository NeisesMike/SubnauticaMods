using System;
using System.Collections.Generic;

namespace BZCommon.Helpers
{
    public struct TechTypeData : IComparable<TechTypeData>
    {
        public TechType TechType { get; set; }
        public string Name { get; set; }

        public TechTypeData(TechType techType, string name)
        {
            TechType = techType;
            Name = name;
        }

        public int CompareTo(TechTypeData other)
        {
            return string.Compare(Name, other.Name);
        }

        public string GetTechName()
        {
            return Name;
        }
    }    
}
