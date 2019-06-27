using UnityEngine;
using System.Collections;

namespace Rise.Core {
	public class RSInputTouchScreen : RSInputController {

		public override void UpdateTouches(RSInputManager oic){
			for(int i = 0 ; i < Input.touchCount; i++){
				UnityEngine.Touch touch = Input.GetTouch(i);
				RSInputTouch fingerTouch = oic.findOrCreateTouch(touch.fingerId);
				fingerTouch.IsActive = true;
				fingerTouch.UpdatePosition(touch.position.x,Screen.height - touch.position.y);
			}
		}

	}
}