using System;
using System.Collections.Generic;
using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public struct Window
    {
        public int windowID;
        public Rect windowRect;
        public string titleText;
        public bool enabled;
        public bool hasMoveable;
        public bool hasMinimizeButton;
        public bool hasCloseButton;
        public bool hasToolTipButton;
        public bool hasTimeInTitle;

        public Window
            (
            int windowID,
            Rect windowRect,
            string titleText,
            bool enabled = true,
            bool hasMoveable = true,
            bool hasMinimizeButton = true,
            bool hasCloseButton = true,
            bool hasToolTipButton = false,
            bool hasTimeInTitle = false
            )
        {
            this.windowID = windowID;
            this.windowRect = windowRect;
            this.titleText = titleText;
            this.enabled = enabled;
            this.hasMoveable = hasMoveable;
            this.hasMinimizeButton = hasMinimizeButton;
            this.hasCloseButton = hasCloseButton;
            this.hasToolTipButton = hasToolTipButton;
            this.hasTimeInTitle = hasTimeInTitle;
        }
    }

    public struct Group
    {
        public int windowID;
        public int groupID;
        public GUI_Group_type groupType;
        public List<GUI_content> itemsContent;
        public string groupLabel;
        public int columns;
        public GUI_Layout layout;
        public int horizontalSpace;
        public int verticalSpace;
        public int itemHeight;
        public int maxShowItems;

        public Group
            (
            int windowID,
            int groupID,
            GUI_Group_type groupType,
            List<GUI_content> itemsContent,
            string groupLabel = "",
            int columns = 1,
            GUI_Layout layout = GUI_Layout.RightAndDown,
            int horizontalSpace = 5,
            int verticalSpace = 5,
            int itemHeight = 24,
            int maxShowItems = 1
            )
        {
            this.windowID = windowID;
            this.groupID = groupID;
            this.groupType = groupType;
            this.itemsContent = itemsContent;
            this.groupLabel = groupLabel;
            this.columns = columns;
            this.layout = layout;
            this.horizontalSpace = horizontalSpace;
            this.verticalSpace = verticalSpace;
            this.itemHeight = itemHeight;
            this.maxShowItems = maxShowItems;
        }

        public List<List<GUI_content>> GetContentMatrix()
        {
            List<List<GUI_content>> rowContents = new List<List<GUI_content>>();

            List<GUI_content> tempList = new List<GUI_content>();

            int rows = GetTotalRows();

            int column = 1;
            int row = 0;

            foreach (GUI_content content in itemsContent)
            {
                tempList.Add(content);

                if (column == columns)
                {
                    rowContents.Add(tempList.ShallowCopy());
                    tempList.Clear();
                    column = 1;
                    row++;
                    continue;
                }

                column++;
            }

            if (row != rows)
            {
                rowContents.Add(tempList.ShallowCopy());
            }

            return rowContents;
        }

        public int GetTotalRows()
        {
            return (int)Math.Ceiling(itemsContent.Count / (float)columns);
        }
    }


    public struct GUI_event
    {
        public readonly int WindowID;
        public readonly int GroupID;
        public readonly int ItemID;
        public readonly int MouseButton;
        public readonly float Value;
        public GUI_item guiItem;

        public GUI_event(int windowID, int groupID, GUI_item guiItem, float value)
        {
            WindowID = windowID;
            this.guiItem = guiItem;
            GroupID = groupID;
            ItemID = guiItem.ID;
            MouseButton = 0;
            Value = value; 
        }

        public GUI_event(int windowID, int groupID, GUI_item guiItem, int mouseButton)
        {
            WindowID = windowID;
            this.guiItem = guiItem;
            GroupID = groupID;
            ItemID = guiItem.ID;
            MouseButton = mouseButton;
            Value = 0;
        }
    }

    public class GUI_textColor
    {
        public GUI_textColor
            (
            GUI_Color normal = GUI_Color.Grey,
            GUI_Color hover = GUI_Color.White,
            GUI_Color active = GUI_Color.Green,
            GUI_Color marked = GUI_Color.Yellow,
            GUI_Color disabled = GUI_Color.Gray
            )
        {
            Normal = normal;
            Hover = hover;
            Active = active;
            Marked = marked;
            Disabled = disabled;
        }
        
        public GUI_Color Normal { get; set; }
        public GUI_Color Hover { get; set; }
        public GUI_Color Active { get; set; }
        public GUI_Color Marked { get; set; }
        public GUI_Color Disabled { get; set; }
    }
}
