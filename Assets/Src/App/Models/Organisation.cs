using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rise.App.Models {
	[System.Serializable]
	public class Organisation {
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
	}
}