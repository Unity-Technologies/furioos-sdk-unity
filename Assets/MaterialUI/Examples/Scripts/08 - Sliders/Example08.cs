//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

public class Example08 : MonoBehaviour
{
	[SerializeField] private Image m_RgbImage;
	[SerializeField] private Slider m_SliderR;
	[SerializeField] private Slider m_SliderG;
	[SerializeField] private Slider m_SliderB;
	[SerializeField] private Slider m_SliderA;

	void Awake()
	{
		OnSliderValueChanged();
	}

	public void OnSliderValueChanged()
	{
		UpdateRGBImage();
	}

	private void UpdateRGBImage()
	{
		m_RgbImage.color = new Color(m_SliderR.value/255f, m_SliderG.value/255f, m_SliderB.value/255f, m_SliderA.value);
	}
}
