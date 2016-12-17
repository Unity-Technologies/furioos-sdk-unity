using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Rise.Core;
using Rise.Features.AssetManager;
using Rise.Features.MovingMode;

namespace Rise.Features.AutoAssign {

	public class AutoAssign : RSBehaviour {
		public GameObject gameObjectToAssign;
		public bool isLightResponsive = false;
		public Cubemap defaultCubemapTexture;
		
		
		public Dictionary<string, Material> materials = new Dictionary<string, Material>();
		public Dictionary<string, MaterialData> materialsData = new Dictionary<string, MaterialData>();
		public Dictionary<string, Transform> meshes = new Dictionary<string, Transform>();
		
		public Texture2D downloadedTexture;
		public Dictionary<string, Texture2D> downloadedTextures = new Dictionary<string, Texture2D>();
		
		public TextAsset textAsset;
		public string url = "http://www.observed.fr/public/apps/dev/WebPlayer/textures/";
		
		public void Start() {
			/*RemoteAssetsManager.Download(
				url+"../tree.xml", 
				new object[1]{0}, 
				RemoteAssetsManager.AssetData.AssetType.TEXT,
				RemoteAssetsManager.AssetData.AssetPriority.HIGH,
				new AssetDownloadedCallBack(XmlCallBack)
			);*/
			
			ParseAndProcess(gameObjectToAssign, textAsset.text);
		}
		
		public void ParseAndProcess(GameObject g, string xmlFileData)
		{	
			gameObjectToAssign = g;
			
			XmlDocument doc = new System.Xml.XmlDocument();
			doc.LoadXml(xmlFileData);
			
			if(doc.SelectNodes("o3d").Count > 0)ParseO3d(doc);
			else ParseXml(doc);
			
			foreach(KeyValuePair<string, Material> pair in materials) {	
				string uid = pair.Key;
				MaterialData matData = materialsData[uid];
				if(matData.diffuseMapName != null) {
					RemoteAssetsManager.Download(
						url+matData.diffuseMapName, 
						new object[4]{0, (Material)materials[uid], TextureType.Standard, matData.diffuseMapName}, 
						RemoteAssetsManager.AssetData.AssetType.ASSETBUNDLE,
						RemoteAssetsManager.AssetData.AssetPriority.MEDIUM,
						new AssetDownloadedCallBack(AssignTextureCallBack)
					);
				}
				
				if(matData.selfillumMapName != null) {
					RemoteAssetsManager.Download(
						url+matData.selfillumMapName, 
						new object[4]{0, (Material)materials[uid], TextureType.Lightmap, matData.selfillumMapName}, 
						RemoteAssetsManager.AssetData.AssetType.ASSETBUNDLE, 
						RemoteAssetsManager.AssetData.AssetPriority.HIGH,
						new AssetDownloadedCallBack(AssignTextureCallBack)
					);
				}
				
				if(matData.bumpMapName != null) {
					RemoteAssetsManager.Download(
						url+matData.bumpMapName, 
						new object[4]{0, (Material)materials[uid], TextureType.Normalmap, matData.bumpMapName}, 
						RemoteAssetsManager.AssetData.AssetType.ASSETBUNDLE, 
						RemoteAssetsManager.AssetData.AssetPriority.LOW,
						new AssetDownloadedCallBack(AssignTextureCallBack)
					);
				}
			}
		}
		
		private void XmlCallBack(object[] asset, object[] args) {
			XmlDocument treeXml = new XmlDocument();
			treeXml.LoadXml(((TextAsset)asset[0]).text);
		}
		
		private void AssignTextureCallBack(object[] asset, object[] args) {
			Material mat = (Material)args[1];
			AssetBundle bundle = asset[0] as AssetBundle;
			
			if(bundle == null) return;
			
			switch((TextureType)args[2]) {
				case TextureType.Standard:
					mat.SetTexture("_MainTex", (Texture2D)bundle.mainAsset);
				break;
				case TextureType.Lightmap:
					mat.SetTexture("_LightMap", (Texture2D)bundle.mainAsset);
				break;
				case TextureType.Normalmap:
					mat.SetTexture("_BumpMap", (Texture2D)bundle.mainAsset);
				break;
			}
		}
		
