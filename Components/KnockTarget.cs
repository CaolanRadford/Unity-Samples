using System;
using System.Collections;
using System.Collections.Generic;
using Adaptors;
using K_LanUtilities;
using ScriptableObjectDefinitions;
using Services;
using UnityEngine;
using UnityEngine.Serialization;

namespace Components
{
    //applies force and torque suddenly to gameObject target based on various movement criteria
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(IKnockTarget))]
    [RequireComponent(typeof(AudioSourceAdapter))]
    public class KnockTarget : MonoBehaviour, IGM_ServiceLocator_Recievable, ISoundPlayableOneShot
    {
        //defined interface
        public delegate void OnKnockTarget(GameObject target);
        private IKnockTarget[] knockTargetsComponents;

        //implemented interfaces
        public event AudioSourceAdapter.ToPlaySoundOneShot OnSoundPlayOneShot;

        //dependencies
        private Rigidbody2D playerRb;
        private GM_ServiceLocator gm;
        
        //design variables
        [SerializeField]
        private float torqueModifier = 0;

        [SerializeField]
        private float forceModifier = 0;

        [SerializeField]
        private RatbagProjectWideAudioLibrary audioLib;

        void OnEnable()
        {
            playerRb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            SetUpKnockInterface();
        }

        void OnDisable()
        {
            DissolveKnockInterface();
        }

        private void KnockingTheTarget(GameObject target)
        {
            StartCoroutine(gm.GetService<DamageEffects>().PauseGameAndInvertColours());
            
            Rigidbody2D targetRb = target.GetComponentInParent<Rigidbody2D>() ?? throw new ArgumentNullException("target.GetComponentInParent<Rigidbody2D>()");

            if (playerRb.velocity.magnitude != 0)
            {
                AddForce_WhenPlayerMoving(targetRb);
            }
            else
            {
                AddForce_WhenPlayerStill(targetRb);
            }

            OnSoundPlayOneShot?.Invoke(audioLib.ratbag_Hitting);
        }

        private void AddForce_WhenPlayerStill(Rigidbody2D targetRb)
        {
            targetRb.AddTorque(torqueModifier);
            targetRb.AddForce(Vector2.up * forceModifier);

            var velocity = targetRb.velocity;
            StartCoroutine(gm.GetService<CameraShake>().ShakingCamera(.1f, velocity.magnitude/4, velocity.magnitude));
        }

        private void AddForce_WhenPlayerMoving(Rigidbody2D targetRb)
        {
            if (targetRb.velocity.magnitude != 0)
            {
                AddForce_TargetMoving_PlayerMoving(targetRb);
            }
            else
            {
                AddForce_TargetStill_PlayerMoving(targetRb);
            }
        }

        private void AddForce_TargetStill_PlayerMoving(Rigidbody2D targetRb)
        {
            targetRb.AddTorque(playerRb.velocity.magnitude * torqueModifier);
            targetRb.AddForce(forceModifier * playerRb.velocity.magnitude * Vector2.up);

            var velocity = playerRb.velocity;
            StartCoroutine(gm.GetService<CameraShake>().ShakingCamera(.1f, velocity.magnitude/4, velocity.magnitude));
        }

        private void AddForce_TargetMoving_PlayerMoving(Rigidbody2D targetRb)
        {
            targetRb.AddTorque(playerRb.velocity.magnitude * torqueModifier * targetRb.velocity.magnitude);
            targetRb.AddForce(targetRb.velocity.magnitude * playerRb.velocity.magnitude * forceModifier * Vector2.up);

            var velocity = playerRb.velocity;
            StartCoroutine(gm.GetService<CameraShake>().ShakingCamera(.1f, velocity.magnitude/4, velocity.magnitude));
        }

        public void AssignGM(GM_ServiceLocator serviceLocator)
        {
            gm = serviceLocator;
        }

        private void SetUpKnockInterface()
        {
            knockTargetsComponents = StaticUtilities.ReturnInterfacesFromComponentsAndAnyControllerStates<IKnockTarget>(gameObject);

            foreach (var knockTarget in knockTargetsComponents)
            {
                knockTarget.OnKnockTarget += KnockingTheTarget;
            }
        }

        private void DissolveKnockInterface()
        {            
            foreach (var knockTarget in knockTargetsComponents)
            {
                knockTarget.OnKnockTarget -= KnockingTheTarget;
            }
        }    
    }

    public interface IKnockTarget
    {
        event KnockTarget.OnKnockTarget OnKnockTarget;
    }
}