using System.Collections.Generic;
using UnityEngine;
using UWE;
using static BZCommon.InputHelper;

namespace BZCommon.Helpers.GUIHelper
{
    public interface IGuiItem
    {
        bool Enabled { get; set; }
        FontStyle FontStyle { get; set; }        
        string Name { get; set; }
        GuiItemColor ItemColor{ get; set; }
        Rect Rect { get; set; }
        GuiItemState State { get; set; }
        TextAnchor TextAnchor { get; set; }
        GuiItemType Type { get; set; }
    }

    public struct GuiItemEvent
    {
        public readonly int ItemID;
        public readonly int MouseButton;
        public readonly bool DoubleClick;

        public GuiItemEvent(int itemID, int mouseButton, bool doubleClick)
        {
            ItemID = itemID;
            MouseButton = mouseButton;
            DoubleClick = doubleClick;
        }
    }

    public enum GuiItemType
    {
        NORMALBUTTON,
        TOGGLEBUTTON,
        TAB,
        LABEL,
        TEXTFIELD,
        TEXTAREA,
        BOX,
        DROPDOWN,
        HORIZONTALSLIDER,
        TITLETEXT
    }

    public enum GuiItemState
    {
        NORMAL,
        PRESSED
    }
    
    public enum GuiColor
    {      
        Black,
        Blue,
        Clear,
        Cyan,
        Green,
        Gray,
        Grey,
        Magenta,
        Red,
        White,
        Yellow
    }

    public class GuiItemColor
    {
        public GuiColor Normal { get; set; }
        public GuiColor Hover  { get; set; }
        public GuiColor Active { get; set; }

        public GuiItemColor(GuiColor normal = GuiColor.Gray, GuiColor active = GuiColor.Green, GuiColor hover = GuiColor.White)
        {
            Normal = normal;
            Hover = hover;
            Active = active;
        }             
    }


    public class GuiItem : IGuiItem
    {
        public GuiItem()
        {
        }

        public GUIContent Content
        {
            get
            {
                return new GUIContent(Name)
                {
                    tooltip = Tooltip
                };
                
            }
        }

        public string Name { get; set; }
        public string Tooltip { get; set; }
        public GuiItemType Type { get; set; }
        public bool Enabled { get; set; }
        public Rect Rect { get; set; }
        public GuiItemColor ItemColor { get; set; }
        public FontStyle FontStyle { get; set; }
        public TextAnchor TextAnchor { get; set; }
        public GuiItemState State { get; set; }
        public Event<object> OnChangedEvent { get; set; }
    }

    public static class SNGUI
    {
        public static bool CreateGuiItemsGroup(
            this List<GuiItem> guiItems,
            string[] names,
            List<Rect> rects,
            GuiItemType type,
            GuiItemColor itemColor,
            string toolTip = null,
            GuiItemState state = GuiItemState.NORMAL,
            bool enabled = true,
            FontStyle fontStyle = FontStyle.Normal,
            TextAnchor textAnchor = TextAnchor.MiddleCenter,
            Event<object> onChangedEvent = null)
        {
            guiItems.Clear();

            for (int i = 0; i < names.Length; i++)
            {
                guiItems.Add(new GuiItem()
                {
                    Name = names[i],
                    Tooltip = toolTip,
                    Type = type,
                    Enabled = enabled,
                    Rect = rects[i],
                    ItemColor = itemColor,
                    State = state,
                    FontStyle = fontStyle,
                    TextAnchor = textAnchor,
                    OnChangedEvent = onChangedEvent
                });
            }

            return true;
        }

        public static bool CreateGuiItemsGroup(
            this List<GuiItem> guiItems,
            List<string> names,
            List<Rect> rects,
            GuiItemType type,
            GuiItemColor itemColor,
            string toolTip = null,
            GuiItemState state = GuiItemState.NORMAL,
            bool enabled = true,
            FontStyle fontStyle = FontStyle.Normal,
            TextAnchor textAnchor = TextAnchor.MiddleCenter,
            Event<object> onChangedEvent = null)
        {
            guiItems.Clear();

            for (int i = 0; i < names.Count; i++)
            {
                guiItems.Add(new GuiItem()
                {
                    Name = names[i],
                    Tooltip = toolTip,
                    Type = type,
                    Enabled = enabled,
                    Rect = rects[i],
                    ItemColor = itemColor,
                    State = state,
                    FontStyle = fontStyle,
                    TextAnchor = textAnchor,
                    OnChangedEvent = onChangedEvent
                });
            }

            return true;
        }

        public static bool AddGuiItemToGroup(this List<GuiItem> guiItems, string name)
        {
            int itemsCount = guiItems.Count;

            GuiItem lastItem1 = guiItems[itemsCount - 1];
            GuiItem lastItem2 = guiItems[itemsCount - 2];

            Rect lastRect1 = lastItem1.Rect;
            Rect lastRect2 = lastItem2.Rect;

            Rect nextRect = new Rect(lastRect1.x, lastRect1.y + (lastRect1.y - lastRect2.y), lastRect1.width, lastRect1.height);

            guiItems.Add(new GuiItem()
            {
                Name = name,
                Tooltip = null,
                Type = lastItem1.Type,
                Enabled = true,
                Rect = nextRect,
                ItemColor = lastItem1.ItemColor,
                State = GuiItemState.NORMAL,
                FontStyle = lastItem1.FontStyle,
                TextAnchor = lastItem1.TextAnchor,
                OnChangedEvent = null
            });

            return true;
        }

