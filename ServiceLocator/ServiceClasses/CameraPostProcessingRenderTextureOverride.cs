using System;
using System.Collections;
using AbstractClasses;
using UnityEngine;

namespace Services
{
    public class CameraPostProcessingRenderTextureOverride : Service
    {
        //fields
        private IEnumerator overridingCameraCoroutine;
        public Material materialToRender;
        public bool overridingCameraImage = false;
        public bool overrideTexture;
        private Texture textureToRender;

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (overridingCameraImage)
            {
                if (overrideTexture)
                {
                    if (textureToRender != null)
                    {
                        Graphics.Blit(textureToRender, dest, materialToRender);
                        return;   
                    }
                }

                if (materialToRender != null)
                {
                    Graphics.Blit(src, dest, materialToRender);
                }
            }

            else
            {
                Graphics.Blit(src, (RenderTexture) null);
            }
        }

        public void SetCameraOverride(Material newMatToRender, float newTime)
        {
            materialToRender = newMatToRender;

            if (overridingCameraCoroutine != null)
            {
                StopCoroutine(overridingCameraCoroutine);
            }

            overridingCameraCoroutine = SetRenderTextureOverride(newTime, false);
            StartCoroutine(overridingCameraCoroutine);
        }
        public void SetCameraOverride(Material newMatToRender,Texture newTextureToRender , float newTime)
        {
            materialToRender = newMatToRender;
            textureToRender = newTextureToRender;
            
            if (overridingCameraCoroutine != null)
            {
                StopCoroutine(overridingCameraCoroutine);
            }

            overridingCameraCoroutine = SetRenderTextureOverride(newTime, true);
            StartCoroutine(overridingCameraCoroutine);
        }

        private IEnumerator SetRenderTextureOverride(float timeOfOverride, bool useTextureSlot)
        {
            overridingCameraImage = true;

            if (useTextureSlot)
            {
                overrideTexture = true;
            }
            
            while (timeOfOverride >= 0)
            {
//                print("time of override: " + timeOfOverride);
                
                timeOfOverride -= Time.deltaTime;

                yield return null;
            }

            overridingCameraImage = false;
            overrideTexture = false;
        }
    }
}
