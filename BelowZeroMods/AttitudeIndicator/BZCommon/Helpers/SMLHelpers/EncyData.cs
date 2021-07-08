using UnityEngine;

namespace BZCommon.Helpers.SMLHelpers
{
    public class EncyData
    {
        public string title;
        public string description;
        public EncyNode node;
        public Texture2D image;

        public EncyData()
        {
        }

        public EncyData(string encyTitle, string encyText, EncyNode encyNode, Texture2D encyPic)
        {
            title = encyTitle;
            description = encyText;
            node = encyNode;
            image = encyPic;
        }
    }
}
