#pragma warning disable 0649
#pragma warning disable 0414

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;

namespace Rise.Core {

	public enum UseDeviceOrientationType { None, Gyroscope, Oculus };

	public class RSPanTiltMovingMode : RSMovingMode {
		
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
		
		public UseDeviceOrientationType useDeviceOrientation = UseDeviceOrientationType.None;
		public float deviceOrientationSmoothing = 0.75f;
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
		private bool deviceOrientationComputed;
		private bool updateCompassAndGyroStatesDone;
		private UseDeviceOrientationType lastUseDeviceOrientation = UseDeviceOrientationType.None;

		private static bool savedUseDeviceOrientationSet = false;
		private static UseDeviceOrientationType savedUseDeviceOrientation;
		
		private Compass compass;
		private Gyroscope gyroscope;

		#region gyroscope
		#if UNITY_EDITOR

		#elif UNITY_ANDROID
		private static AndroidJavaClass javadivepluginclass;
		private static AndroidJavaClass javaunityplayerclass;
		private static AndroidJavaObject currentactivity;
		private static AndroidJavaObject javadiveplugininstance;

		[DllImport("divesensor")]	private static extern void initialize_sensors();
		[DllImport("divesensor")]	private static extern int get_q(ref float q0,ref float q1,ref float q2,ref float q3);
		[DllImport("divesensor")]	private static extern int get_m(ref float m0,ref float m1,ref float m2);
		[DllImport("divesensor")]	private static extern int get_error();
		[DllImport("divesensor")]   private static extern void dive_command(string command);
		#elif UNITY_IPHONE
		[DllImport("__Internal")]	private static extern void initialize_sensors();
		[DllImport("__Internal")]	private static extern float get_q0();
		[DllImport("__Internal")]	private static extern float get_q1();
		[DllImport("__Internal")]	private static extern float get_q2();
		[DllImport("__Internal")]	private static extern float get_q3();
		[DllImport("__Internal")]	private static extern void DiveUpdateGyroData();
		[DllImport("__Internal")]	private static extern int get_q(ref float q0,ref float q1,ref float q2,ref float q3);
		#endif
		#endregion
		

		public bool MouseAndFingerControlInverted{
			get;
			set;
		}

