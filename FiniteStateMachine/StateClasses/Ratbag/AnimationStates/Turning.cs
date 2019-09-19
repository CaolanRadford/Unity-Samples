using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Components;
using Controllers;
using K_LanUtilities;
using ScriptableObjectDefinitions;
using UnityEngine;

//animation state for when Ratbag is up&Active but turning
namespace StateClasses.Ratbag.AnimationStates
{
    public class Turning : RatbagAnimationSubState, ISoundPlayableInterrupt, ISoundStoppable, IAnimatorAdaptorSetBool,
                                                    IRigidbody2dVelocityReceiver, IDirectionUpdateable, ISoundPlayableOneShot
    {
        //interface implementations
        public event SoundBroker.stopAudioSource OnStopAudioSource;
        public event SoundBroker.playSoundInterrupt OnSoundPlayInterrupt;
        public event AnimatorAdaptor.BoolSetting OnSetBool;
        public event SoundBroker.playSoundOneShot OnSoundPlayOneShot;

        //design variable
        private readonly RatbagProjectWideAudioLibrary audioLib = StaticUtilities.ReturnDefaultAudioLibrary();

        //fields
        private static readonly int TurningHash = Animator.StringToHash("turning");
        private Vector2 velocity;
        private Vector2 faceDirection;

        public Turning(Controller controller) : base(controller) {}

        public override void OnStateEnter()
        {
            OnSetBool?.Invoke(TurningHash, true);

//            OnSoundPlayOneShot?.Invoke(audioLib.ratbag_turning);
            OnSoundPlayInterrupt?.Invoke(audioLib.ratbag_turning);
        }

        public override void Update()
        {
            if(Vector2.Dot(faceDirection, velocity) > 0)
            {
                Rat.SetAnimationState(Rat.AvailableAnimationStates[typeof(Running)]);
            }
        }    

        public override void OnStateExit()
        {
            OnSetBool?.Invoke(TurningHash, false);
            
            OnStopAudioSource?.Invoke();
        }

        public override void OnNoButton()
        {
            if(velocity.magnitude > 0)
            {
                Rat.SetAnimationState(Rat.AvailableAnimationStates[typeof(Running)]);
            }
            else
            {
                Rat.SetAnimationState((Rat.AvailableAnimationStates[typeof(Idle)]));
            }
        }

        public void ReceivingVelocity(Vector2 newVelocity)
        {
            velocity = newVelocity;
        }

        public void UpdatingDirection(Vector2 newDirection)
        {
            faceDirection = newDirection;
        }
    }
}
