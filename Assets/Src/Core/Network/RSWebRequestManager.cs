using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using Rise.Core;

namespace Rise.Core {
	public class RSWebRequestManager: RSBehaviour {
		private string baseUrl = "";

		public void Start() {
			baseUrl = Manager.baseUrl;
		}

		public delegate void ResponseCallBack<T>(T result, string rawResult);

		public void Get<T>(string method, ResponseCallBack<List<T>> callback) {
			StartCoroutine(AsyncGet<T> (baseUrl + method, callback));
		}

		private IEnumerator AsyncGet<T>(string uri, ResponseCallBack<List<T>> callback) {
			UnityWebRequest www = UnityWebRequest.Get(uri);
			yield return www.Send();

			if (www.isNetworkError) {
				Debug.LogError(www.error);
			}
            else {
				Debug.Log("[RSWebRequestManager] > Request to " + uri + " OK");
			}

			string wrappedJson = "{ \"values\":" + www.downloadHandler.text + "}";

			JsonWrapper<List<T>> wrapper = JsonUtility.FromJson<JsonWrapper<List<T>>> (
				wrappedJson
			);

			callback(wrapper.values, wrappedJson);
		}

		public void Post() {}
	}
}