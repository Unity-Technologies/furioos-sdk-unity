using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FurioosSDK.Core {
	public class FSPlugins : FSUniqueBehaviour {
		private static Dictionary<string, FSBehaviour> plugins = new Dictionary<string,FSBehaviour>();
		private static int pluginsUniqueId = 0;

		public string GetUniqueId(string prefix) {
			if(!string.IsNullOrEmpty(prefix)) {
				return prefix + "_" + (pluginsUniqueId++);
			}
			else {
				return (pluginsUniqueId++).ToString();
			}
		}

		public bool Register(FSBehaviour plugin) {
			if(!string.IsNullOrEmpty(plugin.id)) {
				if(!plugins.ContainsKey(plugin.id)) {
					if(plugin.IsUnique && GetAllInstances(plugin.GetType()).Count > 0) {
						plugin.enabled = false;

						return false;
					}
					else {
						plugins.Add(plugin.id,plugin);

						return true;
					}
				}
				else {
					plugin.enabled = false;

					return false;
				}
			}
			else {
				return false;
			}
		}

		public bool Unregister(FSBehaviour plugin) {
			if(!string.IsNullOrEmpty(plugin.id)) {
				if(plugins.ContainsKey(plugin.id)) {
					plugins.Remove(plugin.id);
					return true;
				}
				else {
					return false;
				}
			}
			else {
				return false;
			}
		}

		public List<T> GetAllInstances<T>() where T : FSBehaviour {
			List<T> list = new List<T>();

			foreach(KeyValuePair<string,FSBehaviour> keyValue in plugins) {
				FSBehaviour FSBehaviour = keyValue.Value;
				if(FSBehaviour.GetType() == typeof(T) || FSBehaviour.GetType().IsSubclassOf(typeof(T))) {
					list.Add(FSBehaviour as T);
				}
			}

			return list;
		}

		public List<FSBehaviour> GetAllInstances(System.Type type) {
			List<FSBehaviour> list = new List<FSBehaviour>();

			foreach(KeyValuePair<string,FSBehaviour> keyValue in plugins) {
				FSBehaviour FSBehaviour = keyValue.Value;
				if(FSBehaviour.GetType() == type || FSBehaviour.GetType().IsSubclassOf(type)) {
					list.Add(FSBehaviour);
				}
			}

			return list;
		}

		public FSBehaviour GetInstance(string id) {
			if(!plugins.ContainsKey(id)) {
				return null;
			}

			return plugins[id];
		}

		public FSBehaviour GetInstance(System.Type type, int index = 0) {
			int i = 0;

			foreach(KeyValuePair<string,FSBehaviour> keyValue in plugins) {
				FSBehaviour FSBehaviour = keyValue.Value;

				if((FSBehaviour.GetType() == type || FSBehaviour.GetType().IsSubclassOf(type)) && i++ == index) {
					return FSBehaviour;
				}
			}

			return null;
		}

		public T GetInstance<T>(int index = 0) where T : FSBehaviour {
			int i = 0;

			foreach(KeyValuePair<string,FSBehaviour> keyValue in plugins) {
				FSBehaviour FSBehaviour = keyValue.Value;

				if((FSBehaviour.GetType() == typeof(T) || FSBehaviour.GetType().IsSubclassOf(typeof(T))) && i++ == index) {
					return FSBehaviour as T;
				}
			}

			return null;
		}

		public T GetInstance<T>(string id) where T : FSBehaviour {
			foreach(KeyValuePair<string,FSBehaviour> keyValue in plugins) {
				FSBehaviour FSBehaviour = keyValue.Value;
				if((FSBehaviour.GetType() == typeof(T) || FSBehaviour.GetType().IsSubclassOf(typeof(T))) && FSBehaviour.id == id) {
					return FSBehaviour as T;
				}
			}

			return null;
		}
	}
}