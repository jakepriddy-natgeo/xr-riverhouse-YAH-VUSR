using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VusrCore.APIv1;
using VusrCore.APIv1.InputSystems;

public class FaderTeleportHotspot : MonoBehaviour {

	public FaderHotspotType Type;

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
			case FaderHotspotType.OutAndIn:
				StartCoroutine(FaderTeleportController.Instance.FadeOutAndIn());
				break;
			case FaderHotspotType.OutCustom:
				FaderTeleportController.Instance.OutWithCustomAction();
				break;
			case FaderHotspotType.GoToOrigin:
				FaderTeleportController.Instance.GoToOrigin();
				break;
            case FaderHotspotType.GoToPoint1:
                FaderTeleportController.Instance.GoToPoint1();
                break;
            case FaderHotspotType.GoToPoint2:
                FaderTeleportController.Instance.GoToPoint2();
                break;
            case FaderHotspotType.GoToPoint3:
                FaderTeleportController.Instance.OutWithMethod();
                break;
        }
	}

	public enum FaderHotspotType
	{
		OutAndIn,
		OutCustom,
		OutMethod,
        GoToOrigin,
        GoToPoint1,
        GoToPoint2,
        GoToPoint3
    }
}
