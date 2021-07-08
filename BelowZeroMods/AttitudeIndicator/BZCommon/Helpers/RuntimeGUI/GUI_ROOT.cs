using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public class GUI_ROOT : MonoBehaviour
    {
        public GUI_Base GuiBase { get; private set; }

        public void Awake()
        {
            gameObject.AddComponent<Indestructible>();
            GuiBase = gameObject.AddComponent<GUI_Base>();
        }
    }
}
