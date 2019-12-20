using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO.Compression;

namespace FurioosSDK.Editor {
	public class BuildAndPublishFoldout {
		public enum StreamEngines {
			Furioos = 0,
			RenderStreaming = 1
		};

		public enum Platforms {
			Windows = 0,
			Linux = 1
		};

		static bool _showBuildAndPublish = true;

		static int _selectedEngine;
		static int _selectedPlatform;

		public static void Draw() {
			_showBuildAndPublish = EditorGUILayout.BeginFoldoutHeaderGroup(_showBuildAndPublish, "Build And Publish", EditorStyles.foldoutHeader);
			if (_showBuildAndPublish) {
				_selectedEngine = (int)(StreamEngines)EditorGUILayout.EnumPopup(
					"Streaming engine",
					(StreamEngines)_selectedEngine
				);

				_selectedPlatform = (int)(Platforms)EditorGUILayout.EnumPopup(
					"Platform",
					(Platforms)_selectedPlatform
				);

				GUILayout.Space(5);

				GUI.enabled = FurioosInspector.selectedApplication != null && !FurioosInspector.lockUI;
				Rect buildAndPublishButtonRect = EditorGUILayout.BeginVertical();
				buildAndPublishButtonRect.width /= 2;
				buildAndPublishButtonRect.x = buildAndPublishButtonRect.width / 2;

				if (GUI.Button(buildAndPublishButtonRect, "Build & Publish")) {
					string buildPath = EditorUtility.SaveFilePanel("Save build to folder", "", "", "exe");
					string[] scenes = new string[EditorBuildSettings.scenes.Length];

					Debug.Log(EditorBuildSettings.scenes);

					for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
						if(!EditorBuildSettings.scenes[i].enabled) {
							continue;
						}

						scenes[i] = EditorBuildSettings.scenes[i].path;
					}

					BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions {
						scenes = scenes,
						locationPathName = buildPath,
						target = (_selectedPlatform == (int)Platforms.Windows) ? BuildTarget.StandaloneWindows64 : BuildTarget.StandaloneOSX,
						options = BuildOptions.None
					};

					BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
					BuildSummary summary = report.summary;
				}
				GUILayout.Space(40);
				EditorGUILayout.EndVertical();
				GUI.enabled = !FurioosInspector.lockUI;
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
		}
	}
}