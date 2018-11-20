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
using System.Collections;
using UnityEngine;

namespace VusrCore.APIv1.InputSystems
{
	/// <summary>
	/// InputSetup for enabling oculus touch. Connects to a LaserInputModule in the EventSystem. 
	/// </summary>
	[DisallowMultipleComponent]
	public class OculusTouchSetup : BaseInputSetup<OculusTouchInputParams>
	{
		[SerializeField] private OVRCameraRig _ovrCameraRigPrefab;
		[SerializeField] private OvrAvatar _localAvatarPrefab;
		private MotionControllerInputModule _laserInputModule;
		private OvrAvatar _ovrAvatar;
		private bool _isInit;

		/// <inheritdoc />
		public override VRDeviceMask SupportedDevices
		{
			get { return VRDeviceMask.OculusWindows; }
		}

		/// <inheritdoc />
		public override void Initialize(Transform playerRoot, VusrEventSystem eventSystem)
		{
			_laserInputModule = eventSystem.gameObject.AddComponent<MotionControllerInputModule>();

			DontDestroyOnLoad(OvrAvatarSDKManager.Instance.gameObject);

			OVRCameraRig cameraRig = Instantiate(_ovrCameraRigPrefab, playerRoot);
			_localAvatarPrefab.StartWithControllers = true;
			if(_localAvatarPrefab.SurfaceShader == null) _localAvatarPrefab.SurfaceShader = _localAvatarPrefab.SurfaceShaderPBS;
			if(_localAvatarPrefab.SurfaceShaderSelfOccluding == null) _localAvatarPrefab.SurfaceShaderSelfOccluding = _localAvatarPrefab.SurfaceShaderPBS;
			_ovrAvatar = Instantiate(_localAvatarPrefab, playerRoot);
		    _laserInputModule.LeftController = cameraRig.leftHandAnchor.gameObject.AddComponent<OculusTouchController>();
			_ovrAvatar.ShowControllers(true);
			GameObject leftParent = new GameObject("LeftVisuals");
			leftParent.transform.parent = _ovrAvatar.ControllerLeft.transform.parent;
			_ovrAvatar.ControllerLeft.transform.parent = leftParent.transform;
			_ovrAvatar.HandLeft.transform.parent = leftParent.transform;
			_laserInputModule.LeftController.ControllerModel = leftParent;
			(_laserInputModule.LeftController as OculusTouchController).ControllerType = OVRInput.Controller.LTouch;

			_laserInputModule.RightController = cameraRig.rightHandAnchor.gameObject.AddComponent<OculusTouchController>();
			GameObject rightParent = new GameObject("RightVisuals");
			rightParent.transform.parent = _ovrAvatar.ControllerRight.transform.parent;
			_ovrAvatar.ControllerRight.transform.parent = rightParent.transform;
			_ovrAvatar.HandRight.transform.parent = rightParent.transform;
			_laserInputModule.RightController.ControllerModel = rightParent;
			(_laserInputModule.RightController as OculusTouchController).ControllerType = OVRInput.Controller.RTouch;
			StartCoroutine(DelayedApply());
		}

		/// <inheritdoc />
		/// <remarks>This cannot be called until .5 seconds after <see cref="Initialize"/> due to a mesh error.</remarks>
		public override void ApplyParams(OculusTouchInputParams mParams)
		{

			if (mParams.OverridePointer != null)
				_laserInputModule.Pointer = mParams.OverridePointer;

			if (!_isInit)
				return;

			_ovrAvatar.HandLeft.gameObject.SetActive(mParams.ShouldShowHands);
			_ovrAvatar.ControllerLeft.gameObject.SetActive(mParams.ShouldShowControllers);
			_laserInputModule.LeftController.AxisDeadZone = mParams.AxisDeadzone;
			_laserInputModule.LeftController.ScrollDelay = mParams.ScrollDelay;
			_laserInputModule.LeftController.MaxScrollDelay = mParams.MaxScrollDelay;

			_ovrAvatar.HandRight.gameObject.SetActive(mParams.ShouldShowHands);
			_ovrAvatar.ControllerRight.gameObject.SetActive(mParams.ShouldShowControllers);
			_laserInputModule.RightController.AxisDeadZone = mParams.AxisDeadzone;
			_laserInputModule.RightController.ScrollDelay = mParams.ScrollDelay;
			_laserInputModule.RightController.MaxScrollDelay = mParams.MaxScrollDelay;

			_ovrAvatar.Base.gameObject.SetActive(mParams.ShouldShowBase);
		}

