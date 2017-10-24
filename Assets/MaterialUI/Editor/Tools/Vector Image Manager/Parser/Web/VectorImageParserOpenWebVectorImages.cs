//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
	public class VectorImageParserOpenWebVectorImages : VectorImageFontParser
	{
		protected override string GetIconFontUrl()
		{
			return "https://github.com/pfefferle/openwebicons/blob/master/font/openwebicons.ttf?raw=true";
		}

		protected override string GetIconFontLicenseUrl()
	    {
			return "https://github.com/pfefferle/openwebicons/blob/master/License.txt?raw=true";
	    }
		
		protected override string GetIconFontDataUrl()
		{
			return "https://github.com/pfefferle/openwebicons/blob/master/sass/_vars.scss?raw=true";
		}
		
		public override string GetWebsite()
		{
			return "http://pfefferle.github.io/openwebicons/";
		}
		
		public override string GetFontName()
		{
			return "OpenWebIcons";
		}
		
		protected override VectorImageSet GenerateIconSet(string fontDataContent)
		{
			VectorImageSet vectorImageSet = new VectorImageSet();
			
			foreach (string line in fontDataContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				string name = line.Substring(0, line.IndexOf("\"\\")).Trim();
				name = name.Replace("$icons:", string.Empty).Trim();

				string unicode = line.Substring(line.IndexOf("\"\\") + 2);
				unicode = unicode.Substring(0, unicode.IndexOf("\""));

				vectorImageSet.iconGlyphList.Add(new Glyph(name, unicode, false));
			}

            return vectorImageSet;
		}

		protected override string ExtractLicense(string fontDataLicenseContent)
		{
			return fontDataLicenseContent;
		}
	}
}
