using Components;
using K_LanUtilities;
using Services;
using UnityEngine;

//fades the gameObject's sprite renderer and freezes it's animator when the lights go off (for bouncers)
namespace Components
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class DarknessAnimatorFade : MonoBehaviour, ILightsChange_Updatable
    {
        //interface handler
        public delegate void DarknessReigniteHandler();
        private IDarknessReignitable[] reigniters;
        
        //dependencies
        private Animator animator;                    //should go through adaptor
        private SpriteRenderer spriteRenderer;        //should go through adaptor

        //design variables
        [SerializeField]
        private bool fadingAnimation = false;

        [SerializeField]
        private Color alphaColor;

        [SerializeField]
        private float timeToFade = 1.0f;

        private float animatorStartSpeed;               //set in animator

        //fields
        private Color startColor;
        private Color currentColor;

        void OnEnable()
        {
            GetDependencies();
            
            startColor = spriteRenderer.color;
            animatorStartSpeed = animator.speed;
            
            ResetColor();
        }

        private void Start()
        {
            SetUpReigniters();
        }

        private void OnDisable()
        {
            DisableReigniters();
        }

        public void OnLightsChange(bool lightsOn)
        {
            if(!lightsOn)
            {
                if(!fadingAnimation)
                {
                    fadingAnimation = true;
                }
            }
            else
            {
                if(fadingAnimation)
                {
                    fadingAnimation = false;
                }
            }
        }

        void Update()
        {
            if(fadingAnimation)
            {
                currentColor = Color.Lerp(currentColor, alphaColor, timeToFade * Time.deltaTime);
                spriteRenderer.color = currentColor;
//                print("light fading");
                CheckWhetherToFreezeAnimation();
            }
            if(!fadingAnimation)
            {
                if(spriteRenderer.color != startColor)
                {
                    ResetColor();

                    CheckWhetherToFreezeAnimation();
                }
            }
        }

        public void Reignite()
        {
            ResetColor();
            fadingAnimation = true;
        }

        private void ResetColor()
        {
            currentColor = startColor;
            spriteRenderer.color = currentColor;
        }

        private void CheckWhetherToFreezeAnimation()
        {
            if(fadingAnimation)
            {
                animator.speed = 0;

                // Debug.Log("animator frozen: " + animator.speed);
            }

            else if(!fadingAnimation)
            {
                animator.speed = animatorStartSpeed;
            }
        }

        private void SetUpReigniters()
        {
            reigniters = StaticUtilities.ReturnInterfacesFromComponentsAndAnyControllerStates<IDarknessReignitable>(gameObject);

            foreach (var ignite in reigniters)
            {
                ignite.ReigniteRenderer += Reignite;
            }
        }

        private void DisableReigniters()
        {
            foreach (var ignite in reigniters)
            {
                ignite.ReigniteRenderer -= Reignite;
            }
        }
        
        private void GetDependencies()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}

public interface IDarknessReignitable
{
    event DarknessAnimatorFade.DarknessReigniteHandler ReigniteRenderer;
}