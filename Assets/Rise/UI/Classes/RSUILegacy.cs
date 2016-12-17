using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

using Rise.Core;
using Rise.Features.MovingMode;
using Rise.Features.InteractionPoint;

namespace Rise.UI {

	[System.Serializable]
	public class SettingsPanel
	{
		public GameObject panel;
		public Toggle settingsButton;
		public Toggle[] renderModes;
		public Text[] renderModesNotes;
		public Toggle[] qualityLevels;
		public Text[] qualityLevelsNotes;
		public Toggle gyroscope;
		public Text[] gyroscopeNotes;
		public Toggle gps;
		public Text[] gpsNotes;
	}
	
	[System.Serializable]
	public class HelpPanel
	{
		public GameObject panel;
		public Toggle helpButton;
		public Sprite[] helps;
		
		public Image image;
		public Button leftArrow;
		public Button rightArrow;
		
		[HideInInspector]
		public int index;
	}
	
	[System.Serializable]
	public class MovingModesPanel
	{
		public GameObject panel;
		public Toggle[] cameras;
		public GameObject automaticPanel;
		public List<Toggle> automaticCameras;
		public GameObject automaticTogglePrefab;
		public bool enableAnimations = true;
		
		[HideInInspector]
		public float xOffset = 0;
	}
	
	[System.Serializable]
	public class ToolsPanel
	{
		public GameObject panel;
		public Toggle teleportation;
		public Toggle dayNight;
	}
	
	[System.Serializable]
	public class ToggleColor {
		public Color offToggle = Color.white;
		public Color onToggle = Color.black;
		public Color labelToggle = Color.black;
		public Color enableToggle = Color.white;
		public Color disableToggle = new Color(0.0f, 0.0f, 0.0f, 0.05f);
	}
	
	[System.Serializable]
	public class PanelColor {
		public Color panelColor = Color.white;
	}
	
	[System.Serializable]
	public class InteractionPointColor {
		public Color pointColor = Color.white;
		public Color ringColor = Color.white;
	}

	[System.Serializable]
	public class RSUILegacy : RSUniqueBehaviour {
		public Canvas canvas;

		public RectTransform cursor;

		public MovingModesPanel movingModesPanel;
		public SettingsPanel settingsPanel;
		public ToolsPanel toolsPanel;
		public HelpPanel helpPanel;

		public GameObject interactionPoints;
		public GameObject interactionPointUiPrefab;
		public bool goToDefaultInteractionPoint = false;

		public ToggleColor toggleColor;
		public PanelColor panelColor;
		public InteractionPointColor interactionPointColor;

		public bool goToGlobalCameraAtStart = false;

		public float animationSpeed = 2;

		public Shader blurShader;

		private Dictionary<string, EventTrigger> savedDelegates = new Dictionary<string, EventTrigger>();
		private Vector2 ratio;

		void Start() {
			if(!goToDefaultInteractionPoint && goToGlobalCameraAtStart)
				ToGlobal ();

			RSSceneManager.onRenderModeChange += RenderModeChange;
			RSSceneManager.onQualityLevelChange += QualityLevelChange;
			RSSceneManager.onDeviceOrientationTypeChange += DeviceOrientationTypeChange;
			RSSceneManager.onInteractionPointChange += UpdateMovingModesPanel;
			MovingModesManager.MovingModeChanged += MovingModeChange;
			MovingModesManager.MovingModeChanged += UpdateToolsPanel;

			Initialize ();
		}

		void OnDisable() {
			RSSceneManager.onRenderModeChange -= RenderModeChange;
			RSSceneManager.onQualityLevelChange -= QualityLevelChange;
			RSSceneManager.onDeviceOrientationTypeChange -= DeviceOrientationTypeChange;
			RSSceneManager.onInteractionPointChange -= UpdateMovingModesPanel;

			if (MovingModesManager != null) {
				MovingModesManager.MovingModeChanged -= MovingModeChange;
				MovingModesManager.MovingModeChanged -= UpdateToolsPanel;
			}
		}

