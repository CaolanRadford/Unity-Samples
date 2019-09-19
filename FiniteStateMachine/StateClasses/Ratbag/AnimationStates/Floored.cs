using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Controllers;
using UnityEngine;

namespace StateClasses.Ratbag.AnimationStates
{
    public class Floored : RatbagAnimationSubState, IAnimatorAdaptorSetBool
    {
        private static readonly int FlooredHash = Animator.StringToHash("floored");
        
        public event AnimatorAdaptor.BoolSetting OnSetBool;

        public Floored(Controller controller) : base(controller) {}

        public override void Update()
        {
            return;
        }

        public override void OnStateEnter()
        {
            OnSetBool?.Invoke(FlooredHash, true);
        }

        public override void OnStateExit()
        {
            OnSetBool?.Invoke(FlooredHash, false);
        }
    }
}
