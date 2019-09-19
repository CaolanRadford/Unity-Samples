using Controllers;
using K_LanUtilities;
using Services;
using UnityEngine;

namespace Components
{
    //a component for bouncers that charges towards a target using a MovementComponent
    [RequireComponent(typeof(ITargetUpdatable))]
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(FaceDirection))]
    public class ChargeAttack : MonoBehaviour, ITargetUpdatable, IMovable, IDirectionUpdateable, 
                                                IGM_ServiceLocator_Recievable, IGroundedMovable
    {
        //defined interface
        private IChargeUpdatable[] componentsToUpdate;

        //implemented interfaces
        public event Movement.GroundedMove OnMoveGround;
        public event Movement.ToMove OnMove;    
        private Vector2 directionToGround;
        private Vector2 angleOfGround;   

        //dependencies
        private GM_ServiceLocator gm;     

        //design variables
        [SerializeField]
        private float chargeSpeed = 0;
        private bool darknessLock = false;                  //darkness lock keeps the bouncer charging in the dark    

        //fields
        private GameObject target;
        private Vector2 bouncerFacingDirection;

        private void OnEnable()
        {
            componentsToUpdate = StaticUtilities.ReturnOnlyActiveControllers<IChargeUpdatable>(GetComponents<IChargeUpdatable>(), this);
        }

        private void Update()
        {
            if(target != null)
            {
                if(!darknessLock)
                {
                    darknessLock = true;
                }

                if(target == null)
                {
                    if(!gm.GetService<LightsController>().lightsOn)
                    {
                        darknessLock = false;
                    }
                }
                if(StaticUtilities.CheckIdentity<RatbagPlayerBrain>(target))
                {
                    Vector2 targetFacingDirection = target.GetComponent<FaceDirection>().direction;

                    if(bouncerFacingDirection == targetFacingDirection)
                    {
                        Charge(chargeSpeed);
                        // print("charge firing: " + chargeSpeed);

                        for (int i = 0; i < componentsToUpdate.Length; i++)
                        {
                            IChargeUpdatable componentToUpdate = componentsToUpdate[i];
                            componentToUpdate.OnBouncerCharge();
                        }
                    }   
                }
            }
        }

        void Charge(float horizontalAxis)
        {
            if(Vector2.Dot(bouncerFacingDirection, Vector2.right) < 0)
            {
                OnMove?.Invoke(-horizontalAxis);
            }
            else if(Vector2.Dot(bouncerFacingDirection, Vector2.left) < 0)
            {
                OnMove?.Invoke(horizontalAxis);
            }
        }
        public void TargetFound(GameObject targetPassed)
        {
            target = targetPassed;

            // print("target assigned to ChargeAttack_Component: " + target);        
        }

        public void UpdatingDirection(Vector2 updatedDirection)
        {
            bouncerFacingDirection = updatedDirection;
        }

        public void OnTargetLost()
        {
            // print("target should be null");
            target = null;
        }

        public void AssignGM(GM_ServiceLocator newGm) => gm = newGm;
    }

    public interface IChargeUpdatable
    {
        void OnBouncerCharge();
    }
}