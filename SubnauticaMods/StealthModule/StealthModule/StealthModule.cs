using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StealthModule
{
    public enum StealthQuality
    {
        None,
        Low,
        Medium,
        High,
        Debug
    }

    public class StealthModule : MonoBehaviour
    {
        public StealthQuality quality = StealthQuality.None;
        public StealthModule(StealthQuality inputQuality)
        {
            quality = inputQuality;
        }
    }
}
