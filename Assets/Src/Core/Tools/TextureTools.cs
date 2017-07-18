using UnityEngine;
using System.Collections;

public class TextureTools  {

	public static void DestroyRenderTexture (RenderTexture renderTexture)
	{
		if(renderTexture!=null){
			if(RenderTexture.active == renderTexture)RenderTexture.active = null;
			UnityEngine.Object.Destroy(renderTexture);
			renderTexture = null;
		}
	}
}
