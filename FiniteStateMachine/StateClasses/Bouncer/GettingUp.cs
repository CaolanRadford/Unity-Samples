using AbstractClasses;
using Controllers;
using K_LanUtilities;
using Services;
using UnityEngine;

// gameplay and animation state for when the bouncer is getting up from the ground
namespace StateClasses.Bouncer
{
    public class GettingUp : BouncerState, ITriggerUpdatable_Bouncer
    {
        //dependencies
        private Controllers.Bouncer bouncer;

        //design variables
        private float gettingUpTime = 1f;

        public GettingUp(Controller controller) : base(controller)
        {
            bouncer = this.Controller as Controllers.Bouncer;
        }

        public override void OnStateEnter()
        {
            bouncer.animator.SetBool("gettingUp", true);

            gettingUpTime = 1f;
        }

        public override void Update()
        {
            gettingUpTime -= Time.deltaTime;

            if(gettingUpTime <= 0)
            {
                bouncer.SetState(bouncer.availableStates[typeof(TargetLost)]);
            }
        }

        public override void OnStateExit()
        {
            bouncer.animator.SetBool("gettingUp", false);
        }

        public void OnBouncerTrigger(Collider2D trig)
        {
            if(StaticUtilities.CheckIdentity<RatbagPlayerBrain>(trig.gameObject))
            {
                if(bouncer.gm.GetService<LightsController>().lightsOn)
                {
                    RatbagPlayerBrain rc = trig.gameObject.GetComponentInParent<RatbagPlayerBrain>();
                
                    if(rc.CurrentState.IsPlayerTrippable())
                    {
                        bouncer.OnTripPlayer_Wrapper(trig.gameObject);
                    }
                }
            }
        }

        public override bool BouncerKnockable()
        {
            bouncer.SetState(bouncer.availableStates[typeof(Knocked)]);
            return true;
        }
    }
}