		void Initialize() {
			cursor.gameObject.SetActive (false);

			if(!InitializeFeatures (settingsPanel.renderModesNotes, RSSceneManager.allowStereoscopic, RSSceneManager.canStereoscopic, 1, 0))
				DisableToggle(settingsPanel.renderModes[1]);
			if(!InitializeFeatures (settingsPanel.renderModesNotes, RSSceneManager.allowOculus, RSSceneManager.canOculus, 3, 2))
			    DisableToggle(settingsPanel.renderModes[2]);
			if(!InitializeFeatures (settingsPanel.renderModesNotes, RSSceneManager.allowCardboard, RSSceneManager.canCardboard, 5, 4))
				DisableToggle(settingsPanel.renderModes[3]);

			if(!InitializeFeatures (settingsPanel.gyroscopeNotes, RSSceneManager.allowGyroscope, RSSceneManager.canGyroscope, 0, 1))
				DisableToggle (settingsPanel.gyroscope);
			if(!InitializeFeatures (settingsPanel.gpsNotes, RSSceneManager.allowGPS, RSSceneManager.canGPS, 0, 1))
				DisableToggle (settingsPanel.gps);

			movingModesPanel.xOffset = movingModesPanel.cameras[1].GetComponent<RectTransform>().anchoredPosition.x 
				- movingModesPanel.cameras[0].GetComponent<RectTransform>().anchoredPosition.x;

			ratio.x = canvas.GetComponent<RectTransform>().sizeDelta.x / Screen.width;
			ratio.y = canvas.GetComponent<RectTransform>().sizeDelta.y / Screen.height;

			InitMovingModesPanel ();
			UpdateMovingModesPanel();
			UpdateToolsPanel ();
			
			for (int i = 0; i < movingModesPanel.cameras.Length; i++) {
				movingModesPanel.cameras[i].targetGraphic.color = toggleColor.offToggle;
				movingModesPanel.cameras[i].graphic.color = toggleColor.onToggle;
			}

			for (int i = 0; i < movingModesPanel.automaticCameras.Count; i++) {
				movingModesPanel.automaticCameras[i].targetGraphic.color = toggleColor.offToggle;
				movingModesPanel.automaticCameras[i].graphic.color = toggleColor.onToggle;
			}
			
			toolsPanel.teleportation.targetGraphic.color = toggleColor.offToggle;
			toolsPanel.teleportation.graphic.color = toggleColor.onToggle;
			toolsPanel.dayNight.targetGraphic.color = toggleColor.offToggle;
			toolsPanel.dayNight.graphic.color = toggleColor.onToggle;
			
			settingsPanel.settingsButton.targetGraphic.color = toggleColor.offToggle;
			settingsPanel.settingsButton.graphic.color = toggleColor.onToggle;
			
			helpPanel.helpButton.targetGraphic.color = toggleColor.offToggle;
			helpPanel.helpButton.graphic.color = toggleColor.onToggle;

			settingsPanel.panel.GetComponent<Image> ().color = panelColor.panelColor;
			helpPanel.panel.GetComponent<Image> ().color = panelColor.panelColor;
		}

		private bool InitializeFeatures (Text[] notes, bool allowFeature, bool canFeature, int notAllowedNoteIndex, int cannotNoteIndex) {
			if (allowFeature) {
				if (!canFeature) {
					notes[cannotNoteIndex].gameObject.SetActive (true);

					return false;
				}
			} else {
				notes[notAllowedNoteIndex].gameObject.SetActive(true);

				return false;
			}

			return true;
		}

		void Update() {
			if (InputManager.GetAxisRaw ("To Orbital") > 0) {
				ToOrbital ();
			} else if (InputManager.GetAxisRaw ("To FPS") > 0) {
				ToFPS ();
			} else if (InputManager.GetAxisRaw ("Quit") > 0) {
				QuitApplication ();
			} else if (InputManager.GetKeyUp ("Display Mode")) {
				switch (OutputModesManager.Active.id) {
					case "2D":
						SceneManager.SetRenderMode (RenderModes.Stereoscopic);
						break;
					case "3D Splitted":
						SceneManager.SetRenderMode (RenderModes.Oculus);
						break;
					case "3D Oculus":
						SceneManager.SetRenderMode (RenderModes.TwoD);
						break;
				}
			}

			cursor.anchoredPosition = new Vector3(Input.mousePosition.x * ratio.x + 1, (1- (Screen.height - Input.mousePosition.y)) * ratio.y - 1);
		}

