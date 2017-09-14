using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rise.App.Models {
	[System.Serializable]
	public class Package {
		[System.Serializable]
		public struct PlateformDef {
			bool web;
			bool desktop;
			bool ios;
			bool android;
		}

		[System.Serializable]
		public struct FeatureDef {
			bool virtualReality;
			bool augmentedReality;
			bool materialCustomisation;
		}

		[SerializeField]
		private string _id;
		public string Id {
			get {
				return _id;
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
		private PlateformDef plateform;
		public PlateformDef Plateform {
			get {
				return plateform;
			}
			set {
				plateform = value;
			}
		}

		[SerializeField]
		private FeatureDef feature;
		public FeatureDef Feature {
			get {
				return feature;
			}
			set {
				feature = value;
			}
		}
	}
}