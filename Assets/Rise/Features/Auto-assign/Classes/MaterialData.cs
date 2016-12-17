using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Rise.Features.AutoAssign {

	public class MaterialData {
		public bool doubleSided;
		
		public Vector4 diffuseColor;
		public Texture2D diffuseMap;
		public string diffuseMapName;
		
		public float opacityLevel;
		public Texture2D opacityMap;
		public string opacityMapName;
		
		public float bumpLevel;
		public Texture2D bumpMap;
		public string bumpMapName;
		
		public float selfillumLevel;
		public Texture2D selfillumMap;
		public string selfillumMapName;
		
		public float occlusionLevel;
		public Texture2D occlusionMap;
		public string occlusionMapName;
		
		public Texture2D mixedMap;
		public string mixedMapName;
		
		public float reflectGloss;
		public float tangentReflectLevel;
		public float normalReflectLevel;
		public Vector4 reflectColor;
		
		
		public bool isLightResponsive;
		
		public MaterialData(){
			
			diffuseColor = new Vector4(1.0f,1.0f,1.0f,1.0f);
			diffuseMap = null;
			diffuseMapName = null;
			
			opacityLevel = 1.0f;
			opacityMap = null;
			opacityMapName = null;
			
			bumpLevel = 1.0f;
			bumpMap = null;
			bumpMapName = null;
			
			selfillumLevel = 1.0f;
			selfillumMap = null;
			selfillumMapName = null;
			
			occlusionLevel = 1.0f;
			occlusionMap = null;
			occlusionMapName = null;
			
			reflectGloss = 1.0f;
			tangentReflectLevel = 0.0f;
			normalReflectLevel = 0.0f;
			reflectColor = new Vector4(1.0f,1.0f,1.0f,1.0f);
			
			
			isLightResponsive = false;
		}
		
		private Shader FindShader(string path){
			Shader shader = Shader.Find(path);
			//if(shader == null)EditorUtility.DisplayDialog("Auto assignation : 404 Shader not found", "The shader \""+ path+ "\" could not be found, aborting...", "OK");
			return shader;
			
		}
		
		public string GetShaderName(bool useLegacyShaders){
			
			string shadername = "Observ3d/";
			
			if (!useLegacyShaders) {
				return shadername + "Standard";
			}
			
			if(IsLightResponsive)shadername += "Light Responsive/";
			else shadername += "Light Independent/";
			
			if(IsTextured)shadername += "Textured/";
			else shadername += "Color/";
			
			if(IsTransparent)shadername += "Transparent/";
			else shadername += "Opaque/";
			
			if(IsLightResponsive)shadername += "LR";
			else shadername += "LI";
			
			if(IsTextured)shadername += "-Texture";
			else shadername += "-Color";
			
			if(IsTransparent)shadername += "-Alpha";
			if(IsLightMapped)shadername += "-Lightmap";
			if(IsBumped)shadername += "-Bump";
			if(IsReflecting)shadername += "-Reflect";
			if(IsMasked)shadername += "-Mask";
			if(IsCutout)shadername += "-Cutout";
			
			if(doubleSided)shadername += "-DS";
			
			return shadername;
			
		}
		
		public Material CreateMaterial(Cubemap defaultCubemapTexture, bool useLegacyShaders)
		{
			Material mat = null;
			Shader shdr;
			
			if ((shdr = FindShader (GetShaderName (useLegacyShaders))) != null) {
				mat = new Material (shdr);
				
				if(useLegacyShaders) {
					mat = AssignLegacyShader(defaultCubemapTexture, mat);
				}
				else {
					mat = AssignStandardShader(defaultCubemapTexture, mat);
				}
			}

			return mat;
		}

		private Material AssignLegacyShader(Cubemap defaultCubemapTexture, Material mat) {
			mat.SetColor("_Color",diffuseColor);
			
			if(IsTextured){
				mat.SetTexture("_MainTex",diffuseMap);
			}
			
			if(IsMasked){
				mat.SetTexture("_MaskMap", opacityMap);
			}			
			
			if(IsLightMapped)
			{
				if(IsOccluded)
				{
					mat.SetTexture("_LightMap", mixedMap);
					
					mat.SetFloat("_LightMapContrast", 1f);
					mat.SetFloat("_LightMapOffset", 0);					
				}
				else
				{
					mat.SetTexture("_LightMap", selfillumMap);
					mat.SetFloat("_LightMapContrast",selfillumLevel);
					mat.SetFloat("_LightMapOffset",0);
				}
			}
			else if(IsOccluded)
			{				
				mat.SetTexture("_LightMap", mixedMap);
				
				mat.SetFloat("_LightMapContrast", 1f);
				mat.SetFloat("_LightMapOffset", 0);						
			}	
			
			
			if(IsBumped){
				mat.SetTexture("_BumpMap",bumpMap);
				mat.SetFloat("_BumpQuantity",bumpLevel);
			}
			
			if(IsReflecting){
				mat.SetTexture("_ReflectMap", defaultCubemapTexture);
				mat.SetColor("_ReflectColor",reflectColor);
				mat.SetFloat("_ReflectContrast",1.3f);
				mat.SetFloat("_ReflectOffset",-0.55f);
				mat.SetFloat("_Normal_Reflect_Alpha",normalReflectLevel);
				mat.SetFloat("_Fresnel_Curve",3.0f);
				mat.SetFloat("_Tangent_Reflect_Alpha",tangentReflectLevel);
			}
			return mat;
		}

		private Material AssignStandardShader(Cubemap defaultCubemapTexture, Material mat) {
			mat.SetColor("_Color",diffuseColor);

			if (IsTextured) {
				mat.SetTexture("_MainTex", diffuseMap);
			}

			if (IsLightMapped) {
				mat.SetTexture("_EmissionMap", selfillumMap);
			}

			if (IsOccluded) {
				mat.SetTexture("_OcclusionMap", occlusionMap);
			}

			if (IsBumped) {
				mat.SetTexture("_BumpMap", bumpMap);
			}

			return mat;
		}


		public bool IsLightResponsive {
			get{return isLightResponsive;}
		}

		public bool IsTextured {
			get{return (diffuseMap != null || diffuseMapName != null);}
		}

		public bool IsTransparent {
			get{return (opacityMap != null || opacityLevel < 1.0f || opacityMapName != null );}
		}

		public bool IsLightMapped {
			get{return (selfillumMap != null || selfillumMapName != null);}
		}

		public bool IsOccluded {
			get{return (occlusionMap != null || occlusionMapName != null);}
		}	

		public bool IsBumped {
			get{return (bumpMap != null && bumpLevel > 0.0f || bumpMapName != null);}
		}

		public bool IsReflecting {
			get{return (tangentReflectLevel > 0.0f || normalReflectLevel > 0.0f);}
		}

		public bool IsMasked {
			get{return ((opacityMap != null || opacityMapName != null) && opacityMap!=diffuseMap);}
		}

		public bool IsCutout {
			get{return (IsTransparent && (IsTextured || IsMasked));}
		}
	}
}