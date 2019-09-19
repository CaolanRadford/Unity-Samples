using UnityEngine;

//services hold scene state, only one of each allowed per scene, will throw an error if more are found
namespace AbstractClasses
{
    [DisallowMultipleComponent]
    public abstract class Service : MonoBehaviour
    {
        public virtual void Awake()
        {
            UnityEngine.Object[] list = InstancedCopies(this);

            if(list.Length == 1)
            {
                return;
            }
            else if(list.Length != 1)
            {
                Debug.LogError("Duplicate service found: " + list[1].GetType());
                // Debug.Break();
            }
        }

        UnityEngine.Object[] InstancedCopies(UnityEngine.Object service)
        {
            UnityEngine.Object[] services = FindObjectsOfType(service.GetType());       //get type should at run time return the subclass implementing this abstract class

            return services;
        }
    }
}
