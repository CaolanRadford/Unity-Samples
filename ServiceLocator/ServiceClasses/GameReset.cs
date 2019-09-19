using System;
using System.Collections;
using AbstractClasses;
using Adaptors;
using Components;
using Controllers;
using K_LanUtilities;
using ScriptableObjectDefinitions;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;


namespace Services
{
    //defines the spawn time 
    //moves the players transform
    //tells the vent to open
    public class GameReset : Service, IGM_ServiceLocator_Recievable, ISoundPlayableOneShot
    {        
        //implemented interfaces
        public event AudioSourceAdapter.ToPlaySoundOneShot OnSoundPlayOneShot;

        private CameraPostProcessingRenderTextureOverride cam;
        
        public static event Action PlayerReset;
        
        //dependencies
        private RatbagPlayerBrain defaultPlayerBrain;
        private Scene thisScene;
        private GM_ServiceLocator gm;
        private RatbagProjectWideAudioLibrary audioLib;

        [Header("Reset variables")]

        //design variables
        [SerializeField][Tooltip("The time that the player is disabled (kinematic) for, while spawning")]
        private float spawnTime = .5f;    

        [SerializeField] [Tooltip("How many times do we reset the player before loading the scene again?")]
        private int maxRestartCount = 5;           

        [SerializeField][Tooltip("How long do the lights come on automatically on reset?")]
        private float spawningLightsTimer;
        
        [Header("Camera override")]
        [SerializeField][Tooltip("What texture overrides the camera briefly on reset?")]
        private Texture textureOverride;
        
        [SerializeField][Tooltip("What material overrides the camera briefly on reset?")]
        private Material materialOverride;
        
        [SerializeField][Tooltip("How long does the overlaid render texture stick around?")]
        private float renderTextureTime;

        //fields
        private int currentRestartCount = 0;
//        private float temp;

        void OnEnable()
        {
            audioLib = StaticUtilities.ReturnDefaultAudioLibrary();
            
            defaultPlayerBrain = FindObjectOfType<RatbagPlayerBrain>();

            thisScene = SceneManager.GetActiveScene();
        }

        void Start()
        {
            StartCoroutine(ResetPlayer(defaultPlayerBrain.gameObject));

            InitialiseRestartCount();
        }

        private void InitialiseRestartCount()
        {
            currentRestartCount = 0;
        }

        public IEnumerator ResetPlayer(GameObject player)
        {
//            print("static time: " + renderTextureTime);
            if(currentRestartCount < maxRestartCount)
            {
                OnSoundPlayOneShot?.Invoke(audioLib.TVStatic);
                
                RatbagSpawner selected = SelectAVent();

                PlayerTransformToSpawner(selected, player.transform);
                
                gm.GetService<CameraPostProcessingRenderTextureOverride>().SetCameraOverride(materialOverride, textureOverride, renderTextureTime);
                PlayerReset?.Invoke();

                yield return new WaitForSeconds(.5f);

                selected.OpenVent();

                TurnOnLightsTimer(spawningLightsTimer);

                currentRestartCount++;

            }
            else
            {
                ReloadScene();
            }
        }

        private void TurnOnLightsTimer(float timer)
        {
            gm.GetService<LightsController>().ManualLightsSwitch_Timer(timer);
        }

        private void PlayerTransformToSpawner(RatbagSpawner selected, Transform playerT)
        {
            playerT.transform.position = selected.transform.position;
        }

        private RatbagSpawner SelectAVent()
        {
            return gm.GetService<RatbagSpawnerManager>().GetRandomSpawner();
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(thisScene.name, LoadSceneMode.Single);
        }

        public float GetSpawningTime()
        {
            return spawnTime;
        }

        void IGM_ServiceLocator_Recievable.AssignGM(GM_ServiceLocator serviceLocator)
        {
            gm = serviceLocator;
        }

    }
}
