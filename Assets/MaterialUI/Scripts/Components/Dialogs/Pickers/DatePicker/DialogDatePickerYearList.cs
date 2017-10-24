//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Date Picker Year List", 100)]
    public class DialogDatePickerYearList : MonoBehaviour
    {
        [SerializeField]
        private DialogDatePicker m_DatePicker;

		[SerializeField]
		private GameObject m_YearTemplate;

        [SerializeField]
        private ScrollRect m_ScrollRect;

		private List<DialogDatePickerYearItem> m_YearItems = new List<DialogDatePickerYearItem>();

        void Awake()
        {
            List<int> yearList = new List<int>();
            for (int i = 1900; i < 2100; i++)
            {
                yearList.Add(i);
            }

			m_YearItems.Clear();
            for (int i = 0; i < yearList.Count; i++)
            {
				m_YearItems.Add(CreateYearItem(i, yearList[i]));
            }

			Destroy(m_YearTemplate);
        }

		private DialogDatePickerYearItem CreateYearItem(int i, int year)
		{
			DialogDatePickerYearItem yearItem = Instantiate(m_YearTemplate).GetComponent<DialogDatePickerYearItem>();

			RectTransform yearRectTransform = yearItem.GetComponent<RectTransform>();
			yearRectTransform.SetParent(m_YearTemplate.transform.parent);
			yearRectTransform.localScale = Vector3.one;
			yearRectTransform.localEulerAngles = Vector3.zero;

			yearItem.year = year;
			yearItem.index = i;
			yearItem.onClickAction += OnItemClick;
			
			return yearItem;
		}

		public void SetColor(Color accentColor)
		{
			for (int i = 0; i < m_YearItems.Count; i++)
			{
				m_YearItems[i].selectedImage.color = accentColor;
			}
		}

		public void OnItemClick(int index)
		{
			Toggle toggle = m_YearItems[index].toggle;
			toggle.isOn = !toggle.isOn;

			if (!toggle.isOn)
			{
				return;
			}

			m_DatePicker.SetYear(m_YearItems[index].year);
		}

        public void CenterToSelectedYear(int year)
        {
            int selectedIndex = 0;

			for (int i = 0; i < m_YearItems.Count; i++)
            {
				m_YearItems[i].UpdateState(year);

				if (m_YearItems[i].toggle.isOn)
                {
                    selectedIndex = i;
                }
            }

            float verticalPosition = 0;
            if (selectedIndex <= 3)
            {
                verticalPosition = 0;
            }
			else if (selectedIndex >= m_YearItems.Count - 3)
            {
                verticalPosition = 1;
            }
            else
            {
                verticalPosition = selectedIndex - 3; // Padding 3 elements
				verticalPosition /= m_YearItems.Count - 6; // We remove 6 elements, because the 3 first and 3 last can't be centered
            }

            m_ScrollRect.verticalNormalizedPosition = 1 - verticalPosition;
        }
    }
}