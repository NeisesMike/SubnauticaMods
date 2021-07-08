using System.Collections.Generic;
using UnityEngine;

namespace BZCommon.Helpers.SMLHelpers
{
    public class TabNode
    {
        public readonly CraftTree.Type craftTree;
        public readonly string uniqueName;
        public readonly string displayName;
        public readonly Sprite sprite;

        public TabNode(CraftTree.Type craftTree, string uniqueName, string displayName, Sprite sprite)
        {
            this.craftTree = craftTree;
            this.uniqueName = uniqueName;
            this.displayName = displayName;
            this.sprite = sprite;
        }
    }

    public class CraftTreeType
    {
        public CraftTreeType(CraftTree.Type craftTreeType, string[] stepsToTab)
        {
            TreeType = craftTreeType;
            StepsToTab = stepsToTab;
        }

        public CraftTree.Type TreeType { get; }
        public string[] StepsToTab { get; }
    }

    public class CrafTreeTypesData
    {
        public List<CraftTreeType> TreeTypes;

        public CrafTreeTypesData() { }

        public CrafTreeTypesData(List<CraftTreeType> treeTypes)
        {
            TreeTypes = treeTypes;
        }
    }    
}
