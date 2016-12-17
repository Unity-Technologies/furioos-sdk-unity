using UnityEngine;
using System.Collections;
using Rise.Core;

namespace Rise.Features.MovingMode {
	public class MovingModeMapCamera : RSMovingMode {
		public bool autoConfigure = true;
		public float distance = 100;
		public Vector3 rotation = new Vector3(90.0f, 0.0f, 0.0f);
		public LayerMask visibleLayers;

		private bool isInit = false;

		public void Initialize() {
			if (isInit)
				return; 
			
			GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
			GetComponent<Camera>().backgroundColor = new Color(230.0f / 255.0f, 230.0f / 255.0f, 230.0f / 255.0f, 0);
			
			if (!autoConfigure) 
				return;

			MeshCollider[] meshes = (MeshCollider[])Object.FindObjectsOfType(typeof(MeshCollider));		
			Bounds sceneBounds = new Bounds();
			
			float orthographicSize = 0;
			
			Vector3 minVector = Vector3.zero;
			Vector3 maxVector = Vector3.zero;
			
			foreach(MeshCollider mesh in meshes) {
				if(minVector == Vector3.zero) {
					minVector = mesh.bounds.min;	
				}
				if(maxVector == Vector3.zero) {
					maxVector = mesh.bounds.max;
				}
				
				minVector.x = (minVector.x > mesh.bounds.min.x) ? mesh.bounds.min.x : minVector.x;
				minVector.y = (minVector.y > mesh.bounds.min.y) ? mesh.bounds.min.y : minVector.y;
				minVector.z = (minVector.z > mesh.bounds.min.z) ? mesh.bounds.min.z : minVector.z;
				
				maxVector.x = (maxVector.x < mesh.bounds.max.x) ? mesh.bounds.max.x : maxVector.x;
				maxVector.y = (maxVector.y < mesh.bounds.max.y) ? mesh.bounds.max.y : maxVector.y;
				maxVector.z = (maxVector.z < mesh.bounds.max.z) ? mesh.bounds.max.z : maxVector.z;
			}
			
			sceneBounds.SetMinMax(minVector, maxVector);
				
			GetComponent<Camera>().orthographic = true;

			gameObject.transform.Rotate(rotation.x, rotation.y, rotation.y);

			transform.position = new Vector3 (sceneBounds.center.x, distance, sceneBounds.center.z);

			Vector3 worldPositionOfScreenCenter = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, 0.0f));
			
			Vector3 xVector = new Vector3(Mathf.Abs(minVector.x), Mathf.Abs(maxVector.x));
			Vector3 zVector = new Vector3(Mathf.Abs(minVector.z), Mathf.Abs(maxVector.z));
			
			if(xVector.magnitude > zVector.magnitude) {
				orthographicSize = maxVector.x - worldPositionOfScreenCenter.x;
			}
			else {
				orthographicSize = maxVector.z - worldPositionOfScreenCenter.z;
			}
			
			GetComponent<Camera>().orthographicSize = orthographicSize;
			GetComponent<Camera>().cullingMask = visibleLayers.value;

			isInit = true;
		}

		public override void Activate () {
			Initialize ();

			base.Activate ();
			transform.gameObject.SetActive(true);
		}
		
		public override void Desactivate () {
			base.Desactivate ();
			transform.gameObject.SetActive(false);
		}
	}
}