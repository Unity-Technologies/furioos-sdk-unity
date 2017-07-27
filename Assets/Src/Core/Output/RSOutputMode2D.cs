using System.Collections;
using UnityEngine;
using Rise.UI;

namespace Rise.Core {
	public class RSOutputMode2D : RSOutputMode {

		public override void AttachToMovingMode(RSCamera movingMode){
			
			base.AttachToMovingMode(movingMode);
			
			if(movingMode!=null){
				GetComponent<Camera>().cullingMask = 0;
				GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
				GetComponent<Camera>().renderingPath = RenderingPath.VertexLit;
			}
			
		}
		
		public override void RenderImage(RenderTexture source, RenderTexture destination){
			return;
		}

		public override void RenderGui(RenderTexture guiTexture){
		}
	}
}