		#region Tools

		public Vector2 ScreenToCanvas(Vector2 position) {
			RectTransform rectTransform = canvas.GetComponent<RectTransform> ();

			Vector2 ratio = new Vector2(
				rectTransform.sizeDelta.x / Screen.width,
				rectTransform.sizeDelta.y / Screen.height
			);

			return new Vector2(position.x * ratio.x, position.y * ratio.y - rectTransform.sizeDelta.y);
		}

		#endregion

		#region Blur effect

		public void BlurEffect(Toggle toggle) {
			/*if (toggle.isOn) {
				 be = MovingModesManager.ActiveCameraGameObject.AddComponent<BlurEffect> ();
				be.iterations = 12;
				be.blurSpread = 1;
				be.blurShader = blurShader;
				be.enabled = true;
			} else {
				Destroy(MovingModesManager.ActiveCameraGameObject.GetComponent<BlurEffect>());
			}*/
		}

		#endregion

		#region Toggle Tools

		public void EnableToggle(Toggle toggle) {
			if (toggle.enabled)
				return;
			
			toggle.enabled = true;
			toggle.targetGraphic.color = toggleColor.offToggle;
			toggle.interactable = true;

			toggle.graphic.gameObject.SetActive (true);

			if (savedDelegates.ContainsKey(toggle.name) && toggle.GetComponent<EventTrigger>() == null) {
				EventTrigger trigger = savedDelegates[toggle.name];

				System.Type type = trigger.GetType();
				Component copy = toggle.gameObject.AddComponent(type);

				System.Reflection.FieldInfo[] fields = type.GetFields(); 
				foreach (System.Reflection.FieldInfo field in fields)
				{
					field.SetValue(copy, field.GetValue(trigger));
				}
			}

		}

		public void DisableToggle(Toggle toggle) {
			if (!toggle.enabled)
				return;

			toggle.enabled = false;
			toggle.targetGraphic.color = toggleColor.disableToggle;
			toggle.interactable = false;
			
			toggle.graphic.gameObject.SetActive (false);

			EventTrigger trigger = toggle.GetComponent<EventTrigger> ();

			if (trigger != null) {
				if(savedDelegates.ContainsKey(toggle.name)) {
					savedDelegates.Remove(toggle.name);
				}

				savedDelegates.Add (toggle.name, trigger);
				Destroy (trigger);
			}

		}

		public void HidToggle(Toggle toggle) {
			LeanTween.cancel (toggle.gameObject);

			LeanTween.value (
				toggle.gameObject,
				toggle.colors.normalColor.a,
				0.0f,
				animationSpeed
			).setOnUpdate (
				(Action<float>)(newVal => {
					ColorBlock colorBlock = toggle.colors;
				
					Color color = colorBlock.normalColor;
					color.a = (float)newVal;
				
					colorBlock.normalColor = color;
				
					toggle.colors = colorBlock;
				})
			).setOnComplete (
				() => {
					toggle.interactable = false;
					toggle.gameObject.SetActive (false);
				}
			);
		}

		public void ShowToggle(Toggle toggle) {
			LeanTween.cancel (toggle.gameObject);

			if (toggle.gameObject.activeSelf)
				return;

			LeanTween.value (
				toggle.gameObject,
				toggle.colors.normalColor.a,
				1.0f,
				animationSpeed
			).setOnCompleteOnStart(
				true
			).setOnComplete(
				() => {
					toggle.interactable = true;
					toggle.gameObject.SetActive(true);
				}
			).setOnUpdate (
				(Action<float>)(newVal => {
					ColorBlock colorBlock = toggle.colors;
				
					Color color = colorBlock.normalColor;
					color.a = (float)newVal;
				
					colorBlock.normalColor = color;
				
					toggle.colors = colorBlock;
				})
			);

		}

