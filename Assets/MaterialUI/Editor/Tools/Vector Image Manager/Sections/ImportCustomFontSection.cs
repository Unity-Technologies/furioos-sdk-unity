//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    public class ImportCustomFontSection
    {
        private readonly string[] m_IconFontParserNameArray =
        {
            "IcoMoon",
            "Fontello",
            "Fontastic",
        };

		public readonly VectorImageFontParser[] vectorImageFontParserArray =
        {
            new VectorImageFileParserIcoMoon(),
            new VectorImageFileParserFontello(),
            new VectorImageFileParserFontastic(),
		};

        public ImportCustomFontSection() { }

        public void DrawInspector()
        {
            VectorImageManagerWindow.DrawHeader("Import custom icon fonts");
            {
                VectorImageManagerWindow.BeginContents();
                {
                    EditorGUILayout.HelpBox("To create a custom font with your own svg files, you just need to go on one of these websites, follow the steps, download the zip file and finaly import it using the 'Import' button below.", MessageType.Info);

                    for (int i = 0; i < vectorImageFontParserArray.Length; i++)
                    {
                        VectorImageFontParser vectorImageFontParser = vectorImageFontParserArray[i];

						using (new GUILayout.VerticalScope())
                        {
							using (new GUILayout.HorizontalScope())
							{
								EditorGUILayout.LabelField(m_IconFontParserNameArray[i], EditorStyles.boldLabel, GUILayout.Width(110f));
								
								GUILayout.FlexibleSpace();
								
								if (GUILayout.Button("Website", EditorStyles.miniButtonLeft, GUILayout.Width(60f)))
								{
									Application.OpenURL(vectorImageFontParser.GetWebsite());
								}
								
								if (GUILayout.Button("Import", EditorStyles.miniButtonRight, GUILayout.Width(60f)))
								{
									vectorImageFontParser.DownloadIcons(() =>
										{
											Debug.Log("Your custom font has been imported to " + vectorImageFontParser.GetFolderPath());
										});
								}
								
								GUILayout.Space(2f);
							}
                        }
                    }
                }
                VectorImageManagerWindow.EndContents();
            }
        }
    }
}
