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
using UnityEngine.Events;

namespace VusrCore.APIv1.InputSystems
{
	/// <summary>
	/// Sets up input for the GearVR and Oculus Go. On initialization adds a LaserInputModule to the eventSystem and provides 
	/// input state through a GearVRController (which is both the Gear VR Controller and Oculus Go Controller). 
	/// </summary>
	[DisallowMultipleComponent]
	public class GearVRSetup : BaseInputSetup<GearVRInputParams>
	{

		[Serializable]
		public class FloatEvent : UnityEvent<float> { }

		[SerializeField] private OVRCameraRig _ovrCameraRigPrefab;
		[SerializeField] private OVRTrackedRemote _trackedRemotePrefab;

		private MotionControllerInputModule _laserInputModule;
		private FloatEvent _onBack;
		private float? _backDownTime;

		/// <inheritdoc />
		public override VRDeviceMask SupportedDevices
		{
			get { return VRDeviceMask.OculusAndroid; }
		}

		/// <inheritdoc />
		public override void Initialize(Transform playerRoot, VusrEventSystem eventSystem)
		{
			OVRCameraRig cameraRig = Instantiate(_ovrCameraRigPrefab, playerRoot);

			_laserInputModule = eventSystem.gameObject.AddComponent<MotionControllerInputModule>();

			_laserInputModule.LeftController = cameraRig.leftHandAnchor.gameObject.AddComponent<GearVRController>();
			(_laserInputModule.LeftController as GearVRController).ControllerType = Application.isEditor ? OVRInput.Controller.LTouch : OVRInput.Controller.LTrackedRemote;
			OVRTrackedRemote lModel = Instantiate(_trackedRemotePrefab, _laserInputModule.LeftController.transform).GetComponent<OVRTrackedRemote>();
			lModel.m_controller = (_laserInputModule.LeftController as GearVRController).ControllerType;
			_laserInputModule.LeftController.ControllerModel = lModel.gameObject;

			_laserInputModule.RightController = cameraRig.rightHandAnchor.gameObject.AddComponent<GearVRController>();
			(_laserInputModule.RightController as GearVRController).ControllerType = Application.isEditor ? OVRInput.Controller.RTouch : OVRInput.Controller.RTrackedRemote;
			OVRTrackedRemote rModel = Instantiate(_trackedRemotePrefab, _laserInputModule.RightController.transform).GetComponent<OVRTrackedRemote>();
			rModel.m_controller = (_laserInputModule.RightController as GearVRController).ControllerType;
			_laserInputModule.RightController.ControllerModel = rModel.gameObject;

			StartOVRSettings();
		}

		private void Update()
		{
			if (_onBack == null)
				return;
			if (VusrInput.BackDown && !_backDownTime.HasValue)
				_backDownTime = Time.time;
			else if (VusrInput.BackUp && _backDownTime.HasValue)
			{
				_onBack.Invoke(_backDownTime.Value);
				_backDownTime = null;
			}
		}

		/// <summary>Function to set the performance mode on Gear VR</summary>
		/// <param name="newMode">The mode to change to.</param>
		protected virtual void SetGearMode(GearVRInputParams.PerformanceModes newMode)
		{
			int adjustedCPU = 2;
			int adjustGPU = 2;
			int adjustedVsync = 1;

			switch (newMode)
			{
				case GearVRInputParams.PerformanceModes.Standard:
					OVRManager.instance.enableAdaptiveResolution = false;
					break;
				case GearVRInputParams.PerformanceModes.Low:
					adjustGPU = 0;
					adjustedCPU = 0;
					adjustedVsync = 0;
					OVRManager.instance.enableAdaptiveResolution = true;
					break;
				case GearVRInputParams.PerformanceModes.High:
					OVRManager.instance.enableAdaptiveResolution = false;
					adjustedCPU = 3;
					adjustGPU = 3;
					break;
			}
			OVRManager.instance.vsyncCount = adjustedVsync;

			OVRManager.cpuLevel = adjustedCPU;
			OVRManager.gpuLevel = adjustGPU;
		}

		/// <summary>Gear VR specific OVR initialization function</summary>
		protected virtual void StartOVRSettings()
		{
			Application.backgroundLoadingPriority = ThreadPriority.Low;
			OVRManager.instance.useRecommendedMSAALevel = true;
			if (QualitySettings.antiAliasing != 0 && QualitySettings.antiAliasing != 1)
				QualitySettings.antiAliasing = 1;
		}

		/// <summary>Gear VR specific battery temperature reading</summary>
		/// <returns>OVR battery temperature</returns>
		public static float GetCurrentBatteryTemp()
		{
			return OVRManager.batteryTemperature;
		}