		public void MoveToggleTo(Toggle toggle, Vector2 destination) {
			LeanTween.value (
				gameObject,
				toggle.GetComponent<RectTransform> ().anchoredPosition,
				destination,
				animationSpeed
			).setOnUpdate (
				(Action<Vector2>)(newVal => {
					RectTransform rectTransform = toggle.GetComponent<RectTransform> ();
				
					rectTransform.anchoredPosition = (Vector2)newVal;
				})
			);
		}

		#endregion

		#region MovingMode
		public void ToGlobal() {
			DisableMovingModesPanel ();

			GameObject activeMovingModeCamera = MovingModesManager.ActiveCameraGameObject;
			
			MovingModesManager.ActivateMovingMode ("TransitionCamera");

			MovingModeTransitionCamera transitionCamera = (MovingModeTransitionCamera)MovingModesManager.Active;
			
			transitionCamera.ResetTransform (activeMovingModeCamera.transform);
			
			transitionCamera.PreActivate ();

			MovingModeOrbitalCamera globalCamera = null;
			foreach (RSMovingMode movingMode in MovingModesManager.GetAvailableMovingModes()) {
				if(movingMode.GetType() == typeof(MovingModeGlobalCamera)) {
					globalCamera = (MovingModeOrbitalCamera)movingMode;
					break;
				}
			}

			globalCamera.PreActivate ();
			transitionCamera.GoTo (globalCamera);
			transitionCamera.onTransitionCompleted += EnableGlobal;

			SceneManager.InteractionPoint = null;
		}

		private void EnableGlobal() {
			MovingModeTransitionCamera transitionCamera = null;

			if (MovingModesManager.Active.GetType () == typeof(MovingModeTransitionCamera)) {
				transitionCamera = (MovingModeTransitionCamera)MovingModesManager.Active;
			}

			MovingModesManager.ActivateMovingMode (RSSceneManager.GetInstance<MovingModeGlobalCamera>().id);

			if(transitionCamera != null)
				transitionCamera.onTransitionCompleted -= EnableGlobal;
		}

		public void ToOrbital() {
			DisableMovingModesPanel ();

			if (SceneManager.InteractionPoint == null) {
				MovingModesManager.ActivateMovingMode ("OrbitalCamera");
				return;
			}

			if (!SceneManager.InteractionPoint.allowOrbitalCamera)
				return;

			SceneManager.InteractionPoint.ToOrbital ();
		}

		public void ToFPS() {
			DisableMovingModesPanel ();

			if (SceneManager.InteractionPoint == null) {
				MovingModesManager.ActivateMovingMode ("FPS");
				return;
			}
			
			if (!SceneManager.InteractionPoint.allowFPSCamera)
				return;
			
			SceneManager.InteractionPoint.ToFPS ();
		}

		public void ToPanoramic() {
			DisableMovingModesPanel ();

			if (SceneManager.InteractionPoint == null) {
				return;
			}
			
			if (!SceneManager.InteractionPoint.allowPanoramicCamera)
				return;
			
			SceneManager.InteractionPoint.ToPanoramic ();
		}

		public void ToAutomatic(MovingModeAnimatedCamera camera) {
			MovingModesManager.ActivateMovingMode (camera.id);
		}
		
		private void EnableAutomatic() {

		}

		public void ToMap() {
			MovingModeTransitionCamera transitionCamera = RSSceneManager.GetInstance<MovingModeTransitionCamera> ();
			MovingModeMapCamera mapCamera = RSSceneManager.GetInstance<MovingModeMapCamera> ();

			GameObject activeMovingModeCamera = MovingModesManager.ActiveCameraGameObject;
			
			MovingModesManager.ActivateMovingMode ("TransitionCamera");
			
			transitionCamera.ResetTransform (activeMovingModeCamera.transform);
			
			transitionCamera.GoTo (mapCamera);
			transitionCamera.onTransitionCompleted += TransitionCompleted;
		}

		public void TransitionCompleted() {
			MovingModeTransitionCamera transitionCamera = RSSceneManager.GetInstance<MovingModeTransitionCamera> ();
			MovingModesManager.ActivateMovingMode ("MapCamera");

			transitionCamera.onTransitionCompleted -= TransitionCompleted;
		}

