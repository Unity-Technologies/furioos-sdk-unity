using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Rise.Core;

namespace Rise.Core {
	[System.Serializable]
	public class JsonWrapper<T> {
		public T values;
	}

	public class RSWebRequest: RSBehaviour {
		public string baseUrl = "";

		public void Start() {
			baseUrl = Manager.baseUrl;
		}

		public delegate void ResponseCallBack<T>(T result);

		public void Get<T>(string method, ResponseCallBack<T> callback) {
			StartCoroutine (AsyncGet<T> (baseUrl + method, callback));
		}

		private IEnumerator AsyncGet<T>(string uri, ResponseCallBack<T> callback) {
			UnityWebRequest www = UnityWebRequest.Get(uri);
			yield return www.Send();

			if (www.isNetworkError) {
				Debug.Log (www.error);
			} else {
				Debug.Log (www.downloadHandler.text);
			}

			JsonWrapper<T> wrapper = JsonUtility.FromJson<JsonWrapper<T>> (
				"{ \"values\":" + www.downloadHandler.text + "}"
			);

			callback (wrapper.values);
		}
	}
}