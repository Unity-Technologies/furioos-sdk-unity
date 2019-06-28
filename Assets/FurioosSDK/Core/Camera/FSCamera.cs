using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FurioosSDK.Core {
	public class FSCamera : FSBehaviour {
		public delegate void OnActivatedHandler(FSCamera c);
		public delegate void OnDesactivatedHandler(FSCamera c);

		public event OnActivatedHandler OnPreactivate;
		public event OnActivatedHandler OnActivated;
		public event OnDesactivatedHandler OnDesactivated;

		public bool resetOnActivate = true;
		public bool showDebugInfo = false;
		
		public bool IsActivated {
			get;
			private set;
		}
		
		public float LastActivityTime {
			get;
			protected set;
		}
		
		public virtual void Awake() {
			base.Init();
		}

		public virtual void Activate() {
			if(IsActivated) {
				return;
			}
			else {
				if(OnPreactivate!=null) {
					OnPreactivate(this);
				}

				LastActivityTime = Time.time;

				IsActivated = true;
				enabled = true;

				GetCameraGameObject().GetComponent<Camera>().enabled = true;

				if(OnActivated!=null) {
					OnActivated(this);
				}
			}
		}
		
		public virtual void Desactivate() {
			if(!IsActivated) {
				//Debug.LogError("Moving mode " + ToString()+" is already desactivated, it shouldn't be desactivated a new time\r\n"+System.Environment.StackTrace);
			}
			else {
				IsActivated = false;
				enabled = false;
				
				GetCameraGameObject().GetComponent<Camera>().enabled = false;

				if(OnDesactivated!=null) {
					OnDesactivated(this);
				}
			}
		}

		void OnEnable() {
			if(!IsActivated) {
				Activate();
			}
		}
		
		void OnDisable() {
			if(IsActivated) {
				Desactivate();
			}
		}
		
		public virtual Vector3 GetPosition() {
			return transform.position;
		}
		
		public virtual GameObject GetCameraGameObject() {
			return transform.gameObject;
		}
		
		public virtual void PreActivate() {}
	}
}