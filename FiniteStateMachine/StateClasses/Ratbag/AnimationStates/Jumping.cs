using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Components;
using Controllers;
using UnityEngine;

//animation state for when Ratbag is airborne, but ascending
namespace StateClasses.Ratbag.AnimationStates
{
    public class Jumping : RatbagAnimationSubState, IAnimatorAdaptorSetBool
    {
        private static readonly int JumpingHash = Animator.StringToHash("jumping");
        public event AnimatorAdaptor.BoolSetting OnSetBool;

        public Jumping(Controller controller) : base(controller){}

        public override void Update()
        {
            return;
        }

        public override void OnStateEnter()
        {
            OnSetBool?.Invoke(JumpingHash, true);
        }
        
        public override void OnStateExit()
        {            
            OnSetBool?.Invoke(JumpingHash, false);
        }
    }
}
