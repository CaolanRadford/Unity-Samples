using System.Collections.Generic;
using Components;
using Controllers;

//a template for ratbag's player states (gameplay states), exposes OnInput methods
namespace AbstractClasses
{
    public abstract class RatbagPlayerGameplayState : State, IPlayerInput2D
    {
        protected readonly RatbagPlayerBrain RatBrain;

        protected RatbagPlayerGameplayState(Controller controller) : base(controller)
        {
            RatBrain = controller as RatbagPlayerBrain;
        }

        public virtual void PhysicsUpdate(){}
        public virtual void LateUpdate(){}
        public virtual void OnHorizontalAxis(float axis){}
        public virtual void OnJumpButton(){}
        public virtual void OnNoButton(){}
        public abstract bool IsPlayerTripable();
        public abstract bool IsPlayerResetable();
        
    }
}
