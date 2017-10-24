//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
    public class VectorImageParserKenney : VectorImageFontParser
    {
        protected override string GetIconFontUrl()
        {
			return "https://github.com/SamBrishes/kenney-icon-font/blob/master/fonts/kenney-icon-font.ttf?raw=true";
        }

        protected override string GetIconFontLicenseUrl()
        {
            return "https://raw.githubusercontent.com/SamBrishes/kenney-icon-font/master/LICENSE.md?raw=true";
        }

        protected override string GetIconFontDataUrl()
        {
            return "https://github.com/SamBrishes/kenney-icon-font/raw/master/css/kenney-icons.css?raw=true";
        }

        public override string GetWebsite()
        {
            return "http://sambrishes.github.io/kenney-icon-font/";
        }

        public override string GetFontName()
        {
            return "Kenney";
        }

        protected override VectorImageSet GenerateIconSet(string fontDataContent)
        {
            VectorImageSet vectorImageSet = new VectorImageSet();

            bool canStartReading = false;
            foreach (string line in fontDataContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!canStartReading)
                {
                    if (line.StartsWith(".ki-"))
                    {
                        canStartReading = true;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (line.Contains(".ki-"))
                {
                    Glyph currentGlyph = new Glyph();

                    string[] lineParts = line.Split(':');

                    currentGlyph.name = lineParts[0].Replace(".ki-", "");

                    currentGlyph.unicode = lineParts[2].Replace(" ", "").Replace("\"", "").Replace("\\", "").Replace(";}", "");

                    vectorImageSet.iconGlyphList.Add(currentGlyph);
                }
            }

            return vectorImageSet;
        }

        protected override string ExtractLicense(string fontDataLicenseContent)
        {
            return fontDataLicenseContent;
        }
    }
}
