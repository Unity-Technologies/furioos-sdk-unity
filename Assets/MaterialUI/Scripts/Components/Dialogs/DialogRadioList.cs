//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Radio List", 1)]
    public class DialogRadioList : MaterialDialog
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

        [SerializeField]
        private ToggleGroup m_ToggleGroup;
        public ToggleGroup toggleGroup
        {
            get { return m_ToggleGroup; }
            set { m_ToggleGroup = value; }
        }

        private List<DialogRadioOption> m_SelectionItems;
        public List<DialogRadioOption> selectionItems
        {
            get { return m_SelectionItems; }
        }

		private int m_SelectedIndex;
        public int selectedIndex
        {
            get { return m_SelectedIndex; }
        }

        private string[] m_OptionList;
        public string[] optionList
        {
            get { return m_OptionList; }
            set { m_OptionList = value; }
        }

		private Action<int> m_OnAffirmativeButtonClicked;
		public Action<int> onAffirmativeButtonClicked
		{
			get { return m_OnAffirmativeButtonClicked; }
			set { m_OnAffirmativeButtonClicked = value; }
		}

        [SerializeField]
        private GameObject m_OptionTemplate;

		void OnEnable()
		{
			GetComponentInChildren<OverscrollConfig>().Setup();
		}

		public void Initialize(string[] options, Action<int> onAffirmativeButtonClicked, string affirmativeButtonText, string titleText, ImageData icon, Action onDismissiveButtonClicked, string dismissiveButtonText, int selectedIndexStart)
        {
            m_OptionList = options;
            m_SelectionItems = new List<DialogRadioOption>();

            for (int i = 0; i < m_OptionList.Length; i++)
            {
                m_SelectionItems.Add(CreateSelectionItem(i));
            }

			if (selectedIndexStart < 0) selectedIndexStart = 0;
			if (selectedIndexStart >= m_SelectionItems.Count) selectedIndexStart = m_SelectionItems.Count - 1;
			m_SelectionItems[selectedIndexStart].itemRadio.toggle.isOn = true;
			m_SelectedIndex = selectedIndexStart;

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

        private DialogRadioOption CreateSelectionItem(int i)
        {
            DialogRadioOption option = Instantiate(m_OptionTemplate).GetComponent<DialogRadioOption>();
            option.rectTransform.SetParent(m_OptionTemplate.transform.parent);
            option.rectTransform.localScale = Vector3.one;
            option.rectTransform.localEulerAngles = Vector3.zero;
            option.rectTransform.localPosition = new Vector3(option.rectTransform.localPosition.x, option.rectTransform.localPosition.y, 0f);

            Text text = option.itemText;

            text.text = m_OptionList[i];

            option.index = i;
            option.onClickAction += OnItemClick;

            option.itemRadio.toggle.group = m_ToggleGroup;
            option.itemRadio.toggle.isOn = false;

            return option;
        }

        public void OnItemClick(int index)
        {
            Toggle toggle = m_SelectionItems[index].itemRadio.toggle;
            toggle.isOn = !toggle.isOn;
            m_SelectedIndex = index;
        }

        public void AffirmativeButtonClicked()
        {
			m_OnAffirmativeButtonClicked.InvokeIfNotNull(m_SelectedIndex);
            Hide();
        }

        public void DismissiveButtonClicked()
        {
            m_ButtonSection.OnDismissiveButtonClicked();
            Hide();
        }
    }
}