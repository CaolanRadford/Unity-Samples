using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbstractClasses;
using Adaptors;
using Components;
using K_LanUtilities;
using ScriptableObjectDefinitions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Services.SceneManagement
{
//exit conditions for Main scene
    [RequireComponent(typeof(Animator))]
    public class Main : Service, ISoundPlayableOneShot
    {
        //defined interfaces
        public delegate void LevelCompleting();
        public event LevelCompleting OnLevelComplete;
        private IMainLevelComplete_Updateable[] mainLevelCompleteUpdatables;

        //implemented interfaces
        public event AudioSourceAdapter.ToPlaySoundOneShot OnSoundPlayOneShot;

        //dependencies
        private Animator animator;
        private UnityEngine.AudioSource source;
        private PlayerInput2D[] playerInputComponents;

        //design variables
        [FormerlySerializedAs("audioLib")] [SerializeField]
        private RatbagProjectWideAudioLibrary wideAudioLib;

        void OnEnable()
        {
            animator = GetComponent<Animator>();
            source = GetComponent<UnityEngine.AudioSource>();
            playerInputComponents = FindObjectsOfType<PlayerInput2D>();

            SetUpLevelCompleteInterfaces();
        }

        void OnDisable() 
        {
            DisolveLevelCompleteInterfaces();
        }

        public void LoadMain_AnimationWrapper()
        {
            StartCoroutine(LoadScene_Async());
        }

        public void InitiateLevelComplete()
        {
            animator.SetBool("success", true);

            OnLevelComplete?.Invoke();
        }

        public void PlayAudio_AnimationWrapper()
        {
            OnSoundPlayOneShot(wideAudioLib.cashMoney);
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        IEnumerator LoadScene_Async()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(3, LoadSceneMode.Single);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        private void SetUpLevelCompleteInterfaces()
        {
            mainLevelCompleteUpdatables = StaticUtilities.ReturnInterfaceImplementationsFromScene<IMainLevelComplete_Updateable>();

            for (int i = 0; i < mainLevelCompleteUpdatables.Length; i++)
            {
                OnLevelComplete += mainLevelCompleteUpdatables[i].OnMainLevelComplete;
            }
        }

        private void DisolveLevelCompleteInterfaces()
        {
            for (int i = 0; i < mainLevelCompleteUpdatables.Length; i++)
            {
                IMainLevelComplete_Updateable levelCompleteUpdate = mainLevelCompleteUpdatables[i];
                OnLevelComplete -= levelCompleteUpdate.OnMainLevelComplete;
            }
        }        
    }

    public interface IMainLevelComplete_Updateable
    {
        void OnMainLevelComplete();
    }
}