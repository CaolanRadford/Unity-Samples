using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbstractClasses;
using Components;
using K_LanUtilities;
using UnityEngine;

namespace Services
{
    //controls & holds the light's state in the scene
    [RequireComponent (typeof(IPlayerInput2D))]
    [RequireComponent (typeof(SpriteRenderer))]
    public class LightsController : Service, IPlayerInput2D
    {
        //defined interfaces    
        public delegate void ChangingLights(bool lightsOn);
        private ILightsChange_Updatable[] lightsUpdatables;    
        public event ChangingLights OnSwitchLights;  

        //dependencies
        private SpriteRenderer darknessRenderer;

        //working variables
        public bool lightsOn {get; private set;}

        private float autoTimer = 0;

        void OnEnable()
        {
            SetUpDarknessRenderer();
            SetUp_ILightsUpdatables();
        }

        void OnDisable()
        {
            Disolve_ILightsUpdatables();
        }

        void Update()
        {
            if(autoTimer >= 0)
            {
                autoTimer -= Time.deltaTime;
            }
        }

        public void OnHorizontalAxis(float OnHorizontalAxis)
        {
            ChangeLights(true);
        }

        public void OnJumpButton()
        {

            ChangeLights(true);
        }

        public void OnNoButton()
        {
            if(autoTimer <= 0)
            {
                ChangeLights(false);
            }
        }

        public void ManualLightsSwitch_Timer(float countDown)
        {
            // Debug.Log("manual lights on, timer: " + countDown);
            autoTimer = countDown;

            ChangeLights(true);
        }

        private void ChangeLights(bool newStatus)
        {
            if (lightsOn != newStatus)
            {
                lightsOn = newStatus;
                darknessRenderer.enabled = !newStatus;

                OnSwitchLights?.Invoke(newStatus);
            }
            else
            {
                return;
            }
        } 

        private void SetUpDarknessRenderer()
        {
            darknessRenderer = GetComponent<SpriteRenderer>();
            darknessRenderer.sortingLayerName = ("Mid_+1");
        }

        private void SetUp_ILightsUpdatables()
        {
            lightsUpdatables = StaticUtilities.ReturnInterfaceImplementationsFromScene<ILightsChange_Updatable>();

            for (int i = 0; i < lightsUpdatables.Length; i++)
            {
                OnSwitchLights += lightsUpdatables[i].OnLightsChange;
            }
        }

        private void Disolve_ILightsUpdatables()
        {
            for (int i = 0; i < lightsUpdatables.Length; i++)
            {
                OnSwitchLights -= lightsUpdatables[i].OnLightsChange;
            }
        }        
    }

    public interface ILightsChange_Updatable
    {
        void OnLightsChange(bool lightsOn);
    }
}