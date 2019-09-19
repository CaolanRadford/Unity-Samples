using AbstractClasses;
using Components;
using Controllers;
using K_LanUtilities;
using Services;
using UnityEngine;

//gameplay and animation state for when the bouncer has spotted the player
namespace StateClasses.Bouncer
{
    public class TargetSpotted : BouncerState, ITriggerUpdatable_Bouncer, IChangeableCameraTargetingWeight
    {
//        public event SmashBrosCamera.ChangingCameraTargetWeightHandler ChangeCameraWeighting;
        public event CinemachineTargetGroupBroker.ChangingCameraTargetWeight ChangingCameraTargetWeight;

        //dependencies
        private Controllers.Bouncer bouncer;
    
        //working variables
        private float currentWaitTime = 0;
        private float stopRunTimer = 1f;

        public TargetSpotted(Controller controller) : base(controller)
        {
            bouncer = this.Controller as Controllers.Bouncer;
        }

        public override void Update()
        {
            if(bouncer.target != null && bouncer.currentState == this)  //second check needed to make sure Update doesn't fire after StateExit()
            {
                Vector2 targetPosition = bouncer.target.transform.position;
                Vector2 transformPosition = bouncer.transform.position;

                Vector2 directionToTarget = targetPosition - transformPosition;

                float dot = Vector2.Dot(bouncer.facingDirection, directionToTarget.normalized);

                // Debug.Log("dot = " + dot + " || target pos: " + targetPosition + " || bouncer pos: " + transformPosition);

                if(dot < 0)
                {
                    bouncer.OnChangeDirection_Wrapper(-bouncer.facingDirection);
                }

                if(!bouncer.gm.GetService<LightsController>().lightsOn)
                {
                    currentWaitTime -= Time.deltaTime;
                }

                if(currentWaitTime <= 0)
                {
                    bouncer.SetState(bouncer.availableStates[typeof(TargetLost)]);
                }

                if(bouncer.isCharging)
                {
                    //timer to reset running state(a few frames)
                    if(Time.time >= stopRunTimer)
                    {
                        bouncer.isCharging = false;
                        bouncer.animator.SetBool("running", false);

                        stopRunTimer = Time.time + 1f;
                    }
                    else if(bouncer.currentState == this)                                             //safety check
                    {
                        //change the animation state, and change it back
                        bouncer.animator.SetBool("running", true);                                    //does this need to be set each frame?
                    }
                
                    bouncer.animator.SetFloat("runningSpeedMultiplier", (bouncer.rb.velocity.magnitude/10));
                    // Debug.Log("running speed assigned");
                }
            }
        }
    
        public override void OnStateEnter()
        {
            bouncer.rb.gravityScale = 5;
//            ChangeCameraWeighting?.Invoke(TransformToAddToCameraTargetGroup(), 1);
            ChangingCameraTargetWeight?.Invoke(1);
            
            currentWaitTime = 1;

            bouncer.animator.SetBool("targetSpotted", true);

            Vector2 startDirection = Random.value > .5f? Vector2.right : Vector2.left;

            bouncer.OnChangeDirection_Wrapper(startDirection); 
            bouncer.OnSearchingStatusChange_Wrapper(false);

            bouncer.OnSoundPlayOneShot_Wrapper(bouncer.audioLib.bouncer_Targetting);
        }

        public override void OnStateExit()
        {
            bouncer.rb.gravityScale = 1;
            
            bouncer.OnNullTarget_Wrapper();

            bouncer.animator.SetBool("running", false);

            bouncer.animator.SetBool("targetSpotted", false);
        }

        public void OnBouncerTrigger(Collider2D trig)
        {
            if(StaticUtilities.CheckIdentity<RatbagPlayerBrain>(trig.gameObject))
            {
                if(bouncer.gm.GetService<LightsController>().lightsOn)
                {
                    RatbagPlayerBrain rc = trig.gameObject.GetComponentInParent<RatbagPlayerBrain>();
                
                    if(rc.CurrentState.IsPlayerResetable())
                    {
                        bouncer.OnPlayerReset_Wrapper(trig.gameObject);
                    }
                }
            }
        }

        public override bool BouncerKnockable()
        {
            bouncer.OnSoundPlayOneShot_Wrapper(bouncer.audioLib.bouncer_Punching);

            bouncer.SetState(bouncer.availableStates[typeof(Knocked)]);
            return true;
        }

//        public Transform TransformToAddToCameraTargetGroup()
//        {
//            return Controller.transform;
//        }
    }
}