		public void ResetMovingModesPanel() {
			foreach (Toggle movingModeToggle in movingModesPanel.cameras) {
				movingModeToggle.isOn = false;
			}
		}

		public void DisableMovingModesPanel() {
			foreach (Toggle movingModeToggle in movingModesPanel.cameras) {
				movingModeToggle.interactable = false;
			}
		}

		public void EnableMovingModesPanel() {
			foreach (Toggle movingModeToggle in movingModesPanel.cameras) {
				if(movingModeToggle != null)
					movingModeToggle.interactable = true;
			}
		}

		public void ShowHidMovingModesPanel() {
			if (movingModesPanel.panel.activeSelf) {
				movingModesPanel.panel.SetActive (false);
			} else {
				movingModesPanel.panel.SetActive(true);
			}
		}

		public void DisableAllAutomaticCameraToggle(int excludeIndex) {
			for(int i = 0; i < movingModesPanel.automaticCameras.Count; i++) {
				if(excludeIndex == i) 
					continue;

				movingModesPanel.automaticCameras[i].isOn = false;
			}
		}

		public void InitMovingModesPanel() {
			List<MovingModeAnimatedCamera> cameras = RSSceneManager.GetAllInstances<MovingModeAnimatedCamera> ();

			if (cameras.Count == 0) {
				HidToggle(movingModesPanel.cameras[4]);
				return;
			}

			ShowToggle (movingModesPanel.cameras[4]);

			RectTransform automaticPanelTransform = movingModesPanel.automaticPanel.GetComponent<RectTransform> ();
			Vector2 sizeDelta = automaticPanelTransform.sizeDelta;

			sizeDelta.y = movingModesPanel.cameras [0].GetComponent<RectTransform> ().sizeDelta.y * cameras.Count;

			automaticPanelTransform.sizeDelta = sizeDelta;

			for(int i = 0; i < cameras.Count; i++) {
				movingModesPanel.automaticCameras.Add(((GameObject)Instantiate(movingModesPanel.automaticTogglePrefab, Vector3.zero, Quaternion.identity)).GetComponent<Toggle>());
				movingModesPanel.automaticCameras[i].transform.SetParent(movingModesPanel.automaticPanel.transform, false);

				RectTransform rectTransform = movingModesPanel.automaticCameras[i].GetComponent<RectTransform>();
				rectTransform.anchoredPosition = new Vector2(0, rectTransform.sizeDelta.y * i);

				Text label = movingModesPanel.automaticCameras[i].GetComponentInChildren<Text>();
				label.text = cameras[i].id;

				EventTrigger trigger = movingModesPanel.automaticCameras[i].GetComponent<EventTrigger>();

				EventTrigger.TriggerEvent clickTrigger = new EventTrigger.TriggerEvent();
				clickTrigger = AddListenerAutomaticCamera(clickTrigger, new object[] {i, cameras[i]});

				EventTrigger.Entry entry = new EventTrigger.Entry() { callback = clickTrigger, eventID = EventTriggerType.PointerClick };

				trigger.triggers.Add(entry);
			}

			movingModesPanel.automaticPanel.SetActive (false);
		}

		private EventTrigger.TriggerEvent AddListenerAutomaticCamera(EventTrigger.TriggerEvent clickTrigger, object[] args) {
			int index = (int)args [0];
			MovingModeAnimatedCamera animatedCamera = (MovingModeAnimatedCamera)args [1];

			clickTrigger.AddListener((eventData) => {
				for(int i = 0; i < movingModesPanel.cameras.Length; i++) {
					movingModesPanel.cameras[i].isOn = false;
				}
				
				DisableAllAutomaticCameraToggle(index);

				ToAutomatic(animatedCamera);

				movingModesPanel.cameras[4].isOn = false;
			});

			return clickTrigger;
		}

