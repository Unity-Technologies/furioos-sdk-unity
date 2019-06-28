using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FurioosSDK.Core {
	public class FSCamerasManager : FSManagerModule {
		private FSCamera activeCamera;
		
		public delegate void CameraChangedEventHandler(FSCamera camera);
		public event CameraChangedEventHandler CameraChanged;


		public List<FSCamera> GetAvailableCameras() {	
			return Manager.GetAllInstances<FSCamera>();
		}

		public FSCamera Active {
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
				FSCamera camera = Active;
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
			List<FSCamera> cameras = GetAvailableCameras();

			for(int i = 0; i < cameras.Count; i++) {
				FSCamera c = cameras[i];

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

		void OnCameraPreactivate (FSCamera camera) {
			foreach(FSCamera c in GetAvailableCameras()) {
				if(c != camera && c.IsActivated) {
					c.Desactivate();
				}
			}
		}
			
		void OnCameraActivated (FSCamera camera) {
			Active = camera;

			if(OutputsManager.Active != null) {
				OutputsManager.Active.UpdateCamera(camera);
			}

			if(CameraChanged != null) {
				CameraChanged(camera);
			}
		}

		void OnCameraDesactivated (FSCamera camera) {
			if (OutputsManager == null)
				return;

			if(Active == camera){
				if(OutputsManager.Active != null) {
					OutputsManager.Active.DetachFromCamera();
				}
				Active = null;
			}
		}

		public FSCamera ActivateCamera(string id) {
			FSCamera camera = Manager.GetInstance<FSCamera>(id);
			
			if(camera != null) {
				camera.Activate();
			}
			
			return camera;
		}
		
		public void DesactivateAllCameras() {
			List<FSCamera> cameras = Manager.GetAllInstances<FSCamera>();

			foreach(FSCamera c in cameras) {
				if(c.IsActivated) {
					c.Desactivate();
				}
			}
		}

	}
}