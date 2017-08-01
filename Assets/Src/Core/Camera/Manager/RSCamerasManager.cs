using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rise.Core {
	public class RSCamerasManager : RSManagerModule {
		private RSCamera activeCamera;
		
		public delegate void CameraChangedEventHandler(RSCamera camera);
		public event CameraChangedEventHandler CameraChanged;


		public List<RSCamera> GetAvailableCameras() {	
			return Manager.GetAllInstances<RSCamera>();
		}

		public RSCamera Active {
			get {
				return activeCamera;
			}
			protected set{
				if(activeCamera != (activeCamera = value) && CameraChanged != null){
					CameraChanged(activeCamera);
				}
			}
		}
		
		public string ActiveId {
			get {
				return Active != null ? Active.id : "";
			}
		}
		
		public GameObject ActiveCameraGameObject {
			get {
				RSCamera camera = Active;
				return camera != null ? camera.GetCameraGameObject() : null;
			}
		}
		
		public Camera ActiveCamera {
			get {
				GameObject cgo = ActiveCameraGameObject;
				return cgo != null ? cgo.GetComponent<Camera>() : null;
			}
		}

		public void Start() {
			List<RSCamera> cameras = GetAvailableCameras();

			for(int i = 0; i < cameras.Count; i++) {
				RSCamera c = cameras[i];

				c.OnPreactivate += OnCameraPreactivate;
				c.OnActivated += OnCameraActivated;

				if(c.IsActivated) {
					c.Desactivate();
				}

				if(i == 0) {
					c.Activate();
				}

				c.OnDesactivated += OnCameraDesactivated;
			}
		}

		void OnCameraPreactivate (RSCamera camera) {
			foreach(RSCamera c in GetAvailableCameras()) {
				if(c != camera && c.IsActivated) {
					c.Desactivate();
				}
			}
		}
			
		void OnCameraActivated (RSCamera camera) {
			Active = camera;

			if(OutputsManager.Active != null) {
				OutputsManager.Active.UpdateCamera(camera);
			}

			if(CameraChanged != null) {
				CameraChanged(camera);
			}
		}

		void OnCameraDesactivated (RSCamera camera) {
			if (OutputsManager == null)
				return;

			if(Active == camera){
				if(OutputsManager.Active != null) {
					OutputsManager.Active.DetachFromCamera();
				}
				Active = null;
			}
		}

		public RSCamera ActivateCamera(string id) {
			RSCamera camera = Manager.GetInstance<RSCamera>(id);
			
			if(camera != null) {
				camera.Activate();
			}
			
			return camera;
		}
		
		public void DesactivateAllCameras() {
			List<RSCamera> cameras = Manager.GetAllInstances<RSCamera>();

			foreach(RSCamera c in cameras) {
				if(c.IsActivated) {
					c.Desactivate();
				}
			}
		}

	}
}