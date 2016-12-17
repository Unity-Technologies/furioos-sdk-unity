using UnityEngine;
using System.Collections;
using Rise.Core;

namespace Rise.Features.ImageEffects {
	[AddComponentMenu("Rise/Image Effects/Post Production")]
	public class PostProduction : RSImageEffect {
		
		//public float sharpenRadius = 0.00001f;
		//public float sharpenStrength = 100.0f;
		
		
		public Color black = new Color(0.125f,0.125f,0.0f,1.0f);
		public Color gamma = new Color(0.133f,0.117f,0.1f,1.0f);
		public Color white= new Color(0.854f,0.909f,1.0f,1.0f);
		
		public Texture  wadada;
		
		/// The limit on the minimum luminance (0...1) - we won't go above this.
		public float wadadaAlpha = 0.3f;
		
		public float saturation = 1.0f;

		// Called by camera to apply image effect
		void OnRenderImage (RenderTexture source, RenderTexture destination) {
			EffectMaterial.SetTexture ("_WadadaTex", wadada);
			EffectMaterial.SetFloat("_WadadaAlpha",wadadaAlpha);
			EffectMaterial.SetFloat("_Saturation",saturation);
			EffectMaterial.SetColor("_Black",black);
			EffectMaterial.SetColor("_Gamma",gamma);
			EffectMaterial.SetColor("_White",white);
			//material.SetFloat("_Radius",sharpenRadius);
			//material.SetFloat("_Strength",sharpenStrength);
			Graphics.Blit (source, destination, EffectMaterial);
		}
	}
}