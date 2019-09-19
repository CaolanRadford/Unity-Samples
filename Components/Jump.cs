using System.Collections;
using System.Collections.Generic;
using K_LanUtilities;
using UnityEngine;

namespace Components
{
    //Allows the gameObject to jump with the help of an input 
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(IJump))]
    public class Jump : MonoBehaviour, IGroundedUpdatable
    {
        //defined interface
        public delegate void ToJump();
        private IJump[] jumpingComponents;

        //dependencies
        private Rigidbody2D rb;

        //design variables
        [SerializeField]
        private float jumpHeight = 0;

        //fields
        private bool grounded;

        void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            jumpingComponents = StaticUtilities.ReturnInterfacesFromComponentsAndAnyControllerStates<IJump>(gameObject);

            foreach (var jump in jumpingComponents)
            {
                jump.OnJump += DoAJump;
            }
        }
        
        void OnDisable()
        {
            foreach (var jump in jumpingComponents)
            {
                jump.OnJump -= DoAJump;
            }
        }

        void DoAJump()
        {
            if(grounded)
            {
                rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            }
        }

        public void GroundedUpdate(bool newGroundedStatus)
        {
            grounded = newGroundedStatus;
        }
    }

    public interface IJump
    {
        event Jump.ToJump OnJump;
    }
}