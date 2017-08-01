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
		public string baseUrl = "";

		public delegate void OutputHasChanged(Outputs mode);
		public static event OutputHasChanged onOutputChange;
		private static Outputs ouput = Outputs.None;
		public static Outputs Output {
			set {
				ouput = value;

				if(onOutputChange != null) {
					onOutputChange(ouput);
				}
			}
			get {
				return ouput;
			}
		}

		public delegate void DeviceOrientationTypeHasChanged(UseDeviceOrientationType deviceOrientationType);
		public static event DeviceOrientationTypeHasChanged onDeviceOrientationTypeChange;
		private static UseDeviceOrientationType deviceOrientationType = UseDeviceOrientationType.None;
		public static UseDeviceOrientationType DeviceOrientationType {
			set {
				deviceOrientationType = value;

				if(onDeviceOrientationTypeChange != null) {
					onDeviceOrientationTypeChange(deviceOrientationType);
				}
			}
			get {
				return deviceOrientationType;
			}
		}

		private static RSManager manager = null;
		public static RSManager Manager {
			get { 
				return manager ;
			} 
		}

		private RSInputManager inputController;
		public override RSInputManager InputManager {
			get { 
				if (inputController == null) {
					inputController = new RSInputManager();
				}

				return inputController ;
			} 
		}

		private RSOutputManager outputsManager;
		public override RSOutputManager OutputsManager {
			get {
				if (outputsManager == null) {
					outputsManager = new RSOutputManager();
				}
				return outputsManager ;
			} 
		}

		private RSCamerasManager camerasManager;
		public override RSCamerasManager CamerasManager {
			get {
				if (camerasManager == null) {
					camerasManager = new RSCamerasManager();
				}
				return camerasManager ;
			} 
		}

		void Awake() {
			manager = this;

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
			InputManager.Update();
		}
		
		void LateUpdate() {
			InputManager.LateUpdate();
		}

		void OnPostRender() {
			if (OutputsManager.Active == null) {
				return;
			}

			OutputsManager.RenderImage(
				GetComponent<Camera>().targetTexture, 
				GetComponent<Camera>().targetTexture
			);
		}

		void OnGUI() {
			if(OutputsManager != null) {
				OutputsManager.RenderGui(null);
			}
		}

		public void SetOutput(Outputs o) {
			Output = o;
			switch (o) {
				case Outputs.TwoD:
					OutputsManager.ActivateOutputMode ("2D");
				break;
				case Outputs.Stereoscopic:
					Screen.SetResolution(1920, 1080, true);
					Manager.GetInstance<RSOutput3DSplitted>().splitMode = RSOutput3DSplitted.SplitMode.SideBySide;
					break;
				case Outputs.Oculus:
					OutputsManager.ActivateOutputMode ("3D Oculus");
				break;
				case Outputs.Cardboard:
					Manager.GetInstance<RSOutput3DSplitted>().splitMode = RSOutput3DSplitted.SplitMode.SideBySide;
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