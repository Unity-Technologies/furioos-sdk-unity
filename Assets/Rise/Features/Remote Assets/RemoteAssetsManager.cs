using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using Rise.Core;

namespace Rise.Features.AssetManager {
	public delegate void AssetDownloadedCallBack(object[] asset, object[] args);

	public class RemoteAssetsManager : RSBehaviour {
		public struct AssetData {
			public enum AssetType {
				TEXTURE,
				ASSETBUNDLE,
				BYTES,
				TEXT
			}
			
			public enum AssetPriority {
				HIGH = 0,
				MEDIUM = 1,
				LOW = 2
			}
			
			public string Url { get; set; }
			public object[] Args { get; set; }
			public int Version { get; set; }
			public AssetType Type { get; set; }
			public AssetPriority Priority { get; set; }
			public AssetDownloadedCallBack CallBack { get; set; }
			
			public AssetData(string newUrl, object[] args, AssetType type, AssetPriority priority, AssetDownloadedCallBack newCallBack) {
				Url = newUrl;
				Args = args;
				Type = type;
				Priority = priority;
				CallBack = newCallBack;
			}
		}
		
		public static WWW www;
		public static List<AssetData> assets = new List<AssetData>();
		
		private static Dictionary<string, Texture2D> texturesCache = new Dictionary<string, Texture2D>();
		private static Dictionary<string, AssetBundle> assetsBundlesCache = new Dictionary<string, AssetBundle>();
		private static Dictionary<string, string> textsCache = new Dictionary<string, string>();

		
		void Update ()
	    {
	    	if (assets.Count == 0 || www != null) {
	        	return;
	       	}
			
	       	StartCoroutine("StartDownload");
	    }
		
		public IEnumerator StartDownload() {
			AssetData asset = GetAssetDataByPriority(assets);
			
			string uid = asset.Url;
			
			Debug.Log("> [RemoteAssetsManager]: Start download asset: \""+asset.Url+"\"");
			
			if(IsInMemoryCache(asset, uid)) {
				Debug.Log("> [RemoteAssetsManager]: Get asset in memory cache: \""+asset.Url+"\" at "+Application.persistentDataPath+"/"+GetCacheFileName(asset));
				switch(asset.Type) {
					case AssetData.AssetType.TEXTURE:
	       				asset.CallBack(new object[1] { texturesCache[asset.Url] }, asset.Args);	
					break;
					case AssetData.AssetType.TEXT:
	       				asset.CallBack(new object[1] { textsCache[asset.Url] }, asset.Args);	
					break;
					case AssetData.AssetType.ASSETBUNDLE:
						asset.CallBack(new object[1] { assetsBundlesCache[asset.Url] }, asset.Args);
					break;
				}
			}
			else {
				yield return StartCoroutine(Download(asset, uid));
			}
			
			if(www != null)
				while(!www.isDone) {}
			
			if(!IsInCache(asset)) {
				AddToCache(asset, uid);	
			}
			
			assets.Remove(asset);
			www = null;
		}
		
		public IEnumerator Download(AssetData asset, string uid) {
			if(IsInCache(asset)) {
				Debug.Log("> [RemoteAssetsManager]: Get asset in cache: \""+asset.Url+"\" at "+Application.persistentDataPath+"/"+GetCacheFileName(asset));
				www = new WWW("file://"+Application.persistentDataPath+"/"+GetCacheFileName(asset));
			}
			else {
				Debug.Log("> [RemoteAssetsManager]: Downloading asset: \""+asset.Url+"\"");
				if(asset.Url.Contains("http://")) {
					www = new WWW(asset.Url.Replace(" ", "%20"));
				}
				else {
					www = new WWW("file://"+asset.Url);	
				}
			}
			
			yield return www;
			
			asset.Url.Replace("%20", " ");
			
			Debug.Log(asset.Url+" "+www.error);
			
			switch(asset.Type) {
				case AssetData.AssetType.TEXTURE:
					texturesCache.Add(asset.Url, new Texture2D(1, 1, TextureFormat.PVRTC_RGBA4, true));
					www.LoadImageIntoTexture(texturesCache[asset.Url]);
					
					if((texturesCache[asset.Url].width > 1024 || texturesCache[asset.Url].height > 1024) && asset.Args[2].ToString() != "selfillum") {
						texturesCache[asset.Url].Resize(1024, 1024, TextureFormat.DXT5, true);
						texturesCache[asset.Url].Apply();
					}
	       			
					asset.CallBack(new object[1] { texturesCache[asset.Url] }, asset.Args);	
				break;
				case AssetData.AssetType.TEXT:
					textsCache.Add(asset.Url, www.text);
	   				asset.CallBack(new object[1] { www.text }, asset.Args);	
				break;
				case AssetData.AssetType.ASSETBUNDLE:
					assetsBundlesCache.Add(asset.Url, www.assetBundle);
					asset.CallBack(new object[1] { assetsBundlesCache[asset.Url] }, asset.Args);
				break;
			}
			
			RSSceneManager.FreeMemory();
		}
		
