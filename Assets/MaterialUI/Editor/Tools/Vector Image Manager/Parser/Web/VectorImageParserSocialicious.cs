//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
	public class VectorImageParserSocialicious : VectorImageFontParser
	{
		protected override string GetIconFontUrl()
		{
			return "https://github.com/shalinguyen/socialicious/blob/master/font/socialicious.ttf?raw=true";
		}

		protected override string GetIconFontLicenseUrl()
	    {
			return "https://github.com/shalinguyen/socialicious/blob/master/LICENSE?raw=true";
	    }
		
		protected override string GetIconFontDataUrl()
		{
			return "https://github.com/shalinguyen/socialicious/blob/master/css/socialicious.css?raw=true";
		}
		
		public override string GetWebsite()
		{
			return "http://shalinguyen.github.io/socialicious/";
		}
		
		public override string GetFontName()
		{
			return "Socialicious";
		}
		
		protected override VectorImageSet GenerateIconSet(string fontDataContent)
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
}
