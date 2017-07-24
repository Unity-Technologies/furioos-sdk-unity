using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rise.SDK.ModelBuilder {
	public class RSModelBuilder : AssetPostprocessor {
		private const string materialFolderName = "Materials";
		private const string textureFolderName = "textures";
		private const string substanceExtensionName = ".sbsar";
		private const string jsonExtensionName = ".json";
		private const string directorySeparatorChar = "/";

		private static string basePath = "";
		private static string textureFolderPath = "";
		private static string materialsFolderPath = "";

		private static List<string> sbars;
		private static List<string> textures;
		private static Dictionary<int, Material> handledMaterials;

		private static RSMBModel modelDefinition;

		void OnPreprocessModel() {
			ModelImporter modelImporter = (ModelImporter)assetImporter;
			modelImporter.importMaterials = false;
		}

		[MenuItem("Rise SDK/Model/Build")]
		public static void BuildEntry() {
			//try {
				Build();
			/*}
			catch(System.Exception e) {
				Debug.LogError(e.Message);
				EditorUtility.ClearProgressBar();
			}*/
		}

		public static void Build() {
			if(Selection.activeObject == null) {
				Debug.LogError("[ModelBuilder] > Please select a model in project.");
				return;
			}

			EditorUtility.DisplayProgressBar("Model builder", "Initialisation", 0.0f);

			Object model = Selection.activeObject;
			string modelPath = AssetDatabase.GetAssetPath(model);
			string modelName = Path.GetFileNameWithoutExtension(modelPath);

			basePath = Path.GetDirectoryName(modelPath);

			textureFolderPath = basePath + directorySeparatorChar + textureFolderName;
			if(!Directory.Exists(textureFolderPath)) {
				Debug.LogError("[ModelBuilder] > No texture file found.");
				return;
			}

			string jsonFilePath = basePath + directorySeparatorChar + modelName + jsonExtensionName;
			if(!File.Exists(jsonFilePath)) {
				Debug.LogError("[ModelBuilder] > No json file found.");
				return;
			}

			materialsFolderPath = basePath + directorySeparatorChar + materialFolderName;
			if(!Directory.Exists(materialsFolderPath)) {
				AssetDatabase.CreateFolder(basePath, materialFolderName);
			}
				
			EditorUtility.DisplayProgressBar(
				"Model builder", 
				"Handle texture folder",
				0.2f
			);

			HandleTextureFolder(basePath);

			TextAsset jsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonFilePath);

			modelDefinition = JsonConvert.DeserializeObject<RSMBModel>(jsonFile.text);

			GameObject instantiedModel = (GameObject)PrefabUtility.InstantiatePrefab(model);
			PrefabUtility.DisconnectPrefabInstance(instantiedModel);

			handledMaterials = new Dictionary<int, Material>();

			foreach(RSMBMesh mesh in modelDefinition.Meshes) {
				Transform tgo = instantiedModel.transform.Find(mesh.Name);

				if(tgo == null) {
					continue;
				}

				GameObject go = tgo.gameObject;

				if(go.GetComponent<MeshRenderer>() == null) {
					continue;
				}

				GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.LightmapStatic);
				GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.OccluderStatic);

				SerializedObject mr = new SerializedObject(go.GetComponent<MeshRenderer>());
				SerializedProperty mrls = mr.FindProperty("m_LightmapParameters");

				SerializedProperty iterator = mrls.Copy();
				while(iterator.Next(true)) {
					Debug.Log(iterator.name);
				}
					
				int materialLength = mesh.Materials.Length;
				RSMBMaterial[] materialsDefintion = new RSMBMaterial[materialLength];
				for(int i = 0; i < materialLength; i++) {
					int materialId = mesh.Materials[i];
					RSMBMaterial materialDefinition = modelDefinition.Materials.SingleOrDefault(m => m.Id == materialId);

					if(materialDefinition == null) {
						continue;
					}

					materialsDefintion[i] = materialDefinition;
				}

				HandleMaterials(go, materialsDefintion);
			}

			EditorUtility.DisplayProgressBar(
				"Model buider", 
				"Building and saving prefab", 
				1.0f
			);

			PrefabUtility.CreatePrefab(basePath + directorySeparatorChar + modelName + ".prefab", instantiedModel, ReplacePrefabOptions.ConnectToPrefab);
			Object.DestroyImmediate(instantiedModel);

			EditorUtility.ClearProgressBar();
		}

		private static void HandleMaterials(GameObject go, RSMBMaterial[] materialsDefinition) {
			MeshRenderer mr = go.GetComponent<MeshRenderer>();
			Material[] materials = new Material[materialsDefinition.Length];



			for(int i = 0; i < materialsDefinition.Length; i++) {
				int materialId = materialsDefinition[i].Id;

				EditorUtility.DisplayProgressBar(
					"Model buider", 
					"Handle material - " + materialsDefinition[i].Name, 
					0.5f + (i / materialsDefinition.Length) * 0.9f
				);

				if(handledMaterials.ContainsKey(materialId)) {
					materials[i] = handledMaterials[materialId];

					continue;
				}

				RSMBMaterial materialDefinition = materialsDefinition[i];
				switch(materialDefinition.Type) {
					case "SubstanceMtl":
						materials[i] = HandleProceduralMaterial(materialDefinition);
						break;
					case "StdMtl":
						materials[i] = HandleStandardMaterial(materialDefinition);
						break;
					default:
						materials[i] = HandleStandardMaterial(materialDefinition);
						break;
				}

				handledMaterials.Add(materialId, materials[i]);
			}

			mr.sharedMaterials = materials;
		}

		private static Material HandleProceduralMaterial(RSMBMaterial materialDefinition) {
			string materialName = Path.GetFileName(materialDefinition.Path);

			if(string.IsNullOrEmpty(sbars.SingleOrDefault(s => s == materialName))) {
				return new Material(Shader.Find("Standard"));
			}

			SubstanceImporter substanceIpt = (SubstanceImporter)AssetImporter.GetAtPath(materialsFolderPath + directorySeparatorChar + materialName);
			ProceduralMaterial substanceMtl = substanceIpt.GetMaterials()[0];

			bool materialExist = handledMaterials.ContainsValue((Material)substanceMtl);
			if(materialExist) {
				substanceIpt.CloneMaterial(substanceMtl);
				substanceIpt.SaveAndReimport();
				substanceMtl = substanceIpt.GetMaterials()[substanceIpt.GetMaterialCount() - 1];
			}

			substanceIpt.SetPlatformTextureSettings(substanceMtl, "", 1024, 1024, 0, (int)ProceduralLoadingBehavior.BakeAndDiscard);
			substanceIpt.SetPlatformTextureSettings(substanceMtl, "Standalone", 2048, 2048, 0, (int)ProceduralLoadingBehavior.BakeAndDiscard);
			substanceIpt.SetPlatformTextureSettings(substanceMtl, "Android", 1024, 1024, 0, (int)ProceduralLoadingBehavior.BakeAndDiscard);
			substanceIpt.SetPlatformTextureSettings(substanceMtl, "iPhone", 1024, 1024, 0, (int)ProceduralLoadingBehavior.BakeAndDiscard);
			substanceIpt.SetPlatformTextureSettings(substanceMtl, "WebGL", 1024, 1024, 0, (int)ProceduralLoadingBehavior.BakeAndDiscard);
			substanceIpt.SetPlatformTextureSettings(substanceMtl, "Windows Store Apps", 1024, 1024, 0, (int)ProceduralLoadingBehavior.BakeAndDiscard);

			ProceduralPropertyDescription[] propertiesDescription = substanceMtl.GetProceduralPropertyDescriptions();
			foreach(ProceduralPropertyDescription propertyDescription in propertiesDescription) {
				if(!materialDefinition.Parameters.ContainsKey(propertyDescription.name)) {
					continue;
				}

				string value = materialDefinition.Parameters[propertyDescription.name];

				switch(propertyDescription.type) {
				case ProceduralPropertyType.Boolean:
					substanceMtl.SetProceduralBoolean(propertyDescription.name, HandleProceduralPropertyBool(value));
					break;
				case ProceduralPropertyType.Float:
					substanceMtl.SetProceduralFloat(propertyDescription.name, HandleProceduralPropertyFloat(value));
					break;
				case ProceduralPropertyType.Color3:
					substanceMtl.SetProceduralColor(propertyDescription.name, HandleProceduralPropertyColor(value));
					break;
				case ProceduralPropertyType.Color4:
					substanceMtl.SetProceduralColor(propertyDescription.name, HandleProceduralPropertyColor(value));
					break;
				case ProceduralPropertyType.Enum:
					substanceMtl.SetProceduralEnum(propertyDescription.name, HandleProceduralPropertyInt(value) - 1);
					break;
				}
			}


			return substanceMtl;
		}

		private static bool HandleProceduralPropertyBool(string value) {
			Regex boolRgx = new Regex("^([0-1])$");
			Match match = boolRgx.Match(value);

			if(!match.Success) {
				return false;
			}

			return (match.Groups[1].Value == "1");
		}

		private static string HandleProceduralPropertyString(string value) {
			Regex stringRgx = new Regex("^([a-zA-Z0-9.]+)$");
			Match matchString = stringRgx.Match(value);

			if(!matchString.Success) {
				return "";
			}

			return matchString.Groups[1].Value;
		}

		private static float HandleProceduralPropertyFloat(string value) {
			Regex floatRgx = new Regex("^([0-9].*)$");
			Match matchFloat = floatRgx.Match(value);

			if(!matchFloat.Success) {
				return 0.0f;
			}

			return float.Parse(matchFloat.Groups[1].Value);
		}

		private static Color HandleProceduralPropertyColor(string value) {
			Regex colorRgx = new Regex("^\\(color ([0-9]+) ([0-9]+) ([0-9]+)\\)$");
			Match matchColor = colorRgx.Match(value);

			if(!matchColor.Success) {
				return Color.white;
			}

			return new Color(
				float.Parse(matchColor.Groups[1].Value) / 255.0f,
				float.Parse(matchColor.Groups[2].Value) / 255.0f,
				float.Parse(matchColor.Groups[3].Value) / 255.0f,
				(matchColor.Groups.Count == 5) ? float.Parse(matchColor.Groups[4].Value) : 1.0f
			);
		}

		private static int HandleProceduralPropertyInt(string value) {
			Regex intRgx = new Regex("^([0-9]+)$");
			Match matchInt = intRgx.Match(value);

			if(!matchInt.Success) {
				return 0;
			}

			return int.Parse(matchInt.Groups[1].Value);
		}

		private static Material HandleStandardMaterial(RSMBMaterial materialDefinition) {
			string materialName = materialDefinition.Name + "_" + materialDefinition.Id;

			Material stdMaterial = new Material(Shader.Find("Standard"));

			Regex colorRgx = new Regex("^([0-9.]+) ([0-9.]+) ([0-9.]+) ?([0-9.]*)$");

			if(materialDefinition.Parameters == null || materialDefinition.Parameters.Count == 0) {
				return stdMaterial;
			}

			if(materialDefinition.Parameters.ContainsKey("diffuseColor")) {
				Match matchColor = colorRgx.Match(materialDefinition.Parameters["diffuseColor"]);
				Color diffuseColor = Color.white;

				if(matchColor.Success) {
					diffuseColor = new Color(
						float.Parse(matchColor.Groups[1].Value) / 255.0f,
						float.Parse(matchColor.Groups[2].Value) / 255.0f,
						float.Parse(matchColor.Groups[3].Value) / 255.0f,
						(matchColor.Groups.Count == 5) ? float.Parse(matchColor.Groups[4].Value) : 1.0f
					);
				}
			}

			if(materialDefinition.Parameters.ContainsKey("diffuseMap")) {
				string textureName = Path.GetFileName(materialDefinition.Parameters["diffuseMap"]);
				string texturePath = textureFolderPath + directorySeparatorChar + textureName;

				if(File.Exists(texturePath)) {
					TextureImporter diffuseTextureImpt = (TextureImporter)AssetImporter.GetAtPath(texturePath);

					HandleTexturePlateformSettings(diffuseTextureImpt);

					diffuseTextureImpt.SaveAndReimport();

					Texture2D diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

					stdMaterial.mainTexture = diffuseTexture;
				}
			}

			if(materialDefinition.Parameters.ContainsKey("bumpMap")) {
				string textureName = Path.GetFileName(materialDefinition.Parameters["bumpMap"]);
				string texturePath = textureFolderPath + directorySeparatorChar + textureName;

				if(File.Exists(texturePath)) {
					TextureImporter bumpTextureImpt = (TextureImporter)AssetImporter.GetAtPath(texturePath);
					bumpTextureImpt.textureType = TextureImporterType.NormalMap;
					bumpTextureImpt.convertToNormalmap = true;
					bumpTextureImpt.normalmapFilter = TextureImporterNormalFilter.Standard;

					HandleTexturePlateformSettings(bumpTextureImpt);

					bumpTextureImpt.SaveAndReimport();

					Texture2D bumpTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

					stdMaterial.SetTexture("_BumpMap", bumpTexture);
				}
			}

			if(materialDefinition.Parameters.ContainsKey("bumpMapIntensity")) {
				float bumpMapIntensity = float.Parse(materialDefinition.Parameters["bumpMapIntensity"]);

				switch(materialDefinition.Type) {
				case "StdMtl":
					bumpMapIntensity /= 100.0f;
					break;
				case "VrayMtl":
					bumpMapIntensity /= 30.0f;
					break;
				}

				stdMaterial.SetFloat("_BumpScale", bumpMapIntensity);
			}

			AssetDatabase.CreateAsset(stdMaterial, materialsFolderPath + directorySeparatorChar + materialName + ".mat");

			return stdMaterial;
		}

		private static void HandleTexturePlateformSettings(TextureImporter textureIpt) {
			TextureImporterPlatformSettings[] plateformSettings = new TextureImporterPlatformSettings[5];
			plateformSettings[0] = new TextureImporterPlatformSettings() {
				name = "",
				maxTextureSize = 2048,
				textureCompression = TextureImporterCompression.CompressedHQ,
				format = TextureImporterFormat.Automatic,
				overridden = true
			};

			plateformSettings[1] = new TextureImporterPlatformSettings() {
				name = "Standalone",
				maxTextureSize = 2048,
				textureCompression = TextureImporterCompression.CompressedHQ,
				format = TextureImporterFormat.Automatic
			};

			plateformSettings[2] = new TextureImporterPlatformSettings() {
				name = "iPhone",
				maxTextureSize = 1024,
				textureCompression = TextureImporterCompression.CompressedLQ,
				format = TextureImporterFormat.Automatic
			};

			plateformSettings[3] = new TextureImporterPlatformSettings() {
				name = "Android",
				maxTextureSize = 1024,
				textureCompression = TextureImporterCompression.CompressedLQ,
				format = TextureImporterFormat.Automatic
			};

			plateformSettings[4] = new TextureImporterPlatformSettings() {
				name = "WebGL",
				maxTextureSize = 1024,
				textureCompression = TextureImporterCompression.CompressedLQ,
				format = TextureImporterFormat.Automatic
			};
					
			for(int i = 0; i < plateformSettings.Length; i++) {
				textureIpt.SetPlatformTextureSettings(plateformSettings[i]);
			}
		}

		private static void HandleTextureFolder(string basePath) {
			sbars = new List<string>();
			textures = new List<string>();

			string[] paths = Directory.GetFiles(basePath + directorySeparatorChar + textureFolderName);

			for(int i = 0; i < paths.Length; i++) {
				string fileName = Path.GetFileName(paths[i]);

				if(fileName.Contains(substanceExtensionName)) {
					AssetDatabase.MoveAsset(paths[i], basePath + directorySeparatorChar + materialFolderName + directorySeparatorChar + fileName);

					sbars.Add(fileName);

					continue;
				}

				EditorUtility.DisplayProgressBar(
					"Model buider", 
					"Moving - " + fileName,
					0.2f + (i / paths.Length) * 0.3f
				);

				textures.Add(fileName);
			}
		}
	}
}