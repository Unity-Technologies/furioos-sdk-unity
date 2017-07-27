#pragma warning disable 0649
#pragma warning disable 0414

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;

namespace Rise.Core {

	public enum UseDeviceOrientationType { None, Gyroscope, Oculus };

	public class RSPanTiltMovingMode : RSCamera {
		
		public float sensitivity = 1; 
		
		public bool canPan = true;
		public float minPan = -180;
		public float startPan = 0;
		public float maxPan = 180;
		public bool invertPan = false;
		
		public bool canTilt = true;
		public float minTilt = -60;
		public float startTilt = 0;
		public float maxTilt = 60;
		public bool invertTilt = false;
		
		public bool canRoll = true;
		public float startRoll = 0;
		
		public float inertia = 0.1f;

		private Quaternion rot = Quaternion.identity;
		private float q0,q1,q2,q3;
		private Vector3 testGravity = new Vector3(0.0f, -9.81f, 0.0f);
		private float testHeading = 0.0f;
		
		private float pan, panSpeed, devicePan, panSensitivity = 90.0f;
		private float tilt, tiltSpeed, deviceTilt, tiltSensitivity = 90.0f;
		private float roll, rollSpeed, deviceRoll;
		private bool panModified,devicePanModified;
		private bool tiltModified;
		private bool rollModified;

		public bool MouseAndFingerControlInverted{
			get;
			set;
		}
		
		protected float ScenePan{
			get{ return 0; }
		}
		
		
		public float FinalPan {
			get { 
				return Pan + DevicePan + ScenePan;
			}
		}

		public float DevicePan {
			get {
				return devicePan;
			}
			set {
				if(devicePan != value) {
					devicePan = value;
					devicePanModified=true;
				}
			}
		}
		
		
		public float Pan {
			get { 
				return pan;
			}
			set {
				float newPan = (maxPan - minPan < 360.0f) ? Mathf.Clamp (value, minPan, maxPan): value;
				while (newPan>180f) newPan -= 360f;
				while (newPan<-180f) newPan += 360f;
				
				if(pan!=newPan){
					pan = newPan;
					panModified=true;
				}
			}
		}
		
		public float Roll {
			get{
				return roll;
			}
			set {
				if(roll != value) {
					roll = value;
					rollModified=true;
				}
			}
		}
		
		public float Tilt {
			get {
				return tilt;
			}
			set {
				float newTilt = Mathf.Clamp (value, minTilt, maxTilt);
				if(tilt != newTilt) {
					tilt = newTilt;
					tiltModified=true;
				}
			}
		}
		
		protected bool PanModified {
			get {
				return panModified;
			}
		}
		
		protected bool TiltModified {
			get {
				return tiltModified;
			}
		}
		
		protected bool RollModified {
			get {
				return rollModified;
			}
		}
		
		protected bool DevicePanModified {
			get {
				return devicePanModified;
			}
		}
			
		public float ComputeDeltaPan (float destPan,float srcPan) {
			float deltaPan = destPan - srcPan;

			while (deltaPan>180f) {
				deltaPan -= 360f;
			}

			while (deltaPan<-180f) {
				deltaPan += 360f;
			}

			return deltaPan;
		}
		
		
		public void StartPanTilt(){
			Pan = startPan;
			Tilt = startTilt;
		}
		
		protected void UpdateDeltaPan(ref float deltaPan,ref bool panControlled) {
			if(InputManager!=null){
				if(InputManager.IsMoving){
					deltaPan += (invertPan ^ MouseAndFingerControlInverted ? -sensitivity : sensitivity) * InputManager.NormalizedDelta.x * panSensitivity;
					panControlled = true;
				}
				
				
				float lookAxis = InputManager.GetAxisRaw("Look X");
				if(lookAxis!=0){
					deltaPan += (invertPan ? -sensitivity : sensitivity) * lookAxis * Time.deltaTime * panSensitivity;
					panControlled = true;
				}
			}
		}
		
