using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Rise.Features.MovingMode;

namespace Rise.Features.AutoAssign {

	public class AutoAssignEditor : ScriptableWizard {

		public bool isLightResponsive = false;
		
		public bool assignReflect = false;
		public Cubemap defaultCubemapTexture;

		public bool assignBump = false;
		
		#if UNITY_5
		public bool useLegacyShaders = false;
		#else
			public bool useLegacyShaders = false;
		#endif


		private static string assetPath   = "";
		
		private static string localResourcePath = "";
		private static string rootName = "";

		
		static GameObject gameObjectToAssign;
		
		//[MenuItem("Observ3d/Ressources/Assign O3D content")]
	    public static void AutoAssignMaterials () {
	        ScriptableWizard.DisplayWizard<AutoAssignEditor>("Assign from XML", "Assign!");
	    }
		
		public void  OnWizardUpdate () {
	        //helpString = "Select a default cubemap to assign materials";
			isValid = (!assignReflect || defaultCubemapTexture != null);

	    }
		
		public void OnWizardCreate () {

			
			ParseAndProcess(Selection.activeGameObject);
		}
		
		public void ParseAndProcess(GameObject g)
		{
			rootName              = g.name;
			assetPath             = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabObject(g));
			assetPath             = assetPath.Substring(0, assetPath.LastIndexOf("/"));
			
			string resourcePath = assetPath.Substring(assetPath.IndexOf("/") + 1);
			localResourcePath = resourcePath;
			if(localResourcePath.Contains("/"))localResourcePath = localResourcePath.Substring(localResourcePath.IndexOf("/") + 1);
			if(localResourcePath == "Resources")localResourcePath = "";
			
			TextAsset xmlFileData = (TextAsset)Resources.Load(localResourcePath + (localResourcePath == "" ? "" : "/") + rootName, typeof(TextAsset));



			EditorUtility.DisplayProgressBar ("Observ3d auto-assignation", "Loading o3d file...", 0.5f);
			XmlDocument doc = new System.Xml.XmlDocument();
			doc.LoadXml(xmlFileData.text);

			Dictionary<string, Material> materials = new Dictionary<string, Material>();
			Dictionary<string, Transform> meshes = new Dictionary<string, Transform>();
			
			XmlNode sceneNode = doc.SelectSingleNode("/o3d/scene");
			
