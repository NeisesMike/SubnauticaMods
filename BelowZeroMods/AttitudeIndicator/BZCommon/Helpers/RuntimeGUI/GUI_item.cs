using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public class GUI_item
    {
        public GUI_item (int id, bool enabled, Rect rect, GUI_content itemContent)
        {            
            ID = id;            
            Enabled = enabled;
            DrawingRect = rect;
            ContentType = itemContent.ContentType;
            State = itemContent.InitialState;
            TextColor = itemContent.TextColor;
            Content = new GUIContent(itemContent.Text, itemContent.ToolTip ?? string.Empty);            
            FontStyle = itemContent.FontStyle;
            Align = itemContent.TextAlign;
            WordWrap = false;
            Clipping = TextClipping.Overflow;           
        }

        public int ID { get; }
        public bool Enabled { get; set; }
        public Rect DrawingRect { get; set; }
        public GUI_Item_Type ContentType { get; }
        public GUI_Item_State State { get; set; }
        public GUI_textColor TextColor { get; set; }
        public GUIContent Content { get; set; }
        public FontStyle FontStyle { get; set; }
        public TextAnchor Align { get; set; }
        public bool WordWrap { get; set; }
        public TextClipping Clipping { get; set; }

        public virtual float DrawItem()
        {
            Event eventCurrent = Event.current;

            switch (ContentType)
            {
                case GUI_Item_Type.NORMALBUTTON:
                case GUI_Item_Type.TOGGLEBUTTON:
                case GUI_Item_Type.TAB:

                    if (GUI.Button(DrawingRect, Content, GUI_style.GetGuiStyle(this)))
                    {
                        return eventCurrent.button;
                    }
                    break;

                case GUI_Item_Type.LABEL:
                case GUI_Item_Type.GROUPLABEL:

                    GUI.Label(DrawingRect, Content.text, GUI_style.GetGuiStyle(this));
                    break;

                case GUI_Item_Type.TEXTFIELD:

                    GUI.TextField(DrawingRect, Content.text, GUI_style.GetGuiStyle(this));
                    break;
            }

            return -1;
        }
               
        public void Enable()
        {
            Enabled = true;
        }

        public void Disable()
        {
            Enabled = false;
        }
        
        public GUI_Item_State GetState()
        {
            return State;
        }

        public bool GetStateBool()
        {
            return State == GUI_Item_State.PRESSED ? true : false;
        }

        public void SetState(GUI_Item_State state)
        {
            State = state;
        }

        public void SetState(bool state)
        {
            State = state ? GUI_Item_State.PRESSED : GUI_Item_State.NORMAL;
        }

        public void InvertState()
        {
            State = State == GUI_Item_State.NORMAL ? GUI_Item_State.PRESSED : GUI_Item_State.NORMAL; 
        }        

        public GUI_Item_Type GetItemType()
        {
            return ContentType;
        }

        public void SetText(string text)
        {
            Content.text = text;
        }

        public void SetToolTip(string toolTip)
        {
            Content.tooltip = toolTip;
        }

        public void SetImage(Texture texture)
        {
            Content.image = texture;
        }
    }
}
