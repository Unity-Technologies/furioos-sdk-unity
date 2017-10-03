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

		public delegate void GetResponseCallBack<T>(T result, string rawResult);
        public delegate void DownloadProgressCallBack(float progress);
        public delegate void DownloadResponseCallBack(byte[] rawResult);
        public delegate void DownloadAssetBundleResponseCallBack(AssetBundle bundle);

        public void Get<T>(string method, GetResponseCallBack<List<T>> callback) {
			StartCoroutine(AsyncGet<T> (_baseUrl + method, callback));
		}

		private IEnumerator AsyncGet<T>(string uri, GetResponseCallBack<List<T>> callback) {
            UnityWebRequest www = UnityWebRequest.Get(uri);

            www.SetRequestHeader("Authorization", _encodedApiKey);

			yield return www.Send();

			if (www.isNetworkError) {
				Debug.LogError(www.error);
			}
            else {
				Debug.Log("[RSWebRequestManager] > Request to " + uri + " done...");
			}

			string wrappedJson = "{ \"values\":" + www.downloadHandler.text + "}";

			JsonWrapper<List<T>> wrapper = JsonUtility.FromJson<JsonWrapper<List<T>>> (
				wrappedJson
			);

			callback(wrapper.values, wrappedJson);

            wrapper = null;
            wrappedJson = null;
            www = null;
		}

		public void Post() {}

        public void Download(string uri, DownloadResponseCallBack callback, DownloadProgressCallBack progressCallback = null) {
            StartCoroutine(AsyncDownload(uri, callback, progressCallback));
        }

        private IEnumerator AsyncDownload(string uri, DownloadResponseCallBack callback, DownloadProgressCallBack progressCallback = null) {
            UnityWebRequest www = UnityWebRequest.Get(uri);
            AsyncOperation aop = www.Send();

            while(!aop.isDone) {
                if(progressCallback != null) {
                    progressCallback(aop.progress);
                }

                yield return new WaitForEndOfFrame();
            }

            if(www.isNetworkError) {
                Debug.LogError(www.error);
            }
            else {
                Debug.Log("[RSWebRequestManager] > Download " + uri + " done...");
            }

            callback(www.downloadHandler.data);
            
            www = null;
        }

        public void DownloadAssetBundle(string uri, DownloadAssetBundleResponseCallBack callback, DownloadProgressCallBack progressCallback = null) {
            StartCoroutine(AsyncDownloadAssetBundle(uri, callback, progressCallback));
        }

        private IEnumerator AsyncDownloadAssetBundle(string uri, DownloadAssetBundleResponseCallBack callback, DownloadProgressCallBack progressCallback = null) {
            UnityWebRequest www = UnityWebRequest.GetAssetBundle(uri);
            AsyncOperation aop = www.Send();

            while(!aop.isDone) {
                if(progressCallback != null) {
                    progressCallback(aop.progress);
                }

                yield return new WaitForEndOfFrame();
            }

            if(www.isNetworkError) {
                Debug.LogError(www.error);
            }
            else {
                Debug.Log("[RSWebRequestManager] > Download Asset Bundle " + uri + " done...");
            }

            callback(((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle);
            
            www = null;
        }
    }
}