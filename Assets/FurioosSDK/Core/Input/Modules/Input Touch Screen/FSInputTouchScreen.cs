using UnityEngine;
using System.Collections;

namespace FurioosSDK.Core {
	public class FSInputTouchScreen : FSInputController {

		public override void UpdateTouches(FSInputManager oic){
			for(int i = 0 ; i < Input.touchCount; i++){
				UnityEngine.Touch touch = Input.GetTouch(i);
				FSInputTouch fingerTouch = oic.findOrCreateTouch(touch.fingerId);
				fingerTouch.IsActive = true;
				fingerTouch.UpdatePosition(touch.position.x,Screen.height - touch.position.y);
			}
		}

	}
}