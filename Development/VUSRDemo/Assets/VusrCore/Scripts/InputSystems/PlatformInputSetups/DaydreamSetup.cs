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
using System.Linq;
using UnityEngine;

namespace VusrCore.APIv1.InputSystems
{
	/// <summary>
	/// A BaseSetup Class for setting up Daydream input.
	/// </summary>
	[DisallowMultipleComponent]
	public class DaydreamSetup : BaseInputSetup<DaydreamParams>
	{
		[SerializeField] private GvrControllerInput _gvrControllerMainPrefab;
		[SerializeField] private GvrTrackedController _gvrControllerPointerPrefab;
		[SerializeField] private GameObject _gvrControllerTooltipsTemplate;
		[SerializeField] private GvrHeadset _gvrHeadsetPrefab;

		private GameObject _toolTipsObj;

		private GvrTooltip _insideSwipeTooltip;
		private GvrTooltip _outsideSwipeTooltip;
		private GvrTooltip _insideAppTooltip;
		private GvrTooltip _outsideAppTooltip;

		private MotionControllerInputModule _laserInputModule;
		private DaydreamController _controller;
		private GameObject _controllerRoot;

		/// <inheritdoc />
		public override VRDeviceMask SupportedDevices
		{
			get { return VRDeviceMask.Daydream; }
		}
        		
        protected virtual void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                HandleDaydreamXButton();
        }

        protected virtual void HandleDaydreamXButton()
        {
            Application.Quit();
        }

        /// <inheritdoc />
        public override void Initialize(Transform playerRoot, VusrEventSystem eventSystem)
		{
			//Create headset object
			Instantiate(_gvrHeadsetPrefab, playerRoot);

			//Create controller object
			Instantiate(_gvrControllerMainPrefab, playerRoot);
			_controllerRoot = Instantiate(_gvrControllerPointerPrefab, playerRoot).gameObject;
			Destroy(_controllerRoot.GetComponentInChildren<GvrLaserPointer>().gameObject);

			GameObject pointer = new GameObject("ControllerTracker");
			pointer.transform.parent = _controllerRoot.transform;
			pointer.AddComponent<GvrTrackedController>();
			_controller = pointer.gameObject.AddComponent<DaydreamController>();
			_controller.ControllerModel = _controllerRoot.GetComponentInChildren<GvrControllerVisual>().gameObject;
			
			//Tooltips creation
			_toolTipsObj = Instantiate(_gvrControllerTooltipsTemplate, _controller.ControllerModel.transform);
			GvrTooltip[] tooltips = _toolTipsObj.GetComponentsInChildren<GvrTooltip>();
			_insideSwipeTooltip = tooltips.FirstOrDefault(t => t.name == "TouchPadInside");
			_outsideSwipeTooltip = tooltips.FirstOrDefault(t => t.name == "TouchPadOutside");
			_insideAppTooltip = tooltips.FirstOrDefault(t => t.name == "AppButtonInside");
			_outsideAppTooltip = tooltips.FirstOrDefault(t => t.name == "AppButtonOutside");

			_laserInputModule = eventSystem.gameObject.AddComponent<MotionControllerInputModule>();

			if (DaydreamController.IsLeftHanded())
				_laserInputModule.LeftController = _controller;
			else
				_laserInputModule.RightController = _controller;
		}


		/// <summary>
		/// Callback sent to all game objects when the player pauses.
		/// </summary>
		/// <param name="pauseStatus">The pause state of the application.</param>
		protected virtual void OnApplicationPause(bool pauseStatus)
		{
			VusrInput.VrFocusChanged(pauseStatus);
		}

