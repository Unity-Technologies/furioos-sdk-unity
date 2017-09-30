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
        public const string MEDIA_METHOD = "medias";

        public delegate void HandleImageCallback(Texture2D image);
        public delegate void HandleVideoCallback(string path);

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

        private static string _persistentMediasDataPath;
        public static string PersistentMediasDataPath {
            get {
                return _persistentMediasDataPath;
            }
            set {
                _persistentMediasDataPath = value;
            }
        }

        public delegate void SelectedProjectHasChanged(string id);
        public static event SelectedProjectHasChanged OnSelectedProjectChange;

        //View
        public GameObject container;

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

            LoadingViewModel loading = AppController.CreateLoading(container, true, true);

			if(internetReachable) {
				string uri = CategoryController.CATEGORY_METHOD + "/" + _selectedCategoryId + "/" + PROJECT_METHOD;

                WebRequestManager.Get<Project>(uri, delegate (List<Project> result, string rawResult) {
                    _projects = result;

                    using(FileStream st = File.Create(fullPath)) {
                        byte[] data = new UTF8Encoding(true).GetBytes(rawResult);
                        st.Write(data, 0, data.Length);
                    }

                    loading.FadeOut();

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

                loading.FadeOut();

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

            LoadingViewModel loading = AppController.CreateLoading(container, true, true);

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

                    loading.FadeOut();

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

                loading.FadeOut();

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

                if(string.IsNullOrEmpty(project.Thumbnail.PublicURL)) {
                    continue;
                }

                LoadingViewModel loadingViewModel = AppController.CreateLoading(project.ProjectViewModel.gameObject);
                
                HandleImage(project.Thumbnail,
                    delegate(Texture2D image) {
                        float ratio = (float)image.width / (float)image.height;

                        project.ProjectViewModel.image.texture = image;
                        project.ProjectViewModel.aspectRatioFitter.aspectRatio = ratio;

                        project.ProjectMenuItemViewModel.image.texture = image;
                        project.ProjectMenuItemViewModel.aspectRatioFitter.aspectRatio = ratio;

                        loadingViewModel.Destroy();
                    },
                    delegate(float progress) {
                        loadingViewModel.progress.fillAmount = progress;
                    }
                );
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
                _persistentMediasDataPath = _persistentDetailDataPath + "Medias/";

                if(!Directory.Exists(_persistentMediasDataPath)) {
                    Directory.CreateDirectory(_persistentMediasDataPath);
                }

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

				//BuildScenes(project, scenes);
                BuildImages(project, images);
                BuildVideos(project, videos);
                //BuildDocuments(project, documents);
			}
        }

		private void HandleScenes(Project project, Media[] scenes) {
			int sceneLength = scenes.Length;

			if(sceneLength == 0) {
				project.ProjectDetailViewModel.scenePreviewTab.SetActive(false);
				project.ProjectDetailViewModel.scenePreviewContainer.SetActive(false);

				return;
			}
		}

        private void BuildImages(Project project, Media[] images) {
            int imageLength = images.Length;

            if(imageLength == 0) {
                project.ProjectDetailViewModel.imagePreviewTab.SetActive(false);
                project.ProjectDetailViewModel.imagePreviewContainer.SetActive(false);

                return;
            }

            for(int i = 0; i < imageLength; i++) {
                GameObject mediaPreviewView = Instantiate<GameObject>(project.ProjectDetailViewModel.imagePreviewView);
                GameObject mediaPreviewIndicatorView = Instantiate<GameObject>(project.ProjectDetailViewModel.imageIndicatorView);

                ProjectDetailMediaPreviewViewModel mediaPreviewViewModel = mediaPreviewView.GetComponentInChildren<ProjectDetailMediaPreviewViewModel>();
                ProjectDetailMediaIndicatorViewModel mediaPreviewIndicatorViewModel = mediaPreviewIndicatorView.GetComponentInChildren<ProjectDetailMediaIndicatorViewModel>();

                mediaPreviewView.transform.SetParent(project.ProjectDetailViewModel.imagePreviewsContent, false);
                mediaPreviewIndicatorView.transform.SetParent(project.ProjectDetailViewModel.imageIndicatorsContent, false);

                mediaPreviewViewModel.title.text = images[i].Name;
                mediaPreviewViewModel.titleImageIcon.SetActive(true);

                if(string.IsNullOrEmpty(images[i].PublicURL)) {
                    continue;
                }

                LoadingViewModel lvmp = AppController.CreateLoading(mediaPreviewViewModel.gameObject);
                LoadingViewModel lvmpi = AppController.CreateLoading(mediaPreviewIndicatorViewModel.gameObject);

                HandleImage(images[i],
                    delegate(Texture2D image) {
                        float ratio = (float)image.width / (float)image.height;
                        mediaPreviewViewModel.image.texture = image;
                        mediaPreviewViewModel.aspectRatioFitter.aspectRatio = ratio;

                        mediaPreviewIndicatorViewModel.image.texture = image;
                        mediaPreviewIndicatorViewModel.aspectRatioFitter.aspectRatio = ratio;

                        lvmp.Destroy();
                        lvmpi.Destroy();
                    },
                    delegate(float progress) {
                        lvmp.progress.fillAmount = progress;
                        lvmpi.progress.fillAmount = progress;
                    }
                );
            }
        }

        private void HandleImage(Media imageData, HandleImageCallback callback, RSWebRequestManager.DownloadProgressCallBack progressCallback) {
            if(string.IsNullOrEmpty(imageData.PublicURL)) {
                return;
            }

            System.Uri uri = new System.Uri(imageData.PublicURL);
            string filename = Path.GetFileName(uri.AbsolutePath);

            string imagePersistentPath = _persistentMediasDataPath + filename;

            if(File.Exists(imagePersistentPath)) {
                byte[] rawImage = File.ReadAllBytes(imagePersistentPath);

                Texture2D img = new Texture2D(1, 1);
                img.LoadImage(rawImage);

                callback(img);
            }
            else {
                if(internetReachable) {
                    WebRequestManager.Download(imageData.PublicURL, delegate (byte[] rawResult) {
                        File.WriteAllBytes(imagePersistentPath, rawResult);

                        Texture2D img = new Texture2D(1, 1);
                        img.LoadImage(rawResult);

                        callback(img);
                    },
                    delegate(float progress) {
                        progressCallback(progress);
                    });
                }
            }
        }

        private void BuildVideos(Project project, Media[] videos) {
			int videoLength = videos.Length;

			if(videoLength == 0) {
				project.ProjectDetailViewModel.videoPreviewTab.SetActive(false);
				project.ProjectDetailViewModel.videoPreviewContainer.SetActive(false);

				return;
			}

            for(int i = 0; i < videoLength; i++) {
                GameObject mediaPreviewView = Instantiate<GameObject>(project.ProjectDetailViewModel.imagePreviewView);
                GameObject mediaPreviewIndicatorView = Instantiate<GameObject>(project.ProjectDetailViewModel.imageIndicatorView);

                ProjectDetailMediaPreviewViewModel mediaPreviewViewModel = mediaPreviewView.GetComponentInChildren<ProjectDetailMediaPreviewViewModel>();
                ProjectDetailMediaIndicatorViewModel mediaPreviewIndicatorViewModel = mediaPreviewIndicatorView.GetComponentInChildren<ProjectDetailMediaIndicatorViewModel>();

                mediaPreviewView.transform.SetParent(project.ProjectDetailViewModel.videoPreviewsContent, false);
                mediaPreviewIndicatorView.transform.SetParent(project.ProjectDetailViewModel.videoIndicatorsContent, false);

                mediaPreviewViewModel.title.text = videos[i].Name;
                mediaPreviewViewModel.videoIcon.SetActive(true);
                mediaPreviewViewModel.titleVideoIcon.SetActive(true);

                if(string.IsNullOrEmpty(videos[i].PublicURL)) {
                    continue;
                }

                LoadingViewModel lvmp = AppController.CreateLoading(mediaPreviewViewModel.gameObject);
                LoadingViewModel lvmpi = AppController.CreateLoading(mediaPreviewIndicatorViewModel.gameObject);

                HandleVideo(videos[i],
                    delegate (string path) {
                        mediaPreviewViewModel.view.onClick.AddListener(() => {
                            VideoPlayerController.Play(path);
                        });

                        lvmp.Destroy();
                        lvmpi.Destroy();
                    },
                    delegate (float progress) {
                        lvmp.progress.fillAmount = progress;
                        lvmpi.progress.fillAmount = progress;
                    }
                );
            }
        }

        private void HandleVideo(Media videoData, HandleVideoCallback callback, RSWebRequestManager.DownloadProgressCallBack progressCallback) {
            if(string.IsNullOrEmpty(videoData.PublicURL)) {
                return;
            }

            System.Uri uri = new System.Uri(videoData.PublicURL);
            string filename = Path.GetFileName(uri.AbsolutePath);

            string videoPersistentPath = _persistentMediasDataPath + filename;

            if(File.Exists(videoPersistentPath)) {
                callback(videoPersistentPath);
            }
            else {
                if(internetReachable) {
                    WebRequestManager.Download(videoData.PublicURL, delegate (byte[] rawResult) {
                        File.WriteAllBytes(videoPersistentPath, rawResult);

                        callback(videoPersistentPath);
                    },
                    delegate (float progress) {
                        progressCallback(progress);
                    });
                }
            }
        }

        private void HandleDocuments(Project project, Media[] documents) {
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