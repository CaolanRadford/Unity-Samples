using System.Collections.Generic;
using K_LanUtilities;
using UnityEngine;
using Object = System.Object;

//all controllers inherit from this abstract class, gameObjects have one controller per gameObject.
//controllers hold the gameObjects identity within the game logic.
//controllers should always be on the root obj
namespace AbstractClasses
{
    [DisallowMultipleComponent]
    public abstract class Controller : MonoBehaviour
    {
        public Dictionary<object, State> AllStates { get; set; } = new Dictionary<object, State>();
    }
}
