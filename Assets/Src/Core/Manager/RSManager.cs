#pragma warning disable 0414
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using Rise.Features.Loader;

namespace Rise.Core {
	public enum Outputs {
		None,
		TwoD,
		Stereoscopic,
		Oculus,
		Cardboard
	};

	[System.Serializable]
	public class RSManager : RSPlugins {
		public bool standalone = false;

		public delegate void DeviceOrientationTypeHasChanged(UseDeviceOrientationType deviceOrientationType);
		public static event DeviceOrientationTypeHasChanged onDeviceOrientationTypeChange;
		private static UseDeviceOrientationType _deviceOrientationType = UseDeviceOrientationType.None;
		public static UseDeviceOrientationType DeviceOrientationType {
			set {
				_deviceOrientationType = value;

				if(onDeviceOrientationTypeChange != null) {
					onDeviceOrientationTypeChange(_deviceOrientationType);
				}
			}
			get {
				return _deviceOrientationType;
			}
		}

		private static RSManager _manager = null;
		public static new RSManager Manager {
			get { 
				return _manager ;
			}
			set {
				_manager = value;
			}
		}

		private RSInputManager _inputController;
		public override RSInputManager InputManager {
			get { 
				if (_inputController == null) {
					_inputController = new RSInputManager();
				}

				return _inputController ;
			} 
		}

		private RSOutputManager _outputsManager;
		public override RSOutputManager OutputsManager {
			get {
				if (_outputsManager == null) {
					_outputsManager = new RSOutputManager();
				}
				return _outputsManager ;
			} 
		}

		private RSCamerasManager _camerasManager;
		public override RSCamerasManager CamerasManager {
			get {
				if (_camerasManager == null) {
					_camerasManager = new RSCamerasManager();
				}
				return _camerasManager ;
			} 
		}

		void Awake() {
			_manager = this;

			base.Init();
		}
		
		void Start() {
			Debug.Log ("Starting scene manager");

			//Warning Start order is important !!!
			InputManager.Start ();

			CamerasManager.Start ();
			OutputsManager.Start ();
		}

		public override void OnDestroy () {
			base.OnDestroy();
		}

		void Update() {
			base.Update ();

			InputManager.Update();
		}
		
		void LateUpdate() {
			InputManager.LateUpdate();
		}

		public void SetOutput(Outputs o) {
			switch (o) {
				case Outputs.TwoD:
					OutputsManager.ActivateOutputMode ("2D");
				break;
				case Outputs.Stereoscopic:
					OutputsManager.ActivateOutputMode ("Splitted");
					break;
				case Outputs.Oculus:
					OutputsManager.ActivateOutputMode ("Oculus");
				break;
				case Outputs.Cardboard:
					OutputsManager.ActivateOutputMode ("3D Splitted");
				break;
			}
		}

		public void ToFullScreen() {
			Screen.SetResolution (Screen.width, Screen.height, true);
		}

		public void ToWindowed() {
			Screen.SetResolution (Screen.width, Screen.height, false);
		}

		public void Quit() {
			Application.Quit();
		}
			
		public void FreeMemory() {  
			System.GC.Collect();
			Resources.UnloadUnusedAssets();
		}
	}
}