		protected void UpdateDeltaTilt(ref float deltaTilt,ref bool tiltControlled) {			
			if(InputManager!=null) {
				if(InputManager.IsMoving) {
					deltaTilt += (invertTilt ^ MouseAndFingerControlInverted ? -sensitivity : sensitivity) * InputManager.NormalizedDelta.y * tiltSensitivity;
					tiltControlled = true;
				}
				
				float lookAxis = InputManager.GetAxisRaw("Look Y");
				if(lookAxis!=0) {
					deltaTilt += (invertTilt ? -sensitivity : sensitivity) * lookAxis * Time.deltaTime * tiltSensitivity;
					tiltControlled = true;
				}
			}
		}
		
		protected void UpdateDeltaRoll(ref float deltaRoll,ref bool rollControlled) { }
		
		//this function have to be called one and only one time by update in child classes
		protected virtual void UpdatePan(float deltaPan,bool panControlled){
			if (panControlled && canPan){
				Pan += deltaPan;
				//small smoothing of the speeds over two values, avoiding brutal breaks
				panSpeed = (panSpeed + deltaPan / Time.deltaTime)/2.0f;
				LastActivityTime = Time.time;
			}else{
				if(inertia > Time.deltaTime ){
					panSpeed -= panSpeed  * Time.deltaTime / inertia;
					if(panSpeed < - 0.1f ||  panSpeed > 0.1f)Pan += panSpeed * Time.deltaTime;
					else panSpeed = 0.0f;
				}else{
					panSpeed = 0.0f;
				}
			}
		}

		//this function have to be called one and only one time by update in child classes
		protected virtual void UpdateTilt(float deltaTilt,bool tiltControlled){
			if (tiltControlled && canTilt){
				Tilt -= deltaTilt;
				tiltSpeed = (tiltSpeed + deltaTilt / Time.deltaTime)/2.0f;
				LastActivityTime = Time.time;
			} else {
				if(inertia > Time.deltaTime ) {
					tiltSpeed -= tiltSpeed  * Time.deltaTime / inertia;
					if(tiltSpeed < - 0.1f ||  tiltSpeed > 0.1f)Tilt -= tiltSpeed * Time.deltaTime;
					else tiltSpeed = 0.0f;
				} else {
					tiltSpeed = 0.0f;
				}
			}
		}
		
		//this function have to be called one and only one time by update in child classes
		protected virtual void UpdateRoll(float deltaRoll,bool rollControlled){
			if (rollControlled && canRoll){
				Roll += deltaRoll;
				//small smoothing of the speeds over two values, avoiding brutal breaks
				rollSpeed = (rollSpeed + deltaRoll / Time.deltaTime)/2.0f;
				LastActivityTime = Time.time;
			}else{
				if(inertia > Time.deltaTime ){
					rollSpeed -= rollSpeed  * Time.deltaTime / inertia;
					if(rollSpeed < - 0.1f ||  rollSpeed > 0.1f)Roll += rollSpeed * Time.deltaTime;
					else rollSpeed = 0.0f;
				}else{
					rollSpeed = 0.0f;
				}
			}
		}

		protected void UpdateTiltEnd() {
			tiltModified = false;
		}
		
		protected void UpdatePanEnd() {
			panModified = false;
			devicePanModified = false;
		}
		
		protected void UpdateRollEnd() {
			rollModified = false;
		}
		
		
		public override void Activate () {
			if(resetOnActivate) {
				Pan = startPan;
				Tilt = startTilt;
				Roll = startRoll;
			}
			
			panSpeed = 0.0f;
			tiltSpeed = 0.0f;
			rollSpeed = 0.0f;
			tiltModified = true;
			panModified = true;
			rollModified = true;

			Camera cam = GetCameraGameObject().GetComponent<Camera>();
			tiltSensitivity = cam.fieldOfView;
			panSensitivity = 2.0f * Mathf.Atan(Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * cam.aspect) * Mathf.Rad2Deg;
			

			base.Activate ();
		}

		public override void PreActivate() {
			tiltModified = true;
			panModified = true;
			rollModified = true;
		}
	}
}