		protected OVRDevice OculusDevice{
			get;
			private set;
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
			
			get{return devicePan;}
			set{
				if(devicePan != value){
					devicePan = value;
					devicePanModified=true;
				}
			}
		}
		
		
		public float Pan {
			
			get{return pan;}
			set{
				
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
			
			get{return roll;}
			set{
				if(roll != value){
					roll = value;
					rollModified=true;
				}
			}
		}
		
		
		
		public float Tilt {
			
			get{return tilt;}
			set{
				float newTilt = Mathf.Clamp (value, minTilt, maxTilt);
				if(tilt != newTilt){
					tilt = newTilt;
					tiltModified=true;
				}
			}
		}
		

		
		public float ComputeDeltaPan (float destPan,float srcPan){
			float deltaPan = destPan - srcPan;
			while (deltaPan>180f) deltaPan -= 360f;
			while (deltaPan<-180f) deltaPan += 360f;
			return deltaPan;
		}
		
		
		
		protected bool PanModified {
			get{return panModified;}
		}
		
		protected bool TiltModified {
			get{return tiltModified;}
		}
		
		protected bool RollModified {
			get{return rollModified;}
		}
		
		protected bool DevicePanModified {
			get{return devicePanModified;}
		}
		
		
		public void StartPanTilt(){
			if(savedUseDeviceOrientationSet)useDeviceOrientation = savedUseDeviceOrientation;
			OculusDevice = RSSceneManager.GetInstance<OVRDevice>();
			Pan = startPan;
			Tilt = startTilt;
			
		
		}
		
		protected void UpdateDeltaPan(ref float deltaPan,ref bool panControlled){
			UpdateCompassAndGyroStates ();

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
		
		protected void UpdateDeltaTilt(ref float deltaTilt,ref bool tiltControlled){
			UpdateCompassAndGyroStates ();
			
			
			switch(useDeviceOrientation){
				
				case UseDeviceOrientationType.Gyroscope :
					UpdateGyroscopeOrientation();
					deltaTilt = -(deviceTilt-Tilt) * (1.0f-deviceOrientationSmoothing);
					tiltControlled = true;
				break;
				
				case UseDeviceOrientationType.Oculus :
					UpdateOculusOrientation();
					deltaTilt = -(deviceTilt-Tilt);
					tiltControlled = true;
				break;
			}
			
			if(InputManager!=null){
				if(InputManager.IsMoving){
					deltaTilt += (invertTilt ^ MouseAndFingerControlInverted ? -sensitivity : sensitivity) * InputManager.NormalizedDelta.y * tiltSensitivity;
					tiltControlled = true;
				}
				
				float lookAxis = InputManager.GetAxisRaw("Look Y");
				if(lookAxis!=0){
					deltaTilt += (invertTilt ? -sensitivity : sensitivity) * lookAxis * Time.deltaTime * tiltSensitivity;
					tiltControlled = true;
				}
			}
		}
		
		protected void UpdateDeltaRoll(ref float deltaRoll,ref bool rollControlled){
			UpdateCompassAndGyroStates ();
			
			
			switch(useDeviceOrientation){
				
				case UseDeviceOrientationType.Gyroscope :
					UpdateGyroscopeOrientation();
					deltaRoll = (deviceRoll-Roll) * (1.0f-deviceOrientationSmoothing);
					rollControlled = true;
				break;
				
				case UseDeviceOrientationType.Oculus :
					UpdateOculusOrientation();
					deltaRoll = (deviceRoll-Roll);
					rollControlled = true;
				break;
			}
			
		}
		
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
				//small smoothing of the speeds over two values, avoiding brutal breaks
				tiltSpeed = (tiltSpeed + deltaTilt / Time.deltaTime)/2.0f;
				LastActivityTime = Time.time;
			}else{
				if(inertia > Time.deltaTime ){
					tiltSpeed -= tiltSpeed  * Time.deltaTime / inertia;
					if(tiltSpeed < - 0.1f ||  tiltSpeed > 0.1f)Tilt -= tiltSpeed * Time.deltaTime;
					else tiltSpeed = 0.0f;
				}else{
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
		
		void UpdateCompassAndGyroStates ()
		{
			if(!updateCompassAndGyroStatesDone){
				
				if(useDeviceOrientation != lastUseDeviceOrientation){
					
					if(useDeviceOrientation == UseDeviceOrientationType.Gyroscope){
						/*
						Debug.Log("Compass ans Gyroscope started");
						if(compass==null)compass = new Compass();
						compass.enabled = true;
						
						if(gyroscope==null)gyroscope = Input.gyro;
						gyroscope.enabled = true;
						*/

						#if UNITY_EDITOR
						
						if (GetComponent<Rigidbody>())
							GetComponent<Rigidbody>().freezeRotation = true;
						
						#elif UNITY_ANDROID
						
						// Java part
						javadivepluginclass = new AndroidJavaClass("com.shoogee.divejava.divejava") ;
						javaunityplayerclass= new AndroidJavaClass("com.unity3d.player.UnityPlayer");
						currentactivity = javaunityplayerclass.GetStatic<AndroidJavaObject>("currentActivity");
						javadiveplugininstance = javadivepluginclass.CallStatic<AndroidJavaObject>("instance");
						object[] args={currentactivity};
						javadiveplugininstance.Call<string>("set_activity",args);

						initialize_sensors ();

						String answer;
						answer= javadiveplugininstance.Call<string>("initializeDive");
						answer= javadiveplugininstance.Call<string>("getDeviceType");

						answer= javadiveplugininstance.Call<string>("setFullscreen");

						Network.logLevel = NetworkLogLevel.Full;
						
						int err = get_error();

						#elif UNITY_IPHONE
						initialize_sensors();
						#endif


					}else{
						
						Debug.Log("Compass ans Gyroscope stopped");
						if(compass!=null){
							//compass.enabled = false;
							//compass = null;
						}

						if(gyroscope!=null){
							//gyroscope.enabled = false;
							//gyroscope = null;
						}
						
						Roll = 0;
						DevicePan = 0;
					}
					
					lastUseDeviceOrientation = useDeviceOrientation;
					savedUseDeviceOrientation = useDeviceOrientation;
					savedUseDeviceOrientationSet = true;
				}
				
				
				updateCompassAndGyroStatesDone = true;
			}
		}
		
		void UpdateOculusOrientation(){
			if(!deviceOrientationComputed && OculusDevice!=null){
				Quaternion devOri = new Quaternion();
				Vector3 devPos = new Vector3();
				OVRDevice.GetCameraPositionOrientation(ref devPos,ref devOri);
				
				deviceTilt = devOri.eulerAngles.x;
				deviceTilt = deviceTilt>180 ? - deviceTilt + 360 : - deviceTilt;

				DevicePan = devOri.eulerAngles.y;
				
				deviceRoll = devOri.eulerAngles.z;
				deviceRoll = deviceRoll>180 ? deviceRoll - 360 : deviceRoll;

				deviceOrientationComputed = true;
			}
		}
		
			
		void UpdateGyroscopeOrientation ()
		{
			if(!deviceOrientationComputed){

				#if UNITY_EDITOR
				
				#elif UNITY_ANDROID
				
				get_q(ref q0,ref q1,ref q2,ref q3);
				rot.x=-q2;
				rot.y=q3;
				rot.z=-q1;
				rot.w=q0;

				float gyroTilt;
				if(rot.eulerAngles.x > 180.0f) {
					gyroTilt = 360.0f - rot.eulerAngles.x;
				}
				else {
					gyroTilt = rot.eulerAngles.x * -1;
				}

				//deviceRoll = rot.eulerAngles.z;
				deviceTilt = gyroTilt;
				DevicePan = rot.eulerAngles.y;

				#elif UNITY_IPHONE

				DiveUpdateGyroData();
				get_q(ref q0,ref q1,ref q2,ref q3);
				rot.x=-q2;
				rot.y=q3;
				rot.z=-q1;
				rot.w=q0;

				float gyroTilt;
				if(rot.eulerAngles.x > 180.0f) {
					gyroTilt = 360.0f - rot.eulerAngles.x;
				}
				else {
					gyroTilt = rot.eulerAngles.x * -1;
				}
				
				//deviceRoll = rot.eulerAngles.z;
				deviceTilt = gyroTilt;
				DevicePan = rot.eulerAngles.y;

				#endif
			
				/*
			#if UNITY_EDITOR
					float hdg = testHeading;
					Vector3 grav = testGravity;
			#else		
					float hdg =  compass!=null ? compass.trueHeading : testHeading;
					Vector3 grav = gyroscope!=null ? gyroscope.gravity : testGravity;
			#endif
			
					while(hdg >= 360.0f)hdg -= 360.0f;
					while(hdg < 0.0f)hdg += 360.0f;
					
					
					Vector3 relativeXaxis = Vector3.Cross(grav, new Vector3(0.0f, 0.0f, -1.0f)).normalized;
					
					grav.x = 0.0f;
					grav = grav.normalized;
					
					deviceRoll = - Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(grav.normalized, relativeXaxis)) + 90.0f;
					deviceTilt = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(grav.normalized, new Vector3(0.0f, 0.0f, -1.0f))) - 90.0f;
					DevicePan = DevicePan + ComputeDeltaPan(hdg + deviceRoll,DevicePan) * (1.0f - deviceOrientationSmoothing);
					
				*/	
			
				
				deviceOrientationComputed = true;
			}
		}
		
		
		protected void UpdateTiltEnd(){
			tiltModified = false;
			deviceOrientationComputed = false;
			updateCompassAndGyroStatesDone = false;
		}
		
		protected void UpdatePanEnd(){
			panModified = false;
			devicePanModified = false;
			deviceOrientationComputed = false;
			updateCompassAndGyroStatesDone = false;
		}
		
		protected void UpdateRollEnd(){
			rollModified = false;
			deviceOrientationComputed = false;
			updateCompassAndGyroStatesDone = false;
		}
		
		
		public override void Activate ()
		{
			
			if(resetOnActivate){
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