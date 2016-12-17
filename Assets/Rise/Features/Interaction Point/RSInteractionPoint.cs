using UnityEngine;
using System.Collections;
using System;
using Rise.Core;
using Rise.Features.MovingMode;
using Rise.UI;

namespace Rise.Features.InteractionPoint {
	[System.Serializable]
	public class RSInteractionPoint : RSBehaviour {
		[System.Serializable]
		public struct SpawnPoint
		{
			public float pan;
			public float tilt;
			public float roll;

			public Transform spwan;
		}

		public bool isDefault = false;

		public bool allowOrbitalCamera;
		public bool allowPanoramicCamera;
		public bool allowFPSCamera;

		private bool enableFPSCamera;
		private bool enableOrbitalCamera;
		private bool enablePanoramicCamera;


		public MovingModeOrbitalCamera orbitalCamera;
		public MovingModePanoramicCamera panoramicCamera;
		[HideInInspector]public MovingModeFPS fpsCamera;
		public SpawnPoint spawnPoint;

		private GameObject interactionPointUi;
		public GameObject InteractionPointUi {
			get { return interactionPointUi; }
		}

		private RSUILegacy ui;
		private RSInteractionPointParameters interactionPointParameters;

		private MovingModeTransitionCamera transitionCamera;

		private float initialSize;
		private float minSize;

		private bool wasBehindCamera = false;

		public void Start() {
			ui = RSSceneManager.GetInstance<RSUILegacy> ();
			interactionPointParameters = RSSceneManager.GetInstance<RSInteractionPointParameters> ();
			transitionCamera = RSSceneManager.GetInstance<MovingModeTransitionCamera> ();
			fpsCamera = RSSceneManager.GetInstance<MovingModeFPS> ();

			interactionPointUi = ui.AddInteractionPoint (this);

			initialSize = interactionPointUi.GetComponent<RectTransform>().sizeDelta.x;
			minSize = initialSize / 2;

			if (allowFPSCamera) {
				fpsCamera = RSSceneManager.GetInstance<MovingModeFPS>();
			}

			RSSceneManager.onInteractionPointChange += InteractionPointHasChanged;

		}

		public void OnDisable() {
			RSSceneManager.onInteractionPointChange -= InteractionPointHasChanged;
		}

		public void Update() {
			if(SceneManager.InteractionPoint == this) return;

			Camera camera = MovingModesManager.ActiveCamera;
			Vector3 positionOnScreen = camera.WorldToScreenPoint (transform.position);

			if (positionOnScreen.z < 0) {
				Hid();
				wasBehindCamera = true;

				return;
			} else if (wasBehindCamera) {
				Show ();
				wasBehindCamera = false;
			}

			RectTransform rectTransform = interactionPointUi.GetComponent<RectTransform> ();
			rectTransform.anchoredPosition = ui.ScreenToCanvas(positionOnScreen);

			float distance = Vector3.Distance (camera.transform.position, transform.position);
			float distanceRatio = 1.0f - (distance / interactionPointParameters.maxDistanceVisible);


			rectTransform.sizeDelta = new Vector2(
				Mathf.Clamp(initialSize * distanceRatio, minSize, initialSize),
				Mathf.Clamp(initialSize * distanceRatio, minSize, initialSize)
			);

			if (isDefault) {
				GoTo ();
				isDefault = false;
			}
		}

		public void Show() {
			interactionPointUi.SetActive (true);
		}

		public void Hid() {
			interactionPointUi.SetActive (false);
		}

		public void GoTo() {
			ui.ResetMovingModesPanel ();

			GameObject activeMovingModeCamera = MovingModesManager.ActiveCameraGameObject;
			Type movingModeType = MovingModesManager.Active.GetType ();

			MovingModesManager.ActivateMovingMode ("TransitionCamera");

			transitionCamera.ResetTransform (activeMovingModeCamera.transform);

			if (allowOrbitalCamera && movingModeType != typeof(MovingModeMapCamera)) {
				enableOrbitalCamera = true;

				ToOrbital();
			} else if (allowFPSCamera) {
				enableFPSCamera = true;

				ToFPS();
			} else if (allowPanoramicCamera) {
				enablePanoramicCamera = true;

				ToPanoramic();
			}

			SceneManager.InteractionPoint = this;
		}

		public void ToGlobal() {

		}

		public void ToOrbital() {
			GameObject activeMovingModeCamera = MovingModesManager.ActiveCameraGameObject;

			MovingModesManager.ActivateMovingMode ("TransitionCamera");
			
			transitionCamera.ResetTransform (activeMovingModeCamera.transform);

			enableOrbitalCamera = true;

			orbitalCamera.PreActivate ();

			transitionCamera.GoTo (orbitalCamera);
			transitionCamera.onTransitionCompleted += TransitionCompleted;
		}
		
		public void ToPanoramic() {
			GameObject activeMovingModeCamera = MovingModesManager.ActiveCameraGameObject;
			
			MovingModesManager.ActivateMovingMode ("TransitionCamera");
			
			transitionCamera.ResetTransform (activeMovingModeCamera.transform);
			
			enablePanoramicCamera = true;
			
			panoramicCamera.PreActivate ();
			
			transitionCamera.GoTo (panoramicCamera);
			transitionCamera.onTransitionCompleted += TransitionCompleted;
		}

		public void ToFPS() {
			GameObject activeMovingModeCamera = MovingModesManager.ActiveCameraGameObject;
			
			MovingModesManager.ActivateMovingMode ("TransitionCamera");
			
			transitionCamera.ResetTransform (activeMovingModeCamera.transform);
			
			enableFPSCamera = true;

			fpsCamera.transform.position = spawnPoint.spwan.transform.position;

			fpsCamera.startPan = spawnPoint.pan;
			fpsCamera.startTilt = spawnPoint.tilt;
			fpsCamera.startRoll = spawnPoint.roll;

			fpsCamera.Pan = spawnPoint.pan;
			fpsCamera.Tilt = spawnPoint.tilt;
			fpsCamera.Roll = spawnPoint.roll;

			fpsCamera.PreActivate ();
		
			transitionCamera.GoTo (fpsCamera);
			transitionCamera.onTransitionCompleted += TransitionCompleted;
		}

		public void TransitionCompleted() {
			if (enableOrbitalCamera) {
				MovingModesManager.ActivateMovingMode(orbitalCamera.id);
				enableOrbitalCamera = false;
			} else if (enablePanoramicCamera) {
				MovingModesManager.ActivateMovingMode(panoramicCamera.id);
				enablePanoramicCamera = false;
			} else if (enableFPSCamera) {
				MovingModesManager.ActivateMovingMode(fpsCamera.id);
				enableFPSCamera = false;
			} 

			transitionCamera.onTransitionCompleted -= TransitionCompleted;
		}

		public void InteractionPointHasChanged(RSInteractionPoint interactionPoint) {
			if (interactionPoint == this) {
				Hid ();
			} else {
				Show();
			}
		}
	}
}