using System.Collections.Generic;
using UnityEngine;

namespace BZCommon.Helpers.GUIHelper
{
    public static class SNScrollView
    {
        public static GuiItemEvent CreateScrollView(Rect scrollRect, ref Vector2 scrollPos, ref List<GuiItem> scrollItems, string label, string listName, int maxShowItems = 0)
        {
            Vector2 labelSize = SNStyles.GetGuiItemStyle(GuiItemType.LABEL).CalcSize(new GUIContent(label));

            if (maxShowItems > 0)
            {
               scrollRect.height = maxShowItems * (scrollItems[0].Rect.height + 2);                
            }
            else
            {
                scrollRect.height = scrollRect.height - labelSize.y + 10;
            }

            GUI.Label(new Rect(scrollRect.x, scrollRect.y + 5, labelSize.x, labelSize.y), label, SNStyles.GetGuiItemStyle(GuiItemType.LABEL, textAnchor: TextAnchor.MiddleLeft));

            GUI.Label(new Rect(scrollRect.x + labelSize.x + 5, scrollRect.y + 5, scrollRect.width - labelSize.x, labelSize.y), listName, SNStyles.GetGuiItemStyle(GuiItemType.LABEL, GuiColor.Green, textAnchor: TextAnchor.MiddleLeft));

            scrollPos = GUI.BeginScrollView(new Rect(scrollRect.x, scrollRect.y + labelSize.y + 10, scrollRect.width, scrollRect.height), scrollPos, new Rect(scrollItems[0].Rect.x, scrollItems[0].Rect.y, scrollItems[0].Rect.width, scrollItems.Count * (scrollItems[0].Rect.height + 2)));

            GuiItemEvent result = scrollItems.DrawGuiItemsGroup();

            GUI.EndScrollView();

            return result;
        }
    }
}
