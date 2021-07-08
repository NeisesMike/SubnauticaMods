using System.Collections.Generic;

namespace BZCommon.Helpers.SMLHelpers
{
    public static class EncyHelper
    {
        private static readonly Dictionary<EncyNode, string> EncyNodes = new Dictionary<EncyNode, string>()
        {
            { EncyNode.Welcome,             "Welcome" },
            { EncyNode.Advanced,            "Advanced" },
            { EncyNode.Emergency,           "Emergency" },
            { EncyNode.WorkDocs,            "WorkDocs" },
            { EncyNode.PersonalLog,         "PersonalLog" },

            { EncyNode.Sea,                 "Lifeforms/Flora/Sea" },
            { EncyNode.Land,                "Lifeforms/Flora/Land" },
            { EncyNode.SmallHerbivores,     "Lifeforms/Fauna/SmallHerbivores" },
            { EncyNode.LargeHerbivores,     "Lifeforms/Fauna/LargeHerbivores" },
            { EncyNode.Carnivores,          "Lifeforms/Fauna/Carnivores" },
            { EncyNode.Scavengers,          "Lifeforms/Fauna/Scavengers" },
            { EncyNode.Leviathans,          "Lifeforms/Fauna/Leviathans" },
            { EncyNode.FrozenCreature,      "Lifeforms/Fauna/Leviathans/FrozenCreature" },
            { EncyNode.Coral,               "Lifeforms/Coral" },
            { EncyNode.Exploitable,         "Lifeforms/Flora/Exploitable" },

            { EncyNode.Equipment,           "Tech/Equipment" },
            { EncyNode.Habitats,            "Tech/Habitats" },
            { EncyNode.Vehicles,            "Tech/Vehicles" },
            { EncyNode.Power,               "Tech/Power" },

            { EncyNode.PlanetaryGeology,    "PlanetaryGeology" },
            { EncyNode.Precursor,           "DownloadedData/Precursor" },
            { EncyNode.Scan,                "DownloadedData/Precursor/Scan" },
            { EncyNode.Codes,               "DownloadedData/Codes" },
            { EncyNode.Alterra,             "DownloadedData/Alterra" },
            { EncyNode.PublicDocs,          "DownloadedData/PublicDocs" },
            { EncyNode.ShipWreck,           "DownloadedData/ShipWreck" },
            { EncyNode.Marguerit,           "DownloadedData/Marguerit" },
            { EncyNode.Sam,                 "DownloadedData/Sam" },
            { EncyNode.Personnel,           "DownloadedData/Alterra/Personnel" }
        };

        public static string[] GetEncyNodes(EncyNode node)
        {
            return EncyNodes[node].Split('/');
        }

        public static string GetEncyPath(EncyNode node)
        {
            return EncyNodes[node];
        }
    }

    public enum EncyNode
    {
        SmallHerbivores,
        LargeHerbivores,
        Exploitable,
        Carnivores,
        Coral,
        Sea,
        Scavengers,
        Leviathans,
        Welcome,
        Equipment,
        PlanetaryGeology,
        Habitats,
        Land,
        Vehicles,
        Power,
        Scan,
        Advanced,
        Codes,
        Alterra,
        Precursor,
        PublicDocs,
        ShipWreck,
        FrozenCreature,
        Marguerit,
        Emergency,
        WorkDocs,
        PersonalLog,
        Sam,
        Personnel
    }
}
