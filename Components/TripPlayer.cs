using K_LanUtilities;
using Services;
using UnityEngine;

namespace Components
{
    //trips a gameobject target by suddenly adding torque
    public class TripPlayer : MonoBehaviour, IGM_ServiceLocator_Recievable
    {
        //defined interfaces
        public delegate void TrippingPlayer(GameObject player);
        private ITripPlayerable[] tripPlayerables;

        //dependencies
        private GM_ServiceLocator gm;

        //fields
        [SerializeField]
        private float tripForce;

        void OnEnable()
        {
            tripPlayerables = StaticUtilities.ReturnOnlyActiveControllers<ITripPlayerable>(GetComponents<ITripPlayerable>(), this);

            SetUpTrippingInterfaces();
        }

        void OnDisable()
        {
            DisolveTrippingInterfaces();
        }

        public void TripTarget(GameObject target)
        {
            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();

            float torqueToAdd = targetRb.velocity.magnitude * tripForce;

            // Debug.Log("target tripped, torque added: " + torqueToAdd);

            targetRb.AddTorque(torqueToAdd);

            StartCoroutine(gm.GetService<CameraShake>().ShakingCamera(.1f, targetRb.velocity.magnitude/4, targetRb.velocity.magnitude));
        }

        public void AssignGM(GM_ServiceLocator serviceLocator)
        {
            gm = serviceLocator;
        }

        private void SetUpTrippingInterfaces()
        {
            for (int i = 0; i < tripPlayerables.Length; i++)
            {
                tripPlayerables[i].OnTripPlayer += TripTarget;
            }
        }

        private void DisolveTrippingInterfaces()
        {
            for (int i = 0; i < tripPlayerables.Length; i++)
            {
                tripPlayerables[i].OnTripPlayer -= TripTarget;
            }
        }    
    }

    public interface ITripPlayerable
    {
        event TripPlayer.TrippingPlayer OnTripPlayer;
    }
}