using System;
using System.Collections.Generic;
using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public class GUI_Base : MonoBehaviour
    {
        internal IGUI iGUI;

        internal GUI_Base main;

        private List<GUI_window> guiWindows = new List<GUI_window>();

        private List<GUI_group> guiGroups = new List<GUI_group>();
                
        private void Awake()
        {
            main = this;            

            iGUI = gameObject.GetComponent<IGUI>();

            iGUI.WakeUp();

            CreateWindows();

            CreateGroups();
        }

        private void OnGUI()
        {
            foreach (GUI_window guiWindow in guiWindows)
            {
                guiWindow.DrawWindow();
            }
        }

        private void ControlHandler(int windowID)
        {
            foreach (GUI_group guiGroup in guiGroups)
            {
                if (guiGroup.windowID == windowID)
                {
                    guiGroup.DrawGroup();
                }
            }
        }

        private void CloseHandler(int windowID)
        {
            iGUI.OnWindowClose(windowID);
        }

        private void EventControl(GUI_event guiEvent)
        {
            iGUI.GUIEvent(guiEvent);
        }        

        private void CreateWindows()
        {
            List<Window> windows = new List<Window>();

            iGUI.GetWindows(ref windows);

            foreach (Window window in windows)
            {
                guiWindows.Add(new GUI_window(this, window, ControlHandler, CloseHandler));
            }
        }

        private void CreateGroups()
        {
            List<Group> groups = new List<Group>();

            iGUI.GetGroups(ref groups);

            foreach (Group group in groups)
            {
                GUI_window thisWindow = GetWindowByID(group.windowID);

                switch (group.groupType)
                {
                    case GUI_Group_type.Normal:
                        guiGroups.Add(new GUI_group(thisWindow, group, thisWindow.RemainDrawableArea, EventControl));
                        break;
                    case GUI_Group_type.Scroll:
                        guiGroups.Add(new GUI_scrollView(thisWindow, group, thisWindow.RemainDrawableArea, EventControl));
                        break;
                }
                
            }
        }

        public GUI_window GetWindowByID(int ID)
        {
            foreach (GUI_window window in guiWindows)
            {
                if (window.ID == ID)
                {
                    return window;
                }
            }

            return null;
        }

        public void RefreshGroup(int windowID, int groupID)
        {
            foreach (GUI_group group in guiGroups)
            {                
                if (group.windowID == windowID && group.groupID == groupID)
                {
                    group.Refresh();
                }
            }
        }

        public void SetGroupLabel(int windowID, int groupID, string newName)
        {
            foreach (GUI_group group in guiGroups)
            {
                if (group.windowID == windowID && group.groupID == groupID)
                {
                    group.SetLabel(newName);
                }
            }
        }

        public void EnableWindow(int windowID)
        {
            GUI_window window = GetWindowByID(windowID);

            if (window != null)
            {
                window.Enabled = true;
            }
        }

        public void DisableWindow(int windowID)
        {
            GUI_window window = GetWindowByID(windowID);

            if (window != null)
            {
                window.Enabled = false;
            }
        }

        public void ShowMainWindow()
        {
            iGUI.ShowMainWindow();
        }
    }
}
