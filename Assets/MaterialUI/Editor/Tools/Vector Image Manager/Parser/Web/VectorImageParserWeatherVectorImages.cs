//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
	public class VectorImageParserWeatherVectorImages : VectorImageFontParser
	{
		protected override string GetIconFontUrl()
		{
			return "https://github.com/erikflowers/weather-icons/blob/master/font/weathericons-regular-webfont.ttf?raw=true";
		}

		protected override string GetIconFontLicenseUrl()
	    {
			return "https://github.com/erikflowers/weather-icons/blob/master/README.md?raw=true";
	    }
		
		protected override string GetIconFontDataUrl()
		{
			return "https://github.com/erikflowers/weather-icons/raw/master/_docs/gh-pages/index.html?raw=true";
		}
		
		public override string GetWebsite()
		{
			return "https://erikflowers.github.io/weather-icons/";
		}
		
		public override string GetFontName()
		{
			return "WeatherIcons";
		}
		
		protected override VectorImageSet GenerateIconSet(string fontDataContent)
		{
			VectorImageSet vectorImageSet = new VectorImageSet();
			Glyph currentGlyph = null;
			
			foreach (string line in fontDataContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (line.StartsWith("            <div class=\"icon-wrap\">"))
				{
					currentGlyph = new Glyph();
				}

				if (line.StartsWith("              <div class=\"icon-name\">"))
				{
					string name = line.Substring(line.IndexOf(">") + 1).Trim();
					name = name.Substring(0, name.IndexOf("</div>")).Trim();
					currentGlyph.name = name;
				}
				
				if (line.StartsWith("              <div class=\"icon_unicode\">"))
				{
					if (currentGlyph != null)
					{
						string unicode = line.Substring(line.IndexOf(">") + 1).Trim();
						unicode = unicode.Substring(0, unicode.IndexOf("</div>")).Trim();
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
			fontDataLicenseContent = fontDataLicenseContent.Substring(fontDataLicenseContent.IndexOf("## Licensing"));
			fontDataLicenseContent = fontDataLicenseContent.Substring(0, fontDataLicenseContent.IndexOf("## Contact"));
			return fontDataLicenseContent;
		}
	}
}
