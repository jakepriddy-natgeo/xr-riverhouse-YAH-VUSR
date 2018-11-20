using System.Collections;
using UnityEngine;
using VusrCore.APIv1;
using VusrCore.APIv1.InputSystems;

public class ModdedFaderController : MonoBehaviour {

	public static ModdedFaderController Instance;

	//public GameObject OriginFaderCanvas;
    //public GameObject FaderCanvas1;
    //public GameObject FaderCanvas2;
    //public GameObject FaderCanvas3;

    public GameObject OriginPoint;
    public GameObject Point1;
    public GameObject Point2;
    public GameObject Point3;


    public GameObject Tripod;

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}

		//PointerCameraListener listener = GetComponent<PointerCameraListener>();
		//if (listener == null)
		//{
			////OriginFaderCanvas.AddComponent<PointerCameraListener>();
        //    FaderCanvas1.AddComponent<PointerCameraListener>();
        //    //FaderCanvas2.AddComponent<PointerCameraListener>();
        //    //FaderCanvas3.AddComponent<PointerCameraListener>();
        //}
	}

	private void Start()
	{
		SetPointer(true);
	}

	private void SetPointer(bool active)
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

	public void ToOriginPont()
	{
		//SetPointer(false);
		//Fader.FadeToBlack((b) =>
		//{
            Tripod.transform.position = OriginPoint.transform.position;
  //      });

		//StartCoroutine(FadeBack());
	}

    public void ToPoint1()
    {
        //SetPointer(false);
        //Fader.FadeToBlack((b) =>
        //{
            Tripod.transform.position = Point1.transform.position;
 //       });

 //       StartCoroutine(FadeBack());
    }


    public void ToPoint2()
    {
        //SetPointer(false);
        //Fader.FadeToBlack((b) =>
        //{
            Tripod.transform.position = Point2.transform.position;
        //});

        //StartCoroutine(FadeBack());
    }

    public void ToPoint3()
    {
        //SetPointer(false);
        //Fader.FadeToBlack((b) =>
        //{
            Tripod.transform.position = Point3.transform.position;
        //});

        //StartCoroutine(FadeBack());
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

		Debug.Log("<color=orange>This was inside a method called by fader. Open FaderController.cs to find out how.</color>");

		StartCoroutine(FadeBack());
	}

	public IEnumerator FadeBack()
	{
		yield return new WaitForSeconds(3);
		Fader.FadeToClear((b => SetPointer(true)));
	}

	public void GoBack()
	{
		ModdedMainController.Instance.UnloadScene();
	}
}
