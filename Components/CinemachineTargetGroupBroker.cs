using System;
using System.Collections;
using Components;
using K_LanUtilities;
using Services;
using UnityEngine;

namespace Components
{
    //GameObject component that talks to the smash bros camera service
    public class CinemachineTargetGroupBroker : MonoBehaviour, ICameraTargetable, IGM_ServiceLocator_Recievable
    {
        //interface handling
        public delegate void ChangingCameraTargetWeight(float newWeight);
        private IChangeableCameraTargetingWeight[] weightingChangers;
        
        //fields/properties
        private float CurrentWeighting { get; set; }
        private float targetWeighting;
        private GM_ServiceLocator gm;
        private IEnumerator changeWeightCoRoutine;

        private void Start()
        {
            SetUpWeightingChangers();

            GameReset.PlayerReset += OnPlayerReset;
        }

        private void OnDisable()
        {
            DissolveWeightingChangers();
            
            GameReset.PlayerReset -= OnPlayerReset;
        }

        private void ChangeWeighting(float newWeight)
        {
            targetWeighting = newWeight;

            if (targetWeighting != CurrentWeighting)
            {
                CurrentWeighting = targetWeighting;
                
                if (changeWeightCoRoutine != null)
                {
                    StopCoroutine(changeWeightCoRoutine);
                }

                changeWeightCoRoutine = gm.GetService<GroupTargetBrokerSmashBrosCamera>().ChangeCamTargetWeight(transform, CurrentWeighting);
                
                StartCoroutine(changeWeightCoRoutine);
            }
        }

        public Transform TransformToAddToCameraTargetGroup()
        {
            return transform;
        }

        public void AssignGM(GM_ServiceLocator serviceLocator)
        {
            gm = serviceLocator;
        }

        private void SetUpWeightingChangers()
        {
            weightingChangers = StaticUtilities.ReturnInterfacesFromComponentsAndAnyControllerStates<IChangeableCameraTargetingWeight>(gameObject);
            
            foreach (var changer in weightingChangers)
            {
                changer.ChangingCameraTargetWeight += ChangeWeighting;
            }
        }

        private void DissolveWeightingChangers()
        {
            foreach (var changer in weightingChangers)
            {
                changer.ChangingCameraTargetWeight -= ChangeWeighting;
            }
        }
        
        public void OnPlayerReset()
        {
            targetWeighting = 0;

            if (targetWeighting != CurrentWeighting)
            {
                CurrentWeighting = targetWeighting;

                if (changeWeightCoRoutine != null)
                {
                    StopCoroutine(changeWeightCoRoutine);
                    
                    gm.GetService<GroupTargetBrokerSmashBrosCamera>().ResetWithoutLerp(transform);
                }
            }
        }
    }
}

public interface IChangeableCameraTargetingWeight
{
    event CinemachineTargetGroupBroker.ChangingCameraTargetWeight ChangingCameraTargetWeight;
}

