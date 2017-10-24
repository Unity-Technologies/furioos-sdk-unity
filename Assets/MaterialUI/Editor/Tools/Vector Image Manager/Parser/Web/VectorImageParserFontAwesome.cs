//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
	public class VectorImageParserFontAwesome : VectorImageFontParser
	{
		protected override string GetIconFontUrl()
		{
			return "https://github.com/FortAwesome/Font-Awesome/blob/master/fonts/fontawesome-webfont.ttf?raw=true";
		}

		protected override string GetIconFontLicenseUrl()
	    {
			return "https://github.com/FortAwesome/Font-Awesome/blob/master/README.md?raw=true";
	    }
		
		protected override string GetIconFontDataUrl()
		{
			return "https://github.com/FortAwesome/Font-Awesome/raw/master/src/icons.yml";
		}
		
		public override string GetWebsite()
		{
			return "http://fontawesome.io/";
		}
		
		public override string GetFontName()
		{
			return "FontAwesome";
		}
		
		protected override VectorImageSet GenerateIconSet(string fontDataContent)
		{
			VectorImageSet vectorImageSet = new VectorImageSet();
			Glyph currentGlyph = null;
			
			foreach (string line in fontDataContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (line.StartsWith("    id:"))
				{
					currentGlyph = new Glyph();
					currentGlyph.name = line.Substring(line.IndexOf(":") + 1).Trim();
				}
				
				if (line.StartsWith("    unicode:"))
				{
					if (currentGlyph != null)
					{
						currentGlyph.unicode = line.Substring(line.IndexOf(":") + 1).Trim();
						vectorImageSet.iconGlyphList.Add(currentGlyph);
						currentGlyph = null;
					}
				}
			}
			
            return vectorImageSet;
		}

		protected override string ExtractLicense(string fontDataLicenseContent)
		{
			fontDataLicenseContent = fontDataLicenseContent.Substring(fontDataLicenseContent.IndexOf("## License"));
			fontDataLicenseContent = fontDataLicenseContent.Substring(0, fontDataLicenseContent.IndexOf("## Changelog"));
			return fontDataLicenseContent;
		}
	}
}
