//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
	public class VectorImageParserIcoMoon : VectorImageFontParser
	{
		protected override string GetIconFontUrl()
		{
			return "https://github.com/Keyamoon/IcoMoon-Free/blob/master/Font/IcoMoon-Free.ttf?raw=true";
		}

		protected override string GetIconFontLicenseUrl()
	    {
			return "https://github.com/Keyamoon/IcoMoon-Free/blob/master/License.txt?raw=true";
	    }
		
		protected override string GetIconFontDataUrl()
		{
			return "https://github.com/Keyamoon/IcoMoon-Free/raw/master/Font/selection.json?raw=true";
		}
		
		public override string GetWebsite()
		{
			return "https://icomoon.io/#preview-free";
		}
		
		public override string GetFontName()
		{
			return "IcoMoon";
		}
		
		protected override VectorImageSet GenerateIconSet(string fontDataContent)
		{
			return GenerateSpecificIconSet(fontDataContent);
		}

		public static VectorImageSet GenerateSpecificIconSet(string fontDataContent)
		{
			VectorImageSet vectorImageSet = new VectorImageSet();
			Glyph currentGlyph = null;

			foreach (string line in fontDataContent.Split(new [] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (line.StartsWith("				\"code\":"))
				{
					currentGlyph = new Glyph();
					string stringcode = line.Substring(line.IndexOf(":") + 1).Replace(",", "").Replace("\"", "").Trim();
					int intcode = int.Parse(stringcode);

					if (intcode < 1000)
					{
						currentGlyph = null;
						continue;
					}

                    currentGlyph.unicode = intcode.ToString("X4");
				}
				
				if (line.StartsWith("				\"name\":"))
				{
					if (currentGlyph != null)
					{
                        currentGlyph.name = line.Substring(line.IndexOf(":") + 1).Replace(",", "").Replace("\"", "").Trim();
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
}
