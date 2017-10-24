//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Prompt", 1)]
	public class DialogPrompt : MaterialDialog
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
		private MaterialInputField m_FirstInputField;
		public MaterialInputField firstInputField
        {
			get { return m_FirstInputField; }
        }

		[SerializeField]
		private MaterialInputField m_SecondInputField;
		public MaterialInputField secondInputField
		{
			get { return m_SecondInputField; }
		}

		private Action<string> m_OnAffirmativeOneButtonClicked;
		public Action<string> onAffirmativeOneButtonClicked
		{
			get { return m_OnAffirmativeOneButtonClicked; }
			set { m_OnAffirmativeOneButtonClicked = value; }
		}

		private Action<string, string> m_OnAffirmativeTwoButtonClicked;
		public Action<string, string> onAffirmativeTwoButtonClicked
		{
			get { return m_OnAffirmativeTwoButtonClicked; }
			set { m_OnAffirmativeTwoButtonClicked = value; }
		}

		public void Initialize(string firstFieldName, Action<string> onAffirmativeButtonClicked, string affirmativeButtonText, string titleText, ImageData icon, Action onDismissiveButtonClicked, string dismissiveButtonText)
		{
			m_OnAffirmativeOneButtonClicked = onAffirmativeButtonClicked;
			CommonInitialize(firstFieldName, null, affirmativeButtonText, titleText, icon, onDismissiveButtonClicked, dismissiveButtonText);
		}

		public void Initialize(string firstFieldName, string secondFieldName, Action<string, string> onAffirmativeButtonClicked, string affirmativeButtonText, string titleText, ImageData icon, Action onDismissiveButtonClicked, string dismissiveButtonText)
        {
			m_OnAffirmativeTwoButtonClicked = onAffirmativeButtonClicked;
			CommonInitialize(firstFieldName, secondFieldName, affirmativeButtonText, titleText, icon, onDismissiveButtonClicked, dismissiveButtonText);
        }

		private void CommonInitialize(string firstFieldName, string secondFieldName, string affirmativeButtonText, string titleText, ImageData icon, Action onDismissiveButtonClicked, string dismissiveButtonText)
		{
			m_TitleSection.SetTitle(titleText, icon);
			m_ButtonSection.SetButtons(null, affirmativeButtonText, onDismissiveButtonClicked, dismissiveButtonText);
			m_ButtonSection.SetupButtonLayout(rectTransform);

			m_FirstInputField.hintText = firstFieldName;
			m_SecondInputField.hintText = secondFieldName;

			m_FirstInputField.customTextValidator = new EmptyTextValidator();
			m_SecondInputField.customTextValidator = new EmptyTextValidator();

			if (string.IsNullOrEmpty(secondFieldName))
			{
				m_SecondInputField.gameObject.SetActive(false);
			}

			UpdateAffirmativeButtonState();

			Initialize();
		}

        public void AffirmativeButtonClicked()
        {
			if (m_OnAffirmativeOneButtonClicked != null)
			{
				m_OnAffirmativeOneButtonClicked(m_FirstInputField.inputField.text);
			}

			if (m_OnAffirmativeTwoButtonClicked != null)
			{
				m_OnAffirmativeTwoButtonClicked(m_FirstInputField.inputField.text, m_SecondInputField.inputField.text);
			}

            Hide();
        }

        public void DismissiveButtonClicked()
        {
            m_ButtonSection.OnDismissiveButtonClicked();
            Hide();
        }

		public void UpdateAffirmativeButtonState()
		{
			bool isButtonInteractable = m_FirstInputField.customTextValidator.IsTextValid();
			if (m_SecondInputField.gameObject.activeSelf)
			{
				isButtonInteractable &= m_SecondInputField.customTextValidator.IsTextValid();
			}

			m_ButtonSection.affirmativeButton.interactable = isButtonInteractable;
		}

		public override void Show()
		{
			base.Show();

			firstInputField.inputField.Select();
		}
    }
}