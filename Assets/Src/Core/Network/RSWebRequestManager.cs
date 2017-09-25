using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;

using Rise.Core;

namespace Rise.Core {
	public class RSWebRequestManager: RSBehaviour {
		private string _baseUrl = "";
		private string _encodedApiKey = "";

		public void Configure(string apiKey, string apiSecret, string baseUrl) {
			_baseUrl = baseUrl;

			byte[] apiBytes = Encoding.UTF8.GetBytes(apiKey + ":" + apiSecret);
			_encodedApiKey = "BASIC " + System.Convert.ToBase64String(apiBytes);
		}

		public delegate void ResponseCallBack<T>(T result, string rawResult);

		public void Get<T>(string method, ResponseCallBack<List<T>> callback) {
			StartCoroutine(AsyncGet<T> (_baseUrl + method, callback));
		}

		private IEnumerator AsyncGet<T>(string uri, ResponseCallBack<List<T>> callback) {
			UnityWebRequest www = UnityWebRequest.Get(uri);

			www.SetRequestHeader ("Authorization", _encodedApiKey);

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