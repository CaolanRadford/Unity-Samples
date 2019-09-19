using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Components;
using Controllers;
using UnityEngine;

//animation state for when Ratbag is up&Active but running
namespace StateClasses.Ratbag.AnimationStates
{
    public class Running : RatbagAnimationSubState, IAnimatorAdaptorSetBool, IAnimatorAdaptorSetFloat, 
                                                    IRigidbody2dVelocityReceiver, IDirectionUpdateable
    {
        //interface implementations
        public event AnimatorAdaptor.BoolSetting OnSetBool;
        public event AnimatorAdaptor.FloatSetting OnSetFloat;
        
        //fields
        private static readonly int RunningHash = Animator.StringToHash("running");
        private static readonly int RunningSpeedMultiplier = Animator.StringToHash("runningSpeedMultiplier");

        private Vector2 velocity;
        private Vector2 faceDirection;

        public Running(Controller controller) : base(controller){}

        public override void OnStateEnter()
        {
            OnSetBool?.Invoke(RunningHash, true);
        }

        public override void Update()
        {
            OnSetFloat?.Invoke(RunningSpeedMultiplier, velocity.magnitude/100);

        }    

        public override void OnStateExit()
        {
            OnSetBool?.Invoke(RunningHash, false);
        }

        public override void OnHorizontalAxis(float horizontalAxis)
        {
            if(Vector2.Dot(faceDirection, velocity) < 0f)
            {
                Rat.SetAnimationState(Rat.AvailableAnimationStates[typeof(Turning)]);
            }        
        }
        public override void OnNoButton()
        {
            Rat.SetAnimationState(Rat.AvailableAnimationStates[typeof(Idle)]);
        }

        public void ReceivingVelocity(Vector2 newVelocity)
        {
            velocity = newVelocity;
        }

        public void UpdatingDirection(Vector2 newDirection)
        {
            faceDirection = newDirection;
        }
    }
}
