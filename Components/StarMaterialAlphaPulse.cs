using UnityEngine;
using System.Collections;

namespace Components
{
    //a recursive coroutine to brighten and fade environmental stars over time
    public class StarMaterialAlphaPulse : MonoBehaviour
    {
        private SpriteRenderer rend;
        private Material mat;

        [SerializeField] private float randTimeFloor;
        [SerializeField] private float randTimeRoof;

        public float lerpTime = 1f;
        float currentLerpTime;

        private float a;
        private float b;
        
        private Color currentCol;
        private Color targetCol;
        
        private void OnEnable()
        {
            rend = GetComponent<SpriteRenderer>();
            mat = GetComponent<SpriteRenderer>().material;

            lerpTime = UnityEngine.Random.Range(2, 6);
        }

        private void Start()
        {
            float randomAlphaVal = UnityEngine.Random.Range(0f, .8f);

            AssignNewAlpha(randomAlphaVal);

            StartCoroutine(LerpStarAlpha(mat.color.a, 1));
        }

        private void AssignNewAlpha(float newAlphaVal)
        {
            var col = mat.color;
            col.a = newAlphaVal;

            rend.material.color = col;
        }

        public IEnumerator LerpStarAlpha(float aVal, float bVal)
        {
            float randomTime = UnityEngine.Random.Range(randTimeFloor, randTimeRoof);

//            Debug.Log("initial time: " + randomTime);
            
            yield return new WaitForSeconds(randomTime);

//            Debug.Log("wait for seconds done");

            while (currentLerpTime < lerpTime)
            {
                IncreaseTime();

                float t = currentLerpTime / lerpTime;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                float newAlpha = Mathf.Lerp(aVal, bVal, t);
                
//                Debug.Log("lerping, t: " + t);

                AssignNewAlpha(newAlpha);

                yield return null;
            }

            float newRandomTime = UnityEngine.Random.Range(.5f, 2);

//            Debug.Log("2nd wait before descending: " + newRandomTime);

            yield return new WaitForSeconds(newRandomTime);

            currentLerpTime = 0;

            while (currentLerpTime < lerpTime)
            {
                IncreaseTime();

                float t = currentLerpTime / lerpTime;
                t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                
//                Debug.Log("lerping, t: " + t);

                float newAlpha = Mathf.Lerp(bVal, 0, t);

                AssignNewAlpha(newAlpha);
                
                yield return null;
            }

            newRandomTime = UnityEngine.Random.Range(randTimeFloor, randTimeRoof);
            
//            Debug.Log("3rd wait before ascending: " + newRandomTime);

            yield return new WaitForSeconds(newRandomTime);

            float newB = UnityEngine.Random.Range(0, .8f);

            currentLerpTime = 0;

            while (currentLerpTime < lerpTime)
            {
                IncreaseTime();

                float t = currentLerpTime / lerpTime;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

//                Debug.Log("lerping, t: " + t);

                float newAlpha = Mathf.Lerp(0, newB, t);

                AssignNewAlpha(newAlpha);
                
                yield return null;
            }

            StartCoroutine(LerpStarAlpha(mat.color.a, 1));
        }

        private void IncreaseTime()
        {
            currentLerpTime += Time.deltaTime;

            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }
        }

        private float ReturnRandom(float floor, float room)
        {
            return UnityEngine.Random.Range(floor, room);
        }
    }
}
