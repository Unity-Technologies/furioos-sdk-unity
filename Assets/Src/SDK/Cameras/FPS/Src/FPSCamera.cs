using UnityEngine;
using System.Collections;
using Rise.Core;

namespace Rise.Features.MovingMode {
	public class FPSCamera : RSCharacterMovingMode {
		public VirtualJoystick moveJoystick;
		public VirtualJoystick lookJoystick;
		public GameObject doubleTapDestinationMesh;

		public bool useGPSPositionning = true;
		
		public bool useJoysticksOnMobileDevices = true;
		public bool useDoubleTapDisplacement = true;

		public GameObject footPlane;
		public Texture2D footPlaneDisabled;
		public Texture2D footPlaneEnabled;
		public bool autoWalk = false;

		private bool useJoysticks;

		private GameObject destMesh;

		private double lastTimeStamp;

		public override void Start () {
			MouseAndFingerControlInverted = true;
			base.Start ();
		}

		void OnGUI () {
			if(showDebugInfo){
				GUI.Label (new Rect (100, 100, 300, 200), string.Format("Is Stuck: {3}\nSpeed: {4}\nPan: {5}\nTilt: {6}", Input.location.status, IsStuck, velocity.magnitude,Pan,Tilt));
			}
		}
		
		void Update() {
			if(useDoubleTapDisplacement && InputManager!=null && characterCamera!=null){
				if(InputManager.HasGoneInactive  && InputManager.NumClick >= 2){
					if(!DestPositionActive){
						RaycastHit hit;
						Ray ray = InputManager.GetRay(characterCamera);
			
						if(Physics.Raycast(ray, out hit, Mathf.Infinity)) {
							
							SetDestPosition(hit.point);
							
							if(destMesh==null && doubleTapDestinationMesh!=null){
								destMesh = (GameObject)Instantiate(doubleTapDestinationMesh);
							}
							
							destMesh.transform.position = DestPosition;
						}
					} 
					else {
						StopDestPosition();
					}
				}
			}
			
			if(DestPositionActive){
				if(destMesh!=null){
					destMesh.transform.LookAt(characterCamera.transform);
					float scale = 1.0f + 0.2f * Mathf.Cos(Time.time*4.0f);
					destMesh.transform.localScale = new Vector3(scale,scale,0);
				}
			} else{
				if(destMesh!=null) {
					DestroyImmediate(destMesh);
				}
			}

			//pan and tilt movements
			
			float deltaPan = 0;
			float deltaTilt = 0;
			float deltaRoll = 0;
			bool panControlled = false;
			bool tiltControlled = false;
			bool rollControlled = false;

			UpdateDeltaPan(ref deltaPan,ref panControlled);
			UpdateDeltaTilt(ref deltaTilt,ref tiltControlled);
			UpdateDeltaRoll(ref deltaRoll,ref rollControlled);

			if(lookJoystick!=null){
				if(lookJoystick.X!=0.0f){
					panControlled = true;
					deltaPan = (invertPan ? -sensitivity : sensitivity) * lookJoystick.X * Time.deltaTime * 60.0f;
				}
				if(lookJoystick.Y!=0.0f){
					tiltControlled = true;
					deltaTilt = (invertPan ? -sensitivity : sensitivity) * lookJoystick.Y  * Time.deltaTime * 60.0f;
				}
			}

			//character movement
			
			float straffDeltaSpeed = 0;
			float frontDeltaSpeed = 0;
			bool straffControlled = false;
			bool frontControlled = false;

			UpdateDeltaSpeed(ref frontDeltaSpeed,ref straffDeltaSpeed, ref frontControlled, ref straffControlled);

			if(InputManager!=null){

				float moveYAxis = InputManager.GetAxisRaw("Move Y");
				if(moveYAxis!=0){
					if(moveYAxis < 0) frontDeltaSpeed -= moveYAxis * acceleration * (characterSpeedKPH * 0.2777777f - FrontSpeed) * Time.deltaTime;
					else frontDeltaSpeed += moveYAxis * acceleration * (-characterSpeedKPH * 0.2777777f - FrontSpeed)* Time.deltaTime;
					frontControlled = true;
				}

				float moveXAxis = InputManager.GetAxisRaw("Move X");
				if(moveXAxis!=0){
					if(moveXAxis < 0 ) straffDeltaSpeed -= moveXAxis * acceleration * (-straffSpeedKPH * 0.2777777f - StraffSpeed)* Time.deltaTime;
					else straffDeltaSpeed += moveXAxis * acceleration * (straffSpeedKPH * 0.2777777f - StraffSpeed)* Time.deltaTime;
					straffControlled = true;
				}

				if(moveXAxis != 0 || moveYAxis != 0)
					UpdateHeadBobber(moveYAxis, moveXAxis);
			}

			if(moveJoystick!=null){
				if(moveJoystick.Y!=0.0f){
					frontDeltaSpeed = - moveJoystick.Y * characterSpeedKPH * 0.2777777f* Time.deltaTime;
					frontControlled = true;
				}
				if(moveJoystick.X!=0.0f){
					straffDeltaSpeed = moveJoystick.X * straffSpeedKPH * 0.2777777f* Time.deltaTime;
					straffControlled = true;
				}
			}

			if (autoWalk) {
				frontDeltaSpeed = 1.0f * acceleration * (characterSpeedKPH * 0.2777777f - FrontSpeed) * Time.deltaTime;
				frontControlled = true;
			}

			if(InputManager!=null){
				float jump = InputManager.GetAxisRaw("Jet Pack");
				//Debug.Log(jump);
				if(jump>0){
					ApplyDeltaForce(0,jump*1200.0f*Time.deltaTime,0);
				}
			}

			//Parent classes updates
			
			UpdatePan(deltaPan,panControlled);
			UpdateTilt(deltaTilt,tiltControlled);
			UpdateRoll(deltaRoll,rollControlled);
			UpdateCharacter( frontDeltaSpeed, straffDeltaSpeed, frontControlled, straffControlled);

			if(IsStuck)LayDownAtPosition(DestPosition);
			if(IsFalling)ResetPosition();

			/*
			if (useDeviceOrientation == UseDeviceOrientationType.Gyroscope) {

				footPlane.SetActive(true);

				if (Tilt == minTilt && Time.time - autoWalkStartTime > autoWalkTimeEnableDisable) {
					autoWalk = !autoWalk;
					autoWalkStartTime = Time.time;

					if (autoWalk) {
						footPlane.GetComponent<Renderer>().material.mainTexture = footPlaneEnabled;
					} else {
						footPlane.GetComponent<Renderer>().material.mainTexture = footPlaneDisabled;
					}
				}
			} else {
				footPlane.SetActive(false);
			}
			*/
			
			//end of the update
			
			UpdatePanEnd();
			UpdateTiltEnd();
			UpdateRollEnd();
			UpdateCharacterEnd();
			
		}

		public override void PreActivate() {
			base.PreActivate ();

			this.Start ();
			this.Update ();
		}
		
		public override void Activate () {
			useJoysticks = ((Application.platform == RuntimePlatform.IPhonePlayer ||
			                   Application.platform == RuntimePlatform.Android)
			                 && useJoysticksOnMobileDevices);
			
			if(moveJoystick!=null)moveJoystick.enabled = useJoysticks;
			if(lookJoystick!=null)lookJoystick.enabled = useJoysticks;
			
			StopDestPosition();
			
			base.Activate ();
		}
	}
}