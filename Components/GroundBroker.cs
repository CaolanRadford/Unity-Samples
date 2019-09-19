using K_LanUtilities;
using UnityEngine;

namespace Components
{
    //brokers a relationship between the gameObject and the ground, exposing and measuring certain criteria for pass off to other components like
    //the GroundedStatus event(this should probably be a property instead of an event that is raised every frame)). This component is for nuanced
    //character control
    public class GroundBroker : MonoBehaviour, IDirectionUpdateable
    {
        //defined interfaces
        public delegate void NewGroundVectorUpdate(Vector2 groundVector);
        public event NewGroundVectorUpdate OnGroundVectorUpdate;
        private IGroundDirectionVectorUpdatable[] groundVecUpdatables;

        public delegate void GroundedStatusUpdate(bool newStatus);
        public event GroundedStatusUpdate OnGroundedUpdate;
        private IGroundedUpdatable[] groundedUpdatables;

        //utilised interfaces
        private Vector2 currentFacingDirection;

        //dependencies
        private Rigidbody2D rb;                    //should go through adaptors
        private Collider2D thisCol;

        //design variables
        [SerializeField]
        private LayerMask groundMask;
        private ContactFilter2D groundFilter;
        
        [SerializeField]
        private float groundedDistanceThreshold;

        //fields
        private RaycastHit2D[] castResults = new RaycastHit2D[8];

        private Vector2[] currentTargetCentroids = new Vector2[8];

        private Vector2 currentGroundVector;

        [SerializeField]
        private bool grounded = false;    

        //debugging
        [SerializeField]
        private GameObject[] debugObjs;

        void OnEnable()
        {
            InitialiseDependants();
            CreateGroundBasedContactFilter();
        }

        private void Start()
        {
            SetUp_IGroundVectorUpdatables();
            SetUp_IGroundedUpdatables();
        }

        void OnDisable()
        {
            Disolve_IGroundVectorUpdatables();
            Disolve_IGroundedUpdatables();
        }

        void Update()
        {
            CheckGround();
        }

        private void CheckGround()
        {
            int resultCount = CastCollider();

            // DrawDebugCubes(resultCount);

            AssignProspectiveCentroids(resultCount);

            if (resultCount > 0)
            {
                CheckProspectiveCentroids(resultCount);
            }
            else
            {
                ChangeGroundedStatus(false);
            }
        }

        private int CastCollider()
        {
            return thisCol.Cast(Vector2.down, groundFilter, castResults, .1f);
        }

        private void AssignProspectiveCentroids(int amountOfProspects)
        {
            for (int i = 0; i < amountOfProspects; i++)
            {
                currentTargetCentroids[i] = castResults[i].centroid;
            }
        }

        private void CheckProspectiveCentroids(int colliderCastResultCount)
        {
            for (int i = 0; i < colliderCastResultCount; i++)
            {
                if (StaticUtilities.Vector2_Equal_CustomThreshold(transform.position, currentTargetCentroids[i], groundedDistanceThreshold))
                {
                    // Debug.Log("t pos: " + transform.position + " || centroidPos: " + currentTargetCentroids[i] + " || Distance: " + Vector2.Distance(transform.position, currentTargetCentroids[i]));

                    Vector2 newGroundVector = CreateNewGroundVector(i);

                    ChangeGroundVector(newGroundVector);

                    ChangeGroundedStatus(true);
                }

                else
                {
                    ChangeGroundedStatus(false);
                }
            }
        }

        private Vector2 CreateNewGroundVector(int selectedColliderCastResult)
        {
            Vector2 newGroundVector = Vector2.Perpendicular(castResults[selectedColliderCastResult].normal);
            newGroundVector = FaceDirectionCorrectedGroundVector(newGroundVector);

            return newGroundVector;
        }

        private void ChangeGroundVector(Vector2 newGroundVectorDirectionCorrected)
        {
            if(newGroundVectorDirectionCorrected != currentGroundVector)
            {
                OnGroundVectorUpdate?.Invoke (newGroundVectorDirectionCorrected);
            }
        }

