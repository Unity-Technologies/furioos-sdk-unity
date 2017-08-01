using UnityEngine;
using System.Collections;
using Rise.Core;

namespace Rise.SDK.Cameras {
	[RequireComponent (typeof(Camera))]
	public class PanoramicCamera : RSCameraPanTilt {
		public virtual void Start(){
			StartPanTilt();
			MouseAndFingerControlInverted = true;
		}
		
		void Update () {
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
		
			if(PanModified || TiltModified || RollModified ) {
				transform.localEulerAngles = new Vector3(-Tilt, Pan+ScenePan, Roll);
			}
			
			UpdatePanEnd();
			UpdateTiltEnd();
			UpdateRollEnd();

		}
		
		void OnGUI () {
			if(showDebugInfo){
				GUI.Label (new Rect (100, 100, 300, 100), string.Format("Panoramic Camera {0}\nPan : {1}\nTilt : {2}\n",id, Pan, Tilt));
			}
		}
		
		public override void Activate () {
			base.Activate ();
			transform.gameObject.SetActive(true);

		}
		
		public override void Desactivate () {
			base.Desactivate ();
			transform.gameObject.SetActive(false);

		}
	}
}