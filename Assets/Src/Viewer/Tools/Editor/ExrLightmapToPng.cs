using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;


public class ExrLightmapTo : ScriptableWizard {

	public List<Texture2D> srcTextures = new List<Texture2D>();
	
	[MenuItem("Observ3d/Textures/EXR Lightmap to PNG")]
    public static void ConvertLightmapToExr () {
		ScriptableWizard.DisplayWizard<ExrLightmapTo>("Convert Exr to Png", "Convert!");
    }
	
	public void  OnWizardUpdate () {
        //helpString = "Select a default cubemap to assign materials";
		isValid = srcTextures.Count>0;
    }
	
	public void OnWizardCreate () {
		Convert(Selection.activeGameObject);
	}
	
	public void Convert(GameObject g)
	{
		#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
		foreach(Texture2D srcTexture in srcTextures){
			//rootName              = g.name;
			string sourceTexturePath             = AssetDatabase.GetAssetPath(srcTexture);

			TextureImporter srcTextureImporter = (TextureImporter) AssetImporter.GetAtPath(sourceTexturePath) ;
			
			if (srcTextureImporter.isReadable == false)
			{
				srcTextureImporter.isReadable = true;
				AssetDatabase.ImportAsset(sourceTexturePath, ImportAssetOptions.ForceUpdate);
			}

			Debug.Log(sourceTexturePath);

			Texture2D dstTexture = new Texture2D(srcTexture.width, srcTexture.height, TextureFormat.ARGB32, false) ;
			
			string dstTexturePath = sourceTexturePath+".png" ;
			File.WriteAllBytes(dstTexturePath, dstTexture.EncodeToPNG()) ;		
			
			AssetDatabase.ImportAsset(dstTexturePath) ;
			
			dstTexture = (Texture2D) AssetDatabase.LoadMainAssetAtPath(dstTexturePath) ;
			
			AssetDatabase.SaveAssets() ;
			
			TextureImporter dstTextureImporter = (TextureImporter) AssetImporter.GetAtPath(dstTexturePath) ;
			
			dstTextureImporter.textureType   = TextureImporterType.Default ;
			dstTextureImporter.textureCompression = TextureImporterCompression.Uncompressed;
			dstTextureImporter.isReadable = true ;
			dstTextureImporter.mipmapEnabled = false ;
			dstTextureImporter.maxTextureSize = 2048 ;
			
			AssetDatabase.ImportAsset(dstTexturePath, ImportAssetOptions.ForceUpdate) ;
			//string resourcePath = assetPath.Substring(assetPath.IndexOf("/") + 1);

			Color[] colors = new Color[dstTexture.width*dstTexture.height] ;


			
			for(int i = 0 ; i < srcTexture.width ; i++)
			{
				for(int j = 0 ; j < srcTexture.height ; j++)
				{
					int index = i + j*srcTexture.height ;
					

					Color src = srcTexture.GetPixel(i,j);


					//float m = Mathf.Max(src.r,src.g,src.b,1.0f);
					//float avg = Mathf.Max((src.r+src.g+src.b)/3.0f,1.0f);
					//float d = m*avg;
		
					//if(d>0){
						colors[index].r = src.r ;//Mathf.Min ( src.r/d , 1.0f);
						colors[index].g = src.g ;//Mathf.Min ( src.g/d , 1.0f);
						colors[index].b = src.b ;//Mathf.Min ( src.b/d , 1.0f);
						colors[index].a = src.a ;//Mathf.Min (d / 8.0f ,1.0f);
					//}else{
						//colors[index].a = 0.0f;
					//}
				}
			}
			
			dstTexture.SetPixels(colors) ;
			dstTexture.Apply() ;
			
			File.WriteAllBytes(dstTexturePath, dstTexture.EncodeToPNG()) ;
			
			AssetDatabase.ImportAsset(dstTexturePath, ImportAssetOptions.ForceUpdate) ;

			//List<Material> mats = GetMaterials();

			List<Material> mats =GetMaterials("Assets");

			foreach(Material mat in mats){
				if( mat.HasProperty("_LightMap") && mat.GetTexture("_LightMap") == srcTexture) mat.SetTexture ("_LightMap",dstTexture);
			}
		}

		#else

			Debug.LogError("Conversion must be done on a desktop plateform");
		#endif
	}



	public static List<Material> GetMaterials(string startDirectory )
	{
		List<Material> materials = new List<Material>();
		try
		{
			foreach (string file in Directory.GetFiles(startDirectory, "*.mat",SearchOption.AllDirectories))
			{
				materials.Add ((Material) AssetDatabase.LoadMainAssetAtPath(file)) ;
			}
		}
		catch 
		{
		}

		return materials;
	}
}