		private void ParseO3d(XmlDocument doc){
			
			XmlNodeList matNodes = doc.SelectNodes("/o3d/scene/material");
			
			XmlNode animationNode = doc.SelectSingleNode("/o3d/scene/animation");
			float animationFrameRate = 25.0f;
			if(animationNode!=null){
				animationFrameRate = AutoAssignTools.ParseFloatNode(animationNode.SelectSingleNode("framerate"),25.0f);
			}
			
			//EditorUtility.DisplayProgressBar("Observ3d auto-assignation","Loading materials...",0.0f);
			
			int index = 0;
			foreach(XmlNode matNode in matNodes)
			{
				
				string uid = ((XmlElement)matNode).GetAttribute("uid");
				string name = ((XmlElement)matNode).GetAttribute("name");
				string type = ((XmlElement)matNode).GetAttribute("type");
				
				if(!string.IsNullOrEmpty(uid)){
				
					MaterialData matData = CreateMaterialFromO3dNode(matNode);
					Material mat = matData.CreateMaterial(defaultCubemapTexture, false);
					
					if(mat!=null){
						materials.Add(uid,mat);
						materialsData.Add(uid, matData);
					}
					
				}else{
					Debug.LogWarning("Material \"" + name + "\" of type \"" + type + "\" has no uid");
				}
				
				//EditorUtility.DisplayProgressBar("Observ3d auto-assignation","Loading materials (" + (index + 1) + " out of " + matNodes.Count + ")...",( 0.5f  * (float) index / (float)(matNodes.Count)));
				index++;
			}
			
			//EditorUtility.DisplayProgressBar("Observ3d auto-assignation","Loading meshes...",0.5f);
			index = 0;

			XmlNodeList baseMeshNodes = doc.SelectNodes("/o3d/scene/mesh");
			foreach(XmlNode mNode in baseMeshNodes)
			{
				string uid = ((XmlElement)mNode).GetAttribute("uid");
				string name = ((XmlElement)mNode).GetAttribute("name");
				//string type = ((XmlElement)mNode).GetAttribute("type");
				
				Transform mesh = AutoAssignTools.FindRecursive(gameObjectToAssign.transform,name);
				bool visible = AutoAssignTools.ParseBoolNode(mNode.SelectSingleNode("visible"),true);
				
				if(mesh!=null){
					
				//	Debug.Log(mesh.name + " is visible : " +visible);
					
					AutoAssignTools.AssignMaterialToMesh(mesh,mNode,materials);
					mesh.gameObject.SetActive(visible);
					
					if(!string.IsNullOrEmpty(uid)){
						
						meshes.Add(uid,mesh);
					}
					
				}
				
				//EditorUtility.DisplayProgressBar("Observ3d auto-assignation","Loading meshes...",0.5f + (0.45f * (float) index / (float)(baseMeshNodes.Count)));
				index++;
			}
			
			
			XmlNodeList baseObjectNodes = doc.SelectNodes("/o3d/scene/object");
			foreach(XmlNode oNode in baseObjectNodes)
			{
				string uid = ((XmlElement)oNode).GetAttribute("uid");
				string name = ((XmlElement)oNode).GetAttribute("name");
				//string type = ((XmlElement)oNode).GetAttribute("type");
				
				Transform mesh = AutoAssignTools.FindRecursive(gameObjectToAssign.transform,name);
				
				if(mesh!=null){
					
					if(!string.IsNullOrEmpty(uid)){
						
						meshes.Add(uid,mesh);
					}
				}
			}
			
			//EditorUtility.DisplayProgressBar("Observ3d auto-assignation","Loading meshes...",0.95f);
			index = 0;
			
			XmlNodeList baseCameraNodes = doc.SelectNodes("/o3d/scene/camera");
			foreach(XmlNode cNode in baseCameraNodes)
			{
				AssignCamera(cNode,meshes,animationFrameRate,gameObjectToAssign);
				//EditorUtility.DisplayProgressBar("Observ3d auto-assignation","Loading camera...",0.95f + (0.05f * (float) index / (float)(baseCameraNodes.Count)));
				index++;
			}

		}
		
