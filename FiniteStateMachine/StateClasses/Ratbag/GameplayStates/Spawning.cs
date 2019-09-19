using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Components;
using Controllers;
using K_LanUtilities;
using Services;
using StateClasses.Ratbag.AnimationStates;
using UnityEngine;

//gameplay state for when ratbag is spawning out of a vent
namespace StateClasses.Ratbag.GameplayStates
{
    public class Spawning : RatbagPlayerState, IKnockTarget, ITriggerUpdateable, IRigidbody2dGravityActivator, 
                                                IRigidbody2dIsKinematicSetter, IRigidbodyVelocitySetter,
                                                IGM_ServiceLocator_Recievable, ICollisionEnterUpdateable, IChangeDirection
    {
        //interface implementations
        public event KnockTarget.OnKnockTarget OnKnockTarget;
        public event RigidBody2dAdaptor.Rigidbody2dBoolHandler GravityEnabled;
        public event RigidBody2dAdaptor.Rigidbody2dBoolHandler SetIsKinematic;
        public event RigidBody2dAdaptor.Rigidbody2dVector2dHandler SetRigidBodyVelocity;
        public event FaceDirection.changeDirection OnChangeDirection;

        //dependencies
        SpriteRenderer ratRenderer;
        PlayerInput2D playerInput;
        private GM_ServiceLocator gm;
        
        //design variables
        private float timerDefault = 5f;    

        //fields
        private float timer = 5;
        private bool isLocked = false;

        public Spawning(Controller controller) : base(controller)
        {
            InitialiseDependencies();
        }

        public override void OnStateEnter()
        {            
//            Debug.Log(this.GetType() + " is active" + " || currently active bool == " + CurrentlyActive);

            SetDefaultFaceDirection();

            ratBrain.SetAnimationState(ratBrain.AvailableAnimationStates[typeof(Falling)]);

            LockPlayer();

            timer = timerDefault;
        }

        private void SetDefaultFaceDirection()
        {
            OnChangeDirection?.Invoke(Vector2.right);
        }

        public override void Update()
        {
            if(timer >= 0)
            {
                timer -= Time.deltaTime;
//                Debug.Log("timer: " + timer);
            }
            else 
            {
                UnlockPlayer();
            }
        }

        public override void OnStateExit()
        {
//            Debug.Log(this.GetType() + " is no longer active" + " || currently active bool == " + CurrentlyActive);
        }

        private void InitialiseDependencies()
        {
            ratRenderer = ratBrain.GetComponent<SpriteRenderer>();
            playerInput = ratBrain.GetComponent<PlayerInput2D>();            //need some way of catching a null reference here if an input class isn't on the ratController?
        }
        
        private void LockPlayer()
        {
            isLocked = true;
            
            SetRigidBodyVelocity?.Invoke(Vector2.zero);
//            Debug.Log("lock fired");

            SetIsKinematic?.Invoke(true);
            ratRenderer.enabled = false;
            playerInput.enabled = false;
        }
        private void UnlockPlayer()
        {
            GravityEnabled?.Invoke(true);

            isLocked = false;
//            Debug.Log("unlock fired");
            SetIsKinematic?.Invoke(false);
            ratRenderer.enabled = true;
            playerInput.enabled = true;
        }



        private void GetTimerDefaultFromResetService()
        {
            timerDefault = gm.GetService<GameReset>().GetSpawningTime();
        }

        public override bool IsPlayerResetable()
        {
            return false;
        }

        public override bool IsPlayerTrippable()
        {
            return false;
        }

        public void AssignGM(GM_ServiceLocator serviceLocator)
        {
            gm = serviceLocator;
            GetTimerDefaultFromResetService();
//            Debug.Log("GM received: " + GetType());
        }

        public void OnEnterCollision(Collision2D col)
        {
//            Debug.Log("ground hit");
            if(StaticUtilities.CheckIdentity<Ground>(col.gameObject))
            {
                if (!isLocked)
                {
                    ratBrain.SetState(ratBrain.AvailableStates[typeof(UpAndActive)]);
                }
            }
        }

        public void OnTrigger(Collider2D trigger)
        {
            Debug.Log((CurrentlyActive + " currently active" + this.GetType()));
            Debug.Log("trigger fired: " + this.GetType());
            
            if(!isLocked)
            {            
                if(StaticUtilities.CheckIdentity<Controllers.Bouncer>(trigger.gameObject))
                {
                    Controllers.Bouncer bc = trigger.gameObject.GetComponentInParent<Controllers.Bouncer>();

                    if(bc.currentState.BouncerKnockable())
                    {
                        OnKnockTarget?.Invoke(trigger.gameObject);
                    }
                }
            }
        }
    }
}
