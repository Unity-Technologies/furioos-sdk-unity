using UnityEngine;
using System.Collections;
using System;

using FurioosSDK.UI;

namespace FurioosSDK.Core {
	public abstract class FSBehaviour : MonoBehaviour {
		protected float lastTimeInternetReachabilityChecked = 0;
		public delegate void InternetReachabilityHasChanged(bool internetReachability);
		public static event InternetReachabilityHasChanged onInternetReachabilityChange;
		protected static bool internetReachable = true;
		protected static NetworkReachability InternetReachability {
			set {
				if (value == NetworkReachability.NotReachable) {
					if (internetReachable) {
						internetReachable = false;

						if (onInternetReachabilityChange != null) {
							onInternetReachabilityChange (internetReachable);
						}
					}
				} 
				else {
					if (!internetReachable) {
						internetReachable = true;

						if (onInternetReachabilityChange != null) {
							onInternetReachabilityChange (internetReachable);
						}
					}
				}
			}
		}

		public string id;

		public virtual FSManager Manager {
			get { 
				return FSManager.Manager;
			}
		}

		public virtual FSInputManager InputManager {
			get { 
				return FSManager.Manager != null ? FSManager.Manager.InputManager : null; 
			}
		}

		public virtual FSOutputManager OutputsManager {
			get { 
				return FSManager.Manager != null ? FSManager.Manager.OutputsManager : null;
			}
		}

		public virtual FSCamerasManager CamerasManager {
			get { 
				return FSManager.Manager != null ? FSManager.Manager.CamerasManager : null;
			}
		}

		public virtual FSWebRequestManager WebRequestManager {
			get {
				return Manager.GetInstance<FSWebRequestManager>();
			}
		}

		public virtual bool IsUnique {
			get {
				return false;
			}
		}
		
		public virtual bool IsMobileDevice {
			get {
				return Application.isMobilePlatform;
			}
		}

		void Awake() { 
			CheckInternetReachability();

			Init();
		}

		protected virtual void Update() {
			if (Time.time - lastTimeInternetReachabilityChecked > 10.0) {
				CheckInternetReachability ();
			}
		}
		
		protected virtual void Init() {
			if(Manager == null) {
				CreateTemporaryManager();
			}

			if(string.IsNullOrEmpty(id)) {
				id = Manager.GetUniqueId(GetType().ToString());
			}

			Manager.Register(this);
		}
			
		private void CheckInternetReachability() {
			InternetReachability = Application.internetReachability;

			lastTimeInternetReachabilityChecked = Time.time;
		}

		private void CreateTemporaryManager() {
			GameObject go = new GameObject("_Manager");
			FSManager.Manager = go.AddComponent<FSManager>();
		}

		private string GetSettingKey(string key) {
			return GetType().ToString()+"-"+key;
		}
			
		public string GetSetting(string key, string defaultValue) {
			key = GetSettingKey(key);

			if(PlayerPrefs.HasKey(key)) {
				return PlayerPrefs.GetString(key);
			}
			else {
				return defaultValue;
			}
		}

		public void SetSetting(string key, string value) {
			key = GetSettingKey(key);
			PlayerPrefs.SetString(key,value) ;
			PlayerPrefs.Save();
		}


		public int GetSetting(string key, int defaultValue) {
			key = GetSettingKey(key);

			if(PlayerPrefs.HasKey(key)) {
				return PlayerPrefs.GetInt(key);
			}
			else {
				return defaultValue;
			}
		}

		public void SetSetting(string key, int value) {
			key = GetSettingKey(key);
			PlayerPrefs.SetInt(key,value) ;
			PlayerPrefs.Save();
		}


		public float GetSetting(string key, float defaultValue) {
			key = GetSettingKey(key);

			if(PlayerPrefs.HasKey(key)) {
				return PlayerPrefs.GetFloat(key);
			}
			else {
				return defaultValue;
			}
		}

		public void SetSetting(string key, float value) {
			key = GetSettingKey(key);
			PlayerPrefs.SetFloat(key,value) ;
			PlayerPrefs.Save();
		}


		public bool GetSetting(string key, bool defaultValue) {
			key = GetSettingKey(key);
			if(PlayerPrefs.HasKey(key)) {
				bool.TryParse(PlayerPrefs.GetString(key), out defaultValue);
			}

			return defaultValue;
		}

		public void SetSetting(string key, bool value) {
			key = GetSettingKey(key);
			PlayerPrefs.SetString(key,value.ToString()) ;
			PlayerPrefs.Save();
		}


		public Type GetSetting(string key, Type defaultValue) {
			key = GetSettingKey(key);
			Type type = null;

			if(PlayerPrefs.HasKey(key)) {
				type = Type.GetType(PlayerPrefs.GetString(key));
			}

			return type!=null ? type : defaultValue;			
		}

		public void SetSetting(string key, Type value) {
			key = GetSettingKey(key);
			PlayerPrefs.SetString(key,value.ToString()) ;
			PlayerPrefs.Save();		
		}
		
		public virtual void OnDestroy() {
			Manager.Unregister(this);
		}

		public override string ToString() {
			return string.Format("'{0}' ({1})", id, GetType().ToString());
		}
	}
}