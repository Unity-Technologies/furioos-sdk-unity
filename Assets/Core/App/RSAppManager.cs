#pragma warning disable 0414
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using Rise.Features.Loader;

namespace Rise.Core {
	public enum RenderModes {
		None,
		TwoD,
		Stereoscopic,
		Oculus,
		Cardboard
	};

	[System.Serializable]
	public class RSAppManager : RSUniqueBehaviour 
	{
		private static Dictionary<string, RSBehaviour> idPluginsDictionary = new Dictionary<string,RSBehaviour>();
		private static RSAppManager sceneManager = null;
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

		private RSInputManager inputController;
		private RSOutputModesManager outputModesManager;
		private RSMovingModesManager movingModesManager;


		public static RSAppManager SceneManagerInstance {
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

		public void SetRenderMode(RenderModes mode) {
			RenderMode = mode;

			switch (mode) {
				case RenderModes.TwoD:
					OutputModesManager.ActivateOutputMode ("2D");
					SetDeviceOrientation (UseDeviceOrientationType.None);
				break;
				case RenderModes.Stereoscopic:
					Screen.SetResolution(1920, 1080, true);
					RSAppManager.GetInstance<RSOutputMode3DSplitted>().splitMode = RSOutputMode3DSplitted.SplitMode.SideBySide;
					OutputModesManager.ActivateOutputMode ("3D Splitted");
					break;
				case RenderModes.Oculus:
					OutputModesManager.ActivateOutputMode ("3D Oculus");
					SetDeviceOrientation (UseDeviceOrientationType.Oculus);
				break;
				case RenderModes.Cardboard:
					RSAppManager.GetInstance<RSOutputMode3DSplitted>().splitMode = RSOutputMode3DSplitted.SplitMode.SideBySide;
					OutputModesManager.ActivateOutputMode ("3D Splitted");
					SetDeviceOrientation (UseDeviceOrientationType.Gyroscope);
				break;
			}
		}

		public void SetDeviceOrientation(UseDeviceOrientationType newDeviceOrientationType) {
			
			foreach(var mm in RSAppManager.GetAllInstances<RSPanTiltMovingMode>())
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
			AppManager.LoadLevel(loaderSceneIndex);
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
						//Debug.LogWarning("Cannot register " + plugin.ToString()+ " because it must be unique and another instance is already registered.");
						return false;
					}else{
						idPluginsDictionary.Add(plugin.id,plugin);
						if(plugin.GetType() == typeof(RSAppManager)){
							sceneManager=(RSAppManager)plugin;
							//Debug.Log("Registering scene manager " + plugin.ToString());
						}else{
							//Debug.Log("Registering " + plugin.ToString());
						}
						return true;
					}
				}else{
					plugin.enabled = false;
					//Debug.LogWarning("Cannot register " + plugin.ToString()+" as its Id is already registered.");
					return false;
				}
			}else{
				//Debug.LogWarning("Cannot register this instance of \"" + plugin.GetType().ToString() + "\" because it's id has not been set");
				return false;
			}
		}
		
		public static bool Unregister(RSBehaviour plugin)
		{
			if(!string.IsNullOrEmpty(plugin.id)){
				if(idPluginsDictionary.ContainsKey(plugin.id)){
					idPluginsDictionary.Remove(plugin.id);
					//Debug.Log("Unregistering " + plugin.ToString());
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
			
			//Debug.Log("> Free memory");
		}
	}
}