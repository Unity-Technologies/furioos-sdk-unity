#pragma warning disable 0414
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Rise.Core;
using Rise.Features.MovingMode;

namespace Rise.Features.MiniMap {
	[System.Serializable]
	public class OBSMiniMap : RSBehaviour {
		public enum Mode {
			mini,
			full
		}
		
		public GameObject canvas;
		public GameObject miniMap;
		public int size = 2048;
		public LayerMask visibleLayers;
		public Mode mode;
		public Material mapMaterial;
		public Material markerMaterial;
		public RectTransform markerRectDrawer;
		public RawImage textureDrawer;
		public float zoomMini = 0.2f;
		public float zoomFull = 1.2f;
		public float fullMapSize = 3;


		private Texture2D textureBake;
		private GameObject renderCamera;

		private Vector2 referenceResolution;

		private Vector2 screenToCanvasRatio;
		private Vector2 canvasToMapRatio;
		
		public void Start() {
			textureBake = new Texture2D(size, size);

			referenceResolution = canvas.GetComponent<RectTransform>().sizeDelta;

			textureDrawer.material = mapMaterial;

			screenToCanvasRatio = new Vector2(
				Screen.width / referenceResolution.x,
				Screen.height / referenceResolution.y
			);

			Bake();
		}

		public override void OnDestroy(){
			base.OnDestroy();
		}
		
		
		public void Bake() {
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

			if(renderCamera == null) {
				renderCamera = new GameObject("MiniMapCameraRender");
				renderCamera.AddComponent<Camera>();
				
				renderCamera.GetComponent<Camera>().orthographic = true;
				
				renderCamera.transform.Rotate(90.0f, 0.0f, 0.0f);
				
				renderCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
				renderCamera.GetComponent<Camera>().backgroundColor = new Color(230.0f / 255.0f, 230.0f / 255.0f, 230.0f / 255.0f, 0);
			}

			renderCamera.SetActive(true);
			
			renderCamera.transform.position = new Vector3(sceneBounds.center.x, 100.0f, sceneBounds.center.z);
			
			Vector3 worldPositionOfScreenCenter = renderCamera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, 0.0f));
			
			Vector3 xVector = new Vector3(Mathf.Abs(minVector.x), Mathf.Abs(maxVector.x));
			Vector3 zVector = new Vector3(Mathf.Abs(minVector.z), Mathf.Abs(maxVector.z));
			
			if(xVector.magnitude > zVector.magnitude) {
				orthographicSize = maxVector.x - worldPositionOfScreenCenter.x;
			}
			else {
				orthographicSize = maxVector.z - worldPositionOfScreenCenter.z;
			}
			
			renderCamera.GetComponent<Camera>().orthographicSize = orthographicSize;
			renderCamera.GetComponent<Camera>().cullingMask = visibleLayers.value;
			
			RenderTexture renderTexture = new RenderTexture(textureBake.width, textureBake.height, 16,  RenderTextureFormat.Default);
			renderCamera.GetComponent<Camera>().targetTexture = renderTexture;
			renderCamera.GetComponent<Camera>().Render();
			RenderTexture.active = renderTexture;
			
			textureBake.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
			textureBake.Apply();
			textureBake.wrapMode = TextureWrapMode.Repeat;

			textureDrawer.texture = textureBake;
			
			renderCamera.SetActive(false);
		}

		public void Update() {
			Vector3 centerMapPosition;
			Vector3 playerScreenCoordinates;
			
			Matrix4x4 mapRotationMatrix = new Matrix4x4();
			Matrix4x4 translate = new Matrix4x4();
	        
			Quaternion rotation = Quaternion.Euler(0, 0, -GetComponent<MovingModeFPS>().FinalPan);
			
			playerScreenCoordinates = renderCamera.GetComponent<Camera>().WorldToScreenPoint(transform.position);
			
			translate.m00 = 1;
			translate.m11 = 1;
			translate.m22 = 1;
			translate.m33 = 1;
			translate.m03 = -0.5f;
			translate.m13 = -0.5f;
			
			if(mode == Mode.mini) {
				centerMapPosition = new Vector3(playerScreenCoordinates.x / textureBake.width, playerScreenCoordinates.y / textureBake.height, 0);
				mapRotationMatrix = Matrix4x4.TRS(centerMapPosition, rotation, new Vector3(zoomMini, zoomMini, zoomMini)) * translate;
				markerRectDrawer.anchoredPosition = new Vector2(0, 0);
				markerRectDrawer.eulerAngles = new Vector3(0, 0, 0);
			}
			else if(mode == Mode.full) {
				centerMapPosition = new Vector3((textureBake.width / 2.0f) / textureBake.width, (textureBake.height / 2.0f) / textureBake.height, 0);
				
				mapRotationMatrix = Matrix4x4.TRS(centerMapPosition, Quaternion.Euler(0, 0, 0), new Vector3(zoomFull, zoomFull, zoomFull))*translate;

				RectTransform mapRectTransform = miniMap.GetComponent<RectTransform> ();

				Vector2 markerPosition = new Vector2(0, 0);

				markerPosition.x = ((playerScreenCoordinates.x / textureBake.width) * mapRectTransform.sizeDelta.x * zoomFull) - mapRectTransform.sizeDelta.x / 2;
				markerPosition.y = -((((textureBake.height - playerScreenCoordinates.y) / textureBake.height) * mapRectTransform.sizeDelta.y * zoomFull) - mapRectTransform.sizeDelta.y / 2);

				markerRectDrawer.anchoredPosition = markerPosition;

				markerRectDrawer.eulerAngles = new Vector3(
					0,
					0,
					-GetComponent<MovingModeFPS>().FinalPan
				);
			}

			textureDrawer.material.SetMatrix("_Rotation", mapRotationMatrix);
		}

		public void SwitchMode() {
			if (mode == Mode.mini) {
				RectTransform rectTransform = miniMap.GetComponent<RectTransform> ();

				rectTransform.anchoredPosition = new Vector2(
					referenceResolution.x / 2,
					-referenceResolution.y / 2
				);

				rectTransform.sizeDelta = new Vector2(
					rectTransform.sizeDelta.x * fullMapSize,
					rectTransform.sizeDelta.y * fullMapSize
				);

				mode = Mode.full;
			} else {
				RectTransform rectTransform = miniMap.GetComponent<RectTransform> ();

				rectTransform.anchoredPosition = new Vector2(
					rectTransform.sizeDelta.x / 2 / 2,
					-rectTransform.sizeDelta.y / 2 / 2
				);

				rectTransform.sizeDelta = new Vector2(
					rectTransform.sizeDelta.x / fullMapSize,
					rectTransform.sizeDelta.y / fullMapSize
				);

				mode = Mode.mini;
			}
		}
	}
}