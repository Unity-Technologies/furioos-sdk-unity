using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rise.Core;
using Rise.Features.MovingMode;

namespace Rise.Features.ShowHideObjects {
	public class ShowHideObjects : RSBehaviour {

		public GameObject[] objects;
		public string tagName;
		
		public List<RSMovingMode> hideIn;

		private GameObject[]taggedObjects;

		void Start () {
			if(tagName != null && tagName != "")
				taggedObjects = GameObject.FindGameObjectsWithTag(tagName);

			MovingModesManager.MovingModeChanged += HandleMovingModeChanged;
			ShowHide(MovingModesManager.Active);
		}

		void HandleMovingModeChanged (RSMovingMode movingMode){
			ShowHide(movingMode);
		}

		private void ShowHide(RSMovingMode mm){
			bool show = !hideIn.Contains(mm);
		
			foreach(GameObject objectToHide in objects){
				if(objectToHide!=null)objectToHide.SetActive(show);
			}

			if (taggedObjects != null && taggedObjects.Length > 0) {
				foreach (GameObject objectToHide in taggedObjects) {
					if (objectToHide != null)
						objectToHide.SetActive (show);
				}
			}
		}
	}
}