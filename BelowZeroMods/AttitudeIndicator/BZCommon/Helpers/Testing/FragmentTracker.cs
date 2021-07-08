using System.Collections;
using UnityEngine;

namespace BZCommon.Helpers.Testing
{
    public class FragmentTracker : MonoBehaviour
    {        
        PingInstance pingInstance;
        PrefabIdentifier prefabIdentifier;
        private bool isSignalReady = false;
        
        public void Awake()
        {
            pingInstance = gameObject.EnsureComponent<PingInstance>();
            pingInstance.origin = transform;

            prefabIdentifier = gameObject.GetComponent<PrefabIdentifier>();

            if (prefabIdentifier == null)
            {
                DestroyImmediate(this);
            }

            isSignalReady = true;
        }

        public void OnDestroy()
        {
            if (pingInstance != null)
            {
                PingManager.Unregister(pingInstance);
            }
        }

        public void OnEnable()
        {
            StartCoroutine(EnableSignalAsync());
        }

        private IEnumerator EnableSignalAsync()
        {
            while (!isSignalReady)
            {
                yield return null;
            }

            
            pingInstance.displayPingInManager = true;
            pingInstance.pingType = PingType.Signal;
            pingInstance.visible = true;
            pingInstance.minDist = 5;
            pingInstance.range = 10;
            pingInstance.SetType(PingType.Signal);
            pingInstance.SetLabel(prefabIdentifier.ClassId);
            pingInstance.SetColor(2);

            yield break;
        }
    }
}
