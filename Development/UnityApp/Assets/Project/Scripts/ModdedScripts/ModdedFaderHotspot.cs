using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VusrCore.APIv1;
using VusrCore.APIv1.InputSystems;

public class ModdedFaderHotspot : MonoBehaviour {

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
				StartCoroutine(ModdedFaderController.Instance.FadeOutAndIn());
				break;
            case FaderHotspotType.ToOrigin:
				ModdedFaderController.Instance.ToOriginPont();
				break;
            case FaderHotspotType.ToPoint1:
                ModdedFaderController.Instance.ToPoint1();
                break;
            case FaderHotspotType.ToPoint2:
                ModdedFaderController.Instance.ToPoint2();
                break;
            case FaderHotspotType.ToPoint3:
                ModdedFaderController.Instance.ToPoint3();
                break;
            case FaderHotspotType.OutMethod:
				ModdedFaderController.Instance.OutWithMethod();
				break;
		}
	}

	public enum FaderHotspotType
	{
		OutAndIn,
		OutCustom,
		OutMethod,
        ToOrigin,
        ToPoint1,
        ToPoint2,
        ToPoint3,

	}
}
