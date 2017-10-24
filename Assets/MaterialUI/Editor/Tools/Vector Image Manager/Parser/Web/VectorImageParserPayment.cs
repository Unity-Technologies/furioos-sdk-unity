//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
	public class VectorImageParserPayment : VectorImageFontParser
	{
		protected override string GetIconFontUrl()
		{
			return "https://github.com/orlandotm/payment-webfont/blob/master/fonts/payment-webfont.ttf?raw=true";
		}

		protected override string GetIconFontLicenseUrl()
	    {
			return "https://github.com/orlandotm/payment-webfont/blob/master/LICENSE?raw=true";
	    }
		
		protected override string GetIconFontDataUrl()
		{
			return "https://github.com/orlandotm/payment-webfont/blob/master/style.css?raw=true";
		}
		
		public override string GetWebsite()
		{
			return "http://www.orlandotm.com/payment-webfont/";
		}
		
		public override string GetFontName()
		{
			return "Payment";
		}
		
		protected override VectorImageSet GenerateIconSet(string fontDataContent)
		{
			VectorImageSet vectorImageSet = new VectorImageSet();
			Glyph currentGlyph = null;
			
			foreach (string line in fontDataContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (line.StartsWith(".pw-") && line.Contains(":before"))
				{
					currentGlyph = new Glyph();

					string name = line.Replace(".pw-", string.Empty).Trim();
					name = name.Substring(0, name.IndexOf(":")).Trim();

					currentGlyph.name = name;
				}

				if (line.StartsWith("	content:"))
				{
					if (currentGlyph != null)
					{
						string unicode = line.Substring(line.IndexOf("content:") + 11);
						unicode = unicode.Substring(0, unicode.IndexOf("\";"));
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
