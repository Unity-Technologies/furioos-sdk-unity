//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
	public class VectorImageParserOpenIconic : VectorImageFontParser
	{
		protected override string GetIconFontUrl()
		{
			return "https://github.com/iconic/open-iconic/blob/master/font/fonts/open-iconic.ttf?raw=true";
		}

		protected override string GetIconFontLicenseUrl()
	    {
			return "https://github.com/iconic/open-iconic/blob/master/README.md?raw=true";
	    }
		
		protected override string GetIconFontDataUrl()
		{
			return "https://github.com/iconic/open-iconic/blob/master/font/css/open-iconic.css?raw=true";
		}
		
		public override string GetWebsite()
		{
			return "https://useiconic.com/open";
		}
		
		public override string GetFontName()
		{
			return "OpenIconic";
		}
		
		protected override VectorImageSet GenerateIconSet(string fontDataContent)
		{
			VectorImageSet vectorImageSet = new VectorImageSet();
			
			foreach (string line in fontDataContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (line.StartsWith(".oi[data-glyph") && line.Contains("content:"))
				{
					string name = line.Replace(".oi[data-glyph=", string.Empty).Trim();
					name = name.Substring(0, name.IndexOf("]")).Trim();

					string unicode = line.Substring(line.IndexOf("content:'") + 10);
					unicode = unicode.Substring(0, unicode.IndexOf("'; }"));

					Glyph glyph = new Glyph(name, unicode, false);
					vectorImageSet.iconGlyphList.Add(glyph);
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
