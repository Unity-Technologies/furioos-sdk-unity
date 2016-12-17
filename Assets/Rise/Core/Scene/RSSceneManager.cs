#pragma warning disable 0414
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Rise.Features.InteractionPoint;
using Rise.Features.Loader;

namespace Rise.Core {
	public enum RenderModes {
		None,
		TwoD,
		Stereoscopic,
		Oculus,
		Cardboard
	};

	public enum QualityLevels {
		None,
		Low,
		Medium,
		High,
		Extra,
		Oculus,
		Cardboard
	};

	[System.Serializable]
	public class RSSceneManager : RSUniqueBehaviour 
	{
		#region Features permission

		public static bool allowStereoscopic = false;
		public static bool allowOculus = true;
		public static bool allowCardboard = true;

		public static bool allowGyroscope = false;
		public static bool allowGPS = false;
		
		#endregion

		#region Features compatibility

		public static bool canStereoscopic = true;
		public static bool canOculus = true;
		public static bool canCardboard = true;

		public static bool canGyroscope = true;
		public static bool canGPS = true;

		#endregion

		private static Dictionary<string,RSBehaviour> idPluginsDictionary = new Dictionary<string,RSBehaviour>();
		private static RSSceneManager sceneManager = null;
		private static int uniqueIndex = 0;

		private static RenderModes renderMode = RenderModes.None;
		public static RenderModes RenderMode {
			set {
				renderMode = value;

				if(onRenderModeChange != null)
					onRenderModeChange(renderMode);
			}
			get {
				return renderMode;
			}
		}
		public delegate void RenderModeHasChanged(RenderModes mode);
		public static event RenderModeHasChanged onRenderModeChange;

		private static QualityLevels qualityLevel = QualityLevels.None;
		public static QualityLevels QualityLevel {
			set {
				qualityLevel = value;

				if(onQualityLevelChange != null)
					onQualityLevelChange(qualityLevel);
			}
			get {
				return qualityLevel;
			}
		}
		private static QualityLevels lastQualityLevel = QualityLevels.None;
		public delegate void QualityLevelHasChanged(QualityLevels level);
		public static event QualityLevelHasChanged onQualityLevelChange;

		private static UseDeviceOrientationType deviceOrientationType = UseDeviceOrientationType.None;
		public static UseDeviceOrientationType DeviceOrientationType {
			set {
				deviceOrientationType = value;

				if(onDeviceOrientationTypeChange != null)
					onDeviceOrientationTypeChange(deviceOrientationType);
			}
			get {
				return deviceOrientationType;
			}
		}
		public delegate void DeviceOrientationTypeHasChanged(UseDeviceOrientationType deviceOrientationType);
		public static event DeviceOrientationTypeHasChanged onDeviceOrientationTypeChange;

		public RSInteractionPoint interactionPoint;
		public RSInteractionPoint InteractionPoint {
			set {
				interactionPoint = value;

				if(onInteractionPointChange != null) onInteractionPointChange(value);
			}
			get {
				return interactionPoint;
			}
		}
		public delegate void InteractionPointHasChanged(RSInteractionPoint interactionPoint);
		public static event InteractionPointHasChanged onInteractionPointChange;

		private RSInputManager inputController;
		private RSOutputModesManager outputModesManager;
		private RSMovingModesManager movingModesManager;


		public static RSSceneManager SceneManagerInstance {
			get { 
				return sceneManager ;
			} 
		}

		public override RSInputManager InputManager {
			get { 
				if (inputController == null) inputController = new RSInputManager();
				return inputController ;
			} 
		}

		public override RSOutputModesManager OutputModesManager {
			get {
				if (outputModesManager == null) outputModesManager = new RSOutputModesManager();
				return outputModesManager ;
			} 
		}

		public override RSMovingModesManager MovingModesManager {
			get {
				if (movingModesManager == null) movingModesManager = new RSMovingModesManager();
				return movingModesManager ;
			} 
		}
		
		void Start()
		{
			Debug.Log ("Starting scene manager");

			//Warning Start order is important !!!
			InputManager.Start ();

			MovingModesManager.Start ();
			OutputModesManager.Start ();
			Configure ();
		}

