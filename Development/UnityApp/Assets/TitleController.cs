using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VusrCore.APIv1;
using VusrCore.APIv1.InputSystems;


public class TitleController : MonoBehaviour {

    public string sceneName;
    public int IntroSceneLength;
    private AsyncOperation asyncOperation;
    //public static Fader.FaderStates State { get; };

    // Use this for initialization
    IEnumerator Start()
    {
        yield return null;
        VusrInput.IsPointerVisible = false;
        VusrInput.IsControllerVisible = false;

        //How to check to fader state?

        if (Fader.State == Fader.FaderStates.Black)
            {
     
            yield return new WaitForSeconds(2);
            Debug.Log("it's " + Fader.State);
            Fader.FadeToClear();
            }

        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        StartCoroutine(sceneLength());
        Fader.FadeToBlack((b) => loadScene());
    }
	
	// Update is called once per frame
    public IEnumerator loadScene () {
        yield return new WaitForSeconds(2);
        asyncOperation.allowSceneActivation = true;
    }

    public IEnumerator sceneLength()
    {
        yield return new WaitForSeconds(IntroSceneLength);
    }


}
