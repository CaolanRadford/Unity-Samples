using System.Collections;
using AbstractClasses;
using UnityEngine;
using UnityEngine.SceneManagement;

//exit conditions for moving to the next scene
namespace Services.SceneManagement
{
    public class ControlsExplainer : Service
    {
        void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                StartCoroutine(LoadYourAsyncScene());
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
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
