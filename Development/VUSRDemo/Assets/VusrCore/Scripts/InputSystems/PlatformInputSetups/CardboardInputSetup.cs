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

using UnityEngine;

namespace VusrCore.APIv1.InputSystems
{
	public class CardboardInputSetup : BaseInputSetup
	{
		private GazeInputModule _gim;

		public override VRDeviceMask SupportedDevices
		{
			get { return VRDeviceMask.CardboardAndroid | VRDeviceMask.CardboardIos; }
		}

		public override void Initialize(Transform playerRoot, VusrEventSystem eventSystem)
		{
			_gim = eventSystem.gameObject.AddComponent<GazeInputModule>();
			_gim.SetGazeProvider(new CardboardInputProvider());
		}
	}// End CardboardInputSetup class

	public class CardboardInputProvider : IInputProvider, IInputProviderExtended
	{
		public float AxisDeadZone { get { return 0; } }

		public object NativeObject { get { return null; } }

		public bool IsAvailable { get { return true; } }

		public AxisTypes AxisType{ get { return AxisTypes.None; } }

		public float AutoSwipeZone { get; set; }

		public float ScrollDelay { get; set; }

		public float MaxScrollDelay { get; set; }

		public Vector2 GetSwipeAxis() { return Vector2.zero; }

		public bool GetAxisTouch() { return false; }

		public bool GetClickButton() { return Input.GetMouseButton(0); }

		public bool GetBackButton() { return false; }

		public bool IsRequestingSwap() { return Input.GetMouseButton(0); }
	}// End CardboardInputProvider class
}// End VusrCore.APIv1.InputSystems namespace
