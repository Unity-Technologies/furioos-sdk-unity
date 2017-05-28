using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rise.Core {
	public class RSMovingModesManager : RSAppManagerModule {

		private RSMovingMode activeMovingMode;
		
		public delegate void MovingModeChangedEventHandler(RSMovingMode movingMode);
		public event MovingModeChangedEventHandler MovingModeChanged;


		public List<RSMovingMode> GetAvailableMovingModes()
		{	
			return RSManager.GetAllInstances<RSMovingMode>();
		}

		public RSMovingMode Active {
			get {
				return activeMovingMode;
			}
			protected set{
				if(activeMovingMode != (activeMovingMode=value) && MovingModeChanged!=null){
					MovingModeChanged(activeMovingMode);
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
				RSMovingMode amm = Active;
				return amm!=null ? amm.GetCameraGameObject() : null;
			}
		}
		
		public Camera ActiveCamera {
			get {
				GameObject cgo = ActiveCameraGameObject;
				return cgo!=null ? cgo.GetComponent<Camera>() : null;
			}
		}

		public void Start(){
			//Debug.Log("Starting moving modes manager");

			List<RSMovingMode> movingModes = GetAvailableMovingModes();

			for(int i=0;i<movingModes.Count;i++){
				RSMovingMode mm = movingModes[i];

				mm.OnPreactivate += OnMovingModePreactivate;
				mm.OnActivated += OnMovingModeActivated;

				if(mm.IsActivated)mm.Desactivate();
				if(i==0)mm.Activate();

				mm.OnDesactivated += OnMovingModeDesactivated;
			}
		}

		void OnMovingModePreactivate (RSMovingMode movingMode)
		{
			foreach(RSMovingMode mm in GetAvailableMovingModes()){
				if(mm!=movingMode && mm.IsActivated)mm.Desactivate();
			}
		}

		
		void OnMovingModeActivated (RSMovingMode movingMode)
		{
			Active = movingMode;
			if(OutputModesManager.Active!=null)OutputModesManager.Active.AttachToMovingMode(movingMode);
			if(MovingModeChanged!=null)MovingModeChanged(movingMode);
		}

		void OnMovingModeDesactivated (RSMovingMode movingMode)
		{
			if (OutputModesManager == null)
				return;

			if(Active==movingMode){
				if(OutputModesManager.Active!=null)OutputModesManager.Active.DetachFromMovingMode();
				Active = null;
			}
		}

		public RSMovingMode ActivateMovingMode(string id){
			RSMovingMode newMovingMode = RSManager.GetInstance<RSMovingMode>(id);
			
			if(newMovingMode!=null)newMovingMode.Activate();
			
			return newMovingMode;
		}
		
		public void DesactivateAllMovingModes(){
			List<RSMovingMode> movingModes = RSManager.GetAllInstances<RSMovingMode>();
			foreach(RSMovingMode mm in movingModes){
				if(mm.IsActivated)mm.Desactivate();
			}
		}

	}
}