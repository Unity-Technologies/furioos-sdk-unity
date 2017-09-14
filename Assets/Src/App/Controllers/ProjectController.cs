using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

using Rise.Core;
using Rise.App.Models;
using Rise.App.ViewModels;

namespace Rise.App.Controllers {
    public class ProjectController : RSBehaviour {
        public const string PROJECT_METHOD = "projects";
        public const string DATA_METHOD = "datas";

        private static string _persistentDataPath;
        public static string PersistentDataPath {
            get {
                return _persistentDataPath;
            }
            set {
                _persistentDataPath = value;
            }
        }

        public delegate void SelectedProjectHasChanged(string id);
        public static event SelectedProjectHasChanged OnSelectedProjectChange;

        //View
        public HorizontalScrollSnap scrollSnap;
        public Transform listContainer;
        public GameObject listView;
        public GameObject listMessage;

        public GameObject menu;
        public Transform menuContainer;
        public Transform menuIndicatorContainer;
        public GameObject menuView;
        public GameObject menuIndicatorView;

        public Transform detailContainer;
        public GameObject detailView;
        
        //Model
        private string _selectedCategoryId;
        private string _selectedProjectId;

        private List<Project> _projects;
        public List<Project> Projects {
            get {
                return _projects;
            }
            set {
                _projects = value;
            }
        }

        public void Start() {
            _projects = new List<Project>();
            _selectedCategoryId = null;

            onInternetReachabilityChange += delegate (bool internetReachability) {
                if(!string.IsNullOrEmpty(_selectedCategoryId)) {
                    GetAll();
                }
            };

            CategoryController.OnSelectedCategoryChange += delegate(string id) {
                _selectedCategoryId = id;
                GetAll();

                CleanDetail();
            };
        }

        public Project GetById(string id) {
            return _projects.SingleOrDefault(p => p.Id == id);
        }

        public void Select(string id) {
            Project project = GetById(id);

            _selectedProjectId = id;

            if(OnSelectedProjectChange != null) {
                OnSelectedProjectChange(_selectedProjectId);
            }

            BuildDetail();
        }

        private void GetAll() {
            _persistentDataPath = CategoryController.PersistentDataPath + _selectedCategoryId + "/" + "Projects/";
            string fileName = "projects.json";
            string fullPath = _persistentDataPath + fileName;

            string uri = CategoryController.CATEGORY_METHOD + "/" + _selectedCategoryId + "/" + PROJECT_METHOD;

            if(internetReachable) {
                WebRequestManager.Get<Project>(uri, delegate (List<Project> result, string rawResult) {
                    _projects = result;

                    if(!Directory.Exists(_persistentDataPath)) {
                        Directory.CreateDirectory(_persistentDataPath);
                    }

                    using(FileStream st = File.Create(fullPath)) {
                        byte[] data = new UTF8Encoding(true).GetBytes(rawResult);
                        st.Write(data, 0, data.Length);
                    }

                    BuildList();
                });
            }
            else {
                if(File.Exists(fullPath)) {
                    string json = "";

                    using(StreamReader sr = new StreamReader(fullPath)) {
                        json = sr.ReadToEnd();
                    }

                    JsonWrapper<List<Project>> wrapper = JsonUtility.FromJson<JsonWrapper<List<Project>>>(
                        json
                    );

                    _projects = wrapper.values;
                }

                BuildList();
            }
        }

        // List

        private void BuildList() {
            //Test
            if(_projects.Count == 0) {
                listMessage.SetActive(true);

                return;
            }

            //Clear
            scrollSnap.enabled = false;
            listMessage.SetActive(false);
            foreach(Transform child in listContainer) {
                Destroy(child.gameObject);
            }

            foreach(Transform child in menuContainer) {
                Destroy(child.gameObject);
            }

            foreach(Transform child in menuIndicatorContainer) {
                Destroy(child.gameObject);
            }

            //Build
            int count = _projects.Count;
            for(int i = 0; i < count; i++) {
                Project project = _projects[i];

                GameObject entryListGO = Instantiate<GameObject>(listView);
                project.ProjectViewModel = entryListGO.GetComponentInChildren<ProjectViewModel>(true);

                entryListGO.transform.SetParent(listContainer, false);

                project.ProjectViewModel.name.text = project.Name;

                project.ProjectViewModel.view.onClick.RemoveAllListeners();
                project.ProjectViewModel.view.onClick.AddListener(delegate() {
                    Select(project.Id);
                });


                GameObject entryMenuGO = Instantiate<GameObject>(menuView);
                project.ProjectMenuItemViewModel = entryMenuGO.GetComponentInChildren<ProjectMenuItemViewModel>(true);

                entryMenuGO.transform.SetParent(menuContainer, false);

                project.ProjectMenuItemViewModel.name.text = project.Name;
                project.ProjectMenuItemViewModel.view.onClick.AddListener(delegate () {
                    Select(project.Id);
                });


                GameObject entryIndicator = Instantiate<GameObject>(menuIndicatorView);
                entryIndicator.transform.SetParent(menuIndicatorContainer, false);
            }

            StartCoroutine(RebuildListScrollSnap());
        }

        private IEnumerator RebuildListScrollSnap() {
            yield return new WaitForEndOfFrame();

            scrollSnap.enabled = true;
            scrollSnap.UpdateVisible();
            scrollSnap.UpdateLayout();
        }

        private void RebuildList() {
            List<Project> projects = null;
        }

        // Detail

        private void BuildDetail() {
            //Clean
            CleanDetail();

            //Build
            Project project = GetById(_selectedProjectId);
            GameObject entryGo = Instantiate<GameObject>(detailView);
            
            project.ProjectDetailViewModel = entryGo.GetComponentInChildren<ProjectDetailViewModel>(true);

            entryGo.transform.SetParent(detailContainer, false);

            project.ProjectDetailViewModel.name.text = project.Name;
        }

        private void RebuildDetail() {

        }

        private void CleanDetail() {
            foreach(Transform child in detailContainer) {
                Destroy(child.gameObject);
            }
        }
    }
}