		void Configure() {
			if (OVRDevice.IsHMDPresent ()) {
				gameObject.AddComponent<OVRDevice> ();
				Screen.SetResolution (1920, 1080, true);
				SceneManager.SetRenderMode (RenderModes.Oculus);
			} else if (RSSceneManager.RenderMode != RenderModes.None) {
				SceneManager.SetRenderMode (RSSceneManager.RenderMode);
			} else {
				SceneManager.SetRenderMode (RenderModes.TwoD);
				SetToBestQualityLevel();
			}


			if (IsMobileDevice) {
				canStereoscopic = false;
				canOculus = false;

				switch(Application.platform) {
					case RuntimePlatform.IPhonePlayer:
	#if UNITY_IOS
						if(UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPad4Gen || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPadAir1 || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPadMini2Gen)
							canCardboard = false;
						else if(UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone5 || 
					        	UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone5S || 
					        	UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone6 ||
					        	UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone6Plus)
							canCardboard = true;
						else
							SetQualityLevel(QualityLevels.Low);
	#endif
					break;
					case RuntimePlatform.Android:
						canCardboard = true;
					break;
					default:
						canCardboard = false;
					break;
				}

				if(Input.gyro == null) canGyroscope = false;
				if(Input.location == null) canGPS = false;
			} else {
				canStereoscopic = true;
				canOculus = true;
				canCardboard = false;

				canGyroscope = false;
				canGPS = false;
			}

			MovingModesManager.ActivateMovingMode ("GlobalCamera");
		}

		public override void OnDestroy () {
			base.OnDestroy();
		}

		void Update(){
			InputManager.Update();
		}
		
		void LateUpdate(){
			InputManager.LateUpdate();
		}

		void OnPostRender() {
			
			if (OutputModesManager.Active == null) return;

			OutputModesManager.RenderImage(GetComponent<Camera>().targetTexture, GetComponent<Camera>().targetTexture);
		}

		void OnGUI(){
			if(OutputModesManager!=null){
				OutputModesManager.RenderGui(null);
			}
		}

		public void SetQualityLevel(QualityLevels level) {
			lastQualityLevel = QualityLevel;
			QualityLevel = level;

			switch (level) {
				case QualityLevels.Low:
					QualitySettings.SetQualityLevel((SceneManager.IsMobileDevice) ? 4: 0);
					break;
				case QualityLevels.Medium:
					QualitySettings.SetQualityLevel((SceneManager.IsMobileDevice) ? 5: 1);
					break;
				case QualityLevels.High:
					QualitySettings.SetQualityLevel((SceneManager.IsMobileDevice) ? 6: 2);
					break;
				case QualityLevels.Extra:
					QualitySettings.SetQualityLevel((SceneManager.IsMobileDevice) ? 7: 3);
					break;
				case QualityLevels.Oculus:
					QualitySettings.SetQualityLevel(8);
					break;
				case QualityLevels.Cardboard:
					QualitySettings.SetQualityLevel(9);
					break;
			}
		}

		private void SetToBestQualityLevel() {
			if (IsMobileDevice) {
				switch(Application.platform) {
					case RuntimePlatform.IPhonePlayer:
	#if UNITY_IOS
						switch(UnityEngine.iOS.Device.generation) {
							case UnityEngine.iOS.DeviceGeneration.iPad4Gen:
								SetQualityLevel(QualityLevels.Medium);
							break;
							case UnityEngine.iOS.DeviceGeneration.iPadAir1:
								SetQualityLevel(QualityLevels.High);
							break;
							case UnityEngine.iOS.DeviceGeneration.iPadUnknown:
								SetQualityLevel(QualityLevels.Extra);
							break;
							case UnityEngine.iOS.DeviceGeneration.iPhone5S:
								SetQualityLevel(QualityLevels.High);
							break;
							case UnityEngine.iOS.DeviceGeneration.iPhone6:
								SetQualityLevel(QualityLevels.Extra);
							break;
							case UnityEngine.iOS.DeviceGeneration.iPhone6Plus:
								SetQualityLevel(QualityLevels.Extra);
							break;
							case UnityEngine.iOS.DeviceGeneration.iPhoneUnknown:
								SetQualityLevel(QualityLevels.Extra);
							break;
							default:
								SetQualityLevel(QualityLevels.Low);
							break;
						}
	#endif
					break;
				}
			} else {
				SetQualityLevel(QualityLevels.High);
			}
		}

