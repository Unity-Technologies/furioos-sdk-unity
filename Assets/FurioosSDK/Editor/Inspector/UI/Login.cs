using UnityEngine;
using UnityEditor;

namespace FurioosSDK.Editor {
	public class Login: Object {
		static string _email = "m.korbas@obvioos.com";
		static string _password = "MK09J1291";

		public static void Draw(Rect position) {
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();

			string[] logoGUIDS = AssetDatabase.FindAssets("furioos-logo", null);
			if (logoGUIDS.Length > 0) {
				string logoPath = AssetDatabase.GUIDToAssetPath(logoGUIDS[0]);
				Texture2D logo = (Texture2D)AssetDatabase.LoadAssetAtPath(logoPath, typeof(Texture2D));
				GUI.DrawTexture(new Rect((position.width / 2) - 100, 30, 200, 32), logo);
			}

			EditorGUILayout.LabelField("Email");
			_email = EditorGUILayout.TextField(_email);

			EditorGUILayout.LabelField("Password");
			_password = EditorGUILayout.PasswordField(_password);

			GUILayout.Space(5);

			GUI.enabled = (_email != "" && _password != "");
			if (GUILayout.Button("Login")) {
				FurioosInspector.connectionHandler.LoginEmail(_email, _password);
			}
			GUI.enabled = true;

			EditorGUILayout.Space();

			if (GUILayout.Button("Create account")) {

			}

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
	}
}