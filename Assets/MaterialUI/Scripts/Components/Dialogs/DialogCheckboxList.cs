//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Checkbox List", 1)]
    public class DialogCheckboxList : MaterialDialog
    {
        [SerializeField]
        private DialogTitleSection m_TitleSection;
        public DialogTitleSection titleSection
        {
            get { return m_TitleSection; }
            set { m_TitleSection = value; }
        }

        [SerializeField]
        private DialogButtonSection m_ButtonSection;
        public DialogButtonSection buttonSection
        {
            get { return m_ButtonSection; }
            set { m_ButtonSection = value; }
        }

        [SerializeField]
        private VerticalScrollLayoutElement m_ListScrollLayoutElement;
        public VerticalScrollLayoutElement listScrollLayoutElement
        {
            get { return m_ListScrollLayoutElement; }
            set { m_ListScrollLayoutElement = value; }
        }

        private List<DialogCheckboxOption> m_SelectionItems;
        public List<DialogCheckboxOption> selectionItems
        {
            get { return m_SelectionItems; }
        }

		private bool[] m_SelectedIndexes;
        public bool[] selectedIndexes
        {
            get { return m_SelectedIndexes; }
            set { m_SelectedIndexes = value; }
        }

        private string[] m_OptionList;
        public string[] optionList
        {
            get { return m_OptionList; }
            set { m_OptionList = value; }
        }

		private Action<bool[]> m_OnAffirmativeButtonClicked;
		public Action<bool[]> onAffirmativeButtonClicked
		{
			get { return m_OnAffirmativeButtonClicked; }
			set { m_OnAffirmativeButtonClicked = value; }
		}

        [SerializeField]
        private GameObject m_OptionTemplate;

        public delegate void OptionSelectedEvent(int i);
        public OptionSelectedEvent onOptionSelected;

		void OnEnable()
		{
			GetComponentInChildren<OverscrollConfig>().Setup();
		}

		public void Initialize(string[] options, Action<bool[]> onAffirmativeButtonClicked, string affirmativeButtonText, string titleText, ImageData icon, Action onDismissiveButtonClicked, string dismissiveButtonText)
        {
            m_OptionList = options;
            m_SelectionItems = new List<DialogCheckboxOption>();
            m_SelectedIndexes = new bool[options.Length];

            for (int i = 0; i < m_OptionList.Length; i++)
            {
                m_SelectionItems.Add(CreateSelectionItem(i));
            }

            Destroy(m_OptionTemplate);

            m_TitleSection.SetTitle(titleText, icon);
			m_ButtonSection.SetButtons(null, affirmativeButtonText, onDismissiveButtonClicked, dismissiveButtonText);
            m_ButtonSection.SetupButtonLayout(rectTransform);

			m_OnAffirmativeButtonClicked = onAffirmativeButtonClicked;

            float availableHeight = DialogManager.rectTransform.rect.height;

            LayoutGroup textAreaRectTransform = m_TitleSection.text.transform.parent.GetComponent<LayoutGroup>();

            if (textAreaRectTransform.gameObject.activeSelf)
            {
                textAreaRectTransform.CalculateLayoutInputVertical();
                availableHeight -= textAreaRectTransform.preferredHeight;
            }

            m_ListScrollLayoutElement.maxHeight = availableHeight - 98f;

            Initialize();
        }

        private DialogCheckboxOption CreateSelectionItem(int i)
        {
            DialogCheckboxOption option = Instantiate(m_OptionTemplate).GetComponent<DialogCheckboxOption>();
            option.rectTransform.SetParent(m_OptionTemplate.transform.parent);
            option.rectTransform.localScale = Vector3.one;
            option.rectTransform.localEulerAngles = Vector3.zero;
            option.rectTransform.localPosition = new Vector3(option.rectTransform.localPosition.x, option.rectTransform.localPosition.y, 0f);

            Text text = option.itemText;

            text.text = m_OptionList[i];

            option.index = i;
            option.onClickAction += OnItemClick;

            return option;
        }

        public void OnItemClick(int index)
        {
            Toggle toggle = m_SelectionItems[index].itemCheckbox.toggle;
            toggle.isOn = !toggle.isOn;

            m_SelectedIndexes[index] = toggle.isOn;
        }

        public void AffirmativeButtonClicked()
        {
			m_OnAffirmativeButtonClicked.InvokeIfNotNull(m_SelectedIndexes);
            Hide();
        }

        public void DismissiveButtonClicked()
        {
            m_ButtonSection.OnDismissiveButtonClicked();
            Hide();
        }
    }
}