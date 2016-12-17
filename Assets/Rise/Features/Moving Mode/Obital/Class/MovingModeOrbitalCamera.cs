using UnityEngine;
using System.Collections;
using System;
using Rise.Core;

namespace Rise.Features.MovingMode {
	[RequireComponent (typeof(Camera))]
	public class MovingModeOrbitalCamera : RSPanTiltMovingMode {
		public Transform target;
		
		
		public bool canZoom = true;
		public float minDistance = 30;
		public float startDistance = 100;
		public float maxDistance = 200;

		public float distanceCorrectionLimit = 5.0f;
		public bool distanceCorrection = true;

		private float distance = -1;

		private bool distanceModified;
		
		private Vector2 displacementSpeed;

		private bool alreadySaved = false;
		private float savedDistance;
		private Transform savedTarget;

		public float Distance {
			
			get {
				return distance;
			}
			set {
				if(distance!=value) {
					distance = value;
					distance = Mathf.Clamp (distance, minDistance, maxDistance);
					distanceModified=true;
				}
			}
		}
		
		
		void Start() {
			Distance = startDistance;
			distanceModified = true;
			StartPanTilt();
		}

		void Update ()
		{
			if(InputManager!=null) {
				float deltaPan = 0;
				float deltaTilt = 0;
				float deltaRoll = 0;
				bool panControlled = false;
				bool tiltControlled = false;
				bool rollControlled = false;
				
				UpdateDeltaPan(ref deltaPan,ref panControlled);
				UpdateDeltaTilt(ref deltaTilt,ref tiltControlled);
				UpdateDeltaRoll(ref deltaRoll,ref rollControlled);
				
				UpdatePan(deltaPan,panControlled);
				UpdateTilt(deltaTilt,tiltControlled);
				UpdateRoll(deltaRoll,rollControlled);
					
				if(canZoom && InputManager.IsZooming) {	
					Distance /= InputManager.DeltaZoom;	
				}
				
				if(PanModified  || TiltModified || RollModified  || distanceModified) {
					Vector3 targetPosition;
					
					if(target!=null)targetPosition = target.position;
					else targetPosition = new Vector3(0,0,0);
					
					Quaternion rotation = Quaternion.Euler(-Tilt, FinalPan, Roll);
					Vector3 position = rotation * new Vector3(0.0f, 0.0f, -Distance) + targetPosition;
					 	
					transform.rotation = rotation;
					transform.position = position;
				}
				
				UpdatePanEnd();
				UpdateTiltEnd();
				UpdateRollEnd();
			}

			if (distanceCorrection) {
				CorrectDistance();
			}
		}

		public void ZoomOn(Transform newTarget, float zoomLevel) {
			if(!alreadySaved) {
				savedTarget = target;
				savedDistance = distance;
				alreadySaved = true;
			}

			this.target = newTarget;
			this.distance = zoomLevel;
		}

		private void CorrectDistance() {
			RaycastHit forwardHit;
			RaycastHit backHit;

			Vector3 forward = transform.TransformDirection(Vector3.forward);
			Vector3 back = transform.TransformDirection(Vector3.back);
			
			if (Physics.Raycast (transform.position, forward, out forwardHit, distanceCorrectionLimit)) {
				//Debug.Log (forwardHit.point);
			}

			if (Physics.Raycast (transform.position, back, out backHit, distanceCorrectionLimit)) {
				//Debug.Log (backHit.point);
			}
		}

		public void Reset() {
			target = savedTarget;
			distance = savedDistance;

			alreadySaved = false;
		}
		

		public override void Activate () {
			base.Activate ();
			transform.gameObject.SetActive(true);
			
			if(resetOnActivate){
				Distance = startDistance;
			}
		}
		
		public override void Desactivate () {
			base.Desactivate ();
			transform.gameObject.SetActive(false);

		}

		public override void PreActivate() {
			Distance = startDistance;
			Pan = startPan;
			Tilt = startTilt;
			Roll = startRoll;

			base.PreActivate ();

			this.Update ();
		}
		
		void OnGUI () {
			if(showDebugInfo){
				GUI.Label (new Rect (100, 100, 300, 100), string.Format("Orbital Camera {0}\nPan : {1}\nTilt : {2}\nRoll : {3}\nDistance : {4}",id, Pan, Tilt, Roll, Distance));
				
			}
		}
	}
}