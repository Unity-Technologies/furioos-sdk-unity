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

		public delegate void ResultCallBack<T>(T result);
		public void Parse<T>(string method, ResultCallBack<T> callback) {
			StartCoroutine (Get<T> (baseUrl + method, callback));
		}

		private IEnumerator Get<T>(string uri, ResultCallBack<T> callback) {
			UnityWebRequest www = UnityWebRequest.Get(uri);
			yield return www.Send();

			if (www.isError) {
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