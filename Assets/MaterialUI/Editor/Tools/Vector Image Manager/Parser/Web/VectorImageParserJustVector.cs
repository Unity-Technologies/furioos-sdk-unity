//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
	public class VectorImageParserJustVector : VectorImageFontParser
	{
		protected override string GetIconFontUrl()
		{
			return "https://github.com/Seich/JustVector-Icons-Font/blob/master/justVector%20Font/fonts/JustVector_befc36341d9795c51945d4d132517a7a.ttf?raw=true";
		}

		protected override string GetIconFontLicenseUrl()
	    {
			return "https://github.com/Seich/JustVector-Icons-Font/blob/master/justVector%20Font/README.txt?raw=true";
	    }
		
		protected override string GetIconFontDataUrl()
		{
			return "https://github.com/Seich/JustVector-Icons-Font/blob/master/justVector%20Font/stylesheets/justVector.css?raw=true";
		}
		
		public override string GetWebsite()
		{
			return "https://dl.dropboxusercontent.com/u/8252879/justVector%20Font/index.html";
		}
		
		public override string GetFontName()
		{
			return "JustVector";
		}
		
		protected override VectorImageSet GenerateIconSet(string fontDataContent)
		{
			VectorImageSet vectorImageSet = new VectorImageSet();
			
			foreach (string line in fontDataContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (line.StartsWith(".jv-") && line.Contains("content:"))
				{
					string name = line.Replace(".jv-", string.Empty).Trim();
					name = name.Substring(0, name.IndexOf(":before")).Trim();

					string unicode = line.Substring(line.IndexOf("content: ") + 11);
					unicode = unicode.Substring(0, unicode.IndexOf("\";"));

					Glyph glyph = new Glyph(name, unicode, false);
					vectorImageSet.iconGlyphList.Add(glyph);
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
