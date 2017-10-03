using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Rise.App.Models {
	[System.Serializable]
	public class Asset : ISerializationCallbackReceiver {
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
		private string projectID;
		public string ProjectID {
			get {
				return projectID;
			}
			set {
				projectID = value;
			}
		}
			
		public enum AssetType {
			SCENE,
			IMAGE,
			VIDEO,
			DOCUMENT,
			TEXT,
            THUMBNAIL
		}

		[SerializeField]
		private string type;
		private AssetType _type;
		public AssetType Type {
			get {
				return _type;
			}
			set {
				_type = value;
			}
		}

		[SerializeField]
		private string key;
		public string Key {
			get {
				return key;
			}
			set {
				key = value;
			}
		}

		[SerializeField]
		private string value;
		public string Value {
			get {
				return value;
			}
			set {
				this.value = value;
			}
		}

        [SerializeField]
        private string thumbnailID;
        public string ThumbnailID {
            get {
                return thumbnailID;
            }
            set {
                thumbnailID = value;
            }
        }

		[SerializeField]
		private string publicURL;
		public string PublicURL {
			get {
				return publicURL;
			}
			set {
                publicURL = value;
			}
		}

        public enum AssetPlateform {
            ALL,
            DESKTOP,
            MOBILE,
            WINDOWS,
            MAC,
            ANDROID,
            IOS,
            WEBGL
        }

        [SerializeField]
        private string plateform;
        private AssetPlateform _plateform;
        public AssetPlateform Plateform {
            get {
                return _plateform;
            }
            set {
                _plateform = value;
            }
        }

        public void OnBeforeSerialize() {}

		public void OnAfterDeserialize() {
            if(!string.IsNullOrEmpty(type)) {
                _type = (AssetType)System.Enum.Parse(typeof(AssetType), type);
            }

            if(!string.IsNullOrEmpty(plateform)) {
                _plateform = (AssetPlateform)System.Enum.Parse(typeof(AssetPlateform), plateform);
            }
        }
	}
}