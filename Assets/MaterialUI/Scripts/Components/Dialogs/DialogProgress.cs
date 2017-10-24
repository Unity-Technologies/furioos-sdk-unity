//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Progress", 1)]
    public class DialogProgress : MaterialDialog
    {
        [SerializeField]
        private DialogTitleSection m_TitleSection = new DialogTitleSection();
        public DialogTitleSection titleSection
        {
            get { return m_TitleSection; }
            set { m_TitleSection = value; }
        }

        [SerializeField]
        private Text m_BodyText;
        public Text bodyText
        {
            get { return m_BodyText; }
        }

        private ProgressIndicator m_ProgressIndicator;
        public ProgressIndicator progressIndicator
        {
            get { return m_ProgressIndicator; }
            set { m_ProgressIndicator = value; }
        }

        [SerializeField]
        private ProgressIndicator m_LinearIndicator;

        [SerializeField]
        private ProgressIndicator m_CircularIndicator;

		public void Initialize(string bodyText, string titleText, ImageData icon, bool startStationaryAtZero = true)
        {
            m_TitleSection.SetTitle(titleText, icon);

            if (string.IsNullOrEmpty(bodyText))
            {
                m_BodyText.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                m_BodyText.text = bodyText;
            }

            if (!startStationaryAtZero)
            {
                m_ProgressIndicator.StartIndeterminate();
            }
            else
            {
                m_ProgressIndicator.SetProgress(0f, false);
            }

            Initialize();
        }

        public void SetupIndicator(bool isLinear)
        {
            m_LinearIndicator.gameObject.SetActive(isLinear);
            m_CircularIndicator.gameObject.SetActive(!isLinear);
            m_ProgressIndicator = isLinear ? m_LinearIndicator : m_CircularIndicator;
            m_ProgressIndicator.transform.parent.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = isLinear;
        }
    }
}