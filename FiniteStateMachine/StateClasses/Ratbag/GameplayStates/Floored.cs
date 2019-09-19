using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Components;
using K_LanUtilities;
using ScriptableObjectDefinitions;
using UnityEngine;

//gameplay state for when ratbag is falling to the floor
namespace StateClasses.Ratbag.GameplayStates
{
    public class Floored : RatbagPlayerState, ISoundPlayableOneShot, IRigidbody2dRotationLockable
    {
        //interface implementations
        public event SoundBroker.playSoundOneShot OnSoundPlayOneShot;
        public event RigidBody2dAdaptor.Rigidbody2dBoolHandler LockRotation;

        //design variables
        private float timer = .5f;
        private readonly LayerMask groundMask;
        private readonly RatbagProjectWideAudioLibrary audioLib = StaticUtilities.ReturnDefaultAudioLibrary();

        //fields/properties
        private RaycastHit2D snapRayResults;
        private float DistanceToGround {get;} = 1.28f;        //should probably not hard code this

        public Floored(Controller controller) : base(controller)
        {
            groundMask = StaticUtilities.CreateLayerMaskFromLayer((int)Enums.PhysicsLayers.Ground);
        }

        public override void OnStateEnter()
        {
            SetTimerToDefault();

            CheckIfStuckInGround();

            CheckRotation();

            CheckConstraints();

            ratBrain.SetAnimationState(ratBrain.AvailableAnimationStates[typeof(AnimationStates.Floored)]);

            OnSoundPlayOneShot?.Invoke(audioLib.ratbag_Floored);
        }

        private void SetTimerToDefault()
        {
            timer = .5f;
        }

        public override void Update()
        {
            return;
        }

        public override void OnStateExit()
        {
        }

        private void CheckIfStuckInGround()
        {
            snapRayResults = Physics2D.Raycast(ratBrain.transform.position, Vector2.down, DistanceToGround, groundMask);

            SnapColliderAboveGround();
        }

        private void SnapColliderAboveGround()
        {
            if (snapRayResults.collider != null)
            {
                float distanceToContact = snapRayResults.distance;

                float offsetAmount = DistanceToGround - distanceToContact;

                Vector2 offsetVector = new Vector2(0, offsetAmount);

                var transform = ratBrain.transform;
                
                transform.position = (Vector2)transform.position + offsetVector;
            }
        }

        private void CheckRotation()
        {
            if(ratBrain.transform.rotation != Quaternion.identity)
            {
                ratBrain.transform.rotation = Quaternion.identity;
            }
        }

        private void CheckConstraints()
        {
            LockRotation?.Invoke(true);
        }

        public override void OnHorizontalAxis(float axis)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                ratBrain.SetState(ratBrain.AvailableStates[typeof(GettingUp)]);
            }
        }
    
        public override void OnNoButton()
        {
            timer = .5f;
        }
        public override bool IsPlayerTrippable()
        {
            return false;
        }

        public override bool IsPlayerResetable()
        {
            ratBrain.SetState(ratBrain.AvailableStates[typeof(Spawning)]);

            return true;
        }
    }
}
