using AbstractClasses;
using Adaptors;
using Adaptors.Interfaces;
using Controllers;
using UnityEngine;

namespace StateClasses.Ratbag.AnimationStates
{
    public class Tripping : RatbagAnimationSubState, IAnimatorAdaptorSetBool
    {
        //interface implementations
        public event AnimatorAdaptor.BoolSetting OnSetBool;
        
        //fields
        private static readonly int TrippingHash = Animator.StringToHash("tripping");

        public Tripping(Controller controller) : base(controller) {}

        public override void Update()
        {
            return;
        }
        
        public override void OnStateEnter()
        {
            OnSetBool?.Invoke(TrippingHash, true);
        }
        
        public override void OnStateExit()
        {
            OnSetBool?.Invoke(TrippingHash, false);
        }
    }
}
