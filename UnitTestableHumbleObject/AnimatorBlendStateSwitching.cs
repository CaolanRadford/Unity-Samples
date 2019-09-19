using Components.Interfaces;
using Services;
using UnityEngine;
using Enums;
using static K_LanUtilities.StaticUtilities;

//switches the blend state variables in the gameObject's animator, based on both player 
//(input direction) and game state (light's)
//humble object, behaviour has been extracted to allow for unit testing within the Unity framework
namespace Components
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorBlendStateSwitching : MonoBehaviour, IAnimatorBlendSwitching
    {
        //dependencies
        private Behaviours.AnimatorBlendStateSwitching behaviour;
        private Animator Animator { get; set; }

        //fields/properties
        public Vector2 currentFacingDirection { get; private set; }
        public bool lightsOn { get; private set; }
        
        private static readonly int FaceDirHash = Animator.StringToHash("faceDir");

        private void OnEnable()
        {
            behaviour = new Behaviours.AnimatorBlendStateSwitching(this);
            Animator = GetComponent<Animator>();
        }

        public void UpdatingDirection(Vector2 newDirection)
        {
            currentFacingDirection = newDirection;

            var correctBlendState = behaviour.CalculatingAnimatorBlendState();
            Animator.SetFloat(FaceDirHash, correctBlendState);
        }

        public void OnLightsChange(bool newLightsStatus)
        {
            lightsOn = newLightsStatus;

            var correctBlendState = behaviour.CalculatingAnimatorBlendState(lightsOn);
            Animator.SetFloat(FaceDirHash, correctBlendState);
        }
    }
}
