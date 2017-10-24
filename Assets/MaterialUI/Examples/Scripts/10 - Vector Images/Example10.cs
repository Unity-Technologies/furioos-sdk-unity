//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using MaterialUI;
using System.Linq;

public class Example10 : MonoBehaviour
{
	[SerializeField] private VectorImage m_VectorImage;

	public void OnIconNameButtonClicked()
	{
		m_VectorImage.SetImage(MaterialIconHelper.GetIcon("volume_off"));
		//m_VectorImage.SetImage(GetIconFromIconFont("FontAwesome", "gift"));
	}

	public void OnIconEnumButtonClicked()
	{
		m_VectorImage.SetImage(MaterialIconHelper.GetIcon(MaterialIconEnum.SHOPPING_CART));
	}

	public void OnIconRandomButtonClicked()
	{
		m_VectorImage.SetImage(MaterialIconHelper.GetRandomIcon());
	}

	// If you want to get the icon from a icon font you downloaded:
	private ImageData GetIconFromIconFont(string fontName, string iconName)
	{
		VectorImageSet iconSet = VectorImageManager.GetIconSet(fontName);
		Glyph glyph = iconSet.iconGlyphList.Where(x => x.name.ToLower().Equals(iconName.ToLower())).FirstOrDefault();
		if (glyph == null)
		{
			Debug.LogError("Could not find an icon with the name: " + name + " inside the " + fontName + " icon font");
			return null;
		}

		Font font = VectorImageManager.GetIconFont(fontName);
		return new ImageData(new VectorImageData(glyph, font));
	}
}
