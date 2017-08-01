using UnityEngine;
using System.Collections;
using System;

using Rise.UI;

namespace Rise.Core {
	public abstract class RSBehaviour : MonoBehaviour {
		public string id;

		public virtual RSManager Manager {
			get { 
				return RSManager.Manager;
			}
		}

		public virtual RSInputManager InputManager {
			get { 
				return RSManager.Manager != null ? RSManager.Manager.InputManager : null; 
			}
		}

		public virtual RSOutputManager OutputsManager {
			get { 
				return RSManager.Manager != null ? RSManager.Manager.OutputsManager : null;
			}
		}

		public virtual RSCamerasManager CamerasManager {
			get { 
				return RSManager.Manager != null ? RSManager.Manager.CamerasManager : null;
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
			Init();
		}
		
		protected virtual void Init() {
			if(string.IsNullOrEmpty(id)) {
				id = Manager.GetUnique(GetType().ToString());
			}

			Manager.Register(this);
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