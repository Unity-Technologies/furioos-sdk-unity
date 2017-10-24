//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
#if UNITY_EDITOR
    public class VectorImageFileParserFontello : VectorImageFontParser
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

            return "file:///" + m_UnzipPath + "/config.json";
        }

        public override string GetWebsite()
        {
            return "http://fontello.com/";
        }

        public override string GetFontName()
        {
            if (m_ZipPath == null) UnzipFile();

            return "custom-" + Path.GetFileName(GetFirstFont()).Replace(".ttf", "");
        }

        private string GetFirstFont()
        {
            string[] fonts = Directory.GetFiles(m_UnzipPath + "/font", "*.ttf");

            if (fonts.Length > 0)
            {
                return fonts[0].Replace('\\', '/');
            }

            throw new Exception("No fonts in folder");
        }

        private void UnzipFile()
        {
            m_ZipPath = EditorUtility.OpenFilePanel("Please select .zip downloaded from Fontello", "", "zip");

            if (!string.IsNullOrEmpty(m_ZipPath))
            {
                DirectoryInfo info = new DirectoryInfo(m_ZipPath);
                ZipUtil.Unzip(m_ZipPath, Application.temporaryCachePath + "/tempFont");
                m_UnzipPath = Application.temporaryCachePath + "/tempFont/" + info.Name.Replace(".zip", "");
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

            foreach (string line in fontDataContent.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.StartsWith("      \"css\":"))
                {
                    currentGlyph = new Glyph();
                    currentGlyph.name = line.Substring(line.IndexOf(":") + 2).Replace(",", "").Replace("\"", "").Trim();
                    vectorImageSet.iconGlyphList.Add(currentGlyph);
                }

                if (line.StartsWith("      \"code\":"))
                {
                    if (currentGlyph != null)
                    {
                        string stringcode = line.Substring(line.IndexOf(":") + 2).Replace(",", "").Replace("\"", "").Trim();
                        int intcode = int.Parse(stringcode);

                        if (intcode < 1000)
                        {
                            currentGlyph = null;
                            continue;
                        }

                        currentGlyph.unicode = intcode.ToString("X");

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
