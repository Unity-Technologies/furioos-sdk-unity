using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rise.Core;
using Rise.SDK.Cameras;

namespace Rise.Features.ShowHideObjects {
	public class ShowHideObjects : RSBehaviour {
		public GameObject[] objects;
		public string tagName;
		
		public List<RSCamera> hideIn;

		private GameObject[]taggedObjects;

		void Start() {
			if(tagName != null && tagName != "") {
				taggedObjects = GameObject.FindGameObjectsWithTag(tagName);
			}

			CamerasManager.CameraChanged += HandleCameraChanged;
			ShowHide(CamerasManager.Active);
		}

		void HandleCameraChanged(RSCamera camera) {
			ShowHide(camera);
		}

		private void ShowHide(RSCamera camera) {
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