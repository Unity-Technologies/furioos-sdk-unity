//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;
using System;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Day Picker Item", 100)]
    public class DialogDatePickerDayItem : MonoBehaviour
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

		private DateTime m_DateTime;
		public DateTime dateTime
		{
			get { return m_DateTime; }
		}

		private Action<DialogDatePickerDayItem, bool> m_OnItemValueChanged;

		public void Init(DateTime dateTime, Action<DialogDatePickerDayItem, bool> onItemValueChanged)
		{
			m_DateTime = dateTime;
			m_OnItemValueChanged = onItemValueChanged;

			transform.localScale = Vector3.one;

			m_Text.text = m_DateTime.Day.ToString();
        }

        public void UpdateState(DateTime currentDate)
        {
			bool isCurrentMonth = (m_DateTime.Month == currentDate.Month) && !m_DateTime.Equals(default(DateTime));

            toggle.interactable = isCurrentMonth;
            m_Text.gameObject.SetActive(isCurrentMonth);

            if (!isCurrentMonth)
            {
                return;
            }

			bool isToday = m_DateTime.Day == DateTime.Now.Day && m_DateTime.Month == DateTime.Now.Month && m_DateTime.Year == DateTime.Now.Year;
            m_Text.fontStyle = isToday ? FontStyle.Bold : FontStyle.Normal; //TODO: Do not use the unity normal/bold fontStyle, but apply the correct Material font...

			if (m_DateTime.Equals(currentDate))
            {
                toggle.isOn = true;
            }
        }

        public void OnItemValueChange()
        {
			m_Text.color = toggle.isOn ? Color.white : Color.black;

			if (m_OnItemValueChanged != null)
			{
				m_OnItemValueChanged(this, toggle.isOn);
			}
        }
    }
}