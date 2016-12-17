using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class BillboardsBuilder : AssetPostprocessor {
	
	
	 void OnPreprocessTexture () {
        if (assetPath.ToLower().Contains("billboards")) {
            TextureImporter textureImporter = (TextureImporter) assetImporter;
            textureImporter.npotScale = TextureImporterNPOTScale.None;
			textureImporter.wrapMode = TextureWrapMode.Clamp;
			textureImporter.textureFormat = TextureImporterFormat.RGBA32;
			textureImporter.anisoLevel = 5;
			
			
        }
    }
	
	
	void OnPostprocessTexture (Texture2D texture ) {            
        
		
		if (assetPath.ToLower().Contains("billboards")){
		
			string basename;
			int slashIndex = assetPath.LastIndexOf("/")+1;
			int pointIndex = assetPath.LastIndexOf(".");
			if(slashIndex>0 && slashIndex<pointIndex)basename = assetPath.Substring(slashIndex,pointIndex-slashIndex);
			else basename = assetPath;
			
			string basePath;
			slashIndex = assetPath.LastIndexOf("/")+1;
			if(slashIndex>0 && slashIndex<assetPath.Length)basePath = assetPath.Substring(0,slashIndex);
			else basePath = assetPath;
			
			string materialPath = basePath +"Materials/";
			System.IO.Directory.CreateDirectory(materialPath);
			
			string prefabPath = basePath +"Prefabs/";
			System.IO.Directory.CreateDirectory(prefabPath);
			
			string meshesPath = basePath +"Meshes/";
			System.IO.Directory.CreateDirectory(meshesPath);
			
			

			Shader shader  = Shader.Find("Stereograph/Light Independent/LI-Texture-Cutout");
			Material material = new Material(shader);
			material.mainTexture = texture;
			//material.SetTexture("_MainTex", texture);
			
			string materialAssetPath = materialPath + basename + ".mat";
			if(AssetDatabase.LoadAssetAtPath(materialAssetPath, typeof(Material)) != null){
				Debug.LogError("Cannot write material \"" + materialAssetPath + "\" to disk because a file already exists with that name.");
			}else{
				AssetDatabase.CreateAsset(material, materialAssetPath);
			}
			
			
			GameObject plane = new GameObject(basename);
			plane.AddComponent<MeshRenderer>();
			
			Mesh newMesh = new Mesh();
			MeshFilter meshfilter = (MeshFilter)plane.AddComponent<MeshFilter>();
			meshfilter.mesh = newMesh;

			float width = 0.5f*(float)texture.width/texture.height;
			
			
			
			newMesh.vertices =  new Vector3[4]{
										new Vector3(-width,0,0),
										new Vector3(width,0,0),
										new Vector3(width,1,0),
										new Vector3(-width,1,0)};
			newMesh.normals = new Vector3[4]{
										new Vector3(0,0,1),
										new Vector3(0,0,1),
										new Vector3(0,0,1),
										new Vector3(0,0,1)};
			newMesh.uv = new Vector2[4]{
										new Vector2(1,0),
										new Vector2(0,0),
										new Vector2(0,1),
										new Vector2(1,1)};
			newMesh.triangles = new int[6]{0,1,2,2,3,0};
				
			plane.GetComponent<Renderer>().material = material;
			
			
			string meshAssetPath = meshesPath + basename + ".asset";
			if(AssetDatabase.LoadAssetAtPath(meshAssetPath, typeof(Mesh)) != null){
				Debug.LogError("Cannot write material \"" + materialAssetPath + "\" to disk because a file already exists with that name.");
			}else{
				AssetDatabase.CreateAsset(newMesh, meshAssetPath);
			}
			
			
			Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath+basename+".prefab");
			PrefabUtility.ReplacePrefab(plane, prefab, ReplacePrefabOptions.ConnectToPrefab);
			
		
       		
			
		}
        
    }

}