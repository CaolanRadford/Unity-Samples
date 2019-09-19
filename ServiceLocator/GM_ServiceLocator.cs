using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbstractClasses;
using K_LanUtilities;
using UnityEngine;
using Object = System.Object;

namespace Services
{
//holds reference to all game state services, and provides a way of retrieving them. (is also, itself a service)
    public class GM_ServiceLocator : Service
    {
        //working variables
        private Dictionary<object, object> services = new Dictionary<object, object>();

        void OnEnable()
        {
            RegisterServices();
            InjectThisToComponents();
        }

        private void Start()
        {
            InjectThisToStates();
        }

        public T GetService<T>()    where T: MonoBehaviour 
        {
            try
            {
                return (T)services[typeof(T)];
            }
            catch (KeyNotFoundException)
            {
                Debug.LogError("The requested service is not registered");

                return null;
            }
        }

        private void RegisterServices()
        {
            var servicesToFind = FindObjectsOfType<Service>();

            for (int i = 0; i < servicesToFind.Length; i++)
            {
                Service service = servicesToFind[i];

                if (service != this)
                {
                    services.Add(service.GetType(), service);
                }
            }
        }

        private void InjectThisToComponents()
        {
            var componentsThatNeedGM = FindObjectsOfType<MonoBehaviour>().OfType<IGM_ServiceLocator_Recievable>();

            foreach (IGM_ServiceLocator_Recievable component in componentsThatNeedGM)
            {
                component.AssignGM(this);
            }
        }
        private void InjectThisToStates()
        {
            var componentsThatNeedGM = StaticUtilities.FindInterfacesFromStatesSceneWide<IGM_ServiceLocator_Recievable>();

            foreach (IGM_ServiceLocator_Recievable component in componentsThatNeedGM)
            {
                component.AssignGM(this);
            }
        }
  
        private void DebugServiceDictionary()
        {
            foreach (KeyValuePair<object, object> entry in services)
            {
                print("value :" + entry);
            }
        }        
    }

    public interface IGM_ServiceLocator_Recievable                
    {
        void AssignGM(GM_ServiceLocator serviceLocator);
    }
}