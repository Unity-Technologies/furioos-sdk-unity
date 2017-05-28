using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rise.Features.DataModel {
	[System.Serializable]
	public class Data {
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
		private string projectID;
		public string ProjectID {
			get {
				return projectID;
			}
			set {
				projectID = value;
			}
		}

		[SerializeField]
		private string type;
		public string Type {
			get {
				return type;
			}
			set {
				type = value;
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
	}
}