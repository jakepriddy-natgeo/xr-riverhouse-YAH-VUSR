using System.Collections;
using UnityEngine;
using VusrCore.APIv1;
using VusrCore.APIv1.InputSystems;

public class HotspotController : MonoBehaviour
{

    public static HotspotController Instance;

    public GameObject hotspot;
    public bool IsInExample;
    public GameObject Tripod;

    public GameObject[] SpawnPoints;
    public GameObject cube1;
    public GameObject cube2;

    private void Awake()
    {

        StartCoroutine(stallAwake());

        if (Instance == null)
        {
            Instance = this;
        }

        //get click, then "GetHoveredGameObject()", then do something

        //if (VusrInput.HoveredGameObject == cube1)
        //{
        //    // interact with gameobject
        //    cube1.SetActive(false);
        //    cube2.SetActive(true);

        //}

        //This
        PointerCameraListener listener = hotspot.GetComponent<PointerCameraListener>();
        if (listener == null)
        {
            hotspot.AddComponent<PointerCameraListener>();
        }

    }
    private IEnumerator Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        yield return null;


        Fader.FadeToClear((b) =>
        {
            SetPointer(true);
        });
    }

    void Update()
    {
        //Press the back button to complete interactive.
        //This leaves the interactive and goes back into the app.
        if (VusrInput.Back && !IsInExample)
        {
            Fader.FadeToBlack((b) => VusrCore.APIv1.Vusr.InteractiveComplete());
        }
    }

    public void CubeSwapper()
    {
       // if (VusrInput.HoveredGameObject == cube1)
      //  {
            // interact with gameobject
            cube1.SetActive(false);
            cube2.SetActive(true);
       // }
    }

    public void SetPointer(bool active)
    {
        VusrInput.IsPointerVisible = active;
        VusrInput.IsControllerVisible = active;
    }

    public IEnumerator FadeOutAndIn()
    {
        SetPointer(false);
        Fader.FadeToBlack();
        yield return new WaitForSeconds(2);
        Fader.FadeToClear((b) => SetPointer(true));
    }

    public void GoToOrigin()
    {
        SetPointer(false);
        Fader.FadeToBlack((b) =>
        {
            Tripod.transform.position = SpawnPoints[0].transform.position;
            //use spatial audio trigger system
        });

        StartCoroutine(FadeBack());
    }

    public void GoToPoint1()
    {
        SetPointer(false);
        Fader.FadeToBlack((b) =>
        {
            Tripod.transform.position = SpawnPoints[1].transform.position;
        });

        StartCoroutine(FadeBack());
    }

    public void GoToPoint2()
    {
        SetPointer(false);
        Fader.FadeToBlack((b) =>
        {
            Tripod.transform.position = SpawnPoints[2].transform.position;
        });

        StartCoroutine(FadeBack());
    }

    public void GoToPoint3()
    {
        SetPointer(false);
        Fader.FadeToBlack((b) =>
        {
            Tripod.transform.position = SpawnPoints[3].transform.position;
        });

        StartCoroutine(FadeBack());
    }

    public void OutWithCustomAction()
    {
        SetPointer(false);
        Fader.FadeToBlack((b) =>
        {
            Debug.Log("<color=orange>This is a custom action made inside the fader. Open HotspotController.cs to find out how.</color>");
        });

        StartCoroutine(FadeBack());
    }

    public void OutWithMethod()
    {
        SetPointer(false);
        Fader.FadeToBlack((b) => OutMethod(b));
    }

    public void OutMethod(bool interupted)
    {
        if (interupted)
        {
            Debug.Log("Fader was interupted during transition");
        }

        Debug.Log("<color=orange>This was inside a method called by fader. Open HotspotController.cs to find out how.</color>");

        StartCoroutine(FadeBack());
    }

    public IEnumerator FadeBack()
    {
        yield return new WaitForSeconds(3);
        Fader.FadeToClear((b => SetPointer(true)));
    }

    public void GoBack()
    {
        //MainController.Instance.UnloadScene();
    }

    IEnumerator stallAwake()
    {
        yield return new WaitForSeconds(2);
    }


}
