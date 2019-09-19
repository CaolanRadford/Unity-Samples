

//this is an animation sub state for Ratbag, exposes OnInput methods
//animation sub states are found within gameplay states when a gameplay state has more than one animation state

using Components;
using Controllers;

namespace AbstractClasses
{
    public abstract class RatbagAnimationState : State, IPlayerInput2D
    {
        protected readonly RatbagPlayerBrain RatBrain;

        protected RatbagAnimationState(Controller controller) : base(controller)
        {
            RatBrain = controller as RatbagPlayerBrain;
        }

        public virtual void OnHorizontalAxis(float horizontalAxis)
        {
            return;
        }

        public virtual void OnJumpButton()
        {
            return;
        }

        public virtual void OnNoButton()
        {
            return;
        }
    }
}
