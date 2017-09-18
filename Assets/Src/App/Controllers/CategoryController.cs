using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using Rise.Core;
using Rise.App.Models;
using Rise.App.ViewModels;

namespace Rise.App.Controllers {
	public class CategoryController : RSBehaviour {
        public const string CATEGORY_METHOD = "categories";

        private static string _persistentDataPath;
        public static string PersistentDataPath {
            get {
                return _persistentDataPath;
            }
            set {
                _persistentDataPath = value;
            }
        }

        public delegate void SelectedCategoryChanged(string id);
        public static event SelectedCategoryChanged OnSelectedCategoryChange;


        //View
        public Text title;
        public Button backButton;
        public Transform container;
        public GameObject view;
        public GameObject message;

        private string _parentCategoryId;
        private string _selectedCategoryId;

        //Model
		private List<Category> _categories;
        public List<Category> Categories {
            get {
                return _categories;
            }
            set {
                _categories = value;
            }
        }

        public void Start() {
            _categories = new List<Category>();
            _parentCategoryId = null;
            _selectedCategoryId = null;

			onInternetReachabilityChange += delegate(bool internetReachability) {
				GetAll();
			};

			GetAll();
        }

        public Category GetById(string id) {
            return _categories.SingleOrDefault(c => c.Id == id);
        }

        public void Select(string id) {
            if(id == null) {
                _parentCategoryId = id;
                Rebuild();

                return;
            }

            Category category = GetById(id);
            
            if (!category.IsFolder) {
                _selectedCategoryId = id;

                if (OnSelectedCategoryChange != null) {
                    OnSelectedCategoryChange(_selectedCategoryId);
                }

                return;
            }
            else {
                _parentCategoryId = id;
            }

            Rebuild();
        }

        private void GetAll() {
            _persistentDataPath = AppController.PersistentDataPath + "Categories/";
            string fileName = "categories.json";
			string fullPath = _persistentDataPath + fileName;

			if(!Directory.Exists(_persistentDataPath)) {
				Directory.CreateDirectory(_persistentDataPath);
			}

            if (internetReachable) {
				string uri = CATEGORY_METHOD;

				WebRequestManager.Get<Category> (uri, delegate(List<Category> result, string rawResult) {
					_categories = result;

					using(FileStream st = File.Create(fullPath)) {
						byte[] data = new UTF8Encoding(true).GetBytes(rawResult);
						st.Write(data, 0, data.Length);
					}

                    for(int i = 0; i <_categories.Count; i++) {
                        Category category = _categories[i];

                        string categoryPersistentDataPath = _persistentDataPath + category.Id + "/";

                        Directory.CreateDirectory(categoryPersistentDataPath);
                    }

                    Build();
                });
			} 
			else {
                if(File.Exists(fullPath)) {
                    string json = "";

                    using(StreamReader sr = new StreamReader(fullPath)) {
                        json = sr.ReadToEnd();
                    }

                    JsonWrapper<List<Category>> wrapper = JsonUtility.FromJson<JsonWrapper<List<Category>>>(
                        json
                    );

                    _categories = wrapper.values;
                }

                Build();
            }
		}

        private void Build() {
            //Test
            if(_categories.Count == 0) {
                message.SetActive(true);

                return;
            }

            //Clean
            message.SetActive(false);
            foreach (Transform child in container) {
                Destroy(child.gameObject);
            }

            //Build
            int count = _categories.Count;
            for (int i = 0; i < count; i++) {
                Category category = _categories[i];
                GameObject entryG0 = Instantiate<GameObject>(view);
                category.ViewModel = entryG0.GetComponentInChildren<CategoryViewModel>(true);

                entryG0.transform.SetParent(container, false);

                category.ViewModel.name.text = category.Name;

                category.ViewModel.button.onClick.AddListener(delegate() {
                    Select(category.Id);
                });

                if(!string.IsNullOrEmpty(category.Parent)) {
                    entryG0.SetActive(false);

                    Category parentCategory = GetById(category.Parent);
                    parentCategory.ViewModel.indicator.SetActive(true);
                    parentCategory.IsFolder = true;
                }
            }
        }

        private void Rebuild() {
            List<Category> categories = null;

            //Hide all
            for(int i = 0; i < _categories.Count; i++) {
                _categories[i].ViewModel.gameObject.SetActive(false);
            }

            if(string.IsNullOrEmpty(_parentCategoryId)) {
                categories = _categories.Where(c => string.IsNullOrEmpty(c.Parent)).ToList();

                backButton.gameObject.SetActive(false);
                title.text = "Categories";
            }
            else {
                categories = _categories.Where(c => c.Parent == _parentCategoryId).ToList();
                Category category = GetById(_parentCategoryId);

                backButton.gameObject.SetActive(true);
               
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(delegate() {
                    Select(
                        (category.Parent == null) ? null : category.Parent
                    );
                });

                title.text = category.Name;
            }

            //Show
            for(int i = 0; i < categories.Count; i++) {
                categories[i].ViewModel.gameObject.SetActive(true);
            }
        }
    }
}