using UnityEngine;
using System.Collections;
using System;
using Rise.Core;

namespace Rise.Features.MovingMode {
	[System.Serializable]
	[RequireComponent (typeof(Camera))]
	public class Animated : RSMovingMode {
		
		public GameObject animatedGameObject;
		public string clipName = "Take 001";
		public float startTime = 0;
		public float endTime = 0;
		public float animationSpeed = 1.0f;
		public WrapMode wrapMode = WrapMode.Default;
		public bool playOnActivate = true;
		
		void OnGUI () {
			if(showDebugInfo){
				GUI.Label (new Rect (100, 100, 300, 100), string.Format("Animated Camera {0}\n",id));
			}
		}
		
		public override void Awake(){
			if(endTime == 0 && animatedGameObject!=null){
				endTime = animatedGameObject.GetComponent<Animation>()[clipName].length;
			}
			base.Awake();
			
		}
		
		public void Update() {
			LastActivityTime = Time.time;
			if(animatedGameObject.GetComponent<Animation>()[clipName].speed != 0) {
				
			}
		}
		
		public void Play() {
			if(animatedGameObject!=null){
				AnimationState animationState = animatedGameObject.GetComponent<Animation>()[clipName];
				animationState.enabled = true;
				animationState.wrapMode = wrapMode;
				if(animationState.time<startTime)animationState.time = startTime;
				animatedGameObject.GetComponent<Animation>().Blend(clipName, 0.5f, 0.0f);
				animationState.speed = animationSpeed;
			}
		}
		
		public void Stop() {
			if(animatedGameObject!=null){
				animatedGameObject.GetComponent<Animation>().Stop(clipName);
			}
		}
		
		
		public void Pause(){
			if(animatedGameObject!=null){
				AnimationState animationState = animatedGameObject.GetComponent<Animation>()[clipName];
				animationState.speed = 0.0f;
			}
			
		}
		
		public void Goto(float time) {
			if(animatedGameObject!=null){
				AnimationState animationState = animatedGameObject.GetComponent<Animation>()[clipName];
				animationState.enabled = true;
				animationState.time = startTime + time * animationSpeed;
				animatedGameObject.GetComponent<Animation>().Sample();
			
			}
		}
		
		public float getCurrentTime() {
			return (animatedGameObject.GetComponent<Animation>()[clipName].time- startTime) / animationSpeed ;
		}
		
		public float getDuration(){
			return  (animationSpeed !=0 ? (endTime - startTime) / animationSpeed : 0 );
		}
		
		public override void Activate ()
		{
			base.Activate ();
			transform.gameObject.SetActive(true);

			if(playOnActivate) {
				Play();
			}
		}
		
		public override void Desactivate ()
		{
			Stop();
			base.Desactivate();
			transform.gameObject.SetActive(false);
		}
	}
}