		public void UpdateMovingModesPanel(RSInteractionPoint interactionPoint = null) {
			movingModesPanel.panel.SetActive (true);

			float xOrigin = movingModesPanel.cameras [0].GetComponent<RectTransform> ().sizeDelta.x / 2;
			float xIndex = 1;
			float yPosition = movingModesPanel.cameras[0].transform.position.y;

			if (interactionPoint != null) {
				if (interactionPoint.allowOrbitalCamera) {
					ShowToggle (movingModesPanel.cameras [1]);

					if(movingModesPanel.enableAnimations)
						MoveToggleTo (movingModesPanel.cameras [1], new Vector2 (xOrigin + movingModesPanel.xOffset * xIndex, yPosition));
					xIndex++;
				} else {
					HidToggle (movingModesPanel.cameras [1]);
				}

				if (interactionPoint.allowFPSCamera) {
					ShowToggle (movingModesPanel.cameras [2]);
					
					if(movingModesPanel.enableAnimations)
						MoveToggleTo (movingModesPanel.cameras [2], new Vector2 (xOrigin + movingModesPanel.xOffset * xIndex, yPosition));
					
					if(movingModesPanel.enableAnimations)
						MoveToggleTo (toolsPanel.teleportation, new Vector2 (xOrigin + movingModesPanel.xOffset * xIndex, yPosition));
					xIndex++;
				} else {
					HidToggle (movingModesPanel.cameras [2]);
				}

				if (interactionPoint.allowPanoramicCamera) {
					ShowToggle (movingModesPanel.cameras [3]);
					
					if(movingModesPanel.enableAnimations)
						MoveToggleTo (movingModesPanel.cameras [3], new Vector2 (xOrigin + movingModesPanel.xOffset * xIndex, yPosition));
					xIndex++;
				} else {
					HidToggle (movingModesPanel.cameras [3]);
				}
			} else {
				HidToggle(movingModesPanel.cameras[1]);
				HidToggle(movingModesPanel.cameras[2]);
				HidToggle(movingModesPanel.cameras[3]);
			}
			
			if(movingModesPanel.enableAnimations)
				MoveToggleTo(movingModesPanel.cameras[4], new Vector2(xOrigin + movingModesPanel.xOffset * xIndex, yPosition));
			xIndex++;
		}

		public void MovingModeChange(RSMovingMode movingMode) {
			EnableMovingModesPanel ();

			if (movingMode == null)
				return;

			Type movingModeType = movingMode.GetType ();

			if (movingModeType == typeof(MovingModeTransitionCamera)) {
				for (int i = 0; i < movingModesPanel.cameras.Length; i++) {
					DisableToggle (movingModesPanel.cameras [i]);
				}
				DisableToggle (settingsPanel.settingsButton);
			} else {
				for (int i = 0; i < movingModesPanel.cameras.Length; i++) {
					EnableToggle (movingModesPanel.cameras [i]);
				}
				EnableToggle (settingsPanel.settingsButton);
			}

			if (movingModeType == typeof(MovingModeMapCamera)) {
				ShowAllInteractionPoints ();
				return;
			} else if (movingModeType == typeof(MovingModeTransitionCamera)) {
				HidAllInteractionPoints ();
				return;
			} else if (movingModeType == typeof(MovingModeFPS)) {
				HidAllInteractionPoints ();
			} else if (movingModeType == typeof(MovingModeAnimatedCamera)) {
				HidAllInteractionPoints ();
			} else if (movingModeType == typeof(MovingModePanoramicCamera)) {
				HidAllInteractionPoints ();
			} else {
				ShowAllInteractionPoints();
			}

			float movingModeIndex = -1;

			if (movingModeType == typeof(MovingModeGlobalCamera)) {
				movingModeIndex = 0;
			} else if (movingModeType == typeof(MovingModeOrbitalCamera)) {
				movingModeIndex = 1;
			} else if (movingModeType == typeof(MovingModeFPS)) {
				movingModeIndex = 2;
			} else if (movingModeType == typeof(MovingModePanoramicCamera)) {
				movingModeIndex = 3;
			} else if (movingModeType == typeof(MovingModeAnimatedCamera)) {
				movingModeIndex = 4;
			}

			for (int i = 0; i < movingModesPanel.cameras.Length; i++) {
				if(i == movingModeIndex) {
					movingModesPanel.cameras[i].isOn = true;
					continue;
				}
				movingModesPanel.cameras[i].isOn = false;
			}
		}

