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

namespace VusrCore.APIv1.InputSystems
{
	/// <summary>
	/// Input setup class connecting either the oculus remote or the GearVR touchpad to the GazeInputModule.
	/// OVR's joint Input allows us to use the same code for both platforms.
	/// </summary>
	public class OculusGazeSetup : BaseInputSetup<OculusGazeInputParams>
	{
		private GazeInputModule _oculusGazeInputModule;
		private OculusGazeInput _inputProvider;
		
		private Action _onFocusLost;
		private Action _onFocusGained;

		/// <inheritdoc />
		public override VRDeviceMask SupportedDevices
		{
			get { return VRDeviceMask.OculusAndroid | VRDeviceMask.OculusWindows; }
		}

		/// <inheritdoc />
		public override void Initialize(Transform playerRoot, VusrEventSystem eventSystem)
		{
			_oculusGazeInputModule = eventSystem.gameObject.AddComponent<GazeInputModule>();
			_inputProvider = new OculusGazeInput();

			if (VusrInput.CurrentVRDevice == VRDeviceMask.OculusAndroid)
				_inputProvider.ControlType = OVRInput.Controller.Touchpad;
			else if (VusrInput.CurrentVRDevice == VRDeviceMask.OculusWindows)
				_inputProvider.ControlType = OVRInput.Controller.Remote;

			_oculusGazeInputModule.SetGazeProvider(_inputProvider);

			_onFocusLost = () => { VusrInput.VrFocusChanged(true); };
			_onFocusGained = () => { VusrInput.VrFocusChanged(false); };

			OVRManager.VrFocusLost += _onFocusLost;
			OVRManager.VrFocusAcquired += _onFocusGained;
		}

		public override void ApplyParams(OculusGazeInputParams mParams)
		{
			_inputProvider.ScrollDelay = mParams.ScrollDelay;
			_inputProvider.MaxScrollDelay = mParams.MaxScrollDelay;
		}
		
		protected virtual void OnDestroy()
		{
			OVRManager.VrFocusLost -= _onFocusLost;
			OVRManager.VrFocusAcquired -= _onFocusGained;
		}
	}// End OculusGazeSetup class

	[Serializable]
	public struct OculusGazeInputParams : IInputParams
	{
		public float ScrollDelay;
		public float MaxScrollDelay;

		public void InitializeDefaultValues()
		{
			ScrollDelay = 1;
			MaxScrollDelay = 0.5f;
		}
	}

	/// <summary>
	/// InputProvider for Oculus gaze input;
	/// </summary>
	public class OculusGazeInput : IInputProvider, IInputProviderExtended
	{
		/// <summary>
		/// Data object used to interact with OVRInput.
		/// </summary>
		public OVRInput.Controller ControlType;


		/// <summary>OVRInput.IsControllerConnected</summary>
		public bool IsAvailable
		{
			get { return VusrInput.CurrentVRDevice == VRDeviceMask.OculusAndroid || OVRInput.IsControllerConnected(ControlType); }
		}

		/// <summary>OVRInput.Button.Two</summary>
		public float AxisDeadZone { get { return 0; } }

		/// <summary>The <see cref="OVRInput.Controller"/> value used to get input from OVR</summary>
		public object NativeObject { get { return ControlType; } }

		public AxisTypes AxisType { get { return AxisTypes.DPad; } }

		public virtual float AutoSwipeZone { get; set; }

		public float ScrollDelay { get; set; }

		public float MaxScrollDelay { get; set; }

		/// <summary>OVRInput.Button.DpadUp</summary>
		public Vector2 GetSwipeAxis()
		{
			Vector2 result = Vector2.zero;
			if (OVRInput.Get(OVRInput.Button.DpadUp, ControlType))
				result += Vector2.up;
			if (OVRInput.Get(OVRInput.Button.DpadDown, ControlType))
				result += Vector2.down;
			if (OVRInput.Get(OVRInput.Button.DpadRight, ControlType))
				result += Vector2.right;
			if (OVRInput.Get(OVRInput.Button.DpadLeft, ControlType))
				result += Vector2.left;
			return result;
		}

		/// <summary>OVRInput.Touch.PrimaryTouchpad</summary>
		public bool GetAxisTouch()
		{
			return OVRInput.Get(OVRInput.Touch.PrimaryTouchpad, ControlType);
		}

		/// <summary>OVRInput.Button.One</summary>
		public bool GetClickButton()
		{
			return OVRInput.Get(OVRInput.Button.One, ControlType) || 
				Input.GetMouseButton(0);
		}

		/// <summary>OVRInput.Button.Two</summary>
		public bool GetBackButton()
		{
			return OVRInput.Get(OVRInput.Button.Two, ControlType) ||
			       Input.GetKey(KeyCode.Escape);
		}

		/// <summary>OVRInput.GetActiveController</summary>
		public bool IsRequestingSwap()
		{
			return OVRInput.GetActiveController() == ControlType;
		}
	}// End OculusGazeInput class
}// End VusrCore.APIv1.InputSystems namespace
