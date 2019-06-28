using System.Collections;
using UnityEngine;
using FurioosSDK.UI;

namespace FurioosSDK.Core {
	public class FSOutput2D : FSOutput {
		public override void UpdateCamera(FSCamera camera) {
			base.UpdateCamera(camera);
			
			if(camera != null) {
				GetComponent<Camera>().cullingMask = 0;
				GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
				GetComponent<Camera>().renderingPath = RenderingPath.VertexLit;
			}
		}
	}
}