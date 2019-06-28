using UnityEngine;
using System.Collections;

namespace FurioosSDK.Core {
	public class FSInputBinding{
		
		private string positiveJoy = null;
		private string negativeJoy = null;
		private KeyCode positiveKey = KeyCode.None;
		private KeyCode negativeKey = KeyCode.None;
		
		public FSInputBinding(){
			
		}
		
		public void SetJoyStick(string positive, string negative = null){
			this.positiveJoy = positive;
			this.negativeJoy = negative;
		}
		
		public void SetKeyboard(KeyCode positive, KeyCode negative = KeyCode.None){
			this.positiveKey = positive;
			this.negativeKey = negative;
		}
		
		
		public float GetAxisRaw(){
			return Mathf.Clamp (positiveKey!=KeyCode.None && Input.GetKey(positiveKey) ? 1.0f : 0.0f + (!string.IsNullOrEmpty(positiveJoy) ? Input.GetAxisRaw(positiveJoy) : 0.0f), -1.0f, 1.0f)
				- Mathf.Clamp (negativeKey!=KeyCode.None && Input.GetKey(negativeKey) ? 1.0f : 0.0f + (!string.IsNullOrEmpty(negativeJoy) ? Input.GetAxisRaw(negativeJoy) : 0.0f), -1.0f, 1.0f);
		}

		public bool GetKeyUp() {
			return Input.GetKeyUp (positiveKey) || Input.GetButtonUp(positiveJoy);
		}
		
	}
}