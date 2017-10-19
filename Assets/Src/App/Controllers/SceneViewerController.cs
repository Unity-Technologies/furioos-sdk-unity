using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

using Rise.Core;
using Rise.SDK.Cameras;
using Rise.App.ViewModels;
using Rise.Viewer.Loaders;

namespace Rise.App.Controllers {
    public class SceneViewerController : RSBehaviour {
        public const string VIEWER_GLTF = "Viewer_GLTF";

        public delegate void HandleProgressSceneCallback(float progress);
        public delegate void HandleDoneLoadSceneCallback();
        public delegate void HandleDoneUnloadSceneCallback();

        private static SceneViewerController _instance;

        private string _currentScene;
        private AssetBundle _currentBundle;

        //GLTF
        public GameObject glft;

        //View
        public GameObject viewer;

        public Text title;
        public Toggle fpsCameraToggle;
        public Toggle orbitalCameraToggle;
        public Toggle pointOfInterestToggle;
        public Button screenshotButton;

        public static void LoadScene(string path) {
            if(_instance == null) {
                return;
            }

            AppController.SetActiveApp(false);
            SetActiveViewer(true);

            _instance.HandleScene(path);
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

        private void ConfigureViewer() {
            OrbitalCamera[] orbitalCameras = Resources.FindObjectsOfTypeAll<OrbitalCamera>();
            FPSCamera[] fpsCameras = Resources.FindObjectsOfTypeAll<FPSCamera>();

            if(orbitalCameras.Length == 0) {
                orbitalCameraToggle.gameObject.SetActive(false);
            }

            if(fpsCameras.Length == 0) {
                fpsCameraToggle.gameObject.SetActive(false);
            }

            string activeId = CamerasManager.ActiveId;
            Debug.Log(CamerasManager.Active);
        }

        public void HandleScene(string path) {
            LoadingViewModel loading = AppController.CreateLoading(viewer, true);

            string extension = Path.GetExtension(path);

            switch(extension) {
                case ".glb":
                    StartCoroutine(_instance.AsyncLoadGLTF(path,
                        (float progress) => {
                            loading.progress.fillAmount = progress;
                        },
                        () => {
                            loading.Destroy();
                            ConfigureViewer();
                        }
                    ));
                break;
                case ".zip":
                    break;
                default:
                    StartCoroutine(_instance.AsyncLoadAssetBundleScene(path,
                        (float progress) => {
                            loading.progress.fillAmount = progress;
                        },
                        () => {
                            loading.Destroy();
                            ConfigureViewer();
                        }
                    ));
                break;
            }
        }

        private IEnumerator AsyncLoadAssetBundleScene(string path, HandleProgressSceneCallback progressCallback = null, HandleDoneLoadSceneCallback doneCallback = null) {
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

            yield return new WaitForEndOfFrame();
            
            SceneManager.SetActiveScene(
                SceneManager.GetSceneByPath(_currentScene)
            );

            if(doneCallback != null) {
                doneCallback();
            }
        }

        private IEnumerator AsyncLoadGLTF(string path, HandleProgressSceneCallback progressCallback = null, HandleDoneLoadSceneCallback doneCallback = null) {
            _currentScene = VIEWER_GLTF;
            AsyncOperation aop = SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Additive);

            while(!aop.isDone) {
                if(progressCallback != null) {
                    progressCallback(aop.progress);
                }

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
            
            SceneManager.SetActiveScene(
                SceneManager.GetSceneByName(_currentScene)
            );

            GLTFLoader loader = GameObject.Find("GLTF").GetComponentInChildren<GLTFLoader>();
            loader.Load(path);

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

            if(_currentBundle != null) {
                _currentBundle.Unload(true);
            }

            if(doneCallback != null) {
                doneCallback();
            }
        }
    }
}
