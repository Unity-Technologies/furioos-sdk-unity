using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

using Rise.Core;
using Rise.App.ViewModels;

namespace Rise.App.Controllers {
    public class SceneViewerController : RSBehaviour {
        public delegate void HandleProgressSceneCallback(float progress);
        public delegate void HandleDoneLoadSceneCallback();
        public delegate void HandleDoneUnloadSceneCallback();

        private static SceneViewerController _instance;

        private string _currentScene;
        private AssetBundle _currentBundle;

        //View
        public GameObject viewer;

        public static void LoadScene(string path) {
            if(_instance == null) {
                return;
            }

            AppController.SetActiveApp(false);
            SetActiveViewer(true);

            _instance.StartLoadScene(path);
        }

        public static void SetActiveViewer(bool active) {
            if(_instance == null) {
                return;
            }

            _instance.viewer.SetActive(active);
        }

        public void Start() {
            _instance = this;
        }

        public void StartLoadScene(string path) {
            LoadingViewModel loading = AppController.CreateLoading(viewer, true);

            StartCoroutine(_instance.AsyncLoadScene(path,
                (float progress) => {
                    loading.progress.fillAmount = progress;
                },
                () => {
                    loading.Destroy();
                }
            ));
        }

        private IEnumerator AsyncLoadScene(string path, HandleProgressSceneCallback progressCallback = null, HandleDoneLoadSceneCallback doneCallback = null) {
            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(path);
            yield return abcr;

            _currentBundle = abcr.assetBundle;
            if(_currentBundle == null) {
                yield break;
            }

            _currentScene = _currentBundle.GetAllScenePaths()[0];
            AsyncOperation aop = SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Additive);

            while(!aop.isDone) {
                if(progressCallback != null) {
                    progressCallback(aop.progress);
                }

                yield return new WaitForEndOfFrame();
            }

            if(doneCallback != null) {
                doneCallback();
            }
        }

        public void UnloadCurrentScene() {
            LoadingViewModel loading = AppController.CreateLoading(viewer, true);

            StartCoroutine(AsyncUnloadCurrentScene(
                (float progress) => {
                    loading.progress.fillAmount = progress;
                },
                () => {
                    loading.Destroy();

                    AppController.SetActiveApp(true);
                    SetActiveViewer(false);
                }
            ));
        }

        private IEnumerator AsyncUnloadCurrentScene(HandleProgressSceneCallback progressCallback = null, HandleDoneLoadSceneCallback doneCallback = null) {
            AsyncOperation aop = SceneManager.UnloadSceneAsync(_currentScene);

            while(!aop.isDone) {
                if(progressCallback != null) {
                    progressCallback(aop.progress);
                }

                yield return new WaitForEndOfFrame();
            }

            _currentBundle.Unload(true);

            if(doneCallback != null) {
                doneCallback();
            }
        }
    }
}
