using System.Collections.Generic;
using UnityEngine;

namespace BZCommon.Helpers.Testing
{
    public class PrefabTest : MonoBehaviour
    {
        public VFXController fxControl;

        public List<Transform> allChilds;

        public bool isFXfound = false;

        public bool isFXplaying = false;

        public void Awake()
        {
            if (allChilds == null)
            {
                allChilds = new List<Transform>();
            }

            allChilds.Clear();

            print("PrefabTest: Awake Called");

            ScanObjectTransforms(transform);

            ScanForVFXController();
        }       


        public void ScanObjectTransforms(Transform transform)
        {
            foreach (Transform child in transform)
            {
                if (child.name.StartsWith("RH_"))
                {
                    continue;
                }

                allChilds.Add(child);

                print($"PrefabTest: found child: {child.name}");

                ScanObjectTransforms(child);
            }
        }

        public void ScanForVFXController()
        {
            foreach (Transform child in allChilds)
            {
                if (child.TryGetComponent(out VFXController controller))
                {
                    fxControl = controller;

                    isFXfound = true;

                    print($"PrefabTest: found VFXController: {fxControl.name}");

                    break;
                }
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isFXplaying = !isFXplaying;

                if (isFXplaying)
                {
                    PlayFX();
                }
                else
                {
                    StopFX();
                }
            }             
        }

        public void PlayFX()
        {
            if (fxControl != null)
            {
                print("PrefabTest: FX playing");

                fxControl.Play();

                return;                
            }
        }

        public void StopFX()
        {
            if (fxControl != null)
            {
                print("PrefabTest: FX stopped");
                fxControl.Stop();
                return;               
            }
        }
    }
}
