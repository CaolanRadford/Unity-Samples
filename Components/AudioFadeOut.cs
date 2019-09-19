using System.Collections;
using UnityEngine;

namespace Components
{
    public class AudioFadeOut : MonoBehaviour
    {
        public static IEnumerator FadeOut (UnityEngine.AudioSource audioSource, float fadeTime) {
            float startVolume = audioSource.volume;
 
            while (audioSource.volume > 0) {
                audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
 
                yield return null;
            }
 
            audioSource.Stop ();
            audioSource.volume = startVolume;
        }
    }
}
