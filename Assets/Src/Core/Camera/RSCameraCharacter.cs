using UnityEngine;
using System.Collections;

namespace Rise.Core {
	public class RSCameraCharacter : RSCameraPanTilt {
		public float playerHeight = 1.75f;
		public float playerWidth = 0.5f;
		
		public float playerMassKg = 70;
		
		public float bobbingSpeed = 0.15f; 
		public float bobbingAmount = 0.05f; 

		public CharacterController characterController;
		public Camera characterCamera;
		public GameObject capsule;
		
		public float characterSpeedKPH = 15.0f;
		public float straffSpeedKPH = 10.0f;
		public float acceleration = 3.0f;
		public float brakeDecceleration = 6.0f;
		
		public float timeBeforeStuckDetection = 3.0f;
		public float fallDetectionHeight = -50.0f;
		
		public GameObject[] startingPositions;
		private Vector3 savedStartPosition;
		
		//workaround for unaccessible property on character controller
		private float skinWidth = 0.08f;
		
		protected Vector3 velocity;
		private Vector3 forceToApply;
		
		private Vector3 destPosition;
		private Vector3 destDirection;
		private bool destPositionActive;
		private float destDistance;
		private bool destPanActive;
		private float destPan;
		
		protected float destPositionActiveStuckTime = -1;
		private bool destPositionActiveIsStuck = false;
		
		private float midpoint = 0.0f;
		private float timer = 0.0f;  

		public float FrontSpeed {
			get {
				return velocity.z;
			}
		}
		
		public float StraffSpeed {
			get {
				return velocity.x;
			}
		}

		void OnGUI() {
			if(showDebugInfo) {
				GUI.Label (new Rect (100, 100, 300, 100), string.Format("Character Moving Mode {0}\nPan : {1}\nTilt : {2}\nForward Speed : {3}\nSide Speed : {4}",id, Pan, Tilt, velocity.z, velocity.x));
			}
		}
		
		public void SetPosition(Vector3 pos) {
			transform.position = pos;
			//stop the character movement
			velocity = new Vector3(0, 0, 0);
			if(characterController != null) {
				characterController.Move(velocity);
			}
			DestPositionActive = false;
		}
		
		public void ResetPosition() {
			SetPosition(savedStartPosition);
		}
		
		public bool LayDownAtPosition(Vector3 pos) {
			RaycastHit hit;

			if(Physics.Raycast(new Vector3(pos.x,1000.0f,pos.z),new Vector3(0.0f,-1.0f,0.0f), out hit, Mathf.Infinity)) {
				SetPosition(new Vector3(hit.point.x ,hit.point.y + playerHeight / 10, hit.point.z));
				return true;
			}

			Debug.Log("Did not found any ground under my feet");
			return false;
		}
		
		
		protected void SetDestPosition(Vector3 pos) {
			destPosition = pos;
			DestPositionActive = true;
			destPanActive = true;
		}
		
		protected bool IsStuck {
			get {
				return destPositionActiveIsStuck;
			}
			private set {
				destPositionActiveIsStuck = value;
				if(!destPositionActiveIsStuck) {
					destPositionActiveStuckTime = -1;
				}
			}
		}
		
		protected bool IsFalling {
			get {
				return transform.position.y < fallDetectionHeight;
			}
		}
		
		
		public void StopDestPosition() {
			DestPositionActive = false;
			destPanActive = false;
		}
		

		
		protected bool DestPositionActive {
			get {
				return destPositionActive;
			}
			private set {
				destPositionActive = value;
				if(!destPositionActive) {
					IsStuck=false;
				}
			}
		}
		
		protected Vector3 DestPosition {
			get{ 
				return destPosition;
			}
		}
		
		public override void Awake() {
			savedStartPosition = transform.position;
			base.Awake();
		}

		public virtual void Start() {
			if(characterController != null) {
				characterController.center = new Vector3(0, (playerHeight - skinWidth) / 2.0f, 0);
				characterController.height = playerHeight - skinWidth;
				characterController.radius = playerWidth / 2;
			}
			else {
				Debug.LogWarning("Character controller not set in " + id);
			}
			
			if(capsule != null) {
				capsule.transform.localPosition = new Vector3(0, (playerHeight - skinWidth) / 2.0f, 0);
				capsule.transform.localScale = new Vector3(playerWidth, (playerHeight - skinWidth) / 2.0f, playerWidth);
			}
			else {
				Debug.LogWarning("Capsule controller not set in " + id);
			}
			
			if(characterCamera!=null) {
				characterCamera.transform.localPosition = new Vector3(0, playerHeight * 0.92f - skinWidth, 0);
				characterCamera.transform.localRotation = Quaternion.identity;
				characterCamera.nearClipPlane = playerWidth / 4;
			}
			else {
				Debug.LogWarning("Character camera not set in " + id);
			}
			
			midpoint = characterCamera.transform.localPosition.y;
			
			StartPanTilt();
		}
		
