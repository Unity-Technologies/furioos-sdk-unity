using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

using Rise.Core;


namespace Rise.App.Controllers {
    public class AppController : RSBehaviour {
        private static string _persistentDataPath;
        public static string PersistentDataPath {
            get {
                return _persistentDataPath;
            }
            set {
                _persistentDataPath = value;
            }
        }

        public delegate void BackButtonPressed();
        public static event BackButtonPressed OnBackButtonPressed;


        //View
        public Button backButton;

        //Model
        public string _organisationId;
	    public string OrganisationId {
		    get { 
			    return _organisationId; 
		    }
	    }

        public void Start() {
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