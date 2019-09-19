using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Components;
using Controllers;
using K_LanUtilities;
using ScriptableObjectDefinitions;
using Services;
using StateClasses.Ratbag.AnimationStates;
using UnityEngine;

//Gameplay state for when ratbag whenever Ratbag is airborne
namespace StateClasses.Ratbag.GameplayStates
{
    public class AirBorne : RatbagPlayerState, IMovable, IJump, IKnockTarget, IChangeDirection, ISoundPlayableOneShot,
                                                IRigidbody2dGravityActivator, IRigidbody2dVelocityReceiver, IDirectionUpdateable, 
                                                IGrounded_Updatable, IGM_ServiceLocator_Recievable, ICollisionEnterUpdateable, 
                                                ICollisionStayUpdateable, ITriggerUpdateable
    {
        
        //interface implementations
        public event Movement.move OnMove;
        public event Jump.ToJump OnJump;
        public event KnockTarget.OnKnockTarget OnKnockTarget;
        public event FaceDirection.changeDirection OnChangeDirection;
        public event SoundBroker.playSoundOneShot OnSoundPlayOneShot;
        public event RigidBody2dAdaptor.Rigidbody2dBoolHandler GravityEnabled;

        //dependencies
        private RatbagProjectWideAudioLibrary audioLib = StaticUtilities.ReturnDefaultAudioLibrary();
        private Vector2 velocity;
        private Vector2 faceDirection;
        private bool grounded;
        private GM_ServiceLocator gm;

        //fields/properties
        private bool Descending {get; set;}

        public AirBorne(Controller controller) : base(controller) {}

        public override void OnStateEnter()
        {
            CheckDirection();
            SetCorrectAirborneAnimationState();

            GravityEnabled?.Invoke(true);
        }

        public override void Update()
        {  
            CheckDirection();
            
            float dot = Vector2.Dot(velocity, Vector2.down);
            
            if(grounded)
            {
                CheckForHorizontalGroundPlane(dot);
            }       
        }
        
        public override void OnStateExit()
        {
            OnSoundPlayOneShot?.Invoke(audioLib.ratbag_feetLanded);
        }
        
        private void CheckForHorizontalGroundPlane(float dotOfDirectionAndVecDownBeforeLanding)
        {
            if (dotOfDirectionAndVecDownBeforeLanding >= 0)
            {
                SetGroundedAndIdleState();
            }
        }

        private void CheckDirection()
        {
            if (Descending != DescendingCheck())
            {
                Descending = DescendingCheck();

                SetCorrectAirborneAnimationState();
            }
        }

        private void SetCorrectAirborneAnimationState()
        {
            ratBrain.SetAnimationState(Descending
                ? ratBrain.AvailableAnimationStates[typeof(Falling)]
                : ratBrain.AvailableAnimationStates[typeof(Jumping)]);
        }

        public override void OnHorizontalAxis(float horizontalAxis)
        {
            OnMove?.Invoke(horizontalAxis);

            DirectionChange_Horizontal(horizontalAxis);

            if(grounded)
            {
                SetGroundedAndRunningState();
            }
        }

        private void DirectionChange_Horizontal(float horizontalAxis)
        {
            if (horizontalAxis > 0)
            {
                if (faceDirection != Vector2.right)
                {
                    OnChangeDirection?.Invoke(Vector2.right);
                }
            }
            else if (horizontalAxis < 0)
            {
                if (faceDirection != Vector2.left)
                {
                    OnChangeDirection?.Invoke(Vector2.left);
                }
            }
        }

        public override void OnJumpButton()
        {
            OnJump?.Invoke();
        }

        public override void OnNoButton() {}

        private bool DescendingCheck()
        {
            float dot = Vector2.Dot(velocity, Vector2.down);
            
            if (dot > 0)
            {
                return true;
            }

            return false;
        }    
        
        private bool HitTheGroundAtCorrectAngle(Vector2 groundNormal)
        {
            float dot = Vector2.Dot(groundNormal, Vector2.up);

            if(dot > .1f)
            {
                return true;
            }
            return false;
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


        public void ReceivingVelocity(Vector2 newVelocity)
        {
            velocity = newVelocity;
        }

        public void UpdatingDirection(Vector2 newDirection)
        {
            faceDirection = newDirection;
        }

        public void GroundedUpdate(bool newGroundedStatus)
        {
            grounded = newGroundedStatus;
        }

        public void AssignGM(GM_ServiceLocator serviceLocator)
        {
            gm = serviceLocator;
        }

        public void OnEnterCollision(Collision2D col)
        {
            if(StaticUtilities.CheckIdentity<Ground>(col.gameObject))
            {
                foreach(ContactPoint2D contactPoint in col.contacts)
                {
                    if(StaticUtilities.CheckIdentity<Ground>(contactPoint.collider.gameObject))
                    {
                        if(HitTheGroundAtCorrectAngle(contactPoint.normal))
                        {
                            ratBrain.SetState(ratBrain.AvailableStates[typeof(UpAndActive)]);
                            ratBrain.SetAnimationState(ratBrain.AvailableAnimationStates[typeof(Idle)]);
                        }
                        // Debug.DrawRay(rayOrigin, contactPoint.relativeVelocity, Color.blue, 5f);
                    }
                }
            }
        }

        public void OnStayCollision(Collision2D col)
        {
            if(StaticUtilities.CheckIdentity<Ground>(col.gameObject))
            {
                foreach(ContactPoint2D contactPoint in col.contacts)
                {
                    if(StaticUtilities.CheckIdentity<Ground>(contactPoint.collider.gameObject))
                    {
                        if(HitTheGroundAtCorrectAngle(contactPoint.normal))
                        {
                            ratBrain.SetState(ratBrain.AvailableStates[typeof(UpAndActive)]);
                            ratBrain.SetAnimationState(ratBrain.AvailableAnimationStates[typeof(Idle)]);
                        }
                        // Debug.DrawRay(rayOrigin, contactPoint.relativeVelocity, Color.blue, 5f);
                    }
                }
            }
        }

        public void OnTrigger(Collider2D trigger)
        {
            if(StaticUtilities.CheckIdentity<Controllers.Bouncer>(trigger.gameObject))
            {
                Controllers.Bouncer bc = StaticUtilities.ReturnControllerFromTrigger<Controllers.Bouncer>(trigger);

                if (!gm.GetService<LightsController>().lightsOn)
                {
                    if (bc.currentState.BouncerKnockable())
                    {
                        OnKnockTarget?.Invoke(trigger.gameObject);
                    }
                }
            }
        }
        
        private void SetGroundedAndRunningState()
        {
            ratBrain.SetAnimationState(ratBrain.AvailableAnimationStates[typeof(Running)]);
            ratBrain.SetState(ratBrain.AvailableStates[typeof(UpAndActive)]);
        }
        
        private void SetGroundedAndIdleState()
        {
            ratBrain.SetState(ratBrain.AvailableStates[typeof(UpAndActive)]);
            ratBrain.SetAnimationState(ratBrain.AvailableAnimationStates[typeof(Idle)]);
        }
    }
}
