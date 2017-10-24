//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Date Picker Item", 100)]
    public class DialogDatePickerYearItem : DialogClickableOption
    {
        [SerializeField]
        private Text m_Text;
		public Text text
		{
			get { return m_Text; }
		}

		[SerializeField]
		private Toggle m_Toggle;
		public Toggle toggle
		{
			get { return m_Toggle; }
		}

		[SerializeField]
		private VectorImage m_SelectedImage;
		public VectorImage selectedImage
		{
			get { return m_SelectedImage; }
		}

		private int m_Year;
		public int year
		{
			get { return m_Year; }
			set
			{
				m_Year = value;
				text.text = m_Year.ToString();
			}
		}

        public void UpdateState(int currentYear)
        {
            if (year == currentYear)
            {
                toggle.isOn = true;
            }
        }

        public void OnItemValueChange()
        {
            m_Text.color = toggle.isOn ? Color.white : Color.black;
        }
    }
}