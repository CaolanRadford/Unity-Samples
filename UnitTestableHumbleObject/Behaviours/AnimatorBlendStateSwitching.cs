using Components.Interfaces;
using Enums;
using static K_LanUtilities.StaticUtilities;

//the unit testable class that decides the animator blend state
namespace Components.Behaviours
{
    public class AnimatorBlendStateSwitching
    {
        private readonly IAnimatorBlendSwitching component;

        public AnimatorBlendStateSwitching(IAnimatorBlendSwitching component)
        {
            this.component = component;
        }

        public float CalculatingAnimatorBlendState()
        {
            if (FacingRight(component.currentFacingDirection))
            {
                return component.lightsOn
                    ? ((float) AnimatorBlendStates.IlluminatedFacingRight)
                    : ((float) AnimatorBlendStates.DarkenedFacingRight);
            }

            return component.lightsOn
                ? ((float)AnimatorBlendStates.IlluminatedFacingLeft)
                : ((float)AnimatorBlendStates.DarkenedFacingLeft);
        }

        public float CalculatingAnimatorBlendState(bool newLightStatus)
        {
            if (FacingRight(component.currentFacingDirection))
            {
                return newLightStatus
                    ? ((float) AnimatorBlendStates.IlluminatedFacingRight)
                    : ((float) AnimatorBlendStates.DarkenedFacingRight);
            }

            return newLightStatus
                ? ((float) AnimatorBlendStates.IlluminatedFacingLeft)
                : ((float) AnimatorBlendStates.DarkenedFacingLeft);
        }
    }
}