		private void SetToLastQualityLevel() {
			if (lastQualityLevel == QualityLevels.None) {
				return;
			}
			SetQualityLevel(lastQualityLevel);
			lastQualityLevel = QualityLevels.None;
		}

		public void SetRenderMode(RenderModes mode) {
			RenderMode = mode;

			switch (mode) {
				case RenderModes.TwoD:
					OutputModesManager.ActivateOutputMode ("2D");
				
					SetToLastQualityLevel();
					SetDeviceOrientation (UseDeviceOrientationType.None);
				break;
				case RenderModes.Stereoscopic:
					Screen.SetResolution(1920, 1080, true);
					RSSceneManager.GetInstance<RSOutputMode3DSplitted>().splitMode = RSOutputMode3DSplitted.SplitMode.SideBySide;
					OutputModesManager.ActivateOutputMode ("3D Splitted");

					SetToLastQualityLevel();
					break;
				case RenderModes.Oculus:
					OutputModesManager.ActivateOutputMode ("3D Oculus");

					SetQualityLevel(QualityLevels.Oculus);
					SetDeviceOrientation (UseDeviceOrientationType.Oculus);
				break;
				case RenderModes.Cardboard:
					RSSceneManager.GetInstance<RSOutputMode3DSplitted>().splitMode = RSOutputMode3DSplitted.SplitMode.SideBySide;
					OutputModesManager.ActivateOutputMode ("3D Splitted");

					SetQualityLevel(QualityLevels.Cardboard);
					SetDeviceOrientation (UseDeviceOrientationType.Gyroscope);
				break;
			}
		}

		public void SetDeviceOrientation(UseDeviceOrientationType newDeviceOrientationType) {
			
			foreach(var mm in RSSceneManager.GetAllInstances<RSPanTiltMovingMode>())
			{
				mm.useDeviceOrientation = newDeviceOrientationType;
			}

			DeviceOrientationType = newDeviceOrientationType;
		}

		public void ToFullScreen() {
			Screen.SetResolution (Screen.width, Screen.height, true);
		}

		public void ToWindowed() {
			Screen.SetResolution (Screen.width, Screen.height, false);
		}
		
		public void LoadLevel(int loaderSceneIndex,int sceneToLoadIndex){
			Loader.sceneToLoadIndex = sceneToLoadIndex;
			Application.LoadLevel(loaderSceneIndex);
		}

		public void LoadLevel(int sceneToLoadIndex) {
			LoadLevel (0, sceneToLoadIndex);
		}

		public void Quit() {
			Application.Quit ();
		}

		

		public static string GetUnique(string prefix){
			if(!string.IsNullOrEmpty(prefix))return prefix+"_"+(uniqueIndex++);
			else return (uniqueIndex++).ToString();
		}
		
		public static bool Register(RSBehaviour plugin)
		{
			if(!string.IsNullOrEmpty(plugin.id)){
				if(!idPluginsDictionary.ContainsKey(plugin.id)){
					
					if(plugin.IsUnique && GetAllInstances(plugin.GetType()).Count > 0){
						plugin.enabled = false;
						Debug.LogWarning("Cannot register " + plugin.ToString()+ " because it must be unique and another instance is already registered.");
						return false;
					}else{
						idPluginsDictionary.Add(plugin.id,plugin);
						if(plugin.GetType() == typeof(RSSceneManager)){
							sceneManager=(RSSceneManager)plugin;
							Debug.Log("Registering scene manager " + plugin.ToString());
						}else{
							Debug.Log("Registering " + plugin.ToString());
						}
						return true;
					}
				}else{
					plugin.enabled = false;
					Debug.LogWarning("Cannot register " + plugin.ToString()+" as its Id is already registered.");
					return false;
				}
			}else{
				Debug.LogWarning("Cannot register this instance of \"" + plugin.GetType().ToString() + "\" because it's id has not been set");
				return false;
			}
		}
		