		#endregion

		#region Tools 

		public void UpdateToolsPanel(RSMovingMode movingMode = null) {
			if (movingMode == null)
				return;

			Type movingModeType = movingMode.GetType ();

			if (movingModeType == typeof(MovingModeTransitionCamera))
				return;

			if (movingModeType != typeof(MovingModeFPS) && movingModeType != typeof(MovingModeMapCamera)) {
				toolsPanel.teleportation.isOn = false;
				HidToggle(toolsPanel.teleportation);

				return;
			}

			if (movingModeType == typeof(MovingModeFPS)) {
				toolsPanel.teleportation.isOn = false;
			}

			ShowToggle (toolsPanel.teleportation);
		}

		public void ShowHidToolsPanel() {
			if (toolsPanel.panel.activeSelf) {
				toolsPanel.panel.SetActive (false);
			} else {
				toolsPanel.panel.SetActive(true);
			}
		}
		
		#endregion

		#region Options

		#region Quality

		public void SetQualityLevelUI(string level) {
			switch (level) {
				case "Low":
					SceneManager.SetQualityLevel(QualityLevels.Low);
				break;
				case "Medium":
					SceneManager.SetQualityLevel(QualityLevels.Medium);
				break;
				case "High":
					SceneManager.SetQualityLevel(QualityLevels.High);
				break;
				case "Extra":
					SceneManager.SetQualityLevel(QualityLevels.Extra);
				break;
				default:
				break;
			}
		}

		public void QualityLevelChange(QualityLevels level) {
			EnableQualityToggles ();
			OffQualityToggles ();

			settingsPanel.qualityLevelsNotes[0].gameObject.SetActive(true);
			settingsPanel.qualityLevelsNotes[1].gameObject.SetActive(false);
			
			switch (level) {
				case QualityLevels.Low:
					settingsPanel.qualityLevels[0].isOn = true;
				break;
				case QualityLevels.Medium:
					settingsPanel.qualityLevels[1].isOn = true;
				break;
				case QualityLevels.High:
					settingsPanel.qualityLevels[2].isOn = true;
				break;
				case QualityLevels.Extra:
					settingsPanel.qualityLevels[3].isOn = true;
				break;
				default:
					DisableQualityToggles();
					settingsPanel.qualityLevelsNotes[0].gameObject.SetActive(false);
					settingsPanel.qualityLevelsNotes[1].gameObject.SetActive(true);
				break;
			}
		}

		private void OffQualityToggles() {
			foreach (Toggle qualityLevel in settingsPanel.qualityLevels) {
				qualityLevel.isOn = false;
			}
		}

		private void DisableQualityToggles() {
			foreach (Toggle qualityLevel in settingsPanel.qualityLevels) {
				DisableToggle(qualityLevel);
			}
		}

		private void EnableQualityToggles() {
			foreach (Toggle qualityLevel in settingsPanel.qualityLevels) {
				EnableToggle(qualityLevel);
			}
		}

		#endregion

		#region Render mode

		public void SetRenderModeUI(string mode) {
			switch (mode) {
				case "2D":
					SceneManager.SetRenderMode(RenderModes.TwoD);
				break;
				case "Stereoscopic":
					SceneManager.SetRenderMode(RenderModes.Stereoscopic);
				break;
				case "Oculus":
					SceneManager.SetRenderMode(RenderModes.Oculus);
				break;
				case "Cardboard":
					SceneManager.SetRenderMode(RenderModes.Cardboard);
				break;
			}
		}

		public void RenderModeChange(RenderModes mode) {
			OffRenderModesToggles ();

			switch (mode) {
				case RenderModes.TwoD:
					cursor.gameObject.SetActive(false);
					Cursor.visible = true;
					settingsPanel.renderModes[0].isOn = true;
					EnableQualityToggles();
				break;
				case RenderModes.Stereoscopic:
					cursor.gameObject.SetActive(true);
					Cursor.visible = false;
					settingsPanel.renderModes[1].isOn = true;
					EnableQualityToggles();
				break;
				case RenderModes.Oculus:
					cursor.gameObject.SetActive(true);
					Cursor.visible = false;
					settingsPanel.renderModes[2].isOn = true;
				break;
				case RenderModes.Cardboard:
					settingsPanel.renderModes[3].isOn = true;
				break;
			}
		}

