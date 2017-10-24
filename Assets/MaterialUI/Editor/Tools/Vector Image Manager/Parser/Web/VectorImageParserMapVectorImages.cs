//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
	public class VectorImageParserMapVectorImages : VectorImageFontParser
	{
		protected override string GetIconFontUrl()
		{
			return "https://github.com/scottdejonge/map-icons/raw/master/dist/fonts/map-icons.ttf?raw=true";
		}

		protected override string GetIconFontLicenseUrl()
	    {
			return "https://github.com/scottdejonge/Map-Icons/blob/master/README.md?raw=true";
	    }
		
		protected override string GetIconFontDataUrl()
		{
			return "https://github.com/scottdejonge/map-icons/raw/master/dist/css/map-icons.css?raw=true";
		}
		
		public override string GetWebsite()
		{
			return "https://github.com/scottdejonge/Map-Icons/";
		}
		
		public override string GetFontName()
		{
			return "MapIcons";
		}
		
		protected override VectorImageSet GenerateIconSet(string fontDataContent)
		{
			VectorImageSet vectorImageSet = new VectorImageSet();
			Glyph currentGlyph = null;
			
			foreach (string line in fontDataContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (line.StartsWith(".map-icon") && line.EndsWith(":before {"))
				{
					currentGlyph = new Glyph();

					string name = line.Replace(".map-icon-", string.Empty).Replace(":before {", string.Empty).Trim();
					currentGlyph.name = name;
				}

				if (line.StartsWith("	content:"))
				{
					if (currentGlyph != null)
					{
						string unicode = line.Substring(line.IndexOf("\"") + 2).Trim();
						unicode = unicode.Substring(0, unicode.IndexOf("\";")).Trim();
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
			fontDataLicenseContent = fontDataLicenseContent.Substring(fontDataLicenseContent.IndexOf("License"));
			return fontDataLicenseContent;
		}
	}
}
