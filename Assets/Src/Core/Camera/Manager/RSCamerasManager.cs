using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rise.Core {
	public class RSCamerasManager : RSManagerModule {

		private RSCamera activeMovingMode;
		
		public delegate void MovingModeChangedEventHandler(RSCamera movingMode);
		public event MovingModeChangedEventHandler CameraChanged;


		public List<RSCamera> GetAvailableCameras() {	
			return Manager.GetAllInstances<RSCamera>();
		}

		public RSCamera Active {
			get {
				return activeMovingMode;
			}
			protected set{
				if(activeMovingMode != (activeMovingMode=value) && CameraChanged!=null){
					CameraChanged(activeMovingMode);
				}
			}
		}
		
		public string ActiveId {
			get {
				return Active!=null ? Active.id : "";
			}
		}
		
		public GameObject ActiveCameraGameObject {
			get {
				RSCamera amm = Active;
				return amm!=null ? amm.GetCameraGameObject() : null;
			}
		}
		
		public Camera ActiveCamera {
			get {
				GameObject cgo = ActiveCameraGameObject;
				return cgo!=null ? cgo.GetComponent<Camera>() : null;
			}
		}

		public void Start() {
			List<RSCamera> movingModes = GetAvailableCameras();

			for(int i=0;i<movingModes.Count;i++) {
				RSCamera mm = movingModes[i];

				mm.OnPreactivate += OnCameraPreactivate;
				mm.OnActivated += OnCameraActivated;

				if(mm.IsActivated) {
					mm.Desactivate();
				}

				if(i==0) {
					mm.Activate();
				}

				mm.OnDesactivated += OnCameraDesactivated;
			}
		}

		void OnCameraPreactivate (RSCamera movingMode) {
			foreach(RSCamera mm in GetAvailableCameras()) {
				if(mm!=movingMode && mm.IsActivated) {
					mm.Desactivate();
				}
			}
		}
			
		void OnCameraActivated (RSCamera movingMode) {
			Active = movingMode;
			if(OutputModesManager.Active != null) {
				OutputModesManager.Active.AttachToMovingMode(movingMode);
			}

			if(CameraChanged != null) {
				CameraChanged(movingMode);
			}
		}

		void OnCameraDesactivated (RSCamera movingMode) {
			if (OutputModesManager == null)
				return;

			if(Active == movingMode){
				if(OutputModesManager.Active != null) {
					OutputModesManager.Active.DetachFromMovingMode();
				}
				Active = null;
			}
		}

		public RSCamera ActivateMovingMode(string id) {
			RSCamera newMovingMode = Manager.GetInstance<RSCamera>(id);
			
			if(newMovingMode != null) {
				newMovingMode.Activate();
			}
			
			return newMovingMode;
		}
		
		public void DesactivateAllMovingModes() {
			List<RSCamera> movingModes = Manager.GetAllInstances<RSCamera>();

			foreach(RSCamera mm in movingModes) {
				if(mm.IsActivated) {
					mm.Desactivate();
				}
			}
		}

	}
}