		/// <inheritdoc />
		public override void ApplyParams(DaydreamParams mParams)
		{
			if (mParams.OverridePointer != null)
				_laserInputModule.Pointer = mParams.OverridePointer;

			_controller.AxisDeadZone = mParams.AxisDeadZone;
			_controller.AutoSwipeZone = mParams.AutoSwipeZone;

			_insideSwipeTooltip.gameObject.SetActive(mParams.ShouldShowInsideSwipeTooltip);
			_outsideSwipeTooltip.gameObject.SetActive(mParams.ShouldShowOutsideSwipeTooltip);
			_insideAppTooltip.gameObject.SetActive(mParams.ShouldShowInsideAppTooltip);
			_outsideAppTooltip.gameObject.SetActive(mParams.ShouldShowOutsideAppTooltip);

			_insideSwipeTooltip.TooltipText.text = mParams.InsideSwipeTooltipText;
			_outsideSwipeTooltip.TooltipText.text = mParams.OutsideSwipeTooltipText;
			_insideAppTooltip.TooltipText.text = mParams.InsideAppTooltipText;
			_outsideAppTooltip.TooltipText.text = mParams.OutsideAppTooltipText;
		}
	}// End DaydreamSetup class

	/// <summary>
	/// Daydream parameters. Currently none are supported.
	/// </summary>
	[Serializable]
	public struct DaydreamParams : IInputParams
	{

		/// <summary>
		/// A pointer that will override the default pointer selected in VRCameraRig. Will only be applied if not null.
		/// </summary>
		public BasePointer OverridePointer;

		public float AxisDeadZone;
		public float AutoSwipeZone;

		public string InsideSwipeTooltipText;
		public string OutsideSwipeTooltipText;
		public string InsideAppTooltipText;
		public string OutsideAppTooltipText;

		public bool ShouldShowInsideSwipeTooltip;
		public bool ShouldShowOutsideSwipeTooltip;
		public bool ShouldShowInsideAppTooltip;
		public bool ShouldShowOutsideAppTooltip;
		/// <inheritdoc />
		public void InitializeDefaultValues()
		{
			OverridePointer = null;
			AxisDeadZone = 0.25f;
			AutoSwipeZone = 0.8f;
			ShouldShowInsideSwipeTooltip = false;
			ShouldShowOutsideSwipeTooltip = false;
			ShouldShowInsideAppTooltip = false;
			ShouldShowOutsideAppTooltip = false;
		}
	}

	/// <summary>
	/// Controller Script to communicate with <see cref="MotionControllerInputModule"/>
	/// </summary>
	public class DaydreamController : MotionController
	{
		/// <inheritdoc />
		/// <remarks>Will only switch if the controller is connected</remarks>
		public override bool IsAvailable
		{
			get { return GvrControllerInput.State == GvrConnectionState.Connected; }
		}

		/// <inheritdoc />
		/// <remarks>Use GvrController if you want to access native daydream input</remarks>
		public override object NativeObject
		{
			get { return null; }
		}

		public override AxisTypes AxisType { get { return AxisTypes.TouchPad; } }

		/// <inheritdoc />
		/// <remarks>Touching or clicking any of the buttons activates the controller</remarks>
		public override bool IsRequestingSwap()
		{
			return GvrControllerInput.ClickButtonDown || GvrControllerInput.AppButtonDown || GvrControllerInput.IsTouching;
		}

		/// <summary>GvrControllerInput.TouchPos</summary>
		public override Vector2 GetSwipeAxis()
		{
			// X and y need to be inverted to match the scrolling that happens in Daydream Home.
			// That's as close to requirements documentation as we can find.
			Vector2 touchPos = GvrControllerInput.TouchPos;
			touchPos.x *= -1;
			touchPos.y *= -1;

			return touchPos;
		}

		/// <summary>IInputProviderExtended.SwipeAutoZone</summary>
		public override float AutoSwipeZone { get; set; }

		/// <summary>GvrControllerInput.IsTouching</summary>
		public override bool GetAxisTouch()
		{
			return GvrControllerInput.IsTouching;
		}

		/// <summary>GvrControllerInput.ClickButton</summary>
		public override bool GetClickButton()
		{
			return GvrControllerInput.ClickButton;
		}

		/// <summary>GvrControllerInput.AppButton</summary>
		public override bool GetBackButton()
		{
			return GvrControllerInput.AppButton;
		}

		/// <summary>GvrSettings.Handedness</summary>
		public static bool IsLeftHanded()
		{
			return GvrSettings.Handedness == GvrSettings.UserPrefsHandedness.Left;
		}

		public override void UpdateVisibility()
		{
			bool shouldRender = ShouldRenderModel && VusrInput.IsControllerVisible && VusrInput.CurrentInputModule == Module;
			if (ControllerModel != null)
				ControllerModel.transform.localScale = shouldRender ? Vector3.one : Vector3.zero;
		}
	}// End DaydreamController class
}// End VusrCore.APIv1.InputSystems namespace
