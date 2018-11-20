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
using System.Collections.Generic;
using UnityEngine;

namespace VusrCore.APIv1.InputSystems
{
	/// <summary>
	/// Sets up an XboxController in one of two modes: Gamepad or Gaze. The mappings are very similar:
	/// In Gamepad mode, the left stick controls navigation, and the buttons provide actions.
	/// The right stick is used to emulate swipe.
	/// 
	/// </summary>
	[DisallowMultipleComponent]
	public class XboxControllerSetup : BaseInputSetup<XboxControllerInputParams>
	{
		private GamePadInputModule _gamePadInputModule;
		private GazeInputModule _gazeInputModule;
		private XboxInputProvider _gazeProvider;
		private XboxInputProvider _padProvider;

		/// <inheritdoc />
		public override VRDeviceMask SupportedDevices
		{
			get { return VRDeviceMask.SteamVR | VRDeviceMask.OculusWindows; }
		}

		/// <inheritdoc />
		public override void Initialize(Transform playerRoot, VusrEventSystem eventSystem)
		{
			_gazeProvider = new XboxInputProvider();
			_gazeInputModule = eventSystem.gameObject.AddComponent<GazeInputModule>();
			_gazeInputModule.SetGazeProvider(_gazeProvider);

			_padProvider = new XboxInputProvider();
			_gamePadInputModule = eventSystem.gameObject.AddComponent<GamePadInputModule>();
			_gamePadInputModule.SetGamePadProvider(_padProvider);
		}

		/// <inheritdoc />
		public override void ApplyParams(XboxControllerInputParams mParams)
		{

			if (mParams.OverridePointer != null)
				_gazeInputModule.Pointer = mParams.OverridePointer;

			_gazeProvider.IsEnabled = mParams.ShouldUseGazeInput;
			_padProvider.IsEnabled = !mParams.ShouldUseGazeInput;
			_gazeProvider.AxisDeadZone = mParams.AxisDeadzone;
			_padProvider.AxisDeadZone = mParams.AxisDeadzone;
			_gazeProvider.ScrollDelay = mParams.ScrollDelay;
			_gazeProvider.MaxScrollDelay = mParams.MaxScrollDelay;
			_padProvider.ScrollDelay = mParams.ScrollDelay;
			_padProvider.MaxScrollDelay = mParams.MaxScrollDelay;
		}
	} // End XboxControllerSetup class

	/// <summary>
	/// Xbox Controller configuration.
	/// </summary>
	[Serializable]
	public struct XboxControllerInputParams : IInputParams
	{

		/// <summary>
		/// A pointer that will override the default pointer selected in VRCameraRig. Will only be applied if not null.
		/// </summary>
		public BasePointer OverridePointer;

		/// <summary>
		/// If true, the user will select things with their gaze, if false, they will use the left stick.
		/// </summary>
		public bool ShouldUseGazeInput;

		/// <summary>
		/// The magnitude threshold after which an axis will translate into a swipe command.
		/// </summary>
		public float AxisDeadzone;

		public float ScrollDelay;

		public float MaxScrollDelay;

		/// <inheritdoc />
		public void InitializeDefaultValues()
		{
			OverridePointer = null;
			ShouldUseGazeInput = true;
			AxisDeadzone = 0.25f;
			ScrollDelay = 1;
			MaxScrollDelay = 0.5f;
		}
	}

	/// <summary>
	/// Allows UI navigation using Unity's native Xbox controller input. Can be placed along with other InputModules
	/// by using InputModuleSwapper.
	/// </summary>
	public class XboxInputProvider : IGamePadProvider, IInputProviderExtended
	{
		private Dictionary<Axis, string> AxisMap = new Dictionary<Axis, string>()
		{
			{Axis.LX, "Horizontal"},
			{Axis.LY, "Vertical"},
			{Axis.RX, "RStick_Horizontal"},
			{Axis.RY, "RStick_Vertical"},
			{Axis.DPadX, "DPad_Horizontal"},
			{Axis.DPadY, "DPad_Horizontal"},
			{Axis.L2, "Left_Trigger"},
			{Axis.R2, "Right_Trigger"}
		};

		/// <summary>
		/// Used for switching between both XboxController input modules.
		/// </summary>
		public bool IsEnabled;

		//Differentiate input by index.
		private int _xboxNumber = -1;
		//Prevent Doing the same check twice per frame
		private int _swapFrameNumber;

