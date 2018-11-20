using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VusrCore.APIv1;
using VusrCore.APIv1.InputSystems;

public class HotspotScript : MonoBehaviour {

	public HotspotType Type;

    private void Awake()
	{
		PointerCameraListener listener = GetComponent<PointerCameraListener>();
		if(listener == null)
		{
			gameObject.AddComponent<PointerCameraListener>();
		}


    }

	public void Click()
	{
		switch (Type)
		{
            case HotspotType.GoToOrigin:
                HotspotController.Instance.GoToOrigin();
                break;

			case HotspotType.GoToPoint1:
                HotspotController.Instance.GoToPoint1();
                break;

			case HotspotType.GoToPoint2:
                HotspotController.Instance.GoToPoint2();
                break;

            case HotspotType.GoToPoint3:
                HotspotController.Instance.GoToPoint3();
                break;
            case HotspotType.CubeSwapper:
                HotspotController.Instance.CubeSwapper();
                break;
        }
	}

	public enum HotspotType
	{
        GoToOrigin,
        GoToPoint1,
        GoToPoint2,
        GoToPoint3,
        CubeSwapper

    }
}
