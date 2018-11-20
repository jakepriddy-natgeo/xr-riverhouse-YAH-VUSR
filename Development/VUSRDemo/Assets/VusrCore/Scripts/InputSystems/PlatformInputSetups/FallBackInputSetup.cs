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
	/// An InputSetup that has can take over when the other devices have failed. It provides Gaze-based
	/// selection and utilizes the mouse/spaceBar to trigger click events and It also allows backspace
	/// to send back events and the arrow keys to send swipe events. While in the editor, right
	/// clicking will allow you to move the camera. 
	/// </summary>
	public class FallBackInputSetup : BaseInputSetup<FallBackInputParams>
	{
		private GazeInputModule _gazeInputModule;
		private FallBackInputProvider _fip;

		/// <inheritdoc />
		public override VRDeviceMask SupportedDevices
		{
			get { return ~VRDeviceMask.None; }
		}

		/// <inheritdoc />
		public override bool IsSupported(VRDeviceMask currentDevice)
		{
			return true;
		}

		/// <inheritdoc />
		public override void Initialize(Transform playerRoot, VusrEventSystem eventSystem)
		{
			_gazeInputModule = eventSystem.gameObject.AddComponent<GazeInputModule>();
			_fip = new GameObject("FallBackInputProvider").AddComponent<FallBackInputProvider>();
			_fip.transform.parent = playerRoot.transform;
			_gazeInputModule.SetGazeProvider(_fip);
		}

		/// <inheritdoc />
		public override void ApplyParams(FallBackInputParams mParams)
		{

			if (mParams.OverridePointer != null)
				_gazeInputModule.Pointer = mParams.OverridePointer;

			_fip.ForceEnable = mParams.ForceEnable;
		}
	} // End FallBackInputSetup class


	/// <summary>
	/// Parameters for the FallbackInputModule
	/// </summary>
	[Serializable]
	public struct FallBackInputParams : IInputParams
	{

		/// <summary>
		/// A pointer that will override the default pointer selected in VRCameraRig. Will only be applied if not null.
		/// </summary>
		public BasePointer OverridePointer;

		/// <summary>
		/// If true, the fallback input will enable even even if other devices are available
		/// </summary>
		public bool ForceEnable;

		/// <inheritdoc />
		public void InitializeDefaultValues()
		{
			OverridePointer = null;
			ForceEnable = false;
		}
	}

	/// <summary>
	/// Provider class for communicating with the GazeInputModule.
	/// </summary>
	public class FallBackInputProvider : MonoBehaviour, IInputProvider, IInputProviderExtended
	{
		[SerializeField] private float _mouseSensitivity = 100.0f;
		[SerializeField] private float _clampAngle = 80.0f;
		
		/// <summary>
		/// If true, this provider will request to be enabled every single frame
		/// </summary>
		public bool ForceEnable;

		private float _rotY;
		private float _rotX;
		private bool _inActiveMode;

		#region IInputProvider Members

		/// <inheritdoc />
		public bool IsAvailable
		{
			get { return ForceEnable || Application.isEditor; }
		}

		/// <inheritdoc />
		public float AxisDeadZone
		{
			get { return 0; }
		}
		/// <summary> Returns null. The input is accessible through Input.Get()</summary>
		public object NativeObject
		{
			get { return null; }
		}

		public AxisTypes AxisType { get { return AxisTypes.DPad; } }

		public float AutoSwipeZone { get; set; }

		public float ScrollDelay { get; set; }

		public float MaxScrollDelay { get; set; }

		/// <summary>Arrow keys</summary>
		public Vector2 GetSwipeAxis()
		{
			Vector2 result = Vector2.zero;
			result += Input.GetAxis("Horizontal") * Vector2.right;
			result += Input.GetAxis("Vertical") * Vector2.up;
			return result.normalized;
		}

		/// <summary>KeyCode.RightControl</summary>
		public bool GetAxisTouch()
		{
			return Input.GetKey(KeyCode.RightControl);
		}

		/// <summary>KeyCode.Space</summary>
		public bool GetClickButton()
		{
			return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Mouse0);
		}

		/// <summary>KeyCode.Escape</summary>
		public bool GetBackButton()
		{
			return Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Backspace);
		}

		/// <summary>Usually false unless <see cref="ForceEnable"/> is true</summary>
		public bool IsRequestingSwap()
		{
			return ForceEnable;
		}

		#endregion


		private void Start()
		{
			Vector3 rot = VusrInput.PlayerRoot.localRotation.eulerAngles;
			_rotY = rot.y;
			_rotX = rot.x;
		}

		private void Update()
		{
			ForceEnable ^= Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.BackQuote);
			_inActiveMode ^= (ForceEnable || Application.isEditor) && Input.GetKeyDown(KeyCode.Mouse1);

			Cursor.lockState = _inActiveMode ? CursorLockMode.Locked : CursorLockMode.None;

			if (!_inActiveMode)
				return;
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = -Input.GetAxis("Mouse Y");

			_rotY += mouseX * _mouseSensitivity * Time.deltaTime;
			_rotX += mouseY * _mouseSensitivity * Time.deltaTime;

			_rotX = Mathf.Clamp(_rotX, -_clampAngle, _clampAngle);

			Quaternion localRotation = Quaternion.Euler(_rotX, _rotY, 0.0f);
			VusrInput.PlayerRoot.rotation = localRotation;
		}
	}// End FallBackInputProvider class
} // End VusrCore.APIv1.InputSystems namespace