			if (sceneNode != null) {
				
				EditorUtility.DisplayProgressBar ("Observ3d auto-assignation", "Loading animation...", 0.0f);
				
				XmlNode animationNode = sceneNode.SelectSingleNode ("animation");
				float animationFrameRate = 25.0f;
				if (animationNode != null) {
					animationFrameRate = AutoAssignTools.ParseFloatNode (animationNode.SelectSingleNode ("framerate"), 25.0f);
				}

				EditorUtility.DisplayProgressBar ("Observ3d auto-assignation", "Importing fbx...", 0.0f);


				string path = AssetDatabase.GetAssetPath(g);
				ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
				modelImporter.animationType = animationNode == null ? ModelImporterAnimationType.None : ModelImporterAnimationType.Legacy;
				AssetDatabase.ImportAsset(path);

				gameObjectToAssign = (GameObject)GameObject.Instantiate(g);

				EditorUtility.DisplayProgressBar ("Observ3d auto-assignation", "Loading materials...", 0.0f);

				MixedTextureManager.mixedTextures = new System.Collections.Generic.Dictionary<string, UnityEngine.Texture2D>();
				
				string materialsPath = Application.dataPath + "/" + resourcePath + "/" + rootName + "_Materials";
				System.IO.Directory.CreateDirectory(materialsPath);

				XmlNodeList matNodes = sceneNode.SelectNodes ("material");
				int index = 0;
				foreach (XmlNode matNode in matNodes) {
					
					string uid = ((XmlElement)matNode).GetAttribute ("uid");
					string name = ((XmlElement)matNode).GetAttribute ("name");
					string type = ((XmlElement)matNode).GetAttribute ("type");
					
					if (!string.IsNullOrEmpty (uid)) {
						
						Material mat = CreateMaterialFromO3dNode (matNode);
						
						if (mat != null) {
							materials.Add (uid, mat);
						}
						
					} else {
						Debug.LogWarning ("Material \"" + name + "\" of type \"" + type + "\" has no uid");
					}
					
					EditorUtility.DisplayProgressBar ("Observ3d auto-assignation", "Loading materials (" + (index + 1) + " out of " + matNodes.Count + ")...", (0.5f * (float)index / (float)(matNodes.Count)));
					index++;
				}

				
				EditorUtility.DisplayProgressBar ("Observ3d auto-assignation", "Loading meshes...", 0.5f);
				index = 0;
				
				XmlNodeList baseMeshNodes = sceneNode.SelectNodes ("descendant::mesh");
				foreach (XmlNode mNode in baseMeshNodes) {
					string uid = ((XmlElement)mNode).GetAttribute ("uid");
					string name = ((XmlElement)mNode).GetAttribute ("name");

					Transform mesh = AutoAssignTools.FindRecursive (gameObjectToAssign.transform, name);
					bool visible = AutoAssignTools.ParseBoolNode (mNode.SelectSingleNode ("visible"), true);
					
					if (mesh != null) {
						
						//	Debug.Log(mesh.name + " is visible : " +visible);
						
						AutoAssignTools.AssignMaterialToMesh (mesh, mNode, materials);
						mesh.gameObject.SetActive (visible);
						
						if (!string.IsNullOrEmpty (uid)) {
							
							meshes.Add (uid, mesh);
						}
						
					} else {
						Debug.LogWarning ("Mesh \"" + name + "\" (" + uid + ") was not found in " + gameObjectToAssign.name);
					}
					
					EditorUtility.DisplayProgressBar ("Observ3d auto-assignation", "Loading meshes...", 0.5f + (0.45f * (float)index / (float)(baseMeshNodes.Count)));
					index++;
				}
				
				
				XmlNodeList baseObjectNodes = sceneNode.SelectNodes ("descendant::object");
				foreach (XmlNode oNode in baseObjectNodes) {
					string uid = ((XmlElement)oNode).GetAttribute ("uid");
					string name = ((XmlElement)oNode).GetAttribute ("name");
					Transform mesh = AutoAssignTools.FindRecursive (gameObjectToAssign.transform, name);
					
					if (mesh != null) {
						
						if (!string.IsNullOrEmpty (uid)) {
							
							meshes.Add (uid, mesh);
						}
					} else {
						Debug.LogWarning ("Object \"" + name + "\" (" + uid + ") was not found in " + gameObjectToAssign.name);
					}
				}
				
				EditorUtility.DisplayProgressBar ("Observ3d auto-assignation", "Loading meshes...", 0.95f);
				index = 0;
				
				XmlNodeList baseCameraNodes = sceneNode.SelectNodes ("camera");
				foreach (XmlNode cNode in baseCameraNodes) {
					AssignCamera (cNode, meshes, animationFrameRate, gameObjectToAssign);
					EditorUtility.DisplayProgressBar ("Observ3d auto-assignation", "Loading camera...", 0.95f + (0.05f * (float)index / (float)(baseCameraNodes.Count)));
					index++;
				}
				
			}

			
			EditorUtility.DisplayProgressBar("Observ3d auto-assignation","Almost done - Saving new prefab...",0.95f);
			
			Object prefab = PrefabUtility.CreateEmptyPrefab(assetPath + "/" + rootName + "_final.prefab");
			PrefabUtility.ReplacePrefab(gameObjectToAssign, prefab);
			
			Object.DestroyImmediate(gameObjectToAssign);
			
			EditorUtility.ClearProgressBar();
			
			EditorUtility.DisplayDialog("Observ3d auto-assignation", "The prefab with assigned materials has been created as \"" + rootName + "_final\".", "OK");

		}
		


