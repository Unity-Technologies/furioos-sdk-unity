using System.Collections;
using UnityEngine;

namespace Rise.Core {
	public class RSOutput3DSplitted : RSOutput {

		public enum SplitMode { OverUnder, SideBySide};

		public SplitMode stereoscopicMode = SplitMode.SideBySide ;
		public float stereoBase = 0.07f ;
		public float windowDistance = 2.0f ;
		public SplitMode splitMode = SplitMode.OverUnder;
		private SplitMode lastSplitMode = SplitMode.OverUnder;

		public float timeBeforeHidUIInMobile = 3.0f;
		private float lastTouchTime = 0.0f;

		private RenderTexture leftTexture ;
		private RenderTexture rightTexture ;
		private Rect leftRect;
		private Rect rightRect;

		private Shader normalShader;

		private RenderTexture uiTexture;
		
		private GameObject leftCam, rightCam;
		
		private Camera movingModeCamera;

		private void AssignRenderTextures() {
			if(leftCam != null) {
				leftCam.GetComponent<Camera>().targetTexture = leftTexture;
				leftCam.GetComponent<Camera>().enabled = leftTexture != null;
			}
			if(rightCam != null) {
				rightCam.GetComponent<Camera>().targetTexture = rightTexture;
				rightCam.GetComponent<Camera>().enabled = rightTexture != null;
			}
		}

		public override void AttachToMovingMode(RSCamera movingMode) {
			base.AttachToMovingMode(movingMode);

			if(movingMode!=null) {
				GameObject cameraGameObject = movingMode.GetCameraGameObject();
				movingModeCamera = cameraGameObject.GetComponent<Camera>();

				leftCam = new GameObject ("leftCam");
				leftCam.AddComponent(typeof(Camera));
				leftCam.AddComponent<FlareLayer>();

				rightCam = new GameObject ("rightCam");
				rightCam.AddComponent(typeof(Camera));
				rightCam.AddComponent<FlareLayer>();

				AssignRenderTextures();
				
				CopyGameObjectComponents(cameraGameObject,leftCam);
				CopyGameObjectComponents(cameraGameObject,rightCam);
				
				CopyCameraParameters(movingModeCamera,leftCam.GetComponent<Camera>());
				CopyCameraParameters(movingModeCamera,rightCam.GetComponent<Camera>());
				
				leftCam.GetComponent<Camera>().depth = movingModeCamera.depth - 2;
				rightCam.GetComponent<Camera>().depth = movingModeCamera.depth - 1;
				
				leftCam.transform.parent = cameraGameObject.transform;
				rightCam.transform.parent = cameraGameObject.transform;

				leftCam.transform.localEulerAngles = new Vector3(0, 0, 0);
				rightCam.transform.localEulerAngles = new Vector3(0, 0, 0);

				GetComponent<Camera>().depth =  movingModeCamera.depth + 1;
				GetComponent<Camera>().cullingMask = 0;
				GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
				GetComponent<Camera>().backgroundColor = new Color(0, 0, 0, 1);
				GetComponent<Camera>().renderingPath = RenderingPath.VertexLit;

				movingModeCamera.enabled = false;
			}
		}

		public override void DetachFromMovingMode() {
			base.DetachFromMovingMode();

			if(movingModeCamera!=null) {
				movingModeCamera.enabled = true;
			}

			if(leftCam != null && leftCam.gameObject != null) {
				DestroyImmediate(leftCam.gameObject);
				leftCam = null;
			}
			
			if(rightCam !=null && rightCam.gameObject != null) {
				DestroyImmediate(rightCam.gameObject);
				rightCam = null;
			}
		}

