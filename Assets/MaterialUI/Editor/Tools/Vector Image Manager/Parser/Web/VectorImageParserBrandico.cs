//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
	public class VectorImageParserBrandico : VectorImageFontParser
	{
		protected override string GetIconFontUrl()
		{
			return "https://github.com/fontello/brandico.font/blob/master/font/brandico.ttf?raw=true";
		}

		protected override string GetIconFontLicenseUrl()
	    {
			return "https://raw.githubusercontent.com/fontello/brandico.font/master/README.md?raw=true";
	    }
		
		protected override string GetIconFontDataUrl()
		{
			return "https://github.com/fontello/brandico.font/blob/master/config.yml?raw=true";
		}
		
		public override string GetWebsite()
		{
			return "http://fontello.github.io/brandico.font/demo.html";
		}
		
		public override string GetFontName()
		{
			return "Brandico";
		}
		
		protected override VectorImageSet GenerateIconSet(string fontDataContent)
		{
			VectorImageSet vectorImageSet = new VectorImageSet();
			Glyph currentGlyph = null;

			bool canStartReading = false;
			foreach (string line in fontDataContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (!canStartReading)
				{
					if (line.StartsWith("glyphs:"))
					{
						canStartReading = true;
					}

					continue;
				}

				if (line.Contains("css:"))
				{
					currentGlyph = new Glyph();

					string name = line.Substring(line.IndexOf("css:") + 5).Trim();
					currentGlyph.name = name;
				}

				if (line.Contains("code:") && line.Contains("0x"))
				{
					if (currentGlyph != null)
					{
						string unicode = line.Substring(line.IndexOf("code:") + 6).Trim();
						unicode = unicode.Replace("0x", string.Empty);
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
