using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Rise.Core;

namespace Rise.Features.Loader {
	public class Loader : MonoBehaviour {
		private bool waitingForLoadingSceneStreaming = false;
		
		public int firstSceneToLoadIndex = 1;
		public Image loadingSpinner;

		public static int sceneToLoadIndex = -1;
		
		AsyncOperation loadSceneAsyncOperation;

		void Start() {
			if(sceneToLoadIndex == -1) {
				sceneToLoadIndex = firstSceneToLoadIndex;
			}

			waitingForLoadingSceneStreaming = true;
		}
		
		void Update() {
			if(waitingForLoadingSceneStreaming) {
				if(Application.GetStreamProgressForLevel(sceneToLoadIndex) == 1) {
					startLoadTime = Time.realtimeSinceStartup;
					
					loadSceneAsyncOperation = SceneManager.LoadSceneAsync(sceneToLoadIndex);
					waitingForLoadingSceneStreaming = false;
				}
			}

			float progress = 0;
			
			if(waitingForLoadingSceneStreaming) {
				progress = Application.GetStreamProgressForLevel(sceneToLoadIndex);
			}
			else if(loadSceneAsyncOperation != null) {
				progress = loadSceneAsyncOperation.progress;
			}
			else {
				progress = 0;
			}
			
			loadingSpinner.fillAmount = progress;
		}

		private static float startLoadTime = 0.0f;
		public static float LastLoadingTime	{ 
			get; 
			private set; 
		}

		public static void SceneIsLoaded() {
			LastLoadingTime = Time.realtimeSinceStartup - startLoadTime;
		}
	}
}