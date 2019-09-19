using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Controllers;
using UnityEngine;

//animation state for when Ratbag is up&Active but standing still
namespace StateClasses.Ratbag.AnimationStates
{
    public class Idle : RatbagAnimationSubState, IAnimatorAdaptorSetBool
    {
        private static readonly int IdleHash = Animator.StringToHash("idle");
        public event AnimatorAdaptor.BoolSetting OnSetBool;

        public Idle(Controller controller) : base(controller) {}

        public override void OnStateEnter()
        {
            OnSetBool?.Invoke(IdleHash, true);
        }

        public override void Update()
        {
            return;
        }    

        public override void OnStateExit()
        {
            OnSetBool?.Invoke(IdleHash, false);
        }

        public override void OnHorizontalAxis(float horizontalAxis)
        {
            Rat.SetAnimationState(Rat.AvailableAnimationStates[typeof(Running)]);
        }
    }
}
