using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Rise.App.ViewModels;

namespace Rise.App.Models {
	[System.Serializable]
	public class Category {
		[SerializeField]
		private string id;
		public string Id {
			get {
				return id;
			}
			set {
				id = value;
			}
		}

		[SerializeField]
		private string name;
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}

		[SerializeField]
		private string parent;
		public string Parent {
			get {
				return parent;
			}
			set {
				parent = value;
			}
		}

		[SerializeField]
		private string organisationID;
		public string OrganisationID {
			get {
				return organisationID;
			}
			set {
				organisationID = value;
			}
		}

        private bool _isFolder = false;
        public bool IsFolder {
            get {
                return _isFolder;
            }
            set {
                _isFolder = value;
            }
        }

        private CategoryViewModel _viewModel;
        public CategoryViewModel ViewModel {
            get {
                return _viewModel;
            }
            set {
                _viewModel = value;
            }
        }
	}
}