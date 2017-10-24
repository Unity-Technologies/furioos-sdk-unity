//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
	#if UNITY_EDITOR
	public class VectorImageFileParserFontastic : VectorImageFontParser
	{
	    private string m_ZipPath;
	    private string m_UnzipPath;

	    protected override string GetIconFontUrl()
	    {
	        UnzipFile();
			if (string.IsNullOrEmpty(m_ZipPath)) return null; // When user select cancel on the OpenFilePanel

            string newpath = GetFirstFont();
	        return "file:///" + newpath;
		}

		protected override string GetIconFontLicenseUrl()
	    {
	       return null;
	    }

	    protected override string GetIconFontDataUrl()
		{
            if (m_ZipPath == null) UnzipFile();

	        return "file:///" + m_UnzipPath + "/styles.css";
		}
		
		public override string GetWebsite()
		{
			return "http://fontastic.me/";
		}
		
		public override string GetFontName()
		{
            if (m_ZipPath == null) UnzipFile();

            return "custom-" + Path.GetFileName(GetFirstFont()).Replace(".ttf", "");
		}

	    private string GetFirstFont()
	    {
	        string[] fonts = Directory.GetFiles(m_UnzipPath + "/fonts", "*.ttf");

	        if (fonts.Length > 0)
	        {
	            return fonts[0].Replace('\\', '/');
	        }

            throw new Exception("No fonts in folder");
	    }

	    private void UnzipFile()
		{
            m_ZipPath = EditorUtility.OpenFilePanel("Please select .zip downloaded from IcoMoon", "", "zip");

            if (!string.IsNullOrEmpty(m_ZipPath))
			{
				ZipUtil.Unzip(m_ZipPath, Application.temporaryCachePath + "/tempFont");
				m_UnzipPath = Application.temporaryCachePath + "/tempFont";
            }
        }

        protected override void CleanUp()
	    {
			Directory.Delete(m_UnzipPath, true);
	    }
		
		protected override VectorImageSet GenerateIconSet(string fontDataContent)
		{
			return GenerateSpecificIconSet(fontDataContent);
		}

		public static VectorImageSet GenerateSpecificIconSet(string fontDataContent)
		{
			VectorImageSet vectorImageSet = new VectorImageSet();
			Glyph currentGlyph = null;
			
			foreach (string line in fontDataContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (line.StartsWith(".icon-") && line.Contains(":before"))
				{
					currentGlyph = new Glyph();

					string name = line.Replace(".icon-", string.Empty).Trim();
					name = name.Substring(0, name.IndexOf(":")).Trim();

					currentGlyph.name = name;
				}

				if (line.StartsWith("  content:"))
				{
					if (currentGlyph != null)
					{
						string unicode = line.Substring(line.IndexOf("content:") + 10);
						unicode = unicode.Substring(0, unicode.IndexOf("\";"));
						unicode = ((int)unicode[0]).ToString("X4");
						currentGlyph.unicode = unicode;

						vectorImageSet.iconGlyphList.Add(currentGlyph);
						currentGlyph = null;
					}
				}
			}

            return vectorImageSet;
		}

		protected override string ExtractLicense(string fontDataLicenseContent)
		{
			return fontDataLicenseContent;
		}
	}
	#endif
}
