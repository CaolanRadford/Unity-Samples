using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Controllers;
using UnityEngine;

namespace StateClasses.Ratbag.AnimationStates
{
    public class GettingUp : RatbagAnimationSubState, IAnimatorAdaptorSetBool
    {
        public event AnimatorAdaptor.BoolSetting OnSetBool;

        private static readonly int GettingUpHash = Animator.StringToHash("gettingUp");

        public GettingUp(Controller controller) : base(controller) {}

        public override void Update()
        {
            return;
        }

        public override void OnStateEnter()
        {
            OnSetBool?.Invoke(GettingUpHash, true);
        }

        public override void OnStateExit()
        {
            OnSetBool?.Invoke(GettingUpHash, false);
        }
    }
}
