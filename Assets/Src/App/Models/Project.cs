using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Rise.App.ViewModels;

namespace Rise.App.Models {
	[System.Serializable]
	public class Project {
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
        private string description;
        public string Description {
            get {
                return description;
            }
            set {
                description = value;
            }
        }

        [SerializeField]
        private Asset thumbnail;
        public Asset Thumbnail {
            get {
                return thumbnail;
            }
            set {
                thumbnail = value;
            }
        }

        [SerializeField]
		private string parentID;
		public string ParentID {
			get {
				return parentID;
			}
			set {
				parentID = value;
			}
		}

		[SerializeField]
		private string categoryID;
		public string CategoryID {
			get {
				return categoryID;
			}
			set {
				categoryID = value;
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

		[SerializeField]
		private Project[] subProjects;
		public Project[] SubProjects {
			get {
				return subProjects;
			}
			set {
				subProjects = value;
			}
		}

		[SerializeField]
		private Asset[] assets;
		public Asset[] Assets {
			get {
				return assets;
			}
			set {
				assets = value;
			}
		}

		//View models

        private ProjectViewModel _projectViewModel;
        public ProjectViewModel ProjectViewModel {
            get {
                return _projectViewModel;
            }
            set {
                _projectViewModel = value;
            }
        }

        private ProjectMenuItemViewModel _projectMenuItemViewModel;
        public ProjectMenuItemViewModel ProjectMenuItemViewModel {
            get {
                return _projectMenuItemViewModel;
            }
            set {
                _projectMenuItemViewModel = value;
            }
        }

        private ProjectDetailViewModel _projectDetailViewModel;
        public ProjectDetailViewModel ProjectDetailViewModel {
            get {
                return _projectDetailViewModel;
            }
            set {
                _projectDetailViewModel = value;
            }
        }
    }
}