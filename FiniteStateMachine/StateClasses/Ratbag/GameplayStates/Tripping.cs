using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Components;
using K_LanUtilities;
using ScriptableObjectDefinitions;
using UnityEngine;

//Gameplay state for when Ratbag is tripping
namespace StateClasses.Ratbag.GameplayStates
{
    public class Tripping : RatbagPlayerState, IRigidbody2dGravityActivator, IRigidbody2dRotationLockable, ISoundPlayableOneShot
    {
        //interface implementations
        public event RigidBody2dAdaptor.Rigidbody2dBoolHandler GravityEnabled;
        public event RigidBody2dAdaptor.Rigidbody2dBoolHandler LockRotation;
        public event SoundBroker.playSoundOneShot OnSoundPlayOneShot;

        //dependencies
        private RatbagProjectWideAudioLibrary audioLib = StaticUtilities.ReturnDefaultAudioLibrary();
        
        //fields
        private StaticUtilities.Grounded2D_struct groundedStruct;
        private LayerMask groundMask;
        
        public Tripping(Controller controller, Collider2D collider) : base(controller)
        {
            groundedStruct = new StaticUtilities.Grounded2D_struct(collider);
            groundMask = StaticUtilities.CreateLayerMaskFromLayer((int)Enums.PhysicsLayers.Ground);
        }

        public override void OnStateEnter()
        {
            OnSoundPlayOneShot?.Invoke(audioLib.HingeSqueek);
            
            LockRotation?.Invoke(false);

            ratBrain.SetAnimationState(ratBrain.AvailableAnimationStates[typeof(AnimationStates.Tripping)]);
            
            GravityEnabled?.Invoke(true);
        }

        public override void Update()
        {
            if(LookForGround() != null)
            {
                ratBrain.SetAnimationState(ratBrain.AvailableAnimationStates[typeof(AnimationStates.Floored)]);
                ratBrain.SetState(ratBrain.AvailableStates[typeof(Floored)]);
            }
        }

        public override void OnStateExit()
        {
        }

        private Collider2D LookForGround()
        {
            Collider2D groundResult = CastCircleAroundPlayersHeadToLookForGround();
            
            if (groundResult != null)
            {
                return groundResult;
            }

            return null;
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

        private Collider2D CastCircleAroundPlayersHeadToLookForGround()
        {
            return Physics2D.OverlapCircle((Vector2)ratBrain.transform.position, (groundedStruct.radius + .03f), groundMask);
        }
    }
}
