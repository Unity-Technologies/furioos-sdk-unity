//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    public class ImportFontParametersSection
    {
        private string m_IconFontDestinationFolder;

        public ImportFontParametersSection()
        {
			m_IconFontDestinationFolder = VectorImageManager.fontDestinationFolder;
        }

        public void DrawInspector()
        {
            VectorImageManagerWindow.DrawHeader("Parameters");
            {
                VectorImageManagerWindow.BeginContents();
                {
                    EditorGUILayout.HelpBox("Specify where you want to save all the fonts imported using VectorImageManager. (relative to your Assets/ folder)", MessageType.Info);

					using (new GUILayout.HorizontalScope())
					{
						EditorGUI.BeginChangeCheck();
						m_IconFontDestinationFolder = EditorGUILayout.TextField("Destination folder", m_IconFontDestinationFolder);
						if (EditorGUI.EndChangeCheck())
						{
							UpdateFontDestinationFolder(m_IconFontDestinationFolder);
						}
						
						if (GUILayout.Button("Select folder", EditorStyles.miniButton, GUILayout.Width(75f)))
						{
							GUI.FocusControl(null);
							
							string folderPath = EditorUtility.OpenFolderPanel("VectorImage Fonts destination folder", Application.dataPath, null);
							
							if (string.IsNullOrEmpty(folderPath))
							{
								return;
							}
							
							if (!folderPath.Contains(Application.dataPath))
							{
								EditorUtility.DisplayDialog("Error", "The folder you select, must be inside your Assets/ folder", "Ok");
								return;
							}
							
							folderPath = folderPath.Replace(Application.dataPath, string.Empty);
							UpdateFontDestinationFolder(folderPath);
						}
					}
                }
                VectorImageManagerWindow.EndContents();
            }
        }

        private void UpdateFontDestinationFolder(string folderPath)
        {
            m_IconFontDestinationFolder = folderPath;
            if (folderPath[0] != '/')
            {
                folderPath = "/" + folderPath;
            }
			VectorImageManager.fontDestinationFolder = folderPath;
        }
    }
}