		private void ParseXml(XmlDocument doc){
			
			Dictionary<string, Material> materials = new Dictionary<string, Material>();

			
			XmlNodeList matNodes = doc.SelectNodes("/scene/material");
			
			//EditorUtility.DisplayProgressBar("Observ3d auto-assignation","Loading materials...",0.0f);
			
			int index = 0;
			foreach(XmlNode matNode in matNodes)
			{
				
				string uid = ((XmlElement)matNode).GetAttribute("uid");
				string name = ((XmlElement)matNode).GetAttribute("name");
				if(!string.IsNullOrEmpty(uid)){
				
					Material mat = CreateMaterialFromXmlNode(matNode);
					
					if(mat!=null){
						materials.Add(uid,mat);
					}
					
				}else{
					Debug.LogWarning("Material \"" + name + "\" has no uid");
				}

				//EditorUtility.DisplayProgressBar("Observ3d auto-assignation","Loading materials (" + (index + 1) + " out of " + matNodes.Count + ")...",( 0.5f  * (float) index / (float)(matNodes.Count)));
				index++;
			}
			
			//EditorUtility.DisplayProgressBar("Observ3d material auto-assignation","Loading meshes...",0.5f);
			index = 0;

			XmlNodeList baseMeshNodes = doc.SelectNodes("/scene/mesh");
			foreach(XmlNode mNode in baseMeshNodes)
			{
				string name = ((XmlElement)mNode).GetAttribute("name");
				
				Transform mesh = AutoAssignTools.FindRecursive(gameObjectToAssign.transform,name);
				
				if(mesh!=null){
				
					AutoAssignTools.AssignMaterialToMesh(mesh,mNode,materials);
					
				}
				
				//EditorUtility.DisplayProgressBar("Observ3d auto-assignation","Loading meshes...",0.5f + (0.45f * (float) index / (float)(baseMeshNodes.Count)));
				index++;
			}
			
			/*
			
			EditorUtility.DisplayProgressBar(
				"Observ3d material auto-assignation",
				"Loading cameras...",
				0.8f);
			fTmp = 0.0f;
			
			
			GameObject prefabGoInstance = GameObject.Find(rootName);
			XmlNodeList cameraNodes = doc.SelectNodes("/scene/camera");
			foreach(XmlNode cameraNode in cameraNodes)
			{
				EditorUtility.DisplayProgressBar(
					"Observ3d material auto-assignation",
					"Loading cameras...",
					0.8f + fTmp / (float)(cameraNodes.Count));
				fTmp += 0.2f;
				ParseCamera(prefabGoInstance, cameraNode);
			}*/
			
			
			
			//Debug.Log("AutoAssignTools: * assigned " + nMaterials + " materials to " + nMeshes + " meshes.");
			/*if(cameraNodes.Count == 0)
				Debug.Log("AutoAssignTools: * No cameras found in this scene.");
			else
				Debug.Log("AutoAssignTools: * Found and set up " + cameraNodes.Count + " camera" + (cameraNodes.Count == 1 ? "" : "s") + ".");*/
			
			
		}
		
		
		private Material CreateMaterialFromXmlNode(XmlNode matNode)
		{	
			MaterialData matData = new MaterialData();
			
			matData.isLightResponsive = isLightResponsive;
			
			
			//float parameters
			matData.opacityLevel = 1.0f-AutoAssignTools.ParseFloatNode(matNode.SelectSingleNode("Opacity"),0.0f);
			//matData. = ParseFloatNode(matNode.SelectSingleNode("Shininess"),0.0f);
			matData.tangentReflectLevel = AutoAssignTools.ParseFloatNode(matNode.SelectSingleNode("SpecularLevel"),0.0f)/10.0f;
			matData.normalReflectLevel = matData.tangentReflectLevel/5.0f;
			matData.bumpLevel = AutoAssignTools.ParseFloatNode(matNode.SelectSingleNode("BumpLevel"),0.0f);
			
			
			//color parameters
			matData.diffuseColor = AutoAssignTools.ParseVector4Node(matNode.SelectSingleNode("Diffuse"),1,1,1,matData.opacityLevel);
			matData.reflectColor = AutoAssignTools.ParseVector4Node(matNode.SelectSingleNode("Specular"),0,0,0,1);
			
			
			//texture parameters
			/*
			matData.diffuseMap = ParseXmlTextureNode( matNode.SelectSingleNode("DiffuseColor"),"name",TextureType.Standard);
			matData.selfillumMap = ParseXmlTextureNode( matNode.SelectSingleNode("EmissiveColor"),"name",TextureType.Lightmap);
			matData.opacityMap   = ParseXmlTextureNode( matNode.SelectSingleNode("TransparentColor"),"name",TextureType.Standard);
			matData.bumpMap = ParseXmlTextureNode( matNode.SelectSingleNode("Bump"),"name",TextureType.Normalmap);
			
			matData = CreateOrGetMixedTextureFromMaterialData(matData);
			*/
			
			Material mat  = matData.CreateMaterial(defaultCubemapTexture, false);
			
			/*
			if(AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) != null){
				Debug.LogError("Cannot write material \"" + materialName + "\" to disk because a file already exists with that name.");
			}else{
				AssetDatabase.CreateAsset(mat, materialPath);
			}
			*/
			
			return mat;
		}
		
