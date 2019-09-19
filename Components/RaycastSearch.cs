using System.Collections;
using System.Collections.Generic;
using Controllers;
using K_LanUtilities;
using Services;
using UnityEngine;

namespace Components
{
    //shoots a ray a certain distance from the entity, and moves it up and down looking for a target
    public class RaycastSearch : MonoBehaviour, IDirectionUpdateable, IGM_ServiceLocator_Recievable
    {
        //defined interfaces
        public delegate void RaycastSearchDelegate(bool searchActive);
        public delegate void targetNulling();

        private ISearchingStatusChangable[] searchingComponents;
        private ITargetUpdatable[] targettingComponents;
        private ITargetNullable[] targetNullableComponents;  

        //dependencies
        private GM_ServiceLocator gm;

        //working variables
        private bool searching;
        private Vector2 facingDirection;
        private bool isFacingRight;
        private RaycastHit2D searchRayResult;
        private float t = 0;
        private bool tAscending;    

        //design variables
        [SerializeField]
        private float vecLeftUp_Angle;

        [SerializeField]
        private float vecLeftDown_Angle;

        [SerializeField]
        private float vecRightUp_Angle;

        [SerializeField]
        private float vecRightDown_Angle;

        [SerializeField]
        private float searchRange = 0;

        [SerializeField]
        private LayerMask targetLayerMask = 0;

        [SerializeField]
        private float scanSpeed = .05f;

        private float lerpT_minimum = 0;
        private float lerpT_Maximum = 1;

        void OnEnable()
        {
            InitialiseChangeOfSearchingStatusComponents();
            InitialiseTargetUpdatableComponents();
            InitialiseTargetNullingComponents();

            SetUpChangeSearchingStatusInterfaces();
            SetUpTargetNullableInterfaces();

            SetUpSearchBoundaries();
        }

        void OnDisable()
        {
            DisolveChangeSearchStatusInterfaces();
            DisolveTargetNullableInterfaces();
        }

        void Update()
        {
            AnimatedTvariable();

            if(searching)
            {
                if(gm.GetService<LightsController>().lightsOn)
                {
                    CastSearchingRay();

                    if (searchRayResult.collider != null)
                    {
                        Collider2D col = searchRayResult.collider;

                        if(StaticUtilities.CheckIdentity<Ground>(col.gameObject))
                        {
                            return;
                        }

                        if (StaticUtilities.CheckIdentity<RatbagPlayerBrain>(col.gameObject))
                        {
                            foreach (ITargetUpdatable targetter in targettingComponents)
                            {
                                targetter.TargetFound(col.gameObject);
                            }
                        }
                    }
                }
            }
        }

        private void CastSearchingRay()
        {
            if (isFacingRight)
            {
                searchRayResult = CastRightSearchingRay();
                // Debug.DrawRay(transform.position, facingDirection * searchRange, Color.green, .01f);
            }
            else if (!isFacingRight)
            {
                searchRayResult = CastLeftSearchingRay();
            }
        }

        private void AnimatedTvariable()
        {
            t += scanSpeed * Time.deltaTime;

            if (t > 1.0f)
            {
                float temp = lerpT_Maximum;
                lerpT_Maximum = lerpT_minimum;
                lerpT_minimum = temp;
                t = 0.0f;
            }
        }

        private RaycastHit2D CastRightSearchingRay()
        {
            float lerpAngleRight = Mathf.LerpAngle(vecRightUp_Angle, vecRightDown_Angle, t);
            Vector2 lerpVectorRight = StaticUtilities.Vector2FromAngle(lerpAngleRight);
        
            Debug.DrawRay(transform.position, lerpVectorRight * searchRange, Color.magenta, .01f);

            return Physics2D.Raycast(transform.position, lerpVectorRight, searchRange, targetLayerMask);
        }

