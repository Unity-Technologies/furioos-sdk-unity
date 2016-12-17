using UnityEngine;
using System.Collections;

namespace Rise.Core {
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu("")]
	public class RSImageEffect
		: RSBehaviour
	{
		/// Provides a shader property that is set in the inspector
		/// and a material instantiated from the shader
		public Shader   shader;
		private Material m_Material;

		protected override void Init(){
			
			base.Init();
			string shaderPath = "Hidden/Observ3d/"+GetType().ToString();
			if(shader == null)shader = Shader.Find(shaderPath);
			
			if(shader == null) Debug.LogError("Can't find shader \""+ shaderPath + "\" in " + GetType().ToString() + "(" + id +")");
			
			// Disable if we don't support image effects
			if (!SystemInfo.supportsImageEffects) {
				enabled = false;
				return;
			}
			
			// Disable the image effect if the shader can't
			// run on the users graphics card

			if (!shader || !shader.isSupported)
				enabled = false;
		
		}

		public Material EffectMaterial {
			get {
				if (m_Material == null && shader!=null) {
					m_Material = new Material (shader);
					m_Material.hideFlags = HideFlags.HideAndDontSave;
				}
				return m_Material;
			} 
		}
		
		protected virtual void OnDisable() {
			if( m_Material ) {
				DestroyImmediate( m_Material );
			}
		}
		/*
		public void SetShader(Shader s) {
			shader = s;
			Init();
		}
		*/
	}
}