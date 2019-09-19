using System.Collections;
using AbstractClasses;
using UnityEngine;
using UnityEngine.SceneManagement;

//exit conditions for moving to the next scene
namespace Services.SceneManagement
{
    public class Ending : Service
    {
        public bool startLoaded = false;

        void Update()
        {
            if(Input.GetButtonDown("Jump"))
            {
                LoadOpeningScene();
                Debug.Log("startLoaded");
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }        
        }

        public void LoadOpeningScene()
        {
            if(!startLoaded)
            {
                startLoaded = true;
                StartCoroutine(LoadYourAsyncScene());
            }
        }

        IEnumerator LoadYourAsyncScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}