        public static bool RemoveGuiItemFromGroup(this List<GuiItem> guiItems, int index)
        {
            int itemsCount = guiItems.Count;

            if (index >= itemsCount)
            {
                return false;
            }

            if (itemsCount < 2 || index == itemsCount - 1)
            {
                guiItems.RemoveAt(index);
                return true;
            }
            
            GuiItem firstItem = guiItems[0];
            GuiItem secondItem = guiItems[1];

            float distanceY = secondItem.Rect.y - firstItem.Rect.y;

            float x = guiItems[index].Rect.x;
            float y = guiItems[index].Rect.y;
            float width = guiItems[index].Rect.width;
            float height = guiItems[index].Rect.height;

            guiItems.RemoveAt(index);

            int i = index;
            int j = 0;

            do
            {
                guiItems[i].Rect = new Rect(x, y + (j * distanceY), width, height);
                i++;
                j++;
            }
            while (i < guiItems.Count);

            return true;
        }

        public static void SetGuiItemsGroupLabel(this List<GuiItem> guiItems, string name, Rect rect, GuiItemColor itemColor,
                                                 FontStyle fontStyle = FontStyle.Normal, TextAnchor textAnchor = TextAnchor.MiddleLeft)
        {
            guiItems.Add(new GuiItem()
            {
                Name = name,
                Type = GuiItemType.LABEL,
                Enabled = true,
                Rect = rect,
                ItemColor = itemColor,
                FontStyle = fontStyle,
                TextAnchor = textAnchor
            });
        }

        //to be called from OnGui
        public static GuiItemEvent DrawGuiItemsGroup(this List<GuiItem> guiItems)
        {
            var e = Event.current;

            for (int i = 0; i < guiItems.Count; ++i)
            {
                if (!guiItems[i].Enabled)
                {
                    continue;
                }

                switch (guiItems[i].Type)
                {
                    case GuiItemType.NORMALBUTTON:

                        if (GUI.Button(guiItems[i].Rect, guiItems[i].Content, SNStyles.GetGuiItemStyle(guiItems[i])))
                        {                            
                            return new GuiItemEvent(i, e.button, false);
                        }
                        break;

                    case GuiItemType.TOGGLEBUTTON:

                        if (GUI.Button(guiItems[i].Rect, guiItems[i].Content, SNStyles.GetGuiItemStyle(guiItems[i])))
                        {
                            //guiItems[i].State = SetStateInverse(guiItems[i].State);
                            return new GuiItemEvent(i, e.button, false);
                        }
                        break;

                    case GuiItemType.TAB:
                        if (GUI.Button(guiItems[i].Rect, guiItems[i].Content, SNStyles.GetGuiItemStyle(guiItems[i])))
                        {
                            if (e.button == 0)
                            {
                                SetStateInverseTAB(guiItems, i);
                            }

                            return new GuiItemEvent(i, e.button, false);
                        }
                        break;

                    case GuiItemType.LABEL:
                        GUI.Label(guiItems[i].Rect, guiItems[i].Name, SNStyles.GetGuiItemStyle(guiItems[i]));
                        break;

                    case GuiItemType.TEXTFIELD:
                        GUI.TextArea(guiItems[i].Rect, guiItems[i].Name, SNStyles.GetGuiItemStyle(guiItems[i]));
                        break;
                }

            }

            return new GuiItemEvent(-1, -1, false);
        }

        public static void SetStateInverseTAB(this List<GuiItem> guiItems, int setActiveState)
        {
            for (int i = 0; i < guiItems.Count; i++)
            {
                if (i == setActiveState)
                    guiItems[i].State = GuiItemState.PRESSED;
                else
                    guiItems[i].State = GuiItemState.NORMAL;
            }
        }

        public static int GetMarkedItem(this List<GuiItem> guiItems)
        {
            for (int i = 0; i < guiItems.Count; i++)
            {
                if (guiItems[i].State == GuiItemState.PRESSED)
                {
                    return i;
                }
            }

            return -1;
        }

        public static void SetStateInverse(this GuiItem guiItem)
        {
            guiItem.State = InvertState(guiItem.State);
        }

        public static void UnmarkAll(this List<GuiItem> guiItems)
        {
            for (int i = 0; i < guiItems.Count; i++)
            {
                guiItems[i].State = GuiItemState.NORMAL;               
            }
        }

        public static GuiItemState InvertState(GuiItemState state)
        {
            return state == GuiItemState.NORMAL ? GuiItemState.PRESSED : GuiItemState.NORMAL;
        }

        public static bool ConvertStateToBool(GuiItemState state)
        {
            return state == GuiItemState.PRESSED ? true : false;
        }

        public static GuiItemState ConvertBoolToState(bool pressed)
        {
            return pressed ? GuiItemState.PRESSED : GuiItemState.NORMAL;
        }
    }
}
