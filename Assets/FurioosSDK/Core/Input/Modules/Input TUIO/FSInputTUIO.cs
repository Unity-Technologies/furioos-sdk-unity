using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FurioosSDK.Core {
	public class FSInputTUIO : FSInputController {
		private TuioTrackingComponent tuioTracking;
		private bool savedUseTuio;

		void Start() {
			EnableTUIO(true);
		}

		private void EnableTUIO(bool enable) {
			if(enable && !this.enabled) { 
				return; 
			}

			if(enable != savedUseTuio) {
				if(enable && tuioTracking == null) {
					tuioTracking = new TuioTrackingComponent();
					Debug.Log ("Tuio Started");
				}
				else if(tuioTracking != null) {
					tuioTracking.Close();
					tuioTracking = null;
					Debug.Log ("Tuio Stopped");
				}
				savedUseTuio = enable;
			}
		}

		public override void UpdateTouches(FSInputManager oic) {
			if(tuioTracking != null) {
				tuioTracking.BuildTouchDictionary();
				foreach (KeyValuePair<int, Tuio.Touch> touch in tuioTracking.AllTouches) {
					FSInputTouch fingerTouch = oic.findOrCreateTouch(touch.Key);
					fingerTouch.IsActive = true;
					fingerTouch.UpdatePosition(touch.Value.RawPoint.x * Screen.width, touch.Value.RawPoint.y * Screen.height);
				}

			}
		}

		public void OnEnable() {
			EnableTUIO(true);
		}

		public void OnDisable() {
			EnableTUIO(false);
		}
	}
}