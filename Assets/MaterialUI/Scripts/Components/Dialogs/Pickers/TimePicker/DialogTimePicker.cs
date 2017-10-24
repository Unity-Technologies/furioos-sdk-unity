//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Time Picker", 1)]
    public class DialogTimePicker : MaterialDialog
    {
        [SerializeField]
        private Image m_ClockNeedle;
        public Image clockNeedle
        {
            get { return m_ClockNeedle; }
        }

        [SerializeField]
        private Text[] m_ClockTextArray;
        public Text[] clockTextArray
        {
            get { return m_ClockTextArray; }
        }

        [SerializeField]
        private Text m_TextAM;
        public Text textAM
        {
            get { return m_TextAM; }
        }

        [SerializeField]
        private Text m_TextPM;
        public Text textPM
        {
            get { return m_TextPM; }
        }

        [SerializeField]
        private Text m_TextHours;
        public Text textHours
        {
            get { return m_TextHours; }
        }

        [SerializeField]
        private Text m_TextMinutes;
        public Text textMinutes
        {
            get { return m_TextMinutes; }
        }

        private int m_CurrentHour;
        public int currentHour
        {
            get { return m_CurrentHour; }
            set
            {
                SetHours(value);
            }
        }

        private int m_CurrentMinute;
        public int currentMinute
        {
            get { return m_CurrentMinute; }
            set
            {
                SetMinutes(value);
            }
        }

        private bool m_IsAM;
        public bool isAM
        {
            get { return m_IsAM; }
            set
            {
                if (value)
                {
                    OnAMClicked();
                }
                else
                {
                    OnPMClicked();
                }
            }
        }

        private bool m_IsHoursSelected;
        public bool isHoursSelected
        {
            get { return m_IsHoursSelected; }
            set
            {
                if (value)
                {
                    OnHoursClicked();
                }
                else
                {
                    OnMinutesClicked();
                }
            }
        }

        private Action<DateTime> m_OnAffirmativeClicked;
        public Action<DateTime> onAffirmativeClicked
        {
            get { return m_OnAffirmativeClicked; }
            set { m_OnAffirmativeClicked = value; }
        }

        [SerializeField]
        private MaterialButton m_AffirmativeButton;
        public MaterialButton affirmativeButton
        {
            get { return m_AffirmativeButton; }
            set { m_AffirmativeButton = value; }
        }

        [SerializeField]
        private MaterialButton m_DismissiveButton;
        public MaterialButton dismissiveButton
        {
            get { return m_DismissiveButton; }
            set { m_DismissiveButton = value; }
        }

        [SerializeField]
        private Image m_Header;
        public Image header
        {
            get { return m_Header; }
            set { m_Header = value; }
        }

        [SerializeField]
        private VectorImage m_NeedleCenter;
        public VectorImage needleCenter
        {
            get { return m_NeedleCenter; }
            set { m_NeedleCenter = value; }
        }

        [SerializeField]
        private VectorImage m_NeedleEnd;
        public VectorImage needleEnd
        {
            get { return m_NeedleEnd; }
            set { m_NeedleEnd = value; }
        }

        [SerializeField]
        private DialogTimePickerClock m_timePickerClock;

        private float m_NeedleAngle;

        private Text m_ClosestText;

        [SerializeField]
        private Text m_MaskedText;

        public void Initialize(DateTime time, Action<DateTime> onAffirmativeClicked, Color accentColor)
        {
            this.currentHour = time.Hour;
            this.currentMinute = time.Minute;
            this.isAM = isAM;
            this.onAffirmativeClicked = onAffirmativeClicked;

            Vector2 initialSize = rectTransform.sizeDelta;
            Initialize();
            rectTransform.sizeDelta = initialSize;

            SetColor(accentColor);
        }

        public override void Show()
        {
            callbackShowAnimationOver += delegate { m_timePickerClock.Init(); };
            base.Show();
        }

        public void SetColor(Color accentColor)
        {
            m_ClockNeedle.color = accentColor;
            m_NeedleCenter.color = accentColor;
            m_NeedleEnd.color = accentColor;
            m_Header.color = accentColor;
            m_AffirmativeButton.materialRipple.rippleData.Color = accentColor;
            m_DismissiveButton.materialRipple.rippleData.Color = accentColor;
        }

        void Update()
        {
            m_ClockNeedle.transform.localRotation = Quaternion.Slerp(m_ClockNeedle.transform.localRotation, Quaternion.Euler(new Vector3(0, 0, m_NeedleAngle)), Time.deltaTime * 20f);

            int hour = GetHourFromAngle(m_ClockNeedle.transform.eulerAngles.z) - 1;
            if (hour == -1)
            {
                hour = 11;
            }
            m_ClosestText = m_ClockTextArray[hour];
            m_MaskedText.transform.position = m_ClosestText.transform.position;
            m_MaskedText.transform.rotation = m_ClosestText.transform.rotation;
            m_MaskedText.text = m_ClosestText.text;
        }

        private float GetAngleFromHour(int hour)
        {
            return -m_CurrentHour * 30 + 90;
        }

        private int GetHourFromAngle(float angle)
        {
            float approximateHour = -(angle - 90) / 30;
            int hour = Mathf.RoundToInt(approximateHour);

            if (hour < 0) hour += 12;
            return hour;
        }

        private float GetAngleFromMinute(int minute)
        {
            return -m_CurrentMinute / 5.0f * 30 + 90;
        }

        private int GetMinuteFromAngle(float angle)
        {
            float approximateMinute = -(angle - 90) / 30 * 5.0f;
            int minute = Mathf.RoundToInt(approximateMinute);

            if (minute < 0) minute += 60;
            return minute;
        }

        public void SetAngle(float angle)
        {
            if (m_IsHoursSelected)
            {
                SetHours(GetHourFromAngle(angle));
            }
            else
            {
                SetMinutes(GetMinuteFromAngle(angle));
            }
        }

        private void SetHours(int hour, bool updateClock = true)
        {
            if (hour <= 0) hour = 12;
            if (hour > 12) hour = 12;

            m_CurrentHour = hour;
            textHours.text = hour.ToString("0");

            if (updateClock)
            {
                SelectHours();
                UpdateNeedle();
            }
        }

        private void SetMinutes(int minute, bool updateClock = true)
        {
            if (minute < 0) minute = 0;
            if (minute > 60) minute = 60;

            m_CurrentMinute = minute;
            m_TextMinutes.text = minute.ToString("00");

            if (updateClock)
            {
                SelectMinutes();
                UpdateNeedle();
            }
        }

        public void SetTime(int hour, int minute)
        {
            SetHours(hour);
            SetMinutes(minute);
        }

        private void UpdateNeedle()
        {
            float rotation = m_IsHoursSelected ? GetAngleFromHour(m_CurrentHour) : GetAngleFromMinute(m_CurrentMinute);
            UpdateNeedleAngle(rotation);
        }

        private void UpdateNeedleAngle(float angle)
        {
            m_NeedleAngle = angle;
        }

        private void UpdateClockTextArray()
        {
            for (int i = 0; i < m_ClockTextArray.Length; i++)
            {
                if (m_IsHoursSelected)
                {
                    int number = (i + 1);
                    m_ClockTextArray[i].text = number.ToString("0");
                }
                else
                {
                    int number = (i + 1) * 5;
                    number = number % 60;
                    m_ClockTextArray[i].text = number.ToString("00");
                }
            }

            UpdateNeedle();
        }

        public void OnAMClicked()
        {
            m_IsAM = true;
            m_TextAM.color = new Color(m_TextAM.color.r, m_TextAM.color.g, m_TextAM.color.b, 1.0f);
            textPM.color = new Color(textPM.color.r, textPM.color.g, textPM.color.b, 0.5f);
        }

        public void OnPMClicked()
        {
            m_IsAM = false;
            textPM.color = new Color(textPM.color.r, textPM.color.g, textPM.color.b, 1.0f);
            m_TextAM.color = new Color(m_TextAM.color.r, m_TextAM.color.g, m_TextAM.color.b, 0.5f);
        }

        public void OnHoursClicked()
        {
            SelectHours();
        }

        private void SelectHours()
        {
            textHours.color = new Color(textHours.color.r, textHours.color.g, textHours.color.b, 1.0f);
            m_TextMinutes.color = new Color(m_TextMinutes.color.r, m_TextMinutes.color.g, m_TextMinutes.color.b, 0.5f);

            m_IsHoursSelected = true;
            UpdateClockTextArray();
        }

        public void OnMinutesClicked()
        {
            SelectMinutes();
        }

        private void SelectMinutes()
        {
            m_TextMinutes.color = new Color(m_TextMinutes.color.r, m_TextMinutes.color.g, m_TextMinutes.color.b, 1.0f);
            textHours.color = new Color(textHours.color.r, textHours.color.g, textHours.color.b, 0.5f);

            m_IsHoursSelected = false;
            UpdateClockTextArray();
        }

        public void OnButtonOkClicked()
        {
            if (m_OnAffirmativeClicked != null)
            {
                m_OnAffirmativeClicked(DateTime.MinValue.AddHours(m_CurrentHour).AddMinutes(m_CurrentMinute));
            }

            Hide();
        }
    }
}