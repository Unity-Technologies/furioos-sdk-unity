using System.Collections;
using UnityEngine;
using Rise.UI;

namespace Rise.Core {
	public class RSOutputMode2D : RSOutputMode {

		public override void AttachToMovingMode(RSMovingMode movingMode){
			
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
			GL.PushMatrix();
			GL.LoadPixelMatrix(0,OutputModesManager.RenderWidth,OutputModesManager.RenderHeight,0);

			Graphics.DrawTexture (new Rect(0, 0,OutputModesManager.RenderWidth, OutputModesManager.RenderHeight),
			                      RSSceneManager.GetInstance<RSUI>().UiTexture);

			GL.PopMatrix();
		}
	}
}