		private Material CreateMaterialFromO3dNode(XmlNode matNode)
		{
			string materialName           = "Mat_" + ((XmlElement)matNode).GetAttribute("uid").ToString() + " (" + ((XmlElement)matNode).GetAttribute("name").ToString() + ")";
			string materialPath			 = 	assetPath + "/" + rootName + "_Materials/" + materialName + ".mat";

			
			Material existingMat = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));
			if(existingMat != null){
				Debug.LogWarning("Existing material \"" + materialName + "\" have been found.");
				return existingMat;
			}


			XmlNode diffuseNode = matNode.SelectSingleNode("Diffuse");
			XmlNode opacityNode = matNode.SelectSingleNode("Opacity");
			XmlNode bumpNode = matNode.SelectSingleNode("Bump");
			XmlNode selfillumNode = matNode.SelectSingleNode("Selfillum");
			XmlNode reflectNode = matNode.SelectSingleNode("Reflect");
			
			
			MaterialData matData = new MaterialData();

			matData.doubleSided = AutoAssignTools.ParseBoolAttribute(matNode,"doubleSided",false);
			matData.isLightResponsive = isLightResponsive;
			
			if(selfillumNode!=null){
				matData.selfillumMap = ParseO3dTexture( selfillumNode.SelectSingleNode("Bitmap"),"name",TextureType.Lightmap);
				matData.selfillumLevel = AutoAssignTools.ParseFloatNode(bumpNode.SelectSingleNode("SelfillumLevel"),1.0f);
			}
			
			if(opacityNode!=null){
				matData.opacityLevel = AutoAssignTools.ParseFloatNode(opacityNode.SelectSingleNode("OpacityLevel"),1.0f);
				matData.opacityMap   = ParseO3dTexture( opacityNode.SelectSingleNode("Bitmap"),"name",TextureType.Standard);
				if(matData.opacityMap!=null) matData.selfillumLevel *= 0.6f;
			}
			
			if(diffuseNode!=null){
				matData.diffuseColor = AutoAssignTools.ParseVector4Node(diffuseNode.SelectSingleNode("DiffuseColor"),1,1,1,matData.opacityLevel);
				
				XmlNode bitmapNode = diffuseNode.SelectSingleNode("Bitmap");
				XmlNode compositeNode = diffuseNode.SelectSingleNode("Composite");
				if(bitmapNode!=null){
					matData.diffuseMap = ParseO3dTexture( diffuseNode.SelectSingleNode("Bitmap"),"name",TextureType.Standard);
				}else if(compositeNode!=null){
					
					XmlNodeList layerNodes = compositeNode.SelectNodes("Layer");
					foreach(XmlNode layerNode in layerNodes){
						string layerName = ((XmlElement)layerNode).GetAttribute("name").ToString();
						float layerOpacity = AutoAssignTools.ParseFloatAttribute(layerNode,"opacity",1.0f);
						//
						//string layerBlendMode = ((XmlElement)layerNode).GetAttribute("blendmode").ToString();
						if(layerName == "diffuse"){
							matData.diffuseMap = ParseO3dTexture( layerNode.SelectSingleNode("Bitmap"),"name",TextureType.Standard);
						}else if(layerName == "lightmap"){
							matData.selfillumMap = ParseO3dTexture( layerNode.SelectSingleNode("Bitmap"),"name",TextureType.Lightmap);
							matData.selfillumLevel = layerOpacity;
						}else if(layerName == "occlusion"){
							matData.occlusionMap = ParseO3dTexture( layerNode.SelectSingleNode("Bitmap"),"name",TextureType.Occlusion);
							matData.occlusionLevel = layerOpacity;
						}
					}
				}
			}
			

			if(assignBump && bumpNode!=null){
				matData.bumpLevel = AutoAssignTools.ParseFloatNode(bumpNode.SelectSingleNode("BumpLevel"),0.0f);
				matData.bumpMap = ParseO3dTexture( bumpNode.SelectSingleNode("Bitmap"),"name",TextureType.Normalmap);

			}
			

			if(assignReflect && reflectNode!=null){
				matData.reflectGloss = AutoAssignTools.ParseFloatNode(reflectNode.SelectSingleNode("ReflectGloss"),0.0f);
				matData.normalReflectLevel = AutoAssignTools.ParseFloatNode(reflectNode.SelectSingleNode("NormalReflectLevel"),0.0f);
				matData.tangentReflectLevel = AutoAssignTools.ParseFloatNode(reflectNode.SelectSingleNode("TangentReflectLevel"),0.0f);
				matData.reflectColor  = AutoAssignTools.ParseVector4Node(reflectNode.SelectSingleNode("ReflectColor"),0,0,0,1);
			}
			
			matData = CreateOrGetMixedTextureFromMaterialData(matData);
			
			Material mat  = matData.CreateMaterial(defaultCubemapTexture, useLegacyShaders);

			if(AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) != null){
				Debug.LogError("Cannot write material \"" + materialName + "\" to disk because a file already exists with that name.");
			}else{
				AssetDatabase.CreateAsset(mat, materialPath);
			}
			
			return mat;
		}
		
		private void AssignCamera(XmlNode cameraNode,Dictionary<string,Transform> meshes,float animationFrameRate,GameObject animatedGameObject)
	 	{
	 		string cameraName = ((XmlElement)cameraNode).GetAttribute("name");
			
			
			//Animated Object
			Transform elementTransform = AutoAssignTools.FindRecursive(gameObjectToAssign.transform,cameraName);
			
			if(elementTransform != null)
			{
				
				Transform cameraTransform = elementTransform.Find(cameraName+"_Camera");
				GameObject cameraObject; 
				Camera camera;
				
				MovingModeAnimatedCamera mmac = null;
				
				//Is there already a camera inside ?
				if(cameraTransform != null)
				{
					cameraObject = cameraTransform.gameObject;
				  	camera= cameraObject.GetComponent<Camera>();
				}
				else
				{
					cameraObject = new GameObject (cameraName+"_Camera");
					camera = cameraObject.AddComponent<Camera>();
					cameraTransform = cameraObject.transform;
					cameraTransform.parent = elementTransform;
				}
				
				cameraTransform.localPosition = Vector3.zero;
				cameraTransform.localRotation = Quaternion.Euler(0,-90,0);
				cameraTransform.localScale    = new Vector3(1.0f,1.0f,1.0f);
				
				mmac = cameraObject.GetComponent<MovingModeAnimatedCamera>();
	 			if(mmac == null){
					mmac = cameraObject.AddComponent<MovingModeAnimatedCamera>();
				}
				
				mmac.movingModeName = cameraName;
				mmac.startTime = AutoAssignTools.ParseFloatNode(cameraNode.SelectSingleNode("startFrame"),0.0f)/animationFrameRate;
				mmac.endTime = AutoAssignTools.ParseFloatNode(cameraNode.SelectSingleNode("endFrame"),0.0f)/animationFrameRate;
				mmac.animatedGameObject = animatedGameObject;
				
				//Fov
				camera.fieldOfView = AutoAssignTools.ParseFloatNode(cameraNode.SelectSingleNode("fovY"),60.0f);
				camera.nearClipPlane = AutoAssignTools.ParseFloatNode(cameraNode.SelectSingleNode("near"),0.1f);
				camera.farClipPlane = AutoAssignTools.ParseFloatNode(cameraNode.SelectSingleNode("far"),1000.0f);
				
				//Target
				XmlNodeList targets = cameraNode.SelectNodes("target");
				if(targets.Count > 0)
				{
					XmlNode tgt = targets.Item(0);
					string targetId = ((XmlElement)tgt).GetAttribute("targetId");
					if(meshes.ContainsKey(targetId)){
						CameraTarget targetComponent = cameraObject.AddComponent<CameraTarget>();
						targetComponent.target = meshes[targetId];
					}
					
					
				}
				/*
				//Post Rotation
				XmlNodeList postRotations = cameraNode.SelectNodes("postRotation");
				if(postRotations.Count > 0)
				{
					XmlNode postRotationElement = postRotations.Item(0);
					Vector3 postRotation = ParseVector3Node(postRotationElement);
					cameraTransform.Rotate(postRotation);
				}*/
	 		}else{
				Debug.LogWarning("Camera \"" + cameraName + "\" not found in prefab");
			}
	 	}


		private Texture2D ParseO3dTexture(XmlNode node,string attributeName,TextureType type){
			
			if(node!=null){
				XmlNode srcNode = node.SelectSingleNode("Source");
				
				
				if(srcNode!=null){
					
					string textureName = AutoAssignTools.GetFilenameWithoutExtension(srcNode.InnerText) ;
				
					
					
					if(!string.IsNullOrEmpty(textureName)){
						string texturePath = localResourcePath + (localResourcePath == "" ? "" : "/") + rootName + ".fbm/" + textureName;
						
						Texture2D texture = (Texture2D)Resources.Load(texturePath,  typeof(Texture2D));
						
						if(texture!=null){
							
							if(type!= TextureType.Standard){
								texturePath = AssetDatabase.GetAssetPath(texture);
								TextureImporter texImp = (TextureImporter)AssetImporter.GetAtPath(texturePath);
			
								if(type== TextureType.Lightmap){
									string ext = AutoAssignTools.GetFileExtension(texturePath);

									if(ext=="exr" && texImp.textureType != TextureImporterType.Lightmap)texImp.textureType = TextureImporterType.Lightmap;
									
								}else if(type== TextureType.Normalmap){
									if(texImp.textureType != TextureImporterType.Bump)texImp.textureType = TextureImporterType.Bump;
								}
								
								if(type== TextureType.Lightmap || type== TextureType.Occlusion)texImp.maxTextureSize = 4096;
								
								if(type== TextureType.Lightmap){
									texImp.textureFormat = TextureImporterFormat.AutomaticTruecolor;
									texImp.mipmapEnabled = false;
								}else if(type== TextureType.Occlusion){
									texImp.textureFormat = TextureImporterFormat.AutomaticCompressed;
									texImp.mipmapEnabled = false;
								}
								
								
								AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
							}
								
						}else{
							
							Debug.LogWarning("Impossible to find texture at path : " + texturePath);
							
						}
						
						return texture;
					}
				}
			}
			
			return null;
			
		}
		
		private MaterialData CreateOrGetMixedTextureFromMaterialData(MaterialData matData) {
			MixedTextureManager.BaseDatas bLightmap = new MixedTextureManager.BaseDatas() ;
			MixedTextureManager.BaseDatas bOcclusionmap = new MixedTextureManager.BaseDatas() ;								
			
			bLightmap.mixingType = MixedTextureManager.MixingTypes.Lightmap ;
			bLightmap.ratio = matData.selfillumLevel ;
			bLightmap.texture = matData.selfillumMap ;		
			
			//Debug.Log(bLightmap.texture.name);
			
			bOcclusionmap.mixingType = MixedTextureManager.MixingTypes.Occlusion ;
			bOcclusionmap.ratio = matData.occlusionLevel ;
			bOcclusionmap.texture = matData.occlusionMap ;	
			
			if(matData.IsLightMapped) {
				if(matData.IsOccluded) {
					Texture2D texture = MixedTextureManager.GetOrCreateMixedTexture
					(
					 MixedTextureManager.TextureUsage.Lightmap, 
				     new List<MixedTextureManager.BaseDatas>(new MixedTextureManager.BaseDatas[]
					 {
					   bLightmap,
					   bOcclusionmap
					 })
					) ;
					
					matData.mixedMap = texture;
				}
			}
			else if(matData.IsOccluded) {
				Texture2D texture = MixedTextureManager.GetOrCreateMixedTexture
				(
				 MixedTextureManager.TextureUsage.Lightmap,
			     new List<MixedTextureManager.BaseDatas>(new MixedTextureManager.BaseDatas[]
				 {
				   bOcclusionmap
				 })
				) ;
				
				matData.mixedMap = texture;
			}
			
			return matData;
		}
	}
}