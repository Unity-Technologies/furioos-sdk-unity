//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Alert", 1)]
    public class DialogAlert : MaterialDialog
    {
        [SerializeField]
        private DialogTitleSection m_TitleSection = new DialogTitleSection();
        public DialogTitleSection titleSection
        {
            get { return m_TitleSection; }
            set { m_TitleSection = value; }
        }

        [SerializeField]
        private DialogButtonSection m_ButtonSection = new DialogButtonSection();
        public DialogButtonSection buttonSection
        {
            get { return m_ButtonSection; }
            set { m_ButtonSection = value; }
        }

        [SerializeField]
        private Text m_BodyText;
        public Text bodyText
        {
            get { return m_BodyText; }
        }

		public void Initialize(string bodyText, Action onAffirmativeButtonClicked, string affirmativeButtonText, string titleText, ImageData icon, Action onDismissiveButtonClicked, string dismissiveButtonText)
        {
            m_TitleSection.SetTitle(titleText, icon);
			m_ButtonSection.SetButtons(onAffirmativeButtonClicked, affirmativeButtonText, onDismissiveButtonClicked, dismissiveButtonText);

            m_BodyText.text = bodyText;

            m_ButtonSection.SetupButtonLayout(rectTransform);

            Initialize();
        }

        public void AffirmativeButtonClicked()
        {
            m_ButtonSection.OnAffirmativeButtonClicked();
            Hide();
        }

        public void DismissiveButtonClicked()
        {
            m_ButtonSection.OnDismissiveButtonClicked();
            Hide();
        }
    }
}