		private void OffRenderModesToggles() {
			foreach (Toggle renderMode in settingsPanel.renderModes) {
				renderMode.isOn = false;
			}
		}
		
		private void DisableRenderModesToggles() {
			foreach (Toggle renderMode in settingsPanel.renderModes) {
				DisableToggle(renderMode);
			}
		}
		
		private void EnableRenderModesToggles() {
			foreach (Toggle renderMode in settingsPanel.renderModes) {
				EnableToggle(renderMode);
			}
		}
		
		#endregion

		#region AutomaticCamera

		public void EnableDisableAutomaticCamera() {

		}

		#endregion

		#region DeviceOrientation

		public void EnableDisableGyroscopeUI() {
			if (RSSceneManager.DeviceOrientationType == UseDeviceOrientationType.Gyroscope) {
				SceneManager.SetDeviceOrientation (UseDeviceOrientationType.None);
			} else {
				SceneManager.SetDeviceOrientation (UseDeviceOrientationType.Gyroscope);
			}
		}

		public void DeviceOrientationTypeChange(UseDeviceOrientationType deviceOrientationType) {
			if (deviceOrientationType == UseDeviceOrientationType.Gyroscope) {
				settingsPanel.gyroscope.isOn = true;
			} else {
				settingsPanel.gyroscope.isOn = false;
			}
		}

		#endregion

		#region Quit

		public void QuitApplication() {
			SceneManager.Quit ();
		}

		#endregion

		#region Interaction Point

		public GameObject AddInteractionPoint(RSInteractionPoint instance) {
			GameObject ip = (GameObject)Instantiate (interactionPointUiPrefab);
			ip.transform.SetParent(interactionPoints.transform, false);

			RectTransform rectTransform = ip.GetComponent<RectTransform> ();
			rectTransform.localPosition = Vector3.zero;
			rectTransform.localScale = Vector3.one;

			Button button = ip.GetComponent<Button> ();
			button.onClick.AddListener(() => { instance.GoTo(); });

			RectTransform[] ipElements = ip.GetComponentsInChildren<RectTransform> ();
			ipElements [1].GetComponent<Image> ().color = interactionPointColor.pointColor;
			ipElements [2].GetComponent<Image> ().color = interactionPointColor.ringColor;

			return ip;
		}

		public void ShowHidAllInteractionPoints() {
			if (MovingModesManager.Active.GetType () == typeof(MovingModeFPS)) {
				return;
			}

			if (interactionPoints.activeSelf) {
				HidAllInteractionPoints ();
			} else {
				ShowAllInteractionPoints();
			}
		}

		public void ShowAllInteractionPoints() {
			interactionPoints.SetActive(true);
		}

		public void HidAllInteractionPoints() {
			interactionPoints.SetActive (false);
		}

		#endregion

		#region Help

		public void NextHelp() {
			helpPanel.index++;

			helpPanel.image.sprite = helpPanel.helps [helpPanel.index];

			if (helpPanel.index + 1 == helpPanel.helps.Length) {
				helpPanel.rightArrow.gameObject.SetActive (false);
			} else {
				helpPanel.rightArrow.gameObject.SetActive (true);
				helpPanel.leftArrow.gameObject.SetActive (true);
			}
		}

		public void PreviousHelp() {
			helpPanel.index--;
			
			helpPanel.image.sprite = helpPanel.helps [helpPanel.index];
			
			if (helpPanel.index - 1 == -1) {
				helpPanel.leftArrow.gameObject.SetActive (false);
			} else {
				helpPanel.leftArrow.gameObject.SetActive (true);
				helpPanel.rightArrow.gameObject.SetActive (true);
			}
		}

		#endregion
		
		#endregion
	}
}