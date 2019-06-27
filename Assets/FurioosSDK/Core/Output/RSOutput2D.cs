using System.Collections;
using UnityEngine;
using Rise.UI;

namespace Rise.Core {
	public class RSOutput2D : RSOutput {
		public override void UpdateCamera(RSCamera camera) {
			base.UpdateCamera(camera);
			
			if(camera != null) {
				GetComponent<Camera>().cullingMask = 0;
				GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
				GetComponent<Camera>().renderingPath = RenderingPath.VertexLit;
			}
		}
	}
}