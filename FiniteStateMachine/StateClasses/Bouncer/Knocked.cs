using System.Collections;
using AbstractClasses;
using Adaptors.Interfaces;
using Components;
using Controllers;
using UnityEngine;

//gameplay and animation state for when the bouncer is knocked into the air
namespace StateClasses.Bouncer
{
    public class Knocked : BouncerState, ICollisionUpdatable_Bouncer, IRigidbody2dVelocityReceiver, 
                                        IChangeableCameraTargetingWeight
    {
        public event CinemachineTargetGroupBroker.ChangingCameraTargetWeight ChangingCameraTargetWeight;

        private Controllers.Bouncer bouncer;
        private Vector2 velocity;

        private float velocityCount = 1;
        
        public Knocked(Controller controller) : base(controller)
        {
            bouncer = this.Controller as Controllers.Bouncer;
        }
    
        public override void OnStateEnter()
        {
            ChangingCameraTargetWeight?.Invoke(0);
            
            velocityCount = 1;
            
            bouncer.rb.constraints = RigidbodyConstraints2D.None;
            bouncer.animator.SetBool("falling", true);

            bouncer.gameObject.layer = 11;

            bouncer.OnSoundPlayOneShot_Wrapper(bouncer.audioLib.bouncer_Punching);
        }

        public override void Update()
        {
            if (velocity.magnitude < .1f)
            {
                //start counting
                velocityCount -= Time.deltaTime;

                if (velocityCount <= 0)
                {
                    if (velocity.magnitude < .1f)
                    {
                        ToFlooredState();
                    }
                }
            }
        }
        
        public override void OnStateExit()
        {
            bouncer.animator.SetBool("falling", false);

            bouncer.gameObject.layer = 10;
        }

        public void OnBouncerCollision(Collision2D col)
        {
            if(col.gameObject.GetComponent<Ground>())
            {
                foreach(ContactPoint2D colPoint in col.contacts)
                {
                    float wallDot = Vector2.Dot(colPoint.normal, Vector2.up);

                    bouncer.OnSoundPlayOneShot_Wrapper(bouncer.audioLib.bouncer_Punching);

                    if(wallDot > -0.0)                                                            //checks that the bouncer isn't g a wall'
                    {
                        ToFlooredState();
                    }
                }
            }
        }

        private void ToFlooredState()
        {
            bouncer.OnSoundPlayOneShot_Wrapper(bouncer.audioLib.bouncer_Punching);

            bouncer.SetState(bouncer.availableStates[typeof(Floored)]);
        }

        public override bool BouncerKnockable()
        {
            return false;
        }

        public void ReceivingVelocity(Vector2 newVelocity)
        {
            velocity = newVelocity;
        }
    }
}