        private RaycastHit2D CastLeftSearchingRay()
        {
            float lerpAngleLeft = Mathf.LerpAngle(vecLeftUp_Angle, vecLeftDown_Angle, t);
            Vector2 lerpVectorLeft = StaticUtilities.Vector2FromAngle(lerpAngleLeft);

            Debug.DrawRay(transform.position, lerpVectorLeft * searchRange, Color.magenta, .01f);

            return Physics2D.Raycast(transform.position, lerpVectorLeft, searchRange, targetLayerMask);
        }

        private void SetUpSearchBoundaries()
        {
            Vector2 vecLeftUp = StaticUtilities.Vector2FromAngle(vecLeftUp_Angle);
            // Debug.DrawRay(transform.position, vecLeftUp, Color.red, .5f);

            Vector2 vecLeftDown = StaticUtilities.Vector2FromAngle(vecLeftDown_Angle);
            // Debug.DrawRay(transform.position, vecLeftDown, Color.red, .5f);

            Vector2 vecRightUp = StaticUtilities.Vector2FromAngle(vecRightUp_Angle);
            // Debug.DrawRay(transform.position, vecRightUp, Color.red, .5f);

            Vector2 vecRightDown = StaticUtilities.Vector2FromAngle(vecRightDown_Angle);
            // Debug.DrawRay(transform.position, vecRightDown, Color.red, .5f);
        }

        public void UpdatingDirection(Vector2 direction)
        {
            facingDirection = direction;

            float dot = Vector2.Dot(direction, Vector2.right);

            if(dot > 0)
            {
                isFacingRight = true;
            }
            else
            {
                isFacingRight = false;
            }
        }

        public void ChangeSearchingStatus(bool searchActive)
        {
            searching = searchActive;
        }

        public void AssignGM(GM_ServiceLocator serviceLocator)
        {
            gm = serviceLocator;
        }

        public void NullTheTarget()
        {
            foreach(ITargetUpdatable targetter in targettingComponents)
            {
                targetter.TargetFound(null);
            }
        }

        private void SetUpChangeSearchingStatusInterfaces()
        {
            for (int i = 0; i < searchingComponents.Length; i++)
            {
                ISearchingStatusChangable searcher = searchingComponents[i];
                searcher.SearchingStatus += ChangeSearchingStatus;
            }
        }
        private void SetUpTargetNullableInterfaces()
        {
            for (int i = 0; i < targetNullableComponents.Length; i++)
            {
                ITargetNullable targetNullable = targetNullableComponents[i];
                targetNullable.TargetNull += NullTheTarget;
            }
        }
        private void DisolveTargetNullableInterfaces()
        {
            for (int i = 0; i < targetNullableComponents.Length; i++)
            {
                ITargetNullable targetNullable = targetNullableComponents[i];
                targetNullable.TargetNull -= NullTheTarget;
            }
        }
        private void DisolveChangeSearchStatusInterfaces()
        {
            for (int i = 0; i < searchingComponents.Length; i++)
            {
                ISearchingStatusChangable searcher = searchingComponents[i];
                searcher.SearchingStatus -= ChangeSearchingStatus;
            }
        }

        private void InitialiseTargetNullingComponents()
        {
            targetNullableComponents = StaticUtilities.ReturnOnlyActiveControllers<ITargetNullable>(GetComponents<ITargetNullable>(), this);
        }

        private void InitialiseTargetUpdatableComponents()
        {
            targettingComponents = StaticUtilities.ReturnOnlyActiveControllers<ITargetUpdatable>(GetComponents<ITargetUpdatable>(), this);
        }

        private void InitialiseChangeOfSearchingStatusComponents()
        {
            searchingComponents = StaticUtilities.ReturnOnlyActiveControllers<ISearchingStatusChangable>(GetComponents<ISearchingStatusChangable>(), this);
        }            
    }

    public interface ISearchingStatusChangable
    {
        event RaycastSearch.RaycastSearchDelegate SearchingStatus;
    }

    public interface ITargetUpdatable
    {
        void TargetFound(GameObject target);
    }

    public interface ITargetNullable
    {
        event RaycastSearch.targetNulling TargetNull;
    }
}