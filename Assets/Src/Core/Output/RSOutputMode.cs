using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Rise.Core {
	[RequireComponent(typeof(Camera))]
	public abstract class RSOutputMode : RSUniqueBehaviour {

		public delegate void OnActivatedHandler( RSOutputMode mm );
		public delegate void OnDesactivatedHandler( RSOutputMode mm );

		public event OnActivatedHandler OnPreactivate;
		public event OnActivatedHandler OnActivated;
		public event OnDesactivatedHandler OnDesactivated;

		public RSMovingMode AttachedTo{
			get ;
			private set;
		}

		public virtual void Awake ()
		{
			base.Init();
		}
		
		public virtual void RenderImage(RenderTexture destination, RenderTexture source = null){

		}

		public virtual void RenderGui(RenderTexture guiTexture){

		}

		public virtual void AttachToMovingMode(RSMovingMode movingMode){
			//Debug.Log("Attaching " + ToString()+" to "+movingMode.ToString());
		}
		

		public virtual void DetachFromMovingMode(){
			//Debug.Log("Detaching " + ToString()+" from moving mode");
		}

		void OnEnable(){
			if(OnPreactivate!=null)OnPreactivate(this);
			//Debug.Log("Enabling output mode " + ToString());
			if(OnActivated!=null)OnActivated(this);
		}

		void OnDisable(){
			//Debug.Log("Disabling output mode " + ToString());
			if(OnDesactivated!=null)OnDesactivated(this);
		}

		public void CopyCameraParameters(Camera src, Camera dst){
			dst.depth = src.depth;
			dst.nearClipPlane = src.nearClipPlane;
			dst.farClipPlane = src.farClipPlane;
			dst.fieldOfView  = src.fieldOfView ;
			dst.cullingMask  = src.cullingMask ;
			dst.clearFlags = src.clearFlags;
			dst.allowHDR = src.allowHDR;
			dst.renderingPath = src.renderingPath;
			dst.backgroundColor = src.backgroundColor;
			dst.orthographic = src.orthographic;
		}
		
		public void CopyGameObjectComponents(GameObject src,GameObject dst){
			
			Component[] components = src.GetComponents(typeof(Component));
			
			for(int i = 0; i < components.Length; i++)
			{
				Component component = components[i];
				System.Type type = component.GetType();
				
				if(type.IsSubclassOf(typeof(MonoBehaviour) )){// && type!= typeof(CameraStereoscoper)){
					
					Component newComponent = dst.AddComponent(type);
					
					foreach (FieldInfo f in type.GetFields())
					{
						f.SetValue(newComponent, f.GetValue(component));
					}
				}
			}
			
		}

		public void BuildProjectionMatrix(Camera cam,float camOffset, float fov, float aspect, float windowDistance){
			if(fov<10 || fov>170){
				Debug.LogWarning("Field of view was corrected to 60 because it's value was strange ("+fov+")");
				fov=60;
			}
			if(aspect<=float.Epsilon && aspect>=-float.Epsilon){
				Debug.LogWarning("Aspect was corrected to 1.3333 because it's value was strange ("+aspect+")");
				aspect = 1.3333f;
			}

			float left,right,top,bottom,width,height,offset;
			
			left = 0.0f;
			right = 0.0f;
			
			offset = cam.nearClipPlane * camOffset / windowDistance;
			
			top = cam.nearClipPlane * Mathf.Tan(fov*Mathf.PI/360.0f);
			bottom = -top;
			height = top-bottom;
			
			left = bottom * aspect  + offset;
			right = top * aspect + offset;
			
			width = right-left;
			
			Matrix4x4 projection = new Matrix4x4();
			
			projection.m00 = 2.0f*cam.nearClipPlane/width;
			projection.m01 = 0.0f;
			projection.m02 = (right+left)/width;
			projection.m03 = 0.0f;
			
			projection.m10 = 0.0f;
			projection.m11 = 2.0f*cam.nearClipPlane/height;
			projection.m12 = (top+bottom)/height;
			projection.m13 = 0.0f;
			
			projection.m20 = 0.0f;
			projection.m21 = 0.0f;
			projection.m22 = -(cam.farClipPlane+cam.nearClipPlane)/(cam.farClipPlane-cam.nearClipPlane);
			projection.m23 = -(2.0f*cam.farClipPlane*cam.nearClipPlane)/(cam.farClipPlane-cam.nearClipPlane);
			
			projection.m30 = 0.0f;
			projection.m31 = 0.0f;
			projection.m32 = -1.0f;
			projection.m33 = 0.0f;

			cam.projectionMatrix=projection;
		}
	}
}