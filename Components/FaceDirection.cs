using System;
using System.Collections;
using System.Collections.Generic;
using K_LanUtilities;
using UnityEngine;

namespace Components
{
//defines a 2D direction that the gameObject is facing  
    public class FaceDirection : MonoBehaviour
    {
        //interface handlers
        public delegate void NewDirection(Vector2 direction);
        private IChangeDirection[] changeDirectionables;             
        private IDirectionUpdateable[] updateDirectionables;
    
        public Vector2 direction {get; private set;} = Vector2.right;       //a default should maybe created and exposed as a design variable?

        private void Start()
        {
            SetUpDirectionUpdatables();
            SetUpChangeDirectionInterfaces();
        }

        void OnDisable()
        {
            DisolveChangeDirectionInterfaces();
        }

        private void SetUpChangeDirectionInterfaces()
        {
            changeDirectionables = StaticUtilities.ReturnInterfacesFromComponentsAndAnyControllerStates<IChangeDirection>(gameObject);

            for (int i = 0; i < changeDirectionables.Length; i++)
            {
                IChangeDirection controller = changeDirectionables[i];
                controller.OnChangeDirection += ChangeDirection;
            }
        }

        private void DisolveChangeDirectionInterfaces()
        {
            for (int i = 0; i < changeDirectionables.Length; i++)
            {
                IChangeDirection changeController = changeDirectionables[i];
                changeController.OnChangeDirection -= ChangeDirection;
            }
        }

        private void ChangeDirection(Vector2 newDirection)
        {
            direction = newDirection;

            foreach(IDirectionUpdateable updateController in updateDirectionables)
            {
                updateController.UpdatingDirection(newDirection);
            }
        }    
        
        private void SetUpDirectionUpdatables()
        {
            updateDirectionables = StaticUtilities.ReturnInterfacesFromComponentsAndAnyControllerStates<IDirectionUpdateable>(gameObject);
        }
    }

//for components that want to change the direction
    public interface IChangeDirection
    {
        event FaceDirection.NewDirection OnChangeDirection;
    }

//for components that want to be updated when the direction changes
    public interface IDirectionUpdateable
    {
        void UpdatingDirection(Vector2 newDirection);
    }
}