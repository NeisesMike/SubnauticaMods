using UnityEngine;

namespace BZCommon.Helpers.Testing
{
    public sealed class uGUIHelper
    {
        static uGUIHelper()
        {
            BZLogger.Debug("uGUIHelper created.");
        }

        private uGUIHelper()
        {
        }

        private static readonly uGUIHelper instance = new uGUIHelper();

        public static uGUIHelper main => instance;

        public static GameObject __uGUIHelper__ = null;
        public static uGUIListener listener = null;

        public static uGUIListener CreateuGUITest()
        {
            if (__uGUIHelper__ == null)
            {
                __uGUIHelper__ = new GameObject("__uGUIHelper__");

                return __uGUIHelper__.AddComponent<uGUIListener>();              
            }

            return listener;
        }
    }       

    public class uGUIListener : MonoBehaviour
    {
        public delegate void TestMethod();
        public TestMethod testMethod;
        private bool isTriggered = false;        

        public void Update()
        {
            if (!isTriggered && uGUI.isInitialized)
            {
                testMethod?.Invoke();                

                isTriggered = true;
            }
        }
    }
}