		public static bool Unregister(RSBehaviour plugin)
		{
			if(!string.IsNullOrEmpty(plugin.id)){
				if(idPluginsDictionary.ContainsKey(plugin.id)){
					idPluginsDictionary.Remove(plugin.id);
					Debug.Log("Unregistering " + plugin.ToString());
					return true;
				}else{
					//Debug.LogWarning("Cannot unregister this instance of \"" + plugin.GetType ().ToString() + "\" ("+plugin.id+") as its seems not to be registered.");
					return false;
				}
			}else{
				//Debug.LogWarning("Cannot register this instance of \"" + plugin.GetType().ToString() + "\" because it's id has not been set");
				return false;
			}
			
		}

		public static List<T> GetAllInstances<T>() where T : RSBehaviour
		{
			List<T> list = new List<T>();
			
			foreach(KeyValuePair<string,RSBehaviour> keyValue in idPluginsDictionary){
				RSBehaviour RSBehaviour = keyValue.Value;
				if( RSBehaviour.GetType() == typeof(T) || RSBehaviour.GetType().IsSubclassOf(typeof(T))){
					list.Add(RSBehaviour as T);
				}
			}
			
			return list;
		}
		
		public static List<RSBehaviour> GetAllInstances(System.Type type)
		{
			List<RSBehaviour> list = new List<RSBehaviour>();
			
			foreach(KeyValuePair<string,RSBehaviour> keyValue in idPluginsDictionary){
				RSBehaviour RSBehaviour = keyValue.Value;
				if( RSBehaviour.GetType() == type || RSBehaviour.GetType().IsSubclassOf(type)) list.Add(RSBehaviour);
				
			}
			
			return list;
		}
		
		public static RSBehaviour GetInstance(string id)
		{
			if(!idPluginsDictionary.ContainsKey(id))
			{
				//Debug.LogWarning("There are no plugin with id \"" + id + "\" registered.");
				return null;
			}
			
			return idPluginsDictionary[id];
		}
		
		public static RSBehaviour GetInstance(System.Type type, int index = 0)
		{
			int i = 0;
			
			foreach(KeyValuePair<string,RSBehaviour> keyValue in idPluginsDictionary){
				RSBehaviour RSBehaviour = keyValue.Value;
				if( (RSBehaviour.GetType() == type || RSBehaviour.GetType().IsSubclassOf(type)) && i++ == index) return RSBehaviour;
			}
			
			//Debug.LogWarning("The requested plugin \"" + type.ToString() + "["+ index+ "]\" was not found.");
			
			return null;
		}
		
		public static T GetInstance<T>(int index = 0) where T : RSBehaviour
		{
			int i = 0;
			
			foreach(KeyValuePair<string,RSBehaviour> keyValue in idPluginsDictionary){
				RSBehaviour RSBehaviour = keyValue.Value;
				if( (RSBehaviour.GetType() == typeof(T) || RSBehaviour.GetType().IsSubclassOf(typeof(T))) && i++ == index) return RSBehaviour as T;
			}
			
			//Debug.LogWarning("The requested plugin \"" + typeof(T).ToString() + "["+ index+ "]\" was not found.");
			
			return null;
		}
		
		public static T GetInstance<T>(string id) where T : RSBehaviour
		{
			
			foreach(KeyValuePair<string,RSBehaviour> keyValue in idPluginsDictionary){
				RSBehaviour RSBehaviour = keyValue.Value;
				if( (RSBehaviour.GetType() == typeof(T) || RSBehaviour.GetType().IsSubclassOf(typeof(T))) && RSBehaviour.id == id) return RSBehaviour as T;
			}
			
			//Debug.LogWarning("The requested plugin \"" + typeof(T).ToString() + "["+ index+ "]\" was not found.");
			
			return null;
		}

		public static void FreeMemory()
		{  
			System.GC.Collect();
			
			// Beware not to call this anywhere except in the main thread (not in the editor for example)
			Resources.UnloadUnusedAssets();
			
			Debug.Log("> Free memory");
		}
	}
}