		/// <inheritdoc />
		public override void ApplyParams(GearVRInputParams mParams)
		{
			if (mParams.OverridePointer != null)
				_laserInputModule.Pointer = mParams.OverridePointer;

			//set left controller params
			_laserInputModule.LeftController.AxisDeadZone = mParams.AxisDeadZone;
			_laserInputModule.LeftController.AutoSwipeZone = mParams.SwipeAutozone;

			//set right controller params
			_laserInputModule.RightController.AxisDeadZone = mParams.AxisDeadZone;
			_laserInputModule.RightController.AutoSwipeZone = mParams.SwipeAutozone;

			SetGearMode(mParams.PerformanceMode);
			_onBack = mParams.OnBackNavigation;

			// Todo: re-implement
			//HeadTrackedCursor.transform.localScale = new Vector3(gvrParams.GazeReticleScale, gvrParams.GazeReticleScale, 1f);
			//LTrackedCursor.transform.localScale = new Vector3(gvrParams.ControllerReticleScale, gvrParams.ControllerReticleScale, 1f);
			//RTrackedCursor.transform.localScale = new Vector3(gvrParams.ControllerReticleScale, gvrParams.ControllerReticleScale, 1f);
		}
	}// End GearVRSetup class


	/// <summary>
	/// Configurable parameters for the GearVR controller.
	/// </summary>
	[Serializable]
	public struct GearVRInputParams : IInputParams
	{
		/// <summary>
		/// A pointer that will override the default pointer selected in VRCameraRig. Will only be applied if not null.
		/// </summary>
		public BasePointer OverridePointer;

		/// <summary>Size of the Gaze reticle.</summary>
		public float GazeReticleScale;

		public float AxisDeadZone;
		public float SwipeAutozone;

		/// <summary>
		/// Called by the Input system (only on Oculus on Android) when the user releases the back button. 
		/// The first parameter is the amount of time the button was held. Oculus recommends only performing 
		/// actions on presses shorter than 0.25. After being held for .75 seconds, the user will be taken
		/// back to Oculus Home.
		/// </summary>
		public GearVRSetup.FloatEvent OnBackNavigation;
		
		/// <summary>Size of the Controller Reticle </summary>
		public float ControllerReticleScale;

		/// <summary>
		/// Oculus Android specific cpu/gpu throttling adjustment, lower performance levels save more power.
		/// On Oculus Go, this functions as a minimum performance level, and the platform scales up if it can.
		/// </summary>
		public PerformanceModes PerformanceMode;

		/// <inheritdoc />
		public void InitializeDefaultValues()
		{
			OverridePointer = null;
			GazeReticleScale = 1.0f;
			AxisDeadZone = 0.25f;
			SwipeAutozone = 0.75f;
			ControllerReticleScale = 1.0f;
			PerformanceMode = PerformanceModes.Standard;
		}

		/// <summary>
		/// Performance presets for Oculus on Android
		/// </summary>
		public enum PerformanceModes
		{
			Low,
			Standard,
			High
		}
	}

	/// <summary> Provider class used to interact with the LaserInputModule. </summary>
	public class GearVRController : MotionController
	{
		/// <summary>The enum value that will be passed into OVRInput functions to resolve Vusr input.</summary>
		public OVRInput.Controller ControllerType;

		/// <summary>OVRInput.IsControllerConnected</summary>
		public override bool IsAvailable
		{
			get { return OVRInput.IsControllerConnected(ControllerType); }
		}

		/// <summary>Returns the <see cref="OVRInput.Controller"/> that is associated with this object.</summary>
		public override object NativeObject
		{
			get { return ControllerType; }
		}

		/// <summary>OVRInput.GetActiveController</summary>
		public override bool IsRequestingSwap()
		{
			return OVRInput.GetActiveController() == ControllerType;
		}

		public override AxisTypes AxisType { get { return AxisTypes.TouchPad; } }

		/// <summary>IInputProviderExtended.SwipeAutoZone</summary>
		public override float AutoSwipeZone { get; set; }

		/// <summary>
		/// OVRInput.Axis2D.PrimaryTouchpad
		/// Reversed to match Oculus Gear VR swipe gesture requirements
		/// </summary>
		public override Vector2 GetSwipeAxis()
		{
			// Left/Right and Up/Down have to be reversed according to Oculus specs here (under Gear VR Controller Swiping Gestures)
			// https://developer.oculus.com/documentation/unity/latest/concepts/unity-ovrinput/
			Vector2 rawAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, ControllerType);
			rawAxis.x *= -1;
			rawAxis.y *= -1;

			return rawAxis;
		}

		/// <summary>OVRInput.Touch.PrimaryTouchpad</summary>
		public override bool GetAxisTouch()
		{
			return OVRInput.Get(OVRInput.Touch.PrimaryTouchpad, ControllerType);
		}

		/// <summary>OVRInput.Button.One</summary>
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

		public override void UpdateVisibility()
		{
			bool shouldRender = ShouldRenderModel && VusrInput.IsControllerVisible && VusrInput.CurrentInputModule == Module;
			if (ControllerModel != null)
				ControllerModel.transform.localScale = shouldRender ? Vector3.one : Vector3.zero;
		}
	} // End GearVRController class
}// End VusrCore.APIv1.InputSystems namespace
