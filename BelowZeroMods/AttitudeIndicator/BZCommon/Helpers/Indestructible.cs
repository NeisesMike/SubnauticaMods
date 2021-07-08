using UnityEngine;

namespace BZCommon.Helpers
{
    public class Indestructible : MonoBehaviour
    {
        public void Awake()
        {
            SceneCleanerPreserve scp = gameObject.AddComponent<SceneCleanerPreserve>();
            scp.enabled = true;
            DontDestroyOnLoad(gameObject);
        }
    }
}
