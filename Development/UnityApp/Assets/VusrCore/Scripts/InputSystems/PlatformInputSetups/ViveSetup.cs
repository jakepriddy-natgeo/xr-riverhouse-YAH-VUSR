/*************************************************************************
 * 
 * SECRET LOCATION CONFIDENTIAL
 * __________________
 * 
 *  Copyright [2014] - [2017] Secret Location Incorporated 
 *  All Rights Reserved.
 * 
 * NOTICE:  All information contained herein is, and remains
 * the property of Secret Location Incorporated and its suppliers,
 * if any.  The intellectual and technical concepts contained
 * herein are proprietary to Secret Location Incorporated
 * and its suppliers and may be covered by U.S. and Foreign Patents,
 * patents in process, and are protected by trade secret or copyright law.
 * Dissemination of this information or reproduction of this material
 * is strictly forbidden unless prior written permission is obtained
 * from Secret Location Incorporated.
 */

using System;
using UnityEngine;
using Valve.VR;

namespace VusrCore.APIv1.InputSystems
{

	/// <summary>
	/// InputSetup for enabling Vive Input. Connects to a LaserInputModule in the EventSystem. 
	/// </summary>
	[DisallowMultipleComponent]
	public class ViveSetup : BaseInputSetup<ViveInputParams>
	{
		[SerializeField] private SteamVR_ControllerManager _cameraRigPrefab;
		[SerializeField] private SteamVR_Render _steamVRMainPrefab;

		private MotionControllerInputModule _laserInputModule;

		/// <inheritdoc />
		public override VRDeviceMask SupportedDevices
		{
			get { return VRDeviceMask.SteamVR; }
		}

		/// <inheritdoc />
		public override void Initialize(Transform playerRoot, VusrEventSystem eventSystem)
		{
			Instantiate(_steamVRMainPrefab, playerRoot);
			SteamVR_ControllerManager cameraRig = Instantiate(_cameraRigPrefab, playerRoot);

			_laserInputModule = eventSystem.gameObject.AddComponent<MotionControllerInputModule>();

			_laserInputModule.LeftController = cameraRig.left.AddComponent<ViveController>();
			(_laserInputModule.LeftController as ViveController).NativeController = _laserInputModule.LeftController.GetComponent<SteamVR_TrackedObject>();
			_laserInputModule.LeftController.ControllerModel = _laserInputModule.LeftController.GetComponentInChildren<SteamVR_RenderModel>().gameObject;

			_laserInputModule.RightController = cameraRig.right.AddComponent<ViveController>();
			(_laserInputModule.RightController as ViveController).NativeController = _laserInputModule.RightController.GetComponent<SteamVR_TrackedObject>();
			_laserInputModule.RightController.ControllerModel = _laserInputModule.RightController.GetComponentInChildren<SteamVR_RenderModel>().gameObject;


		}

		/// <inheritdoc />
		public override void ApplyParams(ViveInputParams mParams)
		{
			if (mParams.OverridePointer != null)
				_laserInputModule.Pointer = mParams.OverridePointer;

			_laserInputModule.LeftController.AxisDeadZone = mParams.AxisDeadZone;
			_laserInputModule.RightController.AxisDeadZone = mParams.AxisDeadZone;

			_laserInputModule.LeftController.AutoSwipeZone = mParams.SwipeAutozone;
			_laserInputModule.RightController.AutoSwipeZone = mParams.SwipeAutozone;

		}
	}// End ViveSetup class

	/// <summary>
	/// Parameters for controlling <see cref="SteamVR"/> (None implemented yet).
	/// </summary>
	[Serializable]
	public struct ViveInputParams : IInputParams
	{
		/// <summary>
		/// A pointer that will override the default pointer selected in VRCameraRig. Will only be applied if not null.
		/// </summary>
		public BasePointer OverridePointer;

		public float AxisDeadZone;
		public float SwipeAutozone;

		/// <inheritdoc />
		public void InitializeDefaultValues()
		{
			OverridePointer = null;
			AxisDeadZone = 0.25f;
			SwipeAutozone = 0.75f;
		}
	}

	/// <summary>Provides the input mappings to the input module</summary>
	public class ViveController : MotionController
	{
		/// <summary>
		/// The object used to get Input fromSteamVR via <see cref="SteamVR_TrackedObject.index"/>
		/// </summary>
		public SteamVR_TrackedObject NativeController;

		/// <summary>
		/// The utility class obtained from SteamVR for parsing input.
		/// </summary>
		public SteamVR_Controller.Device Input
		{
			get { return SteamVR_Controller.Input(ControllerIndex); }
		}


		private int ControllerIndex
		{
			get { return (int) NativeController.index; }
		}

		/// <summary>ControllerIndex != -1;</summary>
		public override bool IsAvailable
		{
			get { return ControllerIndex != -1; }
		}

		/// <returns><see cref="NativeController"/></returns>
		public override object NativeObject
		{
			get { return NativeController; }
		}

		/// <summary>Returns true if valid and anything is pressed</summary>
		public override bool IsRequestingSwap()
		{
			return NativeController.isValid && IsAnyButtonPressed();
		}

		public override AxisTypes AxisType { get { return AxisTypes.TouchPad; } }

		/// <summary>IInputProviderExtended.SwipeAutoZone</summary>
		public override float AutoSwipeZone { get; set; }

		/// <summary>EVRButtonId.k_EButton_SteamVR_Touchpad</summary>
		public override Vector2 GetSwipeAxis()
		{
			return Input.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
		}

		/// <summary>EVRButtonId.k_EButton_SteamVR_Touchpad</summary>
		public override bool GetAxisTouch()
		{
			return Input.GetTouch(EVRButtonId.k_EButton_SteamVR_Touchpad);
		}

		/// <summary>EVRButtonId.k_EButton_SteamVR_Trigger</summary>
		public override bool GetClickButton()
		{
			return Input.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger) ||
			       Input.GetPress(EVRButtonId.k_EButton_SteamVR_Touchpad);
		}

		/// <summary>EVRButtonId.k_EButton_ApplicationMenu</summary>
		public override bool GetBackButton()
		{
			return Input.GetPress(EVRButtonId.k_EButton_ApplicationMenu);
		}

		public override void UpdateVisibility()
		{
			bool shouldRender = ShouldRenderModel && VusrInput.IsControllerVisible && VusrInput.CurrentInputModule == Module;
			if (ControllerModel != null)
				 ControllerModel.transform.localScale = shouldRender ? Vector3.one : Vector3.zero;
		}
		
		/// <summary>Checks if any button is pressed.</summary>
		public bool IsAnyButtonPressed()
		{
			return Input.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)
			       || Input.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)
			       || Input.GetPressDown(SteamVR_Controller.ButtonMask.Axis0)
			       || Input.GetPressDown(SteamVR_Controller.ButtonMask.Axis1)
			       || Input.GetPressDown(SteamVR_Controller.ButtonMask.Axis2)
			       || Input.GetPressDown(SteamVR_Controller.ButtonMask.Axis3)
			       || Input.GetPressDown(SteamVR_Controller.ButtonMask.Axis4)
			       || Input.GetPressDown(SteamVR_Controller.ButtonMask.Grip)
			       || Input.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu);
		}
	} // End ViveController class
}// End VusrCore.APIv1.InputSystems namespace
