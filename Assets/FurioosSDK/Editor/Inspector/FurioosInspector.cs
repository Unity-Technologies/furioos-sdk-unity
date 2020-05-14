using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FurioosSDK.Editor {
	public class FurioosInspector: EditorWindow {
		const bool SSL = false;
		const string WS_URL = "localhost:3000";

		public static FurioosConnectionHandler connectionHandler;

		public static ApplicationCollection selectedApplication;
		public static int selectedEngine;
		public static int selectedPlatform;

		public static bool lockUI;

		[MenuItem("Window/Furioos")]
		private static void Init() {
			FurioosInspector inspector = (FurioosInspector)EditorWindow.GetWindow(typeof(FurioosInspector), false, "Furioos");

			Texture2D logo = new Texture2D(0, 0);
			string[] iconGUIDS = AssetDatabase.FindAssets("furioos-icon", null);
			if (iconGUIDS.Length > 0) {
				string logoPath = AssetDatabase.GUIDToAssetPath(iconGUIDS[0]);
				logo = (Texture2D)AssetDatabase.LoadAssetAtPath(logoPath, typeof(Texture2D));
			}

			inspector.titleContent = new GUIContent("Furioos", logo);
			inspector.minSize = new Vector2(350.0f, 520.0f);
			inspector.Show();
		}

		private void Awake() {
			selectedApplication = null;

			Connect();
		}

		private void Connect() {
			connectionHandler = new FurioosConnectionHandler();
			connectionHandler.Connect(WS_URL, SSL);
		}

		private void OnGUI() {
			GUI.enabled = !lockUI;

			if(connectionHandler == null) {
				Connect();
			}

			if(!connectionHandler.Connected) {
				DrawRetryConnect();
				return;
			}

			if(!connectionHandler.Logged) {
				Login.Draw(this.position);
				return;
			}

			DrawHeader();

			if(
				connectionHandler.QuotaReady &&
				connectionHandler.StorageReady &&
				connectionHandler.RegionReady
			) {
				StorageFoldout.Draw();

				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			}

			if(connectionHandler.ApplicationReady) {
				ApplicationFoldout.Draw();

				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

				BuildAndPublishFoldout.Draw();
			}

			GUI.enabled = true;

			this.Repaint();
		}

		private void DrawHeader() {

		}

		private void DrawRetryConnect() {
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();

			GUILayout.Label("Cannot connect to Furioos server :'(");
			if(GUILayout.Button("Retry")) {
				Connect();
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		public static void SaveChanges() {
			connectionHandler.SaveChanges(selectedApplication);
		}

		public static string SizeSuffix(float bytes) {
			string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
			int i;
			double dblSByte = bytes;
			for (i = 0; i < Suffix.Length && bytes >= 1024.0f; i++, bytes /= 1024.0f) {
				dblSByte = bytes / 1024.0f;
			}

			return string.Format("{0:0.##}{1}", dblSByte, Suffix[i]);
		}
	}
}