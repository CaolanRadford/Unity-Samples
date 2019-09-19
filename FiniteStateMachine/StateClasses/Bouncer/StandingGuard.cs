using AbstractClasses;
using Components;
using Controllers;
using K_LanUtilities;
using Services;
using UnityEngine;

//gameplay and animation state for when the bouncer is standing guard
namespace StateClasses.Bouncer
{
    public class StandingGuard : BouncerState, ITriggerUpdatable_Bouncer, IChangeableCameraTargetingWeight
    {
        public event CinemachineTargetGroupBroker.ChangingCameraTargetWeight ChangingCameraTargetWeight;

        //dependencies
        private readonly Controllers.Bouncer bouncer;

        //design variables
        private float currentWaitTime = 1;

        public StandingGuard(Controller controller) : base(controller)
        {
            bouncer = this.Controller as Controllers.Bouncer;
        }

        public override void OnStateEnter()
        {
            ChangingCameraTargetWeight?.Invoke(0);
            
            bouncer.animator.SetBool("standingGuard", true);

            PickStartDirection();

            bouncer.OnSearchingStatusChange_Wrapper(true);
        }

        public override void Update()
        {
            if(bouncer.flipFaceDirectionWhenSearching)
            {
                DecreaseTimerBeforeFlippingDirection();
            }

            if (bouncer.target != null)
            {
                bouncer.SetState(bouncer.availableStates[typeof(TargetSpotted)]);
            }
        }

        public override void OnStateExit()
        {
            bouncer.OnSearchingStatusChange_Wrapper(false);
            bouncer.animator.SetBool("standingGuard", false);
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

        private void DecreaseTimerBeforeFlippingDirection()
        {
            currentWaitTime -= Time.deltaTime;

            if (currentWaitTime <= 0)
            {
                ResetWaitTime();

                FlipDirection();
            }
        }

        public override bool BouncerKnockable()
        {
            bouncer.OnSoundPlayOneShot_Wrapper(bouncer.audioLib.bouncer_Punching);

            bouncer.SetState(bouncer.availableStates[typeof(Knocked)]);
            return true;
        }

        private void PickStartDirection()
        {
            if((int)bouncer.defaultFacingDirection == 0)
            {
                bouncer.OnChangeDirection_Wrapper(Vector2.left);
            }
            else if((int)bouncer.defaultFacingDirection == 1)
            {
                bouncer.OnChangeDirection_Wrapper(Vector2.right);
            }
            else if((int)bouncer.defaultFacingDirection == 2)
            {
                Vector2 startDirection = Random.value > .5f ? Vector2.right : Vector2.left;
                bouncer.OnChangeDirection_Wrapper(startDirection);
            }
        } 

        private void FlipDirection()
        {
            bouncer.OnChangeDirection_Wrapper(-bouncer.facingDirection);
        }

        private void ResetWaitTime()
        {
            currentWaitTime = bouncer.RandomWaitTime();
        }
    }
}
