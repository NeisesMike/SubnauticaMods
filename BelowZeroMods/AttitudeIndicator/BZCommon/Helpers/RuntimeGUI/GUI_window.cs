//#define DEBUGMOUSE

using System;
using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public class GUI_window
    {
        public GUI_window(GUI_Base guiBase, Window window, WindowControlHandler controlHandler, WindowCloseHandler closeHandler)
        {
            GuiBase = guiBase;
            WindowRect = window.windowRect;
            TitleText = window.titleText;
            ControlHandler = new WindowControlHandler(controlHandler);
            CloseHandler = new WindowCloseHandler(closeHandler);
            _hasMoveable = window.hasMoveable;
            _hasMinimizeButton = window.hasMinimizeButton;
            _hasCloseButton = window.hasCloseButton;
            _hasToolTipButton = window.hasToolTipButton;
            _HasTimeInTitle = window.hasTimeInTitle;

            ID = window.windowID;
            Enabled = window.enabled;
            PreInitWindow();
        }
                
        private readonly WindowControlHandler ControlHandler;
        private readonly WindowCloseHandler CloseHandler;

        public int ID { get; }
        public string TitleText { get; set; }
        public bool Enabled { get; set; }
        public Rect WindowRect;
        public Rect RemainDrawableArea { get; set; }

        public readonly GUI_Base GuiBase;
        private readonly bool _hasCloseButton;
        private readonly bool _hasMoveable;
        private readonly bool _hasMinimizeButton;
        private readonly bool _hasToolTipButton;
        private readonly bool _HasTimeInTitle;
        private Rect _titleRect;
        private Rect _labelRect;
        private Rect _timeRect;       
        private Rect _minimizeButtonRect;
        private Rect _closeButtonRect;        
        private int _titleHeight;
        private bool showWindow = true;
        private Rect _windowrectOriginal;
        private Rect _toolTipButtonRect;
        private bool showToolTipWindow = false;

        private GUIContent ToolTip = new GUIContent();
        private Rect toolTipRect;
        private float toolTipTimer = 2;
               
        /// <summary>
        /// Internal rect calculation function outside 'OnGUI'.
        /// This is minimize garbage from Rect initializations.
        /// Don't use 'new Rect()' call inside 'OnGUI'!
        /// </summary>
        private void PreInitWindow()
        {
            _windowrectOriginal = new Rect(0, 0, WindowRect.width, WindowRect.height);
           
            _titleHeight = Screen.height / 45;

            _titleRect = new Rect(0, 0, WindowRect.width, _titleHeight);
            
            _closeButtonRect = new Rect(_titleRect.width - _titleHeight, 0, _titleHeight, _titleHeight);
            _minimizeButtonRect = new Rect(_titleRect.width - (_titleHeight * 2), 0, _titleHeight, _titleHeight);
            _toolTipButtonRect = new Rect(_titleRect.width - (_titleHeight * 3), 0, _titleHeight, _titleHeight);
            _timeRect = new Rect(_titleRect.width - (_titleHeight * 3) - 60, 0, 60, _titleHeight);
            _labelRect = new Rect(5, 0, _timeRect.x - 5, _titleHeight);        

            RemainDrawableArea = new Rect(2, _titleHeight, WindowRect.width - 4, WindowRect.height - (int)(_titleHeight * 1.5f));            

            toolTipRect = new Rect(0, 0, 0, 0);
        }

        /// <summary>
        /// Draws the window.
        /// Call only inside 'GUIBase.OnGUI'!
        /// </summary>
        public void DrawWindow()
        {
            // Drawing is disabled when window is not enabled.  
            if (!Enabled)
            {
                return;
            }

            // Disable drawing when ingame menu is active because the GUI crashing!
            if (IngameMenu.main != null && IngameMenu.main.isActiveAndEnabled)
            {
                return;
            }

            // If window is minimized draw the title box only.
            if (showWindow)
            {
                WindowRect.height = _windowrectOriginal.height;
            }
            else
            {
                WindowRect.height = _titleHeight;                
            }

            // Initializing and setting the new GUI Skin.
            GUI.skin = GUI_style.GetSkin();
            
            // Make a popup window.
            WindowRect = GUI.Window(ID, WindowRect, DoWindow, "");

            if (showToolTipWindow && toolTipTimer > 0)
            {
                GUIStyle guiStyle = GUI_style.GetGuiStyle(GUI_Item_Type.TEXTAREA, GUI_Color.Green, align: TextAnchor.UpperLeft, wordWrap: true);

                Vector2 toolTipSize = guiStyle.CalcSize(ToolTip);

                toolTipRect.width = toolTipSize.x < 200 ? toolTipSize.x : 200;

                if (WindowRect.x < Screen.width / 2)
                {
                    toolTipRect.x = WindowRect.x + WindowRect.width + 2;
                }
                else
                {
                    toolTipRect.x = WindowRect.x - (toolTipRect.width + 2);
                }                            

                toolTipRect.y = Event.current.mousePosition.y;
                
                toolTipRect.height = guiStyle.CalcHeight(ToolTip, toolTipRect.width);

                GUI.Label(toolTipRect, ToolTip.text, guiStyle);
            }
        }
                
        /// <summary>
        /// Internal method to draw and control the window base.
        /// </summary>
        private void DoWindow(int id)
        {
            // Drawing the title box.
            GUI.Box(_titleRect, "", GUI_style.GetGuiStyle(GUI_Item_Type.TITLEBOX));

            // Drawing the title text.
            GUI.Label(_labelRect, TitleText, GUI_style.GetGuiStyle(GUI_Item_Type.TITLETEXT, align: TextAnchor.MiddleLeft));

            // Drawing the system time in title box if need.
            if (_HasTimeInTitle)
            {                
                GUI.Label(_timeRect, DateTime.Now.ToString("HH:mm:ss"), GUI_style.GetGuiStyle(GUI_Item_Type.TITLETEXT, align: TextAnchor.MiddleCenter));
            }

            // Drawing the minimize button if need.
            if (_hasMinimizeButton)
            {
                if (GUI.Button(_minimizeButtonRect, showWindow ? "\u2582" : "\u25A0", GUI_style.GetGuiStyle(GUI_Item_Type.TITLEBUTTON, align: TextAnchor.UpperCenter)))
                {
                    showWindow = !showWindow;
                    
                    if (!showToolTipWindow)
                    {
                        ToolTip.text = "";
                    }                    
                }
            }

            // Drawing the close button if need.
            if (_hasCloseButton)
            {                
                if (GUI.Button(_closeButtonRect, "\u03A7", GUI_style.GetGuiStyle(GUI_Item_Type.TITLEBUTTON, align: TextAnchor.MiddleCenter)))
                {
                    Enabled = false;
                    CloseHandler(ID);
                }
            }            
              
            if (showWindow)
            {
                // Showing tooltips if enabled.
                if (_hasToolTipButton)
                {
                    if (GUI.Button(_toolTipButtonRect, "?", GUI_style.GetGuiStyle(GUI_Item_Type.TITLEBUTTON, align: TextAnchor.MiddleCenter, colorNormal: showToolTipWindow ? GUI_Color.Green : GUI_Color.White)))
                    {
                        showToolTipWindow = !showToolTipWindow;

                        if (!showToolTipWindow)
                        {
                            ToolTip.text = "";
                        }
                    }
                }

                // Calling base window control function.                
                ControlHandler(ID);                
            }            

            // Calling Unity DragWindow function if window moveable.
            // Moving enabled only by grab the title bar rectangle.
            if (_hasMoveable)
            {                
                GUI.DragWindow(_titleRect);                
            }

            if (showToolTipWindow)
            {

#if DEBUGMOUSE
                ToolTip.text = $"Mouse position (x: {Event.current.mousePosition.x}, y: {Event.current.mousePosition.y})";                
#else
                if (GUI.tooltip != "")
                {
                    ToolTip.text = GUI.tooltip;

                    toolTipTimer = 0.2f;
                }

                if (toolTipTimer > 0)
                {
                  toolTipTimer -= Time.deltaTime;
                }
                else
                {
                    ToolTip.text = "";                    
                }
#endif
            }
        }
    }
}
