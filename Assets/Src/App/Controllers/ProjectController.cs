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

        private static string _persistentListDataPath;
        public static string PersistentListDataPath {
            get {
				return _persistentListDataPath;
            }
            set {
				_persistentListDataPath = value;
            }
        }

		private static string _persistentDetailDataPath;
		public static string PersistentDetailDataPath {
			get {
				return _persistentDetailDataPath;
			}
			set {
				_persistentDetailDataPath = value;
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

            AppController.OnBackButtonPressed += BackButtonPressed;
        }

        public Project GetById(string id) {
            return _projects.SingleOrDefault(p => p.Id == id);
        }

        public void Select(string id, bool build = true) {
            _selectedProjectId = id;

            if(OnSelectedProjectChange != null) {
                OnSelectedProjectChange(_selectedProjectId);
            }

            if(build) {
				GetDetail(id);
            }
        }

        private void GetAll() {
            _persistentListDataPath = CategoryController.PersistentDataPath + _selectedCategoryId + "/";
            string fileName = "projects.json";
			string fullPath = _persistentListDataPath + fileName;

			if(!Directory.Exists(_persistentListDataPath)) {
				Directory.CreateDirectory(_persistentListDataPath);
			}

			if(internetReachable) {
				string uri = CategoryController.CATEGORY_METHOD + "/" + _selectedCategoryId + "/" + PROJECT_METHOD;

                WebRequestManager.Get<Project>(uri, delegate (List<Project> result, string rawResult) {
                    _projects = result;

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

		private void GetDetail(string id) {
			_persistentDetailDataPath = AppController.PersistentDataPath + "/" + "Projects/" + id + "/";

			string fileName = "project.json";
			string fullPath = _persistentDetailDataPath + fileName;
			Project project = GetById(id);

			if(!Directory.Exists(_persistentDetailDataPath)) {
				Directory.CreateDirectory(_persistentDetailDataPath);
			}

			if(internetReachable) {
				string uri = (!string.IsNullOrEmpty(project.CategoryID)) ? 
					CategoryController.CATEGORY_METHOD + "/" + _selectedCategoryId + "/" + PROJECT_METHOD + "/" + id 
					: 
					PROJECT_METHOD + "/" + id;

				WebRequestManager.Get<Project> (uri, delegate(List<Project> result, string rawResult) {
					if(result.Count == 0) {
						return;
					}

					Project projectFull = result[0];

					using(FileStream st = File.Create(fullPath)) {
						byte[] data = new UTF8Encoding(true).GetBytes(rawResult);
						st.Write(data, 0, data.Length);
					}

					int index = _projects.IndexOf(project);
					_projects[index] = projectFull;


					if(_projects[index].SubProjects != null) {
						foreach(Project subProject in _projects[index].SubProjects) {
							Project existingSubProject = _projects.SingleOrDefault(p => p.Id == subProject.Id);

							if(existingSubProject != null) {
								continue;
							}

							_projects.Add(subProject);
						}
					}

					BuildDetail();
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

					int index = _projects.IndexOf(project);
					_projects[index] = wrapper.values[0];

					if(_projects[index].SubProjects != null) {
						foreach(Project subProject in _projects[index].SubProjects) {
							Project existingSubProject = _projects.SingleOrDefault(p => p.Id == subProject.Id);

							if(existingSubProject != null) {
								continue;
							}

							_projects.Add(subProject);
						}
					}
				}

				BuildDetail();
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
            //Build
            Project project = GetById(_selectedProjectId);
            GameObject entryGo = Instantiate<GameObject>(detailView);
            
            project.ProjectDetailViewModel = entryGo.GetComponentInChildren<ProjectDetailViewModel>(true);

            entryGo.transform.SetParent(detailContainer, false);

            project.ProjectDetailViewModel.name.text = project.Name;

			if(project.SubProjects != null) {
				int subProjectLength = project.SubProjects.Length;

				if(subProjectLength == 0) {
					project.ProjectDetailViewModel.subProjectContainer.SetActive(false);
				}

				for (int i = 0; i < subProjectLength; i++) {
					Project subProject = project.SubProjects [i];

					GameObject projectDetailSubProjectView = project.ProjectDetailViewModel.subProjectPrefab;

					GameObject entrySubProjectGo = Instantiate<GameObject>(projectDetailSubProjectView);

					entrySubProjectGo.transform.SetParent(project.ProjectDetailViewModel.subProjectContent, false);

					ProjectDetailSubProjectViewModel subProjectViewModel = entrySubProjectGo.GetComponentInChildren<ProjectDetailSubProjectViewModel>();
					subProjectViewModel.name.text = subProject.Name;

					subProjectViewModel.view.onClick.AddListener(delegate {
						Select(subProject.Id);
					});
				}
			}

			if(project.Medias != null) {
				Media[] scenes = project.Medias.Where (m => m.Type == Media.MediaType.SCENE).ToArray();
				Media[] images = project.Medias.Where (m => m.Type == Media.MediaType.IMAGE).ToArray();
				Media[] videos = project.Medias.Where (m => m.Type == Media.MediaType.VIDEO).ToArray();
				Media[] documents = project.Medias.Where (m => m.Type == Media.MediaType.DOCUMENT).ToArray();
				Media[] texts = project.Medias.Where(m => m.Type == Media.MediaType.TEXT).ToArray();

				if(texts != null) {
					int textLength = texts.Length;

					if(textLength == 0) {
						project.ProjectDetailViewModel.textContainer.SetActive(false);
					}

					for(int i = 0; i < textLength; i++) {
						Media text = texts [i];

						GameObject projectDetailTextView = project.ProjectDetailViewModel.textView;

						GameObject entryTextViewGO = Instantiate<GameObject>(projectDetailTextView);

						entryTextViewGO.transform.SetParent (project.ProjectDetailViewModel.textContent, false);

						ProjectDetailTextViewModel textViewModel = entryTextViewGO.GetComponentInChildren<ProjectDetailTextViewModel>();
						textViewModel.label.text = text.Key;
						textViewModel.value.text = text.Value;
					}
				}

				BuildDetailScenes(project, scenes);
				BuildDetailImages(project, images);
				BuildDetailVideos(project, videos);
				BuildDetailDocuments(project, documents);
			}
        }

		private void BuildDetailScenes(Project project, Media[] scenes) {
			int sceneLength = scenes.Length;

			if(sceneLength == 0) {
				project.ProjectDetailViewModel.scenePreviewTab.SetActive(false);
				project.ProjectDetailViewModel.scenePreviewContainer.SetActive(false);

				return;
			}
		}

		private void BuildDetailImages(Project project, Media[] images) {
			int imageLength = images.Length;

			if(imageLength == 0) {
				project.ProjectDetailViewModel.imagePreviewTab.SetActive(false);
				project.ProjectDetailViewModel.imagePreviewContainer.SetActive(false);

				return;
			}
		}

		private void BuildDetailVideos(Project project, Media[] videos) {
			int videoLength = videos.Length;

			if(videoLength == 0) {
				project.ProjectDetailViewModel.videoPreviewTab.SetActive(false);
				project.ProjectDetailViewModel.videoPreviewContainer.SetActive(false);

				return;
			}
		}

		private void BuildDetailDocuments(Project project, Media[] documents) {
			int documentLength = documents.Length;

			if(documentLength == 0) {
				project.ProjectDetailViewModel.documentPreviewTab.SetActive(false);
				project.ProjectDetailViewModel.documentPreviewContainer.SetActive(false);

				return;
			}
		}
			
        private void RebuildDetail() {}

        private void CleanDetail() {
            foreach(Transform child in detailContainer) {
                Destroy(child.gameObject);
            }
        }

        private void BackButtonPressed() {
            Project project = GetById(_selectedProjectId);

			if(string.IsNullOrEmpty(project.ParentID)) {
                CleanDetail();

                Select(null, false);
            }
            else {
                Destroy(project.ProjectDetailViewModel.gameObject);

                Select(project.ParentID, false);
            }
        }
    }
}