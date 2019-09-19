using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using K_LanUtilities;

namespace Components
{
/*Movement system pushes with addforce, this gives us dynamism when we change the friction using things like physics materials.
at the cost of consistancy when pushing the actor's rigidbody around, using addforce, the Rigidbody's position is dependant on
collisions that happen along the way. The alternative option is to push the rigidbody directly by changing it's velocity. This 
is smoother and more consistent but less dynamic, constraining emergent behaviour. Another decision taken 
was to addForce with ForceMode2D: Force, instead of Impulse, force allows the actors's mass to be taken into account. Allowing 
for different actors to hold varied weight and associated effects. Collider choice is a capsule as seems standard for characters
of this type, this also allows smoother collision resolve when pushing with addforce, resulting in smoother movement.
Project wide physics timestep is quicker than default to create smoother motion when resolving the many tiny collisions as
we toMove.*/

    [RequireComponent (typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour, IGroundDirectionVectorUpdatable, IGroundedUpdatable
    {
        //defined interfaces
        public delegate void GroundedMove(float horizontalAxis);
        public delegate void ToMove(float horizontalAxis);
        private IMovable[] movableComponents;
        private IGroundedMovable[] groundedMovables;
        
        //dependencies    
        private Rigidbody2D rb;     

        //design variables
        [SerializeField]
        private float moveSpeed = 0;

        //working variables
        private Vector2 currentGroundVector;
        private bool grounded;
        

        void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            groundedMovables = StaticUtilities.ReturnInterfacesFromComponentsAndAnyControllerStates<IGroundedMovable>(gameObject);
            movableComponents = StaticUtilities.ReturnInterfacesFromComponentsAndAnyControllerStates<IMovable>(gameObject);

            SetUpGroundedMoveInterface();   
            SetUpMoveInterface();
        }

        void OnDisable()
        {
            DissolveMoveInterface();
            DissolveGroundedMoveInterface();
        }

        public void Move(float horizontalAxis)
        {
            rb.AddForce(moveSpeed * (horizontalAxis) * Vector2.right, ForceMode2D.Force);

            if (rb.velocity.magnitude != 0)
            {
                if (rb.velocity.x < 0 && horizontalAxis > 0)
                {   
                    rb.AddForce(-(rb.velocity), ForceMode2D.Force);               
                }

                else if (rb.velocity.x > 0 && horizontalAxis < 0)
                {
                    rb.AddForce(-(rb.velocity), ForceMode2D.Force);          
                }
            }     

            // Debug.Log("toMove without ground vec");
        }

        public void GroundMove(float horizontalAxis)
        {
            if(horizontalAxis <= 0) 
            {
                horizontalAxis = horizontalAxis*-1;
            }

            Vector2 forceToAdd = currentGroundVector * (horizontalAxis) * moveSpeed;

            rb.AddForce(forceToAdd, ForceMode2D.Force);
        }

        private void SetUpMoveInterface()
        {
            for (int i = 0; i < movableComponents.Length; i++)
            {
                IMovable movableComp = movableComponents[i];
                movableComp.OnMove += Move;
            }
        }
        
        private void DissolveMoveInterface()
        {
            for (int i = 0; i < movableComponents.Length; i++)
            {
                IMovable movableComp = movableComponents[i];
                movableComp.OnMove -= Move;
            }
        }
        
        private void SetUpGroundedMoveInterface()
        {
            for (int i = 0; i < groundedMovables.Length; i++)
            {
                IGroundedMovable groundedMovable = groundedMovables[i];
                groundedMovable.OnMoveGround += GroundMove;
            }
        }        
        
        private void DissolveGroundedMoveInterface()
        {
            for (int i = 0; i < groundedMovables.Length; i++)
            {
                IGroundedMovable groundedMovable = groundedMovables[i];
                groundedMovable.OnMoveGround -= GroundMove;
            }
        }  
        
        
        public void GroundDirectionUpdate(Vector2 groundVector)
        {
            currentGroundVector = groundVector;
        }

        public void GroundedUpdate(bool newGroundedStatus)
        {
            grounded = newGroundedStatus;
        }
    }

    public interface IMovable
    {    
        event Movement.ToMove OnMove;
    }

    public interface IGroundedMovable
    {
        event Movement.GroundedMove OnMoveGround;
    }
}