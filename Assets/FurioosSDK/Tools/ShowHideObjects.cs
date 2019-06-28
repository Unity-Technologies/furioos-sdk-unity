using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FurioosSDK.Core;
using FurioosSDK.Cameras;

namespace FurioosSDK.Features.ShowHideObjects {
	public class ShowHideObjects : FSBehaviour {
		public GameObject[] objects;
		public string tagName;
		
		public List<FSCamera> hideIn;

		private GameObject[]taggedObjects;

		void Start() {
			if(tagName != null && tagName != "") {
				taggedObjects = GameObject.FindGameObjectsWithTag(tagName);
			}

			CamerasManager.CameraChanged += HandleCameraChanged;
			ShowHide(CamerasManager.Active);
		}

		void HandleCameraChanged(FSCamera camera) {
			ShowHide(camera);
		}

		private void ShowHide(FSCamera camera) {
			bool show = !hideIn.Contains(camera);
		
			foreach(GameObject objectToHide in objects){
				if(objectToHide != null) {
					objectToHide.SetActive(show);
				}
			}

			if(taggedObjects != null && taggedObjects.Length > 0) {
				foreach(GameObject objectToHide in taggedObjects) {
					if(objectToHide != null) {
						objectToHide.SetActive (show);
					}
				}
			}
		}
	}
}