		void Update() {
			RSOutputManager omm = OutputsManager;

			switch(splitMode) {
			case SplitMode.OverUnder:
				leftRect = new Rect(0, 0, omm.RenderWidth, omm.RenderHeight / 2);
				rightRect = new Rect(0, omm.RenderHeight / 2, omm.RenderWidth, omm.RenderHeight - leftRect.height);
				break;
			case SplitMode.SideBySide:
				leftRect = new Rect(0, 0, omm.RenderWidth / 2, omm.RenderHeight);
				rightRect = new Rect(omm.RenderWidth / 2, 0, omm.RenderWidth - leftRect.width, omm.RenderHeight);
				break;
			}
			
			bool splitModeChanged = splitMode != lastSplitMode;
			lastSplitMode = splitMode;
			
			int aa = QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing;
			
			if(leftTexture != null && (leftRect.width!=leftTexture.width || leftRect.height!=leftTexture.height || leftTexture.antiAliasing != aa || splitModeChanged)) {
				TextureTools.DestroyRenderTexture(leftTexture);
				AssignRenderTextures();
			}
			if(rightTexture != null && (rightRect.width!=rightTexture.width || rightRect.height!=rightTexture.height || rightTexture.antiAliasing!= aa || splitModeChanged)) {
				TextureTools.DestroyRenderTexture(rightTexture);
				AssignRenderTextures();
			}

			if(leftTexture == null) {
				if(leftRect.width > 0 && leftRect.height > 0) {
					Debug.Log ("Initialising left render texture to " + leftRect.width + " x " + leftRect.height);
					leftTexture = new RenderTexture((int)leftRect.width, (int)leftRect.height, 24, RenderTextureFormat.ARGB32);
					leftTexture.antiAliasing = aa;
					leftTexture.useMipMap = false;
					leftTexture.filterMode = FilterMode.Point;
					AssignRenderTextures();
				}
			}

			if(rightTexture == null) {
				if(rightRect.width > 0 && rightRect.height > 0) {
					Debug.Log ("Initialising right render texture to "+rightRect.width+" x "+rightRect.height);
					rightTexture = new RenderTexture((int)rightRect.width,(int)rightRect.height, 24, RenderTextureFormat.ARGB32);
					rightTexture.antiAliasing = aa;
					rightTexture.useMipMap = false;
					rightTexture.filterMode = FilterMode.Point;
					AssignRenderTextures();
				}
			}

			if(leftCam != null && rightCam != null && omm.RenderHeight > 0) {
				leftCam.transform.localPosition = new Vector3(-stereoBase/2, 0, 0);
				rightCam.transform.localPosition = new Vector3(stereoBase/2, 0, 0);

				float ratio = (float)omm.RenderWidth / (float)omm.RenderHeight;

				if(omm.RenderWidth>0 && omm.RenderHeight>0) {
					BuildProjectionMatrix(leftCam.GetComponent<Camera>(), stereoBase * 0.5f, movingModeCamera.fieldOfView, ratio, windowDistance);
					BuildProjectionMatrix(rightCam.GetComponent<Camera>(), stereoBase * -0.5f, movingModeCamera.fieldOfView, ratio, windowDistance);
				}
			}
		}

		public override void RenderImage(RenderTexture source, RenderTexture destination){
			GL.PushMatrix();
			GL.LoadPixelMatrix(0,OutputsManager.RenderWidth, OutputsManager.RenderHeight, 0);
			
			RenderTexture.active = destination;
			
			if(leftTexture != null) {
				Graphics.DrawTexture(leftRect, leftTexture);
			}

			if(rightTexture != null) {
				Graphics.DrawTexture(rightRect, rightTexture);
			}
			
			GL.PopMatrix();
		}


		
		public override void RenderGui(RenderTexture guiTexture) {
			if (uiTexture == null || RSManager.Output == Outputs.Cardboard)
				return;

			GL.PushMatrix();
			GL.LoadPixelMatrix(0, OutputsManager.RenderWidth, OutputsManager.RenderHeight, 0);

			if (Manager.IsMobileDevice) {
				if(InputManager.HasGoneActive) {
					lastTouchTime = Time.time;
				}

				if(Time.time - lastTouchTime > timeBeforeHidUIInMobile) {
					return;
				}
			}

			Graphics.DrawTexture(leftRect, uiTexture);
			Graphics.DrawTexture(rightRect, uiTexture);
			
			GL.PopMatrix();
		}

		public override void OnDestroy() {
			TextureTools.DestroyRenderTexture(leftTexture);
			TextureTools.DestroyRenderTexture(rightTexture);
			base.OnDestroy();
		}
	}
}