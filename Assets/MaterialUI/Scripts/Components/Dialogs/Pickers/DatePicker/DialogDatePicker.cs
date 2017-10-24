//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Date Picker", 1)]
    public class DialogDatePicker : MaterialDialog
    {
        [SerializeField]
        private Text m_TextDate;
        public Text textDate
        {
            get { return m_TextDate; }
        }

        [SerializeField]
        private Text m_TextMonth;
        public Text textMonth
        {
            get { return m_TextMonth; }
        }

        [SerializeField]
        private Text m_TextYear;
        public Text textYear
        {
            get { return m_TextYear; }
        }

        [SerializeField]
        private CanvasGroup m_CalendarCanvasGroup;

        [SerializeField]
        private CanvasGroup m_YearCanvasGroup;

        [SerializeField]
		private DialogDatePickerYearList m_DatePickerYearList;

		[SerializeField]
		private Text[] m_DatePickerDayTexts;

        [SerializeField]
        private DialogDatePickerDayItem[] m_DatePickerDayItems;

        [SerializeField]
        private Image m_Header;
        public Image header
        {
            get { return m_Header; }
            set { m_Header = value; }
        }

        private DateTime m_CurrentDate;
        public DateTime currentDate
        {
            get { return m_CurrentDate; }
            set
            {
                SetDate(value);
            }
        }

		private DayOfWeek m_DayOfWeek = DayOfWeek.Sunday;
		public DayOfWeek dayOfWeek
		{
			get { return m_DayOfWeek; }
		}

		private CultureInfo m_CultureInfo;
		public CultureInfo cultureInfo
		{
			get { return m_CultureInfo; }
		}

		private string m_DateFormatPattern = "ddd, MMM dd";
		public string dateFormatPattern
		{
			get { return m_DateFormatPattern; }
		}

        private Action<DateTime> m_OnAffirmativeClicked;
		private Action m_OnDismissiveClicked;

		private Vector2 m_InitialSize;

		void Awake()
		{
			m_InitialSize = rectTransform.sizeDelta;
		}

		public void Initialize(int year, int month, int day, Action<DateTime> onAffirmativeClicked, Action onDismissiveClicked, Color accentColor)
        {
            SetDate(new DateTime(year, month, day));
            OnDateClicked();

            // Callbacks
            m_OnAffirmativeClicked = onAffirmativeClicked;
			m_OnDismissiveClicked = onDismissiveClicked;

            SetColor(accentColor);

			Initialize();
			rectTransform.sizeDelta = m_InitialSize;
        }

        public void SetColor(Color accentColor)
        {
            m_Header.color = accentColor;
            m_DatePickerYearList.SetColor(accentColor);

            for (int i = 0; i < m_DatePickerDayItems.Length; i++)
            {
                m_DatePickerDayItems[i].selectedImage.color = accentColor;
            }
        }

        public void SetDate(DateTime date)
        {
            m_CurrentDate = date;
			UpdateDaysText();
            UpdateDateList(GetMonthDateList(m_CurrentDate.Year, m_CurrentDate.Month));

            UpdateDatesText();
        }

        public void SetYear(int year)
        {
            DateTime newDate = default(DateTime);
            if (!DateTime.IsLeapYear(year) && m_CurrentDate.Month == 2 && m_CurrentDate.Day == 29)
            {
                newDate = new DateTime(year, m_CurrentDate.Month, 28);
            }
            else
            {
                newDate = new DateTime(year, m_CurrentDate.Month, m_CurrentDate.Day);
            }

            SetDate(newDate);

            OnDateClicked();
        }

		public void SetDayOfWeek(DayOfWeek dayOfWeek, CultureInfo cultureInfo)
		{
			m_DayOfWeek = dayOfWeek;
			m_CultureInfo = cultureInfo;

			if (m_CultureInfo == null)
			{
				m_CultureInfo = new CultureInfo("en-US");
			}

			UpdateDaysText();
		}

		public void SetDateFormatPattern(string dateFormatPattern)
		{
			m_DateFormatPattern = dateFormatPattern;
			UpdateDatesText();
		}

        private void UpdateDateList(List<DateTime> dateTime)
        {
            for (int i = 0; i < m_DatePickerDayItems.Length; i++)
            {
                DateTime date = (i < dateTime.Count) ? dateTime[i] : default(DateTime);
                m_DatePickerDayItems[i].Init(date, OnDayItemValueChanged);

                m_DatePickerDayItems[i].UpdateState(m_CurrentDate);
            }
        }

        private void UpdateDatesText()
        {
            m_TextMonth.text = m_CurrentDate.ToString("MMMMM yyyy");
            m_TextYear.text = m_CurrentDate.ToString("yyyy");
            m_TextDate.text = GetFormattedDate(m_CurrentDate);
        }

		private void UpdateDaysText()
		{
            if (m_CultureInfo == null)
            {
                m_CultureInfo = new CultureInfo("en-US");
            }

            for (int i = 0; i < 7; i++)
			{
				int day = ((int)m_DayOfWeek + i) % 7;
				m_DatePickerDayTexts[i].text = m_CultureInfo.DateTimeFormat.GetDayName((DayOfWeek)day).Substring(0, 1).ToUpper();
			}
		}

        private void OnDayItemValueChanged(DialogDatePickerDayItem dayItem, bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            m_CurrentDate = dayItem.dateTime;
            UpdateDatesText();
        }

        public void OnPreviousMonthClicked()
        {
            DateTime date = m_CurrentDate;
            date = date.AddMonths(-1);
            SetDate(new DateTime(date.Year, date.Month, 1));
        }

        public void OnNextMonthClicked()
        {
            DateTime date = m_CurrentDate;
            date = date.AddMonths(1);
            SetDate(new DateTime(date.Year, date.Month, 1));
        }

        private string GetFormattedDate(DateTime date)
        {
			return date.ToString(m_DateFormatPattern, m_CultureInfo);
        }

        private List<DateTime> GetMonthDateList(int year, int month)
        {
            List<DateTime> dateList = new List<DateTime>();

            DateTime firstDate = new DateTime(year, month, 1);
			while (firstDate.DayOfWeek != m_DayOfWeek)
            {
                firstDate = firstDate.AddDays(-1);
            }

            int lastDayInMonth = DateTime.DaysInMonth(year, month);
            while (firstDate.Day != lastDayInMonth || firstDate.Month != month)
            {
                dateList.Add(firstDate);
                firstDate = firstDate.AddDays(1);
            }

            dateList.Add(firstDate);

            return dateList;
        }

        public void OnYearClicked()
        {
            m_YearCanvasGroup.gameObject.SetActive(true);  // HACK: to disable the big list of GameObjects to avoid lag during dialog movement
            TweenManager.TweenFloat(f => m_CalendarCanvasGroup.alpha = f, m_CalendarCanvasGroup.alpha, 0f, 0.5f);
            TweenManager.TweenFloat(f => m_YearCanvasGroup.alpha = f, m_YearCanvasGroup.alpha, 1f, 0.5f, 0.01f);  // HACK: 0.01f of delay, because if not, the animation is not played

            m_DatePickerYearList.CenterToSelectedYear(m_CurrentDate.Year);

            m_CalendarCanvasGroup.blocksRaycasts = false;
            m_YearCanvasGroup.blocksRaycasts = true;

            m_TextDate.color = new Color(m_TextDate.color.r, m_TextDate.color.g, m_TextDate.color.b, 0.5f);
            m_TextYear.color = new Color(m_TextYear.color.r, m_TextYear.color.g, m_TextYear.color.b, 1.0f);
        }

        public void OnDateClicked()
        {
            TweenManager.TweenFloat(f => m_CalendarCanvasGroup.alpha = f, m_CalendarCanvasGroup.alpha, 1f, 0.5f);
            TweenManager.TweenFloat(f => m_YearCanvasGroup.alpha = f, m_YearCanvasGroup.alpha, 0f, 0.5f, 0.01f, callback: () =>
            {
                m_YearCanvasGroup.gameObject.SetActive(false); // HACK: to disable the big list of GameObjects to avoid lag during dialog movement
            });

            m_CalendarCanvasGroup.blocksRaycasts = true;
            m_YearCanvasGroup.blocksRaycasts = false;

            m_TextDate.color = new Color(m_TextDate.color.r, m_TextDate.color.g, m_TextDate.color.b, 1.0f);
            m_TextYear.color = new Color(m_TextYear.color.r, m_TextYear.color.g, m_TextYear.color.b, 0.5f);
        }

        public void OnButtonOkClicked()
        {
            if (m_OnAffirmativeClicked != null)
            {
                m_OnAffirmativeClicked(m_CurrentDate);
            }

            Hide();
        }

		public void OnButtonCancelClicked()
		{
			if (m_OnDismissiveClicked != null)
			{
				m_OnDismissiveClicked();
			}

			Hide();
		}
    }
}