using AbstractClasses;
using Adaptors;
using Components;
using K_LanUtilities;
using ScriptableObjectDefinitions;
using StateClasses.Ratbag.AnimationStates;
using UnityEngine;

//gameplay state for when ratbag is getting up
namespace StateClasses.Ratbag.GameplayStates
{
    public class GettingUp : RatbagPlayerState, ISoundPlayableInterrupt, ITriggerDeactivator
    {
        //interface implementations
        public event SoundBroker.playSoundInterrupt OnSoundPlayInterrupt;
        public event TriggerAdaptor.DeactivateTriggerHandler DeactivateTriggerForTime;

        //design variables
        float timer = .5f;
        private readonly RatbagProjectWideAudioLibrary audioLib = StaticUtilities.ReturnDefaultAudioLibrary();   

        public GettingUp(Controller controller) : base(controller) {}

        public override void OnStateEnter()
        {
            ratBrain.SetAnimationState(ratBrain.AvailableAnimationStates[typeof(AnimationStates.GettingUp)]);
            
            OnSoundPlayInterrupt?.Invoke(audioLib.ratBag_GettingUp);
        }

        public override void Update()
        {
            return;
        }

        public override void OnStateExit()
        {
            DeactivateTriggerForTime?.Invoke(.5f);
            ResetTimer();
            
            OnSoundPlayInterrupt?.Invoke(audioLib.ratBag_Landed);
        }

        public override void OnHorizontalAxis(float axis)
        {
            timer -= Time.deltaTime;
            
            if(timer <= 0)
            {
                ratBrain.SetState(ratBrain.AvailableStates[typeof(UpAndActive)]);
                ratBrain.SetAnimationState(ratBrain.AvailableAnimationStates[typeof(Running)]);
            }
        }

        public override void OnJumpButton()
        {
            ratBrain.SetState(ratBrain.AvailableStates[typeof(Floored)]);
        }   
    
        public override void OnNoButton()
        {
            timer = ResetTimer();
            ratBrain.SetState(ratBrain.AvailableStates[typeof(Floored)]);
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

        private float ResetTimer()
        {
            return timer = (.5f);
        }
    }
}