		private MaterialData CreateMaterialFromO3dNode(XmlNode matNode)
		{	
			XmlNode diffuseNode = matNode.SelectSingleNode("Diffuse");
			XmlNode opacityNode = matNode.SelectSingleNode("Opacity");
			XmlNode bumpNode = matNode.SelectSingleNode("Bump");
			XmlNode selfillumNode = matNode.SelectSingleNode("Selfillum");
			//XmlNode reflectNode = matNode.SelectSingleNode("Reflect");
			
			
			MaterialData matData = new MaterialData();
			
			matData.isLightResponsive = isLightResponsive;
			
			if(selfillumNode!=null){
				//matData.selfillumMap = ParseO3dTexture( selfillumNode.SelectSingleNode("Bitmap"),"name",TextureType.Lightmap);
				if(selfillumNode.SelectSingleNode("Bitmap") != null) {
					matData.selfillumLevel = AutoAssignTools.ParseFloatNode(bumpNode.SelectSingleNode("SelfillumLevel"),1.0f);
					matData.selfillumMapName = AutoAssignTools.GetFilenameWithoutExtension(selfillumNode.SelectSingleNode("Bitmap").SelectSingleNode("Source").InnerText)+".tex.o3d";
				}
			}
			
			if(opacityNode!=null){
				matData.opacityLevel = AutoAssignTools.ParseFloatNode(opacityNode.SelectSingleNode("OpacityLevel"),1.0f);
				//matData.opacityMap   = ParseO3dTexture( opacityNode.SelectSingleNode("Bitmap"),"name",TextureType.Standard);
				if(opacityNode.SelectSingleNode("Bitmap") != null) {
					matData.opacityMapName = AutoAssignTools.GetFilenameWithoutExtension(opacityNode.SelectSingleNode("Bitmap").SelectSingleNode("Source").InnerText)+".tex.o3d";
					if(matData.opacityMap!=null || matData.opacityMapName != null) matData.selfillumLevel *= 0.6f;
				}
			}
			
			if(diffuseNode!=null){
				matData.diffuseColor = AutoAssignTools.ParseVector4Node(diffuseNode.SelectSingleNode("DiffuseColor"),1,1,1,matData.opacityLevel);
				
				XmlNode bitmapNode = diffuseNode.SelectSingleNode("Bitmap");
				XmlNode compositeNode = diffuseNode.SelectSingleNode("Composite");
				if(bitmapNode!=null){
					//matData.diffuseMap = ParseO3dTexture( diffuseNode.SelectSingleNode("Bitmap"),"name",TextureType.Standard);
					matData.diffuseMapName = AutoAssignTools.GetFilenameWithoutExtension(diffuseNode.SelectSingleNode("Bitmap").SelectSingleNode("Source").InnerText)+".tex.o3d";
				}else if(compositeNode!=null){
					
					XmlNodeList layerNodes = compositeNode.SelectNodes("Layer");
					foreach(XmlNode layerNode in layerNodes){
						string layerName = ((XmlElement)layerNode).GetAttribute("name").ToString();
						float layerOpacity = AutoAssignTools.ParseFloatAttribute(layerNode,"opacity",1.0f);
						//string layerBlendMode = ((XmlElement)layerNode).GetAttribute("blendmode").ToString();
						if(layerName == "diffuse"){
							//matData.diffuseMap = ParseO3dTexture( layerNode.SelectSingleNode("Bitmap"),"name",TextureType.Standard);
							if(layerNode.SelectSingleNode("Bitmap") != null)
								matData.diffuseMapName = AutoAssignTools.GetFilenameWithoutExtension(layerNode.SelectSingleNode("Bitmap").SelectSingleNode("Source").InnerText)+".tex.o3d";
						}else if(layerName == "lightmap"){
							//matData.selfillumMap = ParseO3dTexture( layerNode.SelectSingleNode("Bitmap"),"name",TextureType.Lightmap);
							if(layerNode.SelectSingleNode("Bitmap") != null) {
								matData.selfillumMapName = AutoAssignTools.GetFilenameWithoutExtension(layerNode.SelectSingleNode("Bitmap").SelectSingleNode("Source").InnerText)+".tex.o3d";
								matData.selfillumLevel = layerOpacity;
							}
						}else if(layerName == "occlusion"){
							//matData.occlusionMap = ParseO3dTexture( layerNode.SelectSingleNode("Bitmap"),"name",TextureType.Occlusion);
							if(layerNode.SelectSingleNode("Bitmap") != null) {
								matData.occlusionMapName = AutoAssignTools.GetFilenameWithoutExtension(layerNode.SelectSingleNode("Bitmap").SelectSingleNode("Source").InnerText)+".tex.o3d";
								matData.occlusionLevel = layerOpacity;
							}
						}
					}
				}
			}
			
			
			/*if(bumpNode!=null){
				if(bumpNode.SelectSingleNode("Bitmap") != null) {
					matData.bumpLevel = 0.0f;//AutoAssignTools.ParseFloatNode(bumpNode.SelectSingleNode("BumpLevel"),0.0f);
					//matData.bumpMap = ParseO3dTexture( bumpNode.SelectSingleNode("Bitmap"),"name",TextureType.Normalmap);
					matData.bumpMapName = AutoAssignTools.GetFilenameWithoutExtension(bumpNode.SelectSingleNode("Bitmap").SelectSingleNode("Source").InnerText)+".tex.o3d";
				}
			}*/
			
			
			/*if(reflectNode!=null){
				matData.reflectGloss = AutoAssignTools.ParseFloatNode(reflectNode.SelectSingleNode("ReflectGloss"),0.0f);
				matData.normalReflectLevel = AutoAssignTools.ParseFloatNode(reflectNode.SelectSingleNode("NormalReflectLevel"),0.0f);
				matData.tangentReflectLevel = AutoAssignTools.ParseFloatNode(reflectNode.SelectSingleNode("TangentReflectLevel"),0.0f);
				matData.reflectColor  = AutoAssignTools.ParseVector4Node(reflectNode.SelectSingleNode("ReflectColor"),0,0,0,1);
			}*/
			
			/*
			if(AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) != null){
				Debug.LogError("Cannot write material \"" + materialName + "\" to disk because a file already exists with that name.");
			}else{
				AssetDatabase.CreateAsset(mat, materialPath);
			}
			*/
			
			return matData;
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
	}
}