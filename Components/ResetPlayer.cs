using System.Collections;
using System.Collections.Generic;
using Controllers;
using K_LanUtilities;
using ScriptableObjectDefinitions;
using Services;
using UnityEngine;

namespace Components
{
//resets the player using the ResetPlayer_Service
    [RequireComponent(typeof(Bouncer))]
    public class ResetPlayer : MonoBehaviour, IGM_ServiceLocator_Recievable
    {
        //defined interfaces
        public delegate void ResettingThePlayerHandler(GameObject player);
        private IPlayerResettable[] playerResetters;
        
        //dependencies
        private GM_ServiceLocator gm;
        
        //Design variables
        [SerializeField][Tooltip("The texture to override the camera with")]
        public Texture overrideTexture;
        [SerializeField][Tooltip("The material to override the camera with")]
        private Material overrideMaterial;
        [SerializeField][Tooltip("How long to override the camera")]
        private float renderTextureOverrideTime;
        
        private RatbagProjectWideAudioLibrary audioLib;

        void OnEnable()
        {
            audioLib = StaticUtilities.ReturnDefaultAudioLibrary();
            
            playerResetters = StaticUtilities.ReturnOnlyActiveControllers<IPlayerResettable>(GetComponents<IPlayerResettable>(), this);

            for (int i = 0; i < playerResetters.Length; i++)
            {
                playerResetters[i].OnPlayerReset += CallingGameReset;
            }
        }

        void OnDisable()
        {
            for (int i = 0; i < playerResetters.Length; i++)
            {
                playerResetters[i].OnPlayerReset -= CallingGameReset;
            }        
        }

        public void AssignGM(GM_ServiceLocator serviceLocator)
        {
            gm = serviceLocator;
        }

        private void CallingGameReset(GameObject player)
        {
            StartCoroutine(gm.GetService<GameReset>().ResetPlayer(player));
        }
    }

    public interface IPlayerResettable
    {
        event ResetPlayer.ResettingThePlayerHandler OnPlayerReset;
    }
}