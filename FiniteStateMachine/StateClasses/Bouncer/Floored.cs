using AbstractClasses;
using Components;
using Controllers;
using K_LanUtilities;
using ScriptableObjectDefinitions;
using Services;
using UnityEngine;

//gameplay and animation state for when the bouncer has been knocked to the floor
namespace StateClasses.Bouncer
{
    public class Floored : BouncerState, ITriggerUpdatable_Bouncer, IDarknessReignitable
    {
        public event DarknessAnimatorFade.DarknessReigniteHandler ReigniteRenderer;

        //dependencies
        private readonly Controllers.Bouncer bouncer;
        private RatbagProjectWideAudioLibrary  audioLib = StaticUtilities.ReturnDefaultAudioLibrary();
        
        //working variables
        private float flooredTime = 0;
        RaycastHit2D checkingRay;
        
        //string to hash
        private static readonly int FlooredBool = Animator.StringToHash("floored");

        public Floored(Controller controller) : base(controller)
        {
            bouncer = this.Controller as Controllers.Bouncer;
        }

        public override void OnStateEnter()
        {
            bouncer.animator.SetBool(FlooredBool, true);

            CheckIfStuckInGround();

            CheckRotation();

            CheckConstraints();

            if(flooredTime <= 0)
            {
                flooredTime = 5f;                                       //sets it to a default time if we forget to set it when changing state
            }
        
            bouncer.OnSoundPlayInterrupt_Wrapper(bouncer.audioLib.bouncer_Floored);
        }

        public override void Update()
        {
            flooredTime -= Time.deltaTime;

            if(flooredTime <= 0)
            {
                bouncer.SetState(bouncer.availableStates[typeof(GettingUp)]);
            }
        }

        public override void OnStateExit()
        {
            bouncer.animator.SetBool(FlooredBool, false);
        }

        public void SetFlooredTime(Vector2 impactVelocity)
        {
            flooredTime = 1 * impactVelocity.magnitude;
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
                        // Debug.Log("trip fired");
                    }
                }
                else
                {
                    ReigniteRenderer?.Invoke();
                    bouncer.OnSoundPlayOneShot_Wrapper(audioLib.PassingFlooredbouncer);
                }
            }
        }

        private void CheckIfStuckInGround()
        {
            checkingRay = Physics2D.Raycast(bouncer.transform.position, Vector2.down, bouncer.distanceToGround, bouncer.groundMask);

            if(checkingRay.collider != null)
            {
                SnapAboveGround();
            }
        }

        private void SnapAboveGround()
        {
            float distanceToContact = checkingRay.distance;

            float offsetAmount = bouncer.distanceToGround - distanceToContact;

            Vector2 offsetVector = new Vector2(0, offsetAmount);

            bouncer.transform.position = (Vector2)bouncer.transform.position + offsetVector;
        }

        private void CheckRotation()
        {
            if(bouncer.transform.rotation != Quaternion.identity)
            {
                bouncer.transform.rotation = Quaternion.identity;
            }
        }

        private void CheckConstraints()
        {
            if(bouncer.rb.constraints != RigidbodyConstraints2D.FreezeRotation)
            {
                bouncer.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }    
        public override bool BouncerKnockable()
        {
            return false;
        }
    }
}