		/// <summary>
		/// Coroutine Calls <see cref="ApplyParams"/> after .5 seconds. Invoked by the Initializer.
		/// ApplyParams will have no effect until this coroutine is finished.
		/// </summary>
		/// <remarks> This is necessary </remarks>
		private IEnumerator DelayedApply()
		{
			yield return new WaitForSeconds(.5f);
			_isInit = true;
			ApplyParams(CurrentParameters);
		}
	}// End OculusTouchSetup class

	/// <summary>
	/// Parameters for Oculus touch rendering.
	/// </summary>
	[Serializable]
	public struct OculusTouchInputParams : IInputParams
	{

		/// <summary>
		/// A pointer that will override the default pointer selected in VRCameraRig. Will only be applied if not null.
		/// </summary>
		public BasePointer OverridePointer;

		/// <summary>
		/// Should we show the hand meshes?
		/// </summary>
		public bool ShouldShowHands;

		/// <summary>
		/// Should we show the controller meshes?
		/// </summary>
		public bool ShouldShowControllers;

		/// <summary>
		/// Should we show the base meshes?
		/// </summary>
		public bool ShouldShowBase;

		public float AxisDeadzone;

		public float ScrollDelay;

		public float MaxScrollDelay;

		/// <inheritdoc />
		public void InitializeDefaultValues()
		{
			OverridePointer = null;
			ShouldShowHands = true;
			ShouldShowControllers = true;
			ShouldShowBase = false;
			AxisDeadzone = 0.25f;
			ScrollDelay = 1;
			MaxScrollDelay = 0.5f;
		}
	}

	/// <summary>Provides the input mappings to the input module</summary>
	public class OculusTouchController : MotionController
	{
		/// <summary>The enum value that will be passed into OVRInput functions to resolve Vusr input.</summary>
		public OVRInput.Controller ControllerType;

		/// <summary>OVRInput.IsControllerConnected</summary>
		public override bool IsAvailable
		{
			get { return OVRInput.IsControllerConnected(ControllerType); }
		}

		/// <summary>The <see cref="OVRInput.Controller"/> value used to get input from OVR</summary>
		public override object NativeObject
		{
			get { return ControllerType; }
		}

		/// <summary>OVRInput.Button.Any</summary>
		public override bool IsRequestingSwap()
		{
			return OVRInput.GetDown(OVRInput.Button.Any, ControllerType);
		}

		public override AxisTypes AxisType { get { return AxisTypes.Joystick; } }

		public override float AutoSwipeZone { get; set; }

		/// <summary>OVRInput.Axis2D.PrimaryThumbstick</summary>
		public override Vector2 GetSwipeAxis()
		{
			return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, ControllerType);
		}

		/// <summary><see cref="OVRInput.Touch.PrimaryThumbstick"/></summary>
		public override bool GetAxisTouch()
		{
			return OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, ControllerType);
		}

		/// <summary>OVRInput.Button.One | OVRInput.Button.PrimaryIndexTrigger</summary>
		public override bool GetClickButton()
		{
			return OVRInput.Get(OVRInput.Button.One, ControllerType) ||
			       OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, ControllerType);
		}

		/// <summary>OVRInput.Button.Two</summary>
		public override bool GetBackButton()
		{
			return OVRInput.Get(OVRInput.Button.Two, ControllerType);
		}
	}// End OculusTouchController class
}// End VusrCore.APIv1.InputSystems namespace
