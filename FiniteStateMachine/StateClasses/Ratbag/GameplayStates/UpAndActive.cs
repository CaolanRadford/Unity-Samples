using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Components;
using Controllers;
using K_LanUtilities;
using Services;
using StateClasses.Ratbag.AnimationStates;
using StateClasses.Ratbag.GameplayStates;
using UnityEngine;
using Tripping = StateClasses.Ratbag.GameplayStates.Tripping;

//Gameplay state for when Ratbag is grounded and moving around
namespace StateClasses.Ratbag
{
    public class UpAndActive : RatbagPlayerState, IGroundedMovable, IJump, IKnockTarget,
                                                    IChangeDirection, IRigidbody2dGravityActivator, IGrounded_Updatable,
                                                    IGM_ServiceLocator_Recievable, ITriggerUpdateable, IDirectionUpdateable
                                    
    {
        //interface implementations
        public event Jump.ToJump OnJump;
        public event FaceDirection.changeDirection OnChangeDirection;
        public event Movement.groundMove OnMoveGround;
        public event KnockTarget.OnKnockTarget OnKnockTarget;
        public event RigidBody2dAdaptor.Rigidbody2dBoolHandler GravityEnabled;
        
        //dependencies
        private Vector2 faceDirection;
        private bool grounded;
        private GM_ServiceLocator gm;
        
        public UpAndActive(Controller controller) : base(controller) {}

        public override void OnStateEnter()
        {
            ratBrain.SetAnimationState(ratBrain.AvailableAnimationStates[typeof(Idle)]);
            GravityEnabled?.Invoke(false);
        }

        public override void Update()
        {
            if(!grounded)
            {
                ratBrain.SetState(ratBrain.AvailableStates[typeof(AirBorne)]);
            }
        }
    
        public override void OnStateExit()
        {
            return;
        }

        public override void OnHorizontalAxis(float horizontalAxis)
        {
            ratBrain.CurrentAnimationState.OnHorizontalAxis((horizontalAxis));        //should maybe be pulled up to the brain

            OnMoveGround?.Invoke(horizontalAxis);

            if(horizontalAxis > 0)
            {
                if(faceDirection != Vector2.right)
                {
                    OnChangeDirection(Vector2.right);
                }
            }

            else if(horizontalAxis < 0)
            {
                if(faceDirection != Vector2.left)
                {
                    OnChangeDirection(Vector2.left);
                }
            }
        }

        public override void OnJumpButton()
        {
            ratBrain.CurrentAnimationState.OnJumpButton();                    //should maybe pull up to brain

            OnJump?.Invoke();
            ratBrain.SetState(ratBrain.AvailableStates[typeof(AirBorne)]);
        }

        public override void OnNoButton()
        {
            ratBrain.CurrentAnimationState.OnNoButton();
        }

        public override bool IsPlayerTrippable()
        {
            ratBrain.SetState(ratBrain.AvailableStates[typeof(Tripping)]);

            return true;
        }

        public override bool IsPlayerResetable()
        {
            ratBrain.SetState(ratBrain.AvailableStates[typeof(Spawning)]);

            return true;
        }

        public void GroundedUpdate(bool newGroundedStatus)
        {
            grounded = newGroundedStatus;
        }

        public void AssignGM(GM_ServiceLocator serviceLocator)
        {
            gm = serviceLocator;
//            Debug.Log("GM received: " + GetType() + " || gm = " + gm.name);
        }

        public void OnTrigger(Collider2D trigger)
        {
//            Debug.Log("triggered: " + trig.gameObject.name);

            if(StaticUtilities.CheckIdentity<Controllers.Bouncer>(trigger.gameObject))
            {
                Controllers.Bouncer bc = trigger.gameObject.GetComponentInParent<Controllers.Bouncer>();

//                Debug.Log("bc = " + bc + " || lights on: " + gm.GetService<LightsController>().lightsOn);
                
                if(!gm.GetService<LightsController>().lightsOn)
                {
                    if(bc.currentState.BouncerKnockable())
                    {
                        OnKnockTarget?.Invoke(trigger.gameObject);
                    }
                }
            }
        }

        public void UpdatingDirection(Vector2 newDirection)
        {
            faceDirection = newDirection;
        }
    }
}
