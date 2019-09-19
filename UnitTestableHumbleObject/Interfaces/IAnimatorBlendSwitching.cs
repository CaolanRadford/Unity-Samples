using Services;
using UnityEngine;

namespace Components.Interfaces
{
    public interface IAnimatorBlendSwitching : IDirectionUpdateable, ILightsChange_Updatable
    {
        bool lightsOn { get; }
        Vector2 currentFacingDirection { get; }
    }
}