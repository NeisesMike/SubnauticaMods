namespace BZCommon.Helpers.RuntimeGUI
{
    public enum GUI_Item_Type
    {
        TITLEBOX,        
        TITLETEXT,
        TITLEBUTTON,
        NORMALBUTTON,
        TOGGLEBUTTON,
        TAB,
        LABEL,
        GROUPLABEL,
        TEXTFIELD,
        TEXTAREA,
        BOX,
        DROPDOWN,
        HORIZONTALSLIDER
    }

    public enum GUI_Group_type
    {
        Normal,
        Scroll        
    }

    public enum GUI_Item_State
    {
        NORMAL,
        PRESSED,
        MARKED,
        DISABLED
    }

    public enum GUI_Color
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

    public enum GUI_Layout
    {
        RightAndDown,
        DownAndRight
    }

    public enum GUI_Align
    {
        EquallyCentered,
        CloseToTheLeft,
        CloseToTheRight,
        CloseFirstLeft,
        CloseFirstRight
    }


    public enum AlphaMode
    {        
        Ignore,
        Change,
        Negative,
        Positive,
    }

    public enum ColorMode
    {
        Ignore,
        Change,
        Negative,
        Positive
    }    
}
