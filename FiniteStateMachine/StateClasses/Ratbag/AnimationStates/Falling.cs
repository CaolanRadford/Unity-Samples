using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Controllers;
using UnityEngine;

//animation state for when Ratbag is airborne but descending in the air
namespace StateClasses.Ratbag.AnimationStates
{
    public class Falling : RatbagAnimationSubState, IAnimatorAdaptorSetBool
    {
        public event AnimatorAdaptor.BoolSetting OnSetBool;

        private static readonly int FallingHash = Animator.StringToHash("falling");

        public Falling(Controller controller) : base(controller) {}

        public override void OnStateEnter()
        {
            OnSetBool?.Invoke(FallingHash, true);
        }

        public override void Update()
        {
            return;
        }    

        public override void OnStateExit()
        {
            OnSetBool?.Invoke(FallingHash, false);
        }
    }
}
