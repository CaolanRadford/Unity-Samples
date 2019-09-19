using System.Collections;
using AbstractClasses;
using Adaptors;
using Components;
using ScriptableObjectDefinitions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

//Opening scene with play button
namespace Services.SceneManagement
{
    [RequireComponent(typeof(AudioSourceAdapter))]
    [RequireComponent(typeof(Animator))]
    public class Opening : Service, ISoundPlayableOneShot
    {
        //implemented interfaces
        public event AudioSourceAdapter.ToPlaySoundOneShot OnSoundPlayOneShot;

        //dependencies
        private Animator animator;

        //design variables
        [FormerlySerializedAs("audioLib")] [SerializeField]
        private RatbagProjectWideAudioLibrary wideAudioLib;

        void OnEnable()
        {
            animator = GetComponent<Animator>();
        }

        public void PlaySound_AnimationWrapper()
        {
            OnSoundPlayOneShot(wideAudioLib.coinInsert);
        }

        public void LoadMainScene_AnimationWrapper()
        {
            StartCoroutine(LoadScene_Async());
        }

        void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                animator.SetBool("start", true);
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        IEnumerator LoadScene_Async()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}
