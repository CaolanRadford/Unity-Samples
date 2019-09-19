using AbstractClasses;
using Components;
using Controllers;
using K_LanUtilities;
using Services;
using UnityEngine;

//gameplay and animation state for when the bouncer has lost track of the player
namespace StateClasses.Bouncer
{
    public class TargetLost : BouncerState, ITriggerUpdatable_Bouncer, IChangeableCameraTargetingWeight
    {
        public event CinemachineTargetGroupBroker.ChangingCameraTargetWeight ChangingCameraTargetWeight;

        //dependancies
        private Controllers.Bouncer bouncer;

        //design variables
        private int count = 0;

        private float currentWaitTime;

        public TargetLost(Controller controller) : base(controller)
        {
            bouncer = this.Controller as Controllers.Bouncer;
        }

        public override void Update()
        {
            if(bouncer.targetLostTurnCount >= 5)
            {
                bouncer.SetState(bouncer.availableStates[typeof(StandingGuard)]);
            }

            if(bouncer.target != null)
            {
                bouncer.SetState(bouncer.availableStates[typeof(TargetSpotted)]);
            }        
        }

        public override void OnStateEnter()
        {
            ChangingCameraTargetWeight?.Invoke(0);
            
            currentWaitTime = .5f;

            bouncer.OnSearchingStatusChange_Wrapper(true);
            count = 0;

            bouncer.animator.SetFloat("targetLostMultiplier", bouncer.targetLostAnimSpeed);
            bouncer.animator.SetBool("targetLost", true);
        }

        public override void OnStateExit()
        {
            bouncer.OnSearchingStatusChange_Wrapper(false);
            bouncer.animator.SetBool("targetLost", false);

            bouncer.targetLostTurnCount = 5;
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
    }
}
