using Controllers;
using K_LanUtilities;
using UnityEngine;

namespace Components
{
    public class DontDestroy : MonoBehaviour
    {
        void OnEnable()
        {
            var objs = StaticUtilities.ReturnInterfaceImplementationsFromScene<CrowdSounds>();

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);
        }
    }
}