		protected void UpdateDeltaSpeed(ref float frontDeltaSpeed,ref float straffDeltaSpeed, ref bool frontControlled, ref bool straffControlled) {
			velocity = transform.InverseTransformDirection (characterController.velocity);
			
			if(DestPositionActive) {
				destDirection.x = destPosition.x - transform.position.x;
				destDirection.z = destPosition.z - transform.position.z;
				destDistance = destDirection.magnitude;
				destDirection.Normalize();
				
				destPan = Mathf.Acos(destDirection.z) * Mathf.Rad2Deg;
				if(destDirection.x < 0) {
					destPan = -destPan;
				}
				
				destDirection = transform.InverseTransformDirection (destDirection);
				
				if(destDistance < playerWidth * 2) {
					DestPositionActive = false;
				}
				else {
					frontDeltaSpeed += acceleration * (destDirection.z * characterSpeedKPH * 0.2777777f - FrontSpeed) * Time.deltaTime;
					straffDeltaSpeed += acceleration * (destDirection.x * characterSpeedKPH * 0.2777777f - StraffSpeed) * Time.deltaTime;
					frontControlled = true;
					straffControlled = true;
					UpdateHeadBobber(destDirection.z, 0.0f);
				}


				float speed = velocity.magnitude;
				if(speed < 0.3f) {
					if(!IsStuck) {
						if(destPositionActiveStuckTime<0) {
							destPositionActiveStuckTime = Time.time;
						}
						else if(Time.time - destPositionActiveStuckTime > timeBeforeStuckDetection) {
							IsStuck = true;
						}
					}
				}
				else {
					destPositionActiveStuckTime = -1;
					if(IsStuck) {
						IsStuck = false;
					}
				}
				
			} 
			else {
				destPanActive = false;
				
			}
		}

		protected override void UpdatePan(float deltaPan,bool panControlled) {
			if(destPanActive) {
				float doubleTapDeltaPan = ComputeDeltaPan(destPan,Pan);
				if(panControlled || Mathf.Abs(doubleTapDeltaPan) < 1.0f) {
					destPanActive = false;
				}
				else {
					deltaPan += ComputeDeltaPan(destPan,Pan) * Time.deltaTime;
					panControlled = true;
				}
			}
			base.UpdatePan(deltaPan,panControlled);
		}

		protected void UpdateCharacter(float frontDeltaSpeed, float straffDeltaSpeed, bool frontControlled, bool straffControlled) {
			if(TiltModified || RollModified) {
				Quaternion rotation = Quaternion.Euler(-Tilt, 0.0f, Roll);
				if(characterCamera != null) {
					characterCamera.transform.localRotation = rotation;
				}
			}
			
			if(PanModified || DevicePanModified) {
				Quaternion rotation = Quaternion.Euler(0.0f, FinalPan, 0.0f);
				transform.localRotation = rotation;
			}

			if(characterController!=null) {
				if(frontControlled) {
					velocity.z += frontDeltaSpeed;
				}
				else {
					if(velocity.z > 0.1f) {
						velocity.z -= brakeDecceleration * Time.deltaTime;
					}
					else if(velocity.z < -0.1f) {
						velocity.z += brakeDecceleration * Time.deltaTime;
					}
					else {
						velocity.z = 0;
					}
				}
				
				if(straffControlled) {
					velocity.x += straffDeltaSpeed;
				}
				else {
					if(velocity.x > 0.1f) {
						velocity.x -= brakeDecceleration * Time.deltaTime;
					}
					else if(velocity.x < -0.1f) {
						velocity.x += brakeDecceleration * Time.deltaTime;
					}
					else {
						velocity.x = 0;
					}
				}
		
				if(!characterController.isGrounded) {
					velocity.y -= 9.81f * Time.deltaTime /2.0f;
				}
				else {
					velocity.y = 0;
				}
				
				if(frontControlled || straffControlled) {
					LastActivityTime = Time.time;
				}
				
				velocity += forceToApply / (2.0f * playerMassKg);

				Vector3 movement = transform.TransformDirection(velocity.x * Time.deltaTime, 0, velocity.z * Time.deltaTime);
				movement.y = velocity.y * Time.deltaTime;

				characterController.Move(movement);
				forceToApply = new Vector3(0,0,0);
			}
		}

		public void UpdateHeadBobber(float moveXAxis, float moveYAxis) {
			float waveslice = 0.0f;

			if(Mathf.Abs(moveXAxis) == 0 && Mathf.Abs(moveYAxis) == 0) {
				timer = 0.0f; 
			} 
			else { 
				waveslice = Mathf.Sin(timer); 
				timer = timer + bobbingSpeed; 
				if (timer > Mathf.PI * 2) { 
					timer = timer - (Mathf.PI * 2); 
				} 
			} 

			if(waveslice != 0) { 
				float translateChange = waveslice * bobbingAmount; 
				float totalAxes = Mathf.Abs(moveXAxis) + Mathf.Abs(moveYAxis); 
				
				totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f); 
				translateChange = totalAxes * translateChange; 
				
				Vector3 localPosition = characterCamera.transform.localPosition;
				localPosition.y = midpoint + translateChange;
				characterCamera.transform.localPosition = localPosition;
			} 
			else { 
				Vector3 localPosition = characterCamera.transform.localPosition;
				localPosition.y = midpoint;
				characterCamera.transform.localPosition = localPosition; 
			}
		}
		
		public void ApplyDeltaForce(float fx,float fy,float fz) {
			forceToApply += new Vector3(fx,fy,fz);
		}

		public override void Activate() {
			base.Activate ();
			transform.gameObject.SetActive(true);
			
			if(resetOnActivate) {
				if(startingPositions.Length > 0) {
					SetPosition(startingPositions[0].transform.position);
				}
				else {
					SetPosition(savedStartPosition);
				}
			}
		}
		
		public override void Desactivate() {
			base.Desactivate();
			transform.gameObject.SetActive(false);
		}
		
		public override GameObject GetCameraGameObject() {
			return characterCamera.gameObject;
		}

		protected void UpdateCharacterEnd() {
			forceToApply = new Vector3(0, 0, 0);
		}
	}
}