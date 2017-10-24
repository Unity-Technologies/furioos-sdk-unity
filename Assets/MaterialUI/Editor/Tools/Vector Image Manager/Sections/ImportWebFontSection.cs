//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;
using System.IO;

namespace MaterialUI
{
    public class ImportWebFontSection
    {
		public readonly VectorImageFontParser materialDesignVectorImageFont = new VectorImageParserMaterialDesign();
		public readonly VectorImageFontParser[] vectorImageFontParserArray =
		{
            new VectorImageParserBrandico(),
            new VectorImageParserCommunityMD(),
            new VectorImageParserFontAwesome(),
            new VectorImageParserFontelico(),
            new VectorImageParserIcoMoon(),
            new VectorImageParserIonicons(),
            new VectorImageParserJustVector(),
            new VectorImageParserKenney(),
            new VectorImageParserMapVectorImages(),
            new VectorImageParserOcticons(),
            new VectorImageParserOpenIconic(),
            new VectorImageParserOpenWebVectorImages(),
            new VectorImageParserPayment(),
            new VectorImageParserSocialicious(),
            new VectorImageParserWeatherVectorImages(), 
        };

        public ImportWebFontSection() { }

        public void DrawInspector()
        {
            VectorImageManagerWindow.DrawHeader("Import web icon fonts");
            {
                VectorImageManagerWindow.BeginContents();
                {
                    DrawWebFontLine(materialDesignVectorImageFont, false);

                    EditorGUILayout.Separator();
                    EditorGUILayout.HelpBox("Please check the license that will be downloaded with each font before using an icon font.", MessageType.Info);

                    for (int i = 0; i < vectorImageFontParserArray.Length; i++)
                    {
                        DrawWebFontLine(vectorImageFontParserArray[i]);
                    }
                }
                VectorImageManagerWindow.EndContents();
            }
        }

        private void DrawWebFontLine(VectorImageFontParser vectorImageFontParser, bool showDeleteButton = true)
        {
			using (new GUILayout.VerticalScope())
            {
				using (new GUILayout.HorizontalScope())
				{
					string displayedFontName = vectorImageFontParser.GetFontName();
					if (displayedFontName.Length > 15) displayedFontName = displayedFontName.Substring(0, 15);
					EditorGUILayout.LabelField(displayedFontName, EditorStyles.boldLabel, GUILayout.Width(110f));
					
					string iconCountInfo = string.Empty;
					if (vectorImageFontParser.IsFontAvailable())
					{
						iconCountInfo = vectorImageFontParser.GetCachedIconSet().iconGlyphList.Count + " icons";
					}
					EditorGUILayout.LabelField(iconCountInfo, GUILayout.Width(60f));
					
					GUILayout.FlexibleSpace();
					
					if (GUILayout.Button("Website", EditorStyles.miniButtonLeft, GUILayout.Width(60f)))
					{
						Application.OpenURL(vectorImageFontParser.GetWebsite());
					}
					
					string downloadInfo = vectorImageFontParser.IsFontAvailable() ? "Update" : "Download";
					if (GUILayout.Button(downloadInfo, showDeleteButton ? EditorStyles.miniButtonMid : EditorStyles.miniButtonRight, GUILayout.Width(60f)))
					{
						EditorUtility.DisplayProgressBar("Downloading font icon", "Downloading " + vectorImageFontParser.GetFontName() + "...", 0.0f);
						
						vectorImageFontParser.DownloadIcons(() =>
							{
								EditorUtility.ClearProgressBar();
							});
					}
					
					if (showDeleteButton)
					{
						GUI.enabled = vectorImageFontParser.IsFontAvailable();
						if (GUILayout.Button("Delete", EditorStyles.miniButtonRight, GUILayout.Width(60f)))
						{
							if (EditorUtility.DisplayDialog("Delete " + vectorImageFontParser.GetFontName(), "Are you sure you want to delete this font icon?", "Delete", "Cancel"))
							{
								vectorImageFontParser.Delete();
							}
						}
						GUI.enabled = true;
					}
				}
            }
        }
    }
}