        private void ChangeGroundedStatus(bool newStatus)
        {
            if(grounded != newStatus)
            {
                grounded = newStatus;

                OnGroundedUpdate?.Invoke(newStatus);
            }
        }

        public void UpdatingDirection(Vector2 newDirection)
        {
            currentFacingDirection = newDirection;

            Vector2 flippedGroundVector = FaceDirectionCorrectedGroundVector(currentFacingDirection);

            ChangeGroundVector(flippedGroundVector);
        }

        private Vector2 FaceDirectionCorrectedGroundVector(Vector2 newGroundVector)
        {
            if (Vector2.Dot(currentFacingDirection, newGroundVector) > 0)
            {
                return newGroundVector;
            }
            else
            {
                return -newGroundVector;
            }
        }

        private Vector2 VelocityDivided()
        {
            return rb.velocity / 3;
        }

        private void InitialiseDependants()
        {
            rb = GetComponent<Rigidbody2D>();
            thisCol = GetComponent<Collider2D>();
        }

        private void CreateGroundBasedContactFilter()
        {
            groundFilter.layerMask = groundMask;
            groundFilter.useLayerMask = true;
        }

        private void SetUp_IGroundVectorUpdatables()
        {
            groundVecUpdatables = StaticUtilities.ReturnInterfacesFromComponentsAndAnyControllerStates<IGroundDirectionVectorUpdatable>(gameObject);
            
            for (int i = 0; i < groundVecUpdatables.Length; i++)
            {
                OnGroundVectorUpdate += groundVecUpdatables[i].GroundDirectionUpdate;
            }
        }

        private void SetUp_IGroundedUpdatables()
        {
            groundedUpdatables = StaticUtilities.ReturnInterfacesFromComponentsAndAnyControllerStates<IGroundedUpdatable>(gameObject);
            
            for (int i = 0; i < groundedUpdatables.Length; i++)
            {
                OnGroundedUpdate += groundedUpdatables[i].GroundedUpdate;
            }
        }

        private void Disolve_IGroundVectorUpdatables()
        {
            for (int i = 0; i < groundVecUpdatables.Length; i++)
            {
                OnGroundVectorUpdate -= groundVecUpdatables[i].GroundDirectionUpdate;
            }
        }

        private void Disolve_IGroundedUpdatables()
        {
            for (int i = 0; i < groundedUpdatables.Length; i++)
            {
                OnGroundedUpdate -= groundedUpdatables[i].GroundedUpdate;
            }
        }

        private void ChangeGroundVector(Vector2 newGroundVectorDirectionCorrected, Vector2 debugStartRayPos)
        {
            // Debug.Log("Ground Broker groundVector changed");

            if(newGroundVectorDirectionCorrected != currentGroundVector)
            {
                Debug.DrawRay(debugStartRayPos, newGroundVectorDirectionCorrected, Color.green, .01f);
                OnGroundVectorUpdate?.Invoke (newGroundVectorDirectionCorrected);
            }
        }  

        private void DrawDebugCubes(int resultCount)
        {
            // Debug.Log("Number of hits: " + resultCount);

            for (int i = 0; i < resultCount; i++)
            {
                debugObjs[i].SetActive(true);

                Debug.DrawRay(castResults[i].centroid, (castResults[i].point - castResults[i].centroid), Color.red, 1f);

                debugObjs[i].transform.position = castResults[i].point;

                Debug.Log("Hit gameObject.name: " + castResults[i].transform.name);
                // Debug.Log("transform.position: " + (Vector2)transform.position);
                // Debug.Log("target centroid Position: " + currentTargetCentroids[i]);

                // Debug.Break();
            }

            for(int i = resultCount; i < castResults.Length; i++)
            {
                debugObjs[i].SetActive(false);
            }
        }
    }

    public interface IGroundDirectionVectorUpdatable
    {
        void GroundDirectionUpdate(Vector2 groundVector);
    }

    public interface IGroundedUpdatable
    {
        void GroundedUpdate(bool newGroundedStatus);
    }
}