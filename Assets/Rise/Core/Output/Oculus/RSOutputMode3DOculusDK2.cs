using UnityEngine;
using System.Collections;

namespace Rise.Core {
	public class RSOutputMode3DOculusDK2 : RSOutputMode {
		public float guiDistance = 2.0f;

		private GameObject leftCam, rightCam, guiObj;
		
		private OVRDevice oculusDevice;

		private Camera movingModeCamera;

		private float savedFOV;

		private RenderTexture uiTexture;

		public override void AttachToMovingMode(RSMovingMode movingMode){
			base.AttachToMovingMode(movingMode);
			
			if(movingMode!=null){
				GameObject cameraGameObject = movingMode.GetCameraGameObject();
				
				movingModeCamera = cameraGameObject.GetComponent<Camera>();

				savedFOV = movingModeCamera.fieldOfView;
				
				movingModeCamera.enabled = false;
				
				leftCam = new GameObject ("LeftCam");
				leftCam.AddComponent(typeof(Camera));
				leftCam.AddComponent<FlareLayer>();
				leftCam.AddComponent(typeof(OVRCamera));
				
				rightCam = new GameObject ("RightCam");
				rightCam.AddComponent(typeof(Camera));
				rightCam.AddComponent<FlareLayer>();
				rightCam.AddComponent(typeof(OVRCamera));

				
				CopyGameObjectComponents(cameraGameObject,leftCam);
				CopyGameObjectComponents(cameraGameObject,rightCam);
				
				CopyCameraParameters(movingModeCamera,leftCam.GetComponent<Camera>());
				CopyCameraParameters(movingModeCamera,rightCam.GetComponent<Camera>());
				
				leftCam.GetComponent<Camera>().depth = movingModeCamera.depth - 2;
				rightCam.GetComponent<Camera>().depth = movingModeCamera.depth - 1;
				
				leftCam.transform.parent = cameraGameObject.transform;
				rightCam.transform.parent = cameraGameObject.transform;
				
				leftCam.transform.localEulerAngles = new Vector3(0,0,0);
				rightCam.transform.localEulerAngles = new Vector3(0,0,0);

				OVRCameraController cameraController = (OVRCameraController)cameraGameObject.AddComponent(typeof(OVRCameraController));
				cameraController.FollowOrientation = cameraGameObject.transform.parent.transform;
				cameraController.TimeWarp = true;
				cameraController.NeckPosition.y = 0.0f;
				cameraController.FarClipPlane = movingModeCamera.farClipPlane;
				cameraController.Mirror = true;

				Quaternion savedRotation = movingMode.gameObject.transform.rotation;
				movingMode.gameObject.transform.rotation = Quaternion.identity;

				guiObj = (GameObject)Instantiate(Resources.Load("OVRGuiObjectMain"));
				Vector3 ls = guiObj.transform.localScale;
				Vector3 lp = guiObj.transform.localPosition;
				Quaternion lr = guiObj.transform.localRotation;

				cameraController.AttachGameObjectToCamera(ref guiObj);
				
				guiObj.transform.localScale = ls;
				guiObj.transform.localRotation = lr;

				float ipdOffsetDirection = 1.0f;
				Transform guiParent = guiObj.transform.parent;
				if (guiParent != null)
				{
					OVRCamera ovrCamera = guiParent.GetComponent<OVRCamera>();
					if (ovrCamera != null && ovrCamera.RightEye)
						ipdOffsetDirection = -1.0f;
				}
				
				float ipd = 0.0f;
				cameraController.GetIPD(ref ipd);
				lp.x += ipd * 0.5f * ipdOffsetDirection;
				guiObj.transform.localPosition = lp;

				movingMode.gameObject.transform.rotation = savedRotation;
				
				GetComponent<Camera>().depth =  movingModeCamera.depth + 1;
				GetComponent<Camera>().cullingMask = 0;
				GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
				GetComponent<Camera>().backgroundColor = new Color(0,0,0,1);
				GetComponent<Camera>().renderingPath = RenderingPath.VertexLit;


				if(UiCamera != null)
					UiCamera.clearFlags = CameraClearFlags.Color;
			}
		}

		public override void DetachFromMovingMode(){
			base.DetachFromMovingMode();

			CopyCameraParameters (rightCam.GetComponent<Camera>(), movingModeCamera);
			movingModeCamera.fieldOfView = savedFOV;

			if(leftCam!= null && leftCam.gameObject != null){
				DestroyImmediate(leftCam.gameObject);
				leftCam = null;
			}
			
			if(rightCam!=null && rightCam.gameObject != null){
				DestroyImmediate(rightCam.gameObject);
				rightCam = null;
			}
			
			if (movingModeCamera != null) {
				movingModeCamera.enabled = true;
				Destroy(movingModeCamera.gameObject.GetComponent(typeof(OVRCameraController)));
			}

			if (UiCamera != null) {
				UiCamera.clearFlags = CameraClearFlags.Depth;
				UiCamera.targetTexture = null;
			}
		}

		void Start(){
			if(UiCamera != null)
				uiTexture = new RenderTexture ((int)UiCamera.pixelWidth, (int)UiCamera.pixelHeight, 24);
		}

		void Update() {
			if(UiCamera != null)
				UiCamera.targetTexture = uiTexture;
		}

		public override void RenderImage(RenderTexture source, RenderTexture destination){

		}
		
		public override void RenderGui(RenderTexture guiTexture){
			guiObj.transform.localPosition = new Vector3(0.0f, 0.0f, guiDistance);

			guiObj.GetComponent<Renderer>().material.mainTexture = uiTexture;
		}
		
		public override void OnDestroy(){
			base.OnDestroy();
		}
	}
}