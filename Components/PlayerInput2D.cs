using K_LanUtilities;
using Services.SceneManagement;
using UnityEngine;

namespace Components
{
    //takes input from Input manager and updates any components on the gameObject that are interested
    [DisallowMultipleComponent]
    [RequireComponent (typeof(IPlayerInput2D))]
    public class PlayerInput2D : MonoBehaviour, IMainLevelComplete_Updateable
    {
        //defined interfaces
        private IPlayerInput2D[] controllers;

        //fields
        private float horizontalAxis = 0;

        [SerializeField]
        private bool lockInput = false;

        void OnEnable()
        {
            lockInput = false;
            controllers = StaticUtilities.ReturnOnlyActiveControllers<IPlayerInput2D>(GetComponents<IPlayerInput2D>(), this);

            if(controllers == null)
            {
                StaticUtilities.NoControllerFound();
            }
        }

        void Update()
        {
            if(!lockInput)
            {
                for (int i = 0; i < controllers.Length; i++)
                {
                    if (Input.GetButtonDown("Jump"))
                    {
                        controllers[i].OnJumpButton();
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if(!lockInput)
            {
                for (int i = 0; i < controllers.Length; i++)
                {        
                    if(Input.GetAxisRaw("Horizontal") != 0)
                    {
                        horizontalAxis = Input.GetAxisRaw("Horizontal");

                        controllers[i].OnHorizontalAxis(horizontalAxis);
                    }

                    //reset the horizontal axis
                    else if(Input.GetAxisRaw("Horizontal") == 0 && horizontalAxis != 0)
                    {
                        horizontalAxis = 0;

                        controllers[i].OnHorizontalAxis(horizontalAxis);
                    }

                    //affirm that no keys are being held down (maybe this should be in Update()??)
                    else if(Input.GetAxisRaw("Horizontal") == 0 && !Input.GetButton("Jump"))
                    {
                        controllers[i].OnNoButton();
                    }            
                }
            }
        }

        public void OnMainLevelComplete()
        {
            lockInput = true;
        }
    }

    public interface IPlayerInput2D
    {
        void OnHorizontalAxis(float horizontalAxis);
        void OnJumpButton();
        void OnNoButton();
    }
}