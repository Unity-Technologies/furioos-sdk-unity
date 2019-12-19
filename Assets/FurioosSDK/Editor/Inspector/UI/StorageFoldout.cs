using UnityEngine;
using UnityEditor;

namespace FurioosSDK.Editor {
	public class StorageFoldout: Object {
		static bool _showStorage = true;

		public static void Draw() {
			_showStorage = EditorGUILayout.BeginFoldoutHeaderGroup(_showStorage, "Storage", EditorStyles.foldoutHeader);
			if (_showStorage) {
				QuotaCollection quota = FurioosInspector.connectionHandler.Quota;
				RegionCollection region = FurioosInspector.connectionHandler.Region;

				double amount = double.Parse((quota.storage + quota.extra.storage).ToString(), System.Globalization.NumberStyles.Float);
				double usage = double.Parse(quota.usage.storage.ToString(), System.Globalization.NumberStyles.Float);
				float percent = (float)usage / (float)amount;

				EditorGUILayout.BeginHorizontal();
					GUILayout.Label("Space");
					Rect progressRect = EditorGUILayout.BeginVertical();
						progressRect.x += 2;
						progressRect.width -= 5;

						EditorGUI.ProgressBar(
							progressRect,
							percent,
							FurioosInspector.SizeSuffix((float)usage) + " / " + FurioosInspector.SizeSuffix((float)amount)
						);
						GUILayout.Space(20);
					EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
					GUILayout.Label("Region");
					GUILayout.FlexibleSpace();
					GUILayout.Label(region.name);
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
		}
	}
}