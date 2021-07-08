using System.Collections.Generic;

namespace BZCommon.Helpers.RuntimeGUI
{
    public interface IGUI
    {        
        void WakeUp();        
        void GetWindows(ref List<Window> windows);
        void GetGroups(ref List<Group> groups);
        void GUIEvent(GUI_event gui_Event);
        void OnWindowClose(int windowID);
        void ShowMainWindow();
    }
}

