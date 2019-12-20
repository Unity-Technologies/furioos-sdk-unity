using UnityEngine;
using UnityEditor;
using System;

namespace FurioosSDK.Editor {
	public class ApplicationFoldout {
		public enum QualityModes {
			Low = 0,
			Medium = 1,
			High = 2,
			Ultra = 3,
			Auto = 4
		};

		public enum GPUTypes {
			Standard = 0,
		};

		static bool _showApplication = true;

		static int _selectedApplicationIndex = -1;
		static Texture2D _selectedApplicationThumbnail = new Texture2D(0, 0);

		public static void Draw() {
			_showApplication = EditorGUILayout.BeginFoldoutHeaderGroup(_showApplication, "Application", EditorStyles.foldoutHeader);
			if (_showApplication) {
				ApplicationCollection[] applications = FurioosInspector.connectionHandler.Applications;

				GUIContent dropDownContent = new GUIContent(
					(FurioosInspector.selectedApplication != null) ? FurioosInspector.selectedApplication.name : "Select an application"
				);

				Rect dropdownRect = EditorGUILayout.BeginHorizontal();
				if (EditorGUILayout.DropdownButton(dropDownContent, FocusType.Passive)) {
					GenericMenu menu = new GenericMenu();

					for (int i = 0; i < applications.Length; i++) {
						string id = FurioosInspector.selectedApplication?._id ?? "";
						menu.AddItem(
							new GUIContent(applications[i].name),
							(id == applications[i]._id),
							OnApplicationSelected,
							i
						);
					}

					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Create new application..."), false, OnCreateSelected);

					menu.DropDown(dropdownRect);
					menu.ShowAsContext();
				}
				EditorGUILayout.EndHorizontal();

				if(FurioosInspector.selectedApplication == null) {
					EditorGUILayout.EndFoldoutHeaderGroup();
					return;
				}

				FurioosInspector.selectedApplication.name = EditorGUILayout.TextField(new GUIContent("Name"), FurioosInspector.selectedApplication.name);

				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Description");
					Rect textAreaRect = EditorGUILayout.BeginVertical();
						textAreaRect.x += 2;
						textAreaRect.width -= 5;

						FurioosInspector.selectedApplication.description = EditorGUI.TextArea(textAreaRect, FurioosInspector.selectedApplication.description);
						GUILayout.Space(70);
					EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(1);

				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Thumbnail");
						Rect previewRect = EditorGUILayout.BeginVertical();
							previewRect.x += 2;
							previewRect.width -= 5;

						if(FurioosInspector.selectedApplication.thumbnailUrl != "") {
							//EditorGUI.DrawPreviewTexture(previewRect, _selectedApplicationThumbnail);
						}
					EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();

				FurioosInspector.selectedApplication.parameters.defaultQuality = (int)(QualityModes)EditorGUILayout.EnumPopup(
					"Quality",
					(QualityModes)FurioosInspector.selectedApplication.parameters.defaultQuality
				);

				FurioosInspector.selectedApplication.parameters.mouseLock = EditorGUILayout.Toggle(
					"Mouse lock",
					FurioosInspector.selectedApplication.parameters.mouseLock
				);

				FurioosInspector.selectedApplication.parameters.touchConvert = EditorGUILayout.Toggle(
					"Convert touch",
					FurioosInspector.selectedApplication.parameters.touchConvert
				);

				GUI.enabled = false;
				EditorGUILayout.EnumPopup("GPU type", GPUTypes.Standard);
				GUI.enabled = !FurioosInspector.lockUI;

				EditorGUILayout.BeginHorizontal();
					GUILayout.Label("Size");
					GUILayout.FlexibleSpace();
					GUILayout.Label(
						FurioosInspector.SizeSuffix(FurioosInspector.selectedApplication.binaries[0].size)
					);
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(5);

				Rect saveButtonRect = EditorGUILayout.BeginVertical();
				saveButtonRect.width /= 2;
				saveButtonRect.x = saveButtonRect.width / 2;

				if (GUI.Button(saveButtonRect, "Save changes")) {
					FurioosInspector.SaveChanges();
				}
				GUILayout.Space(40);
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
		}

		public static void OnApplicationSelected(object index) {
			FurioosInspector.selectedApplication = FurioosInspector.connectionHandler.Applications[(int)index];
			_selectedApplicationIndex = (int)index;
		}

        public static void OnCreateSelected() {
			//FurioosInspector.connectionHandler.Applications
		}
	}
}