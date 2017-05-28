using UnityEngine;
using System.Collections;
using Rise.Core;

namespace Rise.Features.MovingMode {
	public class VirtualJoystick : RSBehaviour {
		public float positionX = 0.16f;
		public float positionY = 0.8f;
		public float size = 0.1f;
		public float amplitude = 0.08f;
		public float deadZone = 0.1f;

		public Texture texture;
		
		private Rect joystickRect;
		
		private float screenWidth, screenHeight;
		private float screenAmplitudeX, screenAmplitudeY;
		
		private RSInputManager mouseAndFingerControl;
		
		private float displayOffsetX;
		private float displayOffsetY;
		
		private float valX;
		private float valY;
		
		private RSFingerTouch touch;
		
		public float X {
			get {
				return valX;
			}
		}
		
		public float Y {
			get {
				return valY;
			}
		}

		void Start () {
			mouseAndFingerControl = RSManager.AppManager.InputManager;
			
			joystickRect = new Rect();
			
			float textureRatio = (float)texture.width / texture.height;
			float screenRatio = (float)Screen.width / Screen.height;
			
			screenWidth = size;
			screenHeight = size * screenRatio / textureRatio;
			
			screenAmplitudeX = amplitude;
			screenAmplitudeY = amplitude * screenRatio;
		}

		void Update () {
			if(mouseAndFingerControl!=null) {
				if(touch==null){
					foreach(RSFingerTouch testTouch in mouseAndFingerControl.Touches) {
						if(testTouch.HasGoneActive && IsInside(testTouch.Position)) {
							touch = testTouch;
							break;
						}
					}
				}
				
				if(touch!=null) {
					if(touch.IsActive) {
						displayOffsetX = Mathf.Clamp(touch.NormalizedPosition.x - positionX, -screenAmplitudeX, screenAmplitudeX);
						displayOffsetY = Mathf.Clamp(touch.NormalizedPosition.y - positionY, -screenAmplitudeY, screenAmplitudeY);
						
						float tmpValX = displayOffsetX / screenAmplitudeX;
						float tmpValY = displayOffsetY / screenAmplitudeY;
						
						float signX = Mathf.Sign(tmpValX);
						float signY = Mathf.Sign(tmpValY);
						
						if(deadZone<1){
							valX = signX * (Mathf.Clamp((signX * tmpValX - deadZone) / (1 - deadZone), 0, 1));
							valY = signY * (Mathf.Clamp((signY * tmpValY - deadZone) / (1 - deadZone), 0, 1));
						}
					} else {
						touch = null;
					}
				} else {
					valX = 0;
					valY = 0;
					displayOffsetX = 0;
					displayOffsetY = 0;
				}
			}
			
			joystickRect.x = (float)Screen.width * (positionX - size / 2.0f + displayOffsetX);
			joystickRect.y = (float)Screen.height * (positionY - screenHeight / 2.0f + displayOffsetY);
			joystickRect.width = (float)Screen.width * screenWidth;
			joystickRect.height = (float)Screen.height * screenHeight;
		}
		
		bool IsInside(Vector2 position) {
			if(position.x>joystickRect.x && position.x < joystickRect.xMax) {
				if(position.y>joystickRect.y && position.y < joystickRect.yMax)
					return true;
			}
			return false;
		}

		void OnGUI() {
			GUI.DrawTexture(joystickRect,texture);
		}
	}
}