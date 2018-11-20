using System.Collections;
using UnityEngine;
using VusrCore.APIv1;
using VusrCore.APIv1.InputSystems;

public class FaderTeleportController : MonoBehaviour {

	public static FaderTeleportController Instance;

	public GameObject FaderCanvas;
    public GameObject Tripod;
    public GameObject[] SpawnPoints;

    private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}

		PointerCameraListener listener = FaderCanvas.GetComponent<PointerCameraListener>();
		if (listener == null)
		{
			FaderCanvas.AddComponent<PointerCameraListener>();
		}
	}

	private void Start()
	{
		SetPointer(true);
	}

	public void SetPointer(bool active)
	{
		VusrInput.IsPointerVisible = active;
		VusrInput.IsControllerVisible = active;
	}

    public void GoToOrigin()
    {

        Tripod.transform.position = SpawnPoints[0].transform.position;

    }

    public void GoToPoint1()
    {

        Tripod.transform.position = SpawnPoints[1].transform.position;

    }

    public void GoToPoint2()
    {

        Tripod.transform.position = SpawnPoints[2].transform.position;

    }

    public void GoToPoint3()
    {

        Tripod.transform.position = SpawnPoints[3].transform.position;
    }


    public IEnumerator FadeOutAndIn()
	{
		SetPointer(false);
		Fader.FadeToBlack();
		yield return new WaitForSeconds(2);
		Fader.FadeToClear((b) => SetPointer(true));
	}

	public void OutWithCustomAction()
	{
		SetPointer(false);
		Fader.FadeToBlack((b) =>
		{
			Debug.Log("<color=orange>This is a custom action made inside the fader. Open FaderController.cs to find out how.</color>");
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
//		ModdedMainController.Instance.UnloadScene();
	}
}
