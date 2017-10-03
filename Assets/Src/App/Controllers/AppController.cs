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
    public class AppController : RSBehaviour {
        private static AppController _instance;

        private static string _persistentDataPath;
        public static string PersistentDataPath {
            get {
                return _persistentDataPath;
            }
            set {
                _persistentDataPath = value;
            }
        }

        private static GameObject LoadingView {
            get {
                return _instance.loadingView;
            }
        }

        public delegate void BackButtonPressed();
        public static event BackButtonPressed OnBackButtonPressed;


        //View
        public GameObject app;

        public GameObject loadingView;

        public Button backButton;
        public Toggle toggleMenu;
        public Text title;

        //Model
        public string _apiKey;
		public string ApiKey {
			get {
				return _apiKey;
			}
		}

		public string _apiSecret;
		public string ApiSecret {
			get {
				return _apiSecret;
			}
		}

		public string _baseUrl;
		public string BaseUrl {
			get {
				return _baseUrl;
			}
		}

        public string _organisationId;
	    public string OrganisationId {
		    get { 
			    return _organisationId; 
		    }
	    }

        public static void SetTitle(string title) {
            if(_instance == null) {
                return;
            }
        
            _instance.title.text = (string.IsNullOrEmpty(title)) ? "Home" : title;
        }

        public static LoadingViewModel CreateLoading(GameObject container, bool isOpaque = false, bool isAutoFill = false) {
            GameObject loading = Instantiate<GameObject>(AppController.LoadingView);
            loading.transform.SetParent(container.transform, false);

            LoadingViewModel loadingViewModel = loading.GetComponentInChildren<LoadingViewModel>();

            if(isOpaque) {
                loadingViewModel.Opaque();
            }

            if(isAutoFill) {
                loadingViewModel.AutoFill();
            }

            return loadingViewModel;
        }

        public static void HideMenu() {
            if(_instance == null) {
                return;
            }

            _instance.toggleMenu.isOn = false;
        }

        public static void SetActiveApp(bool active) {
            if(_instance == null) {
                return;
            }

            _instance.app.SetActive(active);
        }

        public void Start() {
			WebRequestManager.Configure (_apiKey, _apiSecret, _baseUrl + "organisations/" + _organisationId + "/");

            Debug.Log("[AppController] > Persistent data path : " + Application.persistentDataPath);

            CategoryController.OnSelectedCategoryChange += delegate(string id) {
                backButton.gameObject.SetActive(false);
            };

            ProjectController.OnSelectedProjectChange += delegate(string id) {
                backButton.gameObject.SetActive(
                    (string.IsNullOrEmpty(id)) ? false : true
                );
            };

            //First launch
            BuildCacheArchitecture();
            Build();

            _instance = this;
        }

        private void BuildCacheArchitecture() {
            _persistentDataPath = Application.persistentDataPath + "/" + "App/";

            if(!Directory.Exists(_persistentDataPath)) {
                Directory.CreateDirectory(_persistentDataPath);
            }
        }

        public void Build() {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(delegate() {
                if(OnBackButtonPressed != null) {
                    OnBackButtonPressed();
                }
            });
        }
    }
}