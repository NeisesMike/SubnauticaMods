using UnityEngine;

namespace BZCommon.Helpers.GUIHelper
{
    public static class SNDropDown
    {
        public static void CreateDropdown(Rect rect, ref bool showList, ref int listEntry, GUIContent[] listContent)
        {
            int dropDownListHash = "DropDownList".GetHashCode();
            int controlID = GUIUtility.GetControlID(dropDownListHash, FocusType.Passive);
            bool done = false;

            Rect listRect = new Rect(rect.x, rect.y + rect.height, rect.width, /*Styles.GetGUIStyle(null, Button.BUTTONTYPE.NORMAL_CENTER).CalcHeight(listContent[0], 1.0f)*/ rect.height * listContent.Length);     
           
            if (Event.current.GetTypeForControl(controlID) == EventType.MouseDown)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl = controlID;
                    showList = !showList;                    
                }
            }

            if (Event.current.GetTypeForControl(controlID) == EventType.MouseUp && showList)
            {
                if (listRect.Contains(Event.current.mousePosition))
                {                    
                    done = true;                    
                }
            }
            
            GUI.Button(rect, listContent[listEntry]);

            if (showList)
            {                
                GUI.Box(listRect, "");
                
                listEntry = GUI.SelectionGrid(listRect, listEntry, listContent, 1, SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON));
            }            

            if (done)
            {
                showList = false;
            }            
        }
    }
}