		public static void Download(string url, AssetData.AssetType type, AssetData.AssetPriority priority, AssetDownloadedCallBack callBack) {
			Debug.Log("> [RemoteAssetsManager]: Add asset: \""+url+"\" to queue with download id: 0");
			
			assets.Add(new AssetData(url, null, type, priority, callBack));
		}
		
		public static void Download(string url, object[] args, AssetData.AssetType type, AssetData.AssetPriority priority, AssetDownloadedCallBack callBack) {
			Debug.Log("> [RemoteAssetsManager]: Add asset: \""+url+"\" to queue with download id: "+(int)args[0]);
			
			assets.Add(new AssetData(url, args, type, priority, callBack));
		}
		
		private void AddToCache(AssetData asset, string uid) {
			Debug.Log("> [RemoteAssetsManager]: Add asset in cache: \""+asset.Url+"\"");
			
			FileStream cache = new FileStream(Application.persistentDataPath+"/"+GetCacheFileName(asset), System.IO.FileMode.Create);
			cache.Write(www.bytes, 0, www.bytes.Length);
			cache.Close();
		}
		
		private bool IsInCache(AssetData asset) {
			return System.IO.File.Exists(Application.persistentDataPath+"/"+GetCacheFileName(asset));
		}
		
		private bool IsInMemoryCache(AssetData asset, string uid) {
			bool success = false;

			switch(asset.Type) {
				case AssetData.AssetType.TEXTURE:
					success = texturesCache.ContainsKey(asset.Url);
				break;
				case AssetData.AssetType.TEXT:
					success = textsCache.ContainsKey(asset.Url);
				break;
				case AssetData.AssetType.ASSETBUNDLE:
					success = assetsBundlesCache.ContainsKey(asset.Url);
				break;
			}

			return success;
		}
		
		private string GetCacheFileName(AssetData asset) {
			string fileName = MD5(asset.Url+asset.Version);
			string[] fileExtensionSplit = asset.Url.Split(new char[] { '/' });
			string fileExtension = fileExtensionSplit[fileExtensionSplit.Length-1];	
			
			return fileName+"."+fileExtension;
		}
		
		private AssetData GetAssetDataByPriority(List<AssetData> assetsDataList) {
			List<AssetData> highPriority = new List<AssetData>();
			List<AssetData> mediumPriority = new List<AssetData>();
			List<AssetData> lowPriority = new List<AssetData>();
			
			foreach(AssetData currentAssetData in assetsDataList) {
				switch(currentAssetData.Priority) {
					case AssetData.AssetPriority.LOW:
						lowPriority.Add(currentAssetData);
					break;
					case AssetData.AssetPriority.MEDIUM:
						mediumPriority.Add(currentAssetData);
					break;
					case AssetData.AssetPriority.HIGH:
						highPriority.Add(currentAssetData);
					break;
				}
			}
			
			if(highPriority.Count > 0) return highPriority[0];
			if(mediumPriority.Count > 0) return mediumPriority[0];
			if(lowPriority.Count > 0) return lowPriority[0];
			
			return new AssetData();
		}
		
		public static string MD5(string strToEncrypt) {
			System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
			byte[] bytes = ue.GetBytes(strToEncrypt);
		
			System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] hashBytes = md5.ComputeHash(bytes);
		
			string hashString = "";
		 
			for (int i = 0; i < hashBytes.Length; i++) {
				hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
			}
		 
			return hashString.PadLeft(32, '0');
		}
		
		public static void EmptyCache() {
			Debug.Log("> [RemoteAssetsManager]: Start empty cache");
			DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
	#if !UNITY_WEBPLAYER
			foreach(System.IO.FileInfo file in directory.GetFiles()) {
				Debug.Log("> [RemoteAssetsManager]: Delete "+file.Name+" from cache");
				file.Delete();
			}
	#endif
			Debug.Log("> [RemoteAssetsManager]: Complete empty cache");
		}
	}
}