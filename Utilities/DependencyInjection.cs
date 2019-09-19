using AbstractClasses;
using Boo.Lang;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace K_LanUtilities
{
    //the main methods used to grab interfaces that are both mono behaviours and non-monobehaviours during initialisation
    public class DependencyInjection : MonoBehaviour
    {
        public static TInterface[] ReturnInterfacesFromComponentsAndAnyControllerStates<TInterface>(GameObject caller)             //works on per gameObject
            where TInterface : class 
        {
            List<TInterface> tempList = new List<TInterface>();

//            Debug.Log("get component controller = " + caller.GetComponent<Controller>());
            
            if (caller.GetComponent<Controller>())
            {
                var states = caller.GetComponent<Controller>().AllStates;

//                Debug.Log("states found by utility = " + states.Count);
                
                foreach(var state in states)
                {
                    if(state.Value is TInterface)
                    {
                        tempList.Add(states[state.Key] as TInterface);
                    }
                }
            }

            foreach(var component in caller.GetComponents<TInterface>())
            {
                tempList.Add(component);
            }

            TInterface[] interfacesToReturn = tempList.ToArray();

            // Debug.Log("Number of interfaces returned: " + interfacesToReturn.Length + " || interface type: " + typeof(TInterface));

            return interfacesToReturn;
        }
        
        public static TInterface[] FindInterfacesFromStatesSceneWide<TInterface>() where TInterface : class                            //looks scene wide
        {
            List<TInterface> interfaces = new List<TInterface>();
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach( var rootGameObject in rootGameObjects)
            {
                Controller[] controllers = rootGameObject.GetComponentsInChildren<Controller>();
                
                if (controllers != null)
                {
                    foreach (var controller in controllers)
                    {
                        var states = controller.GetComponent<Controller>().AllStates;
                        
                        foreach(var state in states)
                        {
                            if(state.Value is TInterface)
                            {
                                interfaces.Add(states[state.Key] as TInterface);
                            }
                        }
                    }
                }
            }
            
            return interfaces.ToArray();
        }
    }
}
