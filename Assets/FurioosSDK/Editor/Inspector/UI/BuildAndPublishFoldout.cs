using UnityEngine;
using UnityEditor;

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

				GUI.enabled = FurioosInspector.selectedApplication != null;
				Rect buildAndPublishButtonRect = EditorGUILayout.BeginVertical();
					buildAndPublishButtonRect.width /= 2;
					buildAndPublishButtonRect.x = buildAndPublishButtonRect.width / 2;

					if (GUI.Button(buildAndPublishButtonRect, "Build & Publish")) {

					}
					GUILayout.Space(40);
				EditorGUILayout.EndVertical();
				GUI.enabled = true;
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
		}
	}
}