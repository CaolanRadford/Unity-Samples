using System;
using System.Collections;
using AbstractClasses;
using UnityEngine;

namespace Services
{
    public class DamageEffects : Service, IGM_ServiceLocator_Recievable
    {
        //design variables
        [SerializeField] [Tooltip("Material To Override")]
        private Material materialOverride;

        [SerializeField] [Tooltip("How long to pause the game for")]
        private float pauseTime;
        
        //fields
        private GM_ServiceLocator gm;
        private float temp;

        private void Start()
        {
            temp = pauseTime;
        }

        public IEnumerator PauseGameAndInvertColours()
        {
            gm.GetService<CameraPostProcessingRenderTextureOverride>().SetCameraOverride(materialOverride, pauseTime);
        
            yield return new WaitForSeconds(.03f);
        
            while (pauseTime >= 0)
            {
                pauseTime -= .1f;
//            print("damage effects called, length of time: " + pauseLengthOfTime.ToString());

                Time.timeScale = 0;

                yield return null;
            }

            Time.timeScale = 1;
            pauseTime = temp;
        }

        public void AssignGM(GM_ServiceLocator serviceLocator)
        {
            gm = serviceLocator;
        }
    }
}
