using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Rise.Core;

namespace Rise.Features.Loader {
	public class Loader : RSBehaviour 
	{
		private bool waitingForLoadingSceneStreaming = false;
		
		public int firstSceneToLoadIndex   =  1;
		public Slider loadingSlider;

		public static int sceneToLoadIndex = -1;
		
		AsyncOperation loadSceneAsyncOperation;

		public void Awake() { base.Init(); }

		void Start() 
		{
			if(sceneToLoadIndex == -1) sceneToLoadIndex = firstSceneToLoadIndex ;
			waitingForLoadingSceneStreaming = true ;
			RSSceneManager.FreeMemory() ;
		}
		
		void Update()
		{
			if(waitingForLoadingSceneStreaming)
			{
				if(Application.GetStreamProgressForLevel(sceneToLoadIndex) == 1)
				{
					startLoadTime = Time.realtimeSinceStartup;
					
					loadSceneAsyncOperation = Application.LoadLevelAsync(sceneToLoadIndex);
					waitingForLoadingSceneStreaming = false;
				}
			}

			float progress = 0;
			
			if(waitingForLoadingSceneStreaming)
			{
				progress = Application.GetStreamProgressForLevel(sceneToLoadIndex);
			}
			else if(loadSceneAsyncOperation != null)
			{
				progress = loadSceneAsyncOperation.progress;
			}
			else
			{
				progress = 0;
			}
			
			loadingSlider.value = progress;
		}

	/** Gestion du calcul des temps de téléchargement/chargement *****************************************************************/	
		
		/** Retourne le temps de chargement de la scène en cours.
		 */
		private static float startLoadTime = 0f;
		public static float LastLoadingTime	{ get; private set; }
		
		/** Callback déclenché lorsque la scène a finie d'etre chargée.
		 */	
		public static void SceneIsLoaded() 
		{
			LastLoadingTime = Time.realtimeSinceStartup - startLoadTime;
		}
	}
}