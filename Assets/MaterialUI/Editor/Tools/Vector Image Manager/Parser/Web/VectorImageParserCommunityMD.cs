//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;

namespace MaterialUI
{
    public class VectorImageParserCommunityMD : VectorImageFontParser
    {
        protected override string GetIconFontUrl()
        {
            return "https://github.com/Templarian/MaterialDesign-Webfont/blob/master/fonts/materialdesignicons-webfont.ttf?raw=true";
        }

        protected override string GetIconFontLicenseUrl()
        {
            return "https://raw.githubusercontent.com/Templarian/MaterialDesign-Webfont/75dd211af99cc3e171278c3320f6a7ae655b75d5/license.txt";
        }

        protected override string GetIconFontDataUrl()
        {
            return "https://raw.githubusercontent.com/Templarian/MaterialDesign-Webfont/master/scss/_variables.scss";
        }

        public override string GetWebsite()
        {
            return "https://materialdesignicons.com/";
        }

        public override string GetFontName()
        {
            return "CommunityMD";
        }

        protected override VectorImageSet GenerateIconSet(string fontDataContent)
        {
            VectorImageSet vectorImageSet = new VectorImageSet();

            fontDataContent = fontDataContent.Split(new[] { "$mdi-icons: (" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace(" ", "").Replace("\n", "").Replace(");", "").Replace("\"", "");

            string[] splitContent = fontDataContent.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < splitContent.Length; i++)
            {
                string[] splitLine = splitContent[i].Split(':');
                vectorImageSet.iconGlyphList.Add(new Glyph(splitLine[0], splitLine[1].ToLower(), false));
            }

            return vectorImageSet;
        }

        protected override string ExtractLicense(string fontDataLicenseContent)
        {
            fontDataLicenseContent = fontDataLicenseContent.Substring(fontDataLicenseContent.IndexOf("License"));
            return fontDataLicenseContent;
        }
    }
}