		#region IGamePadProvider Members

		/// <inheritdoc />
		public bool IsAvailable
		{
			get
			{
				if (!IsEnabled)
					return false;
				CheckForController();
				return _xboxNumber != -1;
			}
		}

		/// <inheritdoc />
		public Vector2 GetPrimaryAxis()
		{
			Vector2 move = Vector2.zero;
			move.x = AxisRaw(Axis.LX);
			move.y = AxisRaw(Axis.LY);
			move.x = AbsMax(move.x, AxisRaw(Axis.DPadX));
			move.y = AbsMax(move.y, AxisRaw(Axis.DPadY));
			return move;
		}

		/// <inheritdoc />
		public float AxisDeadZone { get; set; }


		/// <summary> returns null, you can check input through <see cref="Input"/></summary>
		public object NativeObject
		{
			get { return null; }
		}

		public AxisTypes AxisType { get { return AxisTypes.Joystick; } }

		public float AutoSwipeZone { get; set; }

		public float ScrollDelay { get; set; }

		public float MaxScrollDelay { get; set; }

		/// <summary> Right stick as well as triggers.</summary>
		public Vector2 GetSwipeAxis()
		{
			Vector2 move = Vector2.zero;

			move.x = AbsMax(move.x, AxisRaw(Axis.RX));
			move.y = AxisRaw(Axis.RY);

			float shoulderScroll = AxisRaw(Axis.R2) - AxisRaw(Axis.L2);
			move.x = AbsMax(move.x, shoulderScroll);

			return move;
		}

		/// <summary>Unsupported</summary>
		public bool GetAxisTouch()
		{
			return false;
		}

		/// <summary>Unsupported</summary>
		public bool GetClickButton()
		{
			return Button(Buttons.A);
		}

		/// <summary>Unsupported</summary>
		public bool GetBackButton()
		{
			return Button(Buttons.B);
		}

		/// <summary>Unsupported</summary>
		public bool IsRequestingSwap()
		{
			return IsAnyButtonDown();
		}

		#endregion

		/// <summary>
		/// Because unity maps all sort of devices to the same axes, we have to find the index of the xbox controller..
		/// This method does so and saves it locally.
		/// </summary>
		private bool CheckForController()
		{
			if (_swapFrameNumber == Time.frameCount)
				return _xboxNumber != -1;

			_swapFrameNumber = Time.frameCount;
			string[] joystickNames = Input.GetJoystickNames();
			for (int i = 0; i < joystickNames.Length; i++)
				if (joystickNames[i].ToLowerInvariant().Contains("xbox"))
				{
					_xboxNumber = i;
					return true;
				}
			_xboxNumber = -1;
			return false;
		}

		private bool ButtonDown(Buttons butt)
		{
			if (!CheckForController())
				return false;

			return Input.GetKeyDown("joystick " + (_xboxNumber + 1) + " button " + (int) butt);
		}

		private bool Button(Buttons butt)
		{
			if (!CheckForController())
				return false;

			return Input.GetKeyDown("joystick " + (_xboxNumber + 1) + " button " + (int) butt);
		}

		private bool IsAnyButtonDown()
		{
			if (!CheckForController())
				return false;
			foreach (Buttons b in Enum.GetValues(typeof(Buttons)))
			{
				if (ButtonDown(b))
					return true;
			}
			return false;
		}

		private float AxisRaw(Axis axis)
		{
			return !CheckForController() ? 0 : Input.GetAxis(AxisMap[axis]);
		}

		/// <summary>Returns the max of two floats' absolute values.</summary>
		public static float AbsMax(float a, float b)
		{
			return Mathf.Abs(a) > Mathf.Abs(b) ? a : b;
		}

		#region Nested type: Axis

		private enum Axis
		{
			LX,
			LY,
			RX,
			RY,
			DPadX,
			DPadY,
			L2,
			R2,
		}

		#endregion

		#region Nested type: Buttons

		private enum Buttons
		{
			A = 0,
			B = 1,
			X = 2,
			Y = 3,
			L1 = 4,
			R1 = 5,
			Back = 6,
			Menu = 7,
			L3 = 8,
			R3 = 9,
		}

		#endregion
	} // End XboxInputProvider class
} // End VusrCore.APIv1.InputSystems namespace
