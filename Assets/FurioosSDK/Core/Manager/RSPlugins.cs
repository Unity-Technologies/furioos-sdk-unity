using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rise.Core {
	public class RSPlugins : RSUniqueBehaviour {
		private static Dictionary<string, RSBehaviour> plugins = new Dictionary<string,RSBehaviour>();
		private static int pluginsUniqueId = 0;

		public string GetUniqueId(string prefix) {
			if(!string.IsNullOrEmpty(prefix)) {
				return prefix + "_" + (pluginsUniqueId++);
			}
			else {
				return (pluginsUniqueId++).ToString();
			}
		}

		public bool Register(RSBehaviour plugin) {
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

		public bool Unregister(RSBehaviour plugin) {
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

		public List<T> GetAllInstances<T>() where T : RSBehaviour {
			List<T> list = new List<T>();

			foreach(KeyValuePair<string,RSBehaviour> keyValue in plugins) {
				RSBehaviour RSBehaviour = keyValue.Value;
				if(RSBehaviour.GetType() == typeof(T) || RSBehaviour.GetType().IsSubclassOf(typeof(T))) {
					list.Add(RSBehaviour as T);
				}
			}

			return list;
		}

		public List<RSBehaviour> GetAllInstances(System.Type type) {
			List<RSBehaviour> list = new List<RSBehaviour>();

			foreach(KeyValuePair<string,RSBehaviour> keyValue in plugins) {
				RSBehaviour RSBehaviour = keyValue.Value;
				if(RSBehaviour.GetType() == type || RSBehaviour.GetType().IsSubclassOf(type)) {
					list.Add(RSBehaviour);
				}
			}

			return list;
		}

		public RSBehaviour GetInstance(string id) {
			if(!plugins.ContainsKey(id)) {
				return null;
			}

			return plugins[id];
		}

		public RSBehaviour GetInstance(System.Type type, int index = 0) {
			int i = 0;

			foreach(KeyValuePair<string,RSBehaviour> keyValue in plugins) {
				RSBehaviour RSBehaviour = keyValue.Value;

				if((RSBehaviour.GetType() == type || RSBehaviour.GetType().IsSubclassOf(type)) && i++ == index) {
					return RSBehaviour;
				}
			}

			return null;
		}

		public T GetInstance<T>(int index = 0) where T : RSBehaviour {
			int i = 0;

			foreach(KeyValuePair<string,RSBehaviour> keyValue in plugins) {
				RSBehaviour RSBehaviour = keyValue.Value;

				if((RSBehaviour.GetType() == typeof(T) || RSBehaviour.GetType().IsSubclassOf(typeof(T))) && i++ == index) {
					return RSBehaviour as T;
				}
			}

			return null;
		}

		public T GetInstance<T>(string id) where T : RSBehaviour {
			foreach(KeyValuePair<string,RSBehaviour> keyValue in plugins) {
				RSBehaviour RSBehaviour = keyValue.Value;
				if((RSBehaviour.GetType() == typeof(T) || RSBehaviour.GetType().IsSubclassOf(typeof(T))) && RSBehaviour.id == id) {
					return RSBehaviour as T;
				}
			}

			return null;
		}
	}
}