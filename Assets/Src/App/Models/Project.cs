using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rise.Features.DataModel {
	[System.Serializable]
	public class Project {
		[SerializeField]
		private string _id;
		public string Id {
			get {
				return _id;
			}
			set {
				_id = value;
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
	}
}