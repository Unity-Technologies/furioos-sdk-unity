using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Rise.SDK.Importer {
	public class RSImporter : AssetPostprocessor {
		private const string materialFolderName = "Materials";

		private static List<string> sbsarPaths;

		void OnPreprocessModel() {
			ModelImporter modelImporter = (ModelImporter)assetImporter;
			modelImporter.importMaterials = false;

			sbsarPaths = new List<string> ();
		}

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			string fbxPath = importedAssets.SingleOrDefault (ia => ia.Contains (".fbx"));
			string jsonPath = importedAssets.SingleOrDefault(ia => ia.Contains (".json"));

			HandleSubstancePaths (importedAssets);

			if (string.IsNullOrEmpty(fbxPath) || string.IsNullOrEmpty(fbxPath)) {
				return;
			}

			string basePath = Path.GetDirectoryName (fbxPath);

			AssetDatabase.CreateFolder (basePath, materialFolderName);

			sbsarPaths.ForEach (smp => {
				string smName = Path.GetFileName(smp);
				string newPath = basePath + "/" + materialFolderName + "/" + smName;
				AssetDatabase.MoveAsset(smp, newPath);
			});

			AssetDatabase.SaveAssets ();

			Object fbx = AssetDatabase.LoadAssetAtPath (fbxPath, typeof(Object));
			TextAsset json = (TextAsset)AssetDatabase.LoadAssetAtPath (jsonPath, typeof(TextAsset));


		}

		static void HandleSubstancePaths(string[] importedAssets) {
			IEnumerable<string> substanceMaterialPaths = importedAssets.Where (ia => ia.Contains (".sbsar"));

			substanceMaterialPaths.ToList ().ForEach (smp => {
				string path = sbsarPaths.SingleOrDefault (sbp => sbp == smp);

				if (string.IsNullOrEmpty (path)) {
					return;
				}

				sbsarPaths.Add (smp);
			});
		}
	}
}