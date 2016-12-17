using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Rise.Features.InteractionPoint {
	[CustomEditor(typeof(RSInteractionPoint))]
	public class RSInteractionPointEditor : Editor {
		public void Awake() {
			EditorApplication.playmodeStateChanged += PlayModeChange;
		}

		private static PanTiltRollDistance orbitalCameraData = new PanTiltRollDistance ();
		private static PanTiltRollDistance fpsCameraData = new PanTiltRollDistance();
		private static PanTiltRollDistance panoramicCameraData = new PanTiltRollDistance();

		private bool orbitalCameraFoldout;
		private bool fpsCameraFoldout;
		private bool panoramicCameraFoldout;

		private bool isInEditSpawnPointMode = false;

		private RSInteractionPoint interactionPoint;

		public override void OnInspectorGUI() {
			interactionPoint = (RSInteractionPoint)target;

			interactionPoint.isDefault = EditorGUILayout.Toggle ("Go at start", interactionPoint.isDefault);

			if (interactionPoint.isDefault) {
				foreach(RSInteractionPoint ip in Resources.FindObjectsOfTypeAll(typeof(RSInteractionPoint))) {
					if(ip == interactionPoint)
						continue;

					ip.isDefault = false;
				}
			}
			
			interactionPoint.allowOrbitalCamera = EditorGUILayout.Toggle ("Allow Oribtal Camera", interactionPoint.allowOrbitalCamera);
			interactionPoint.allowFPSCamera = EditorGUILayout.Toggle ("Allow FPS Camera", interactionPoint.allowFPSCamera);
			interactionPoint.allowPanoramicCamera = EditorGUILayout.Toggle ("Allow Panoramic Camera", interactionPoint.allowPanoramicCamera);

			if(interactionPoint.allowOrbitalCamera)
				orbitalCameraFoldout = EditorGUILayout.Foldout (orbitalCameraFoldout, "Orbital Camera configuration");

			if (orbitalCameraFoldout) {
				EditorGUI.indentLevel = 1;
				if(interactionPoint.orbitalCamera.enabled && EditorApplication.isPlaying) {
					if (GUILayout.Button ("Set as start position")) {
						orbitalCameraData.Pan = interactionPoint.orbitalCamera.Pan;
						orbitalCameraData.Tilt = interactionPoint.orbitalCamera.Tilt;
						orbitalCameraData.Distance = interactionPoint.orbitalCamera.Distance;
					}

					if(GUILayout.Button ("Set as min distance")) {
						orbitalCameraData.MinDistance = interactionPoint.orbitalCamera.Distance;
					}

					if(GUILayout.Button("Set as max distance")) {
						orbitalCameraData.MaxDistance = interactionPoint.orbitalCamera.Distance;
					}
				}
				else {
					EditorGUILayout.LabelField("For editing camera configuration, you need to go in play mode and enable this camera");
				}
			}
			
			EditorGUI.indentLevel = 0;

			if(interactionPoint.allowFPSCamera)
				fpsCameraFoldout = EditorGUILayout.Foldout (fpsCameraFoldout, "FPS Camera configuration");

			if (fpsCameraFoldout) {
				EditorGUI.indentLevel = 1;
				if(EditorApplication.isPlaying) {
					if (GUILayout.Button ("Set as start position")) {
						fpsCameraData.Pan = interactionPoint.fpsCamera.Pan;
						fpsCameraData.Tilt = interactionPoint.fpsCamera.Tilt;
						fpsCameraData.Roll = interactionPoint.fpsCamera.Roll;
					}
				}
				else {
					EditorGUILayout.LabelField("For editing camera configuration, you need to go in play mode and enable this camera");
				}

				if(!isInEditSpawnPointMode) {
					if(GUILayout.Button("Set spawn point")) {
						isInEditSpawnPointMode = true;
						ActiveEditorTracker.sharedTracker.isLocked = true;
						interactionPoint.spawnPoint.spwan.gameObject.SetActive(true);
					}
				}
				else {
					if(GUILayout.Button("Save spawn Point")) {
						isInEditSpawnPointMode = false;
						ActiveEditorTracker.sharedTracker.isLocked = false;
						interactionPoint.spawnPoint.spwan.gameObject.SetActive(false);
					}
				}
			}
			
			EditorGUI.indentLevel = 0;

			if(interactionPoint.allowPanoramicCamera)
				panoramicCameraFoldout = EditorGUILayout.Foldout (panoramicCameraFoldout, "Panoramic Camera configuration");

			if (panoramicCameraFoldout) {
				EditorGUI.indentLevel = 1;
				if(interactionPoint.panoramicCamera.enabled && EditorApplication.isPlaying) {
					if(GUILayout.Button("Set  as start position")) {
						panoramicCameraData.Pan = interactionPoint.panoramicCamera.Pan;
						panoramicCameraData.Tilt = interactionPoint.panoramicCamera.Tilt;
						panoramicCameraData.Roll = interactionPoint.panoramicCamera.Roll;
					}
				}
				else {
					EditorGUILayout.LabelField("For editing camera configuration, you need to go in play mode and enable this camera");
				}
			}
			
			EditorGUI.indentLevel = 0;
		}

		public void OnSceneGUI() {
			if (isInEditSpawnPointMode) {
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

				Vector2 mousePosition = Event.current.mousePosition;
				mousePosition.y = Screen.height - mousePosition.y;

				Ray ray = Camera.current.ScreenPointToRay(mousePosition);
				RaycastHit hit;

				if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {
					if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
						Vector3 position = hit.point;
						position.y += 0.1f;
						interactionPoint.spawnPoint.spwan.position = position;
					}
				}
			}
		}

		private void SaveCamerasConfiguration() {
			if (orbitalCameraData.HasBeenEdited) {
				if(orbitalCameraData.StartPositionHasBeenEdited) {
					interactionPoint.orbitalCamera.startPan = orbitalCameraData.Pan;
					interactionPoint.orbitalCamera.startTilt = orbitalCameraData.Tilt;
					interactionPoint.orbitalCamera.startDistance = orbitalCameraData.Distance;
				}

				if(orbitalCameraData.MinDistance != -1)
					interactionPoint.orbitalCamera.minDistance = orbitalCameraData.MinDistance;

				if(orbitalCameraData.MaxDistance != -1)
					interactionPoint.orbitalCamera.maxDistance = orbitalCameraData.MaxDistance;

			}

			if (fpsCameraData.HasBeenEdited) {
				interactionPoint.spawnPoint.pan = fpsCameraData.Pan;
				interactionPoint.spawnPoint.tilt = fpsCameraData.Tilt;
				interactionPoint.spawnPoint.roll = fpsCameraData.Roll;
			}

			if (panoramicCameraData.HasBeenEdited) {
				interactionPoint.panoramicCamera.startPan = panoramicCameraData.Pan;
				interactionPoint.panoramicCamera.startTilt = panoramicCameraData.Tilt;
				interactionPoint.panoramicCamera.startRoll = panoramicCameraData.Roll;
			}
		}

		public void PlayModeChange() {
			if (!EditorApplication.isPlaying) {
				SaveCamerasConfiguration();
			}
		}
	}
}