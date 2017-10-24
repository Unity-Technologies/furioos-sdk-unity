//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Simple List", 1)]
    public class DialogSimpleList : MaterialDialog
    {
        [SerializeField]
        private DialogTitleSection m_TitleSection = new DialogTitleSection();
        public DialogTitleSection titleSection
        {
            get { return m_TitleSection; }
            set { m_TitleSection = value; }
        }

        [SerializeField]
        private VerticalScrollLayoutElement m_ListScrollLayoutElement;
        public VerticalScrollLayoutElement listScrollLayoutElement
        {
            get { return m_ListScrollLayoutElement; }
            set { m_ListScrollLayoutElement = value; }
        }

        private List<DialogSimpleOption> m_SelectionItems;
        public List<DialogSimpleOption> selectionItems
        {
            get { return m_SelectionItems; }
        }

        private OptionDataList m_OptionDataList;
        public OptionDataList optionDataList
        {
            get { return m_OptionDataList; }
            set { m_OptionDataList = value; }
        }

        [SerializeField]
        private GameObject m_OptionTemplate;

        private Action<int> m_OnItemClick;

		void OnEnable()
		{
			GetComponentInChildren<OverscrollConfig>().Setup();
		}

		public void Initialize(OptionDataList optionDataList, Action<int> onItemClick, string titleText, ImageData icon)
        {
            m_TitleSection.SetTitle(titleText, icon);

            m_OptionDataList = optionDataList;
            m_SelectionItems = new List<DialogSimpleOption>();

            Image imageIcon = m_OptionTemplate.GetChildByName<Image>("Icon");
            VectorImage vectorIcon = m_OptionTemplate.GetChildByName<VectorImage>("Icon");

			if (m_OptionDataList.options.Count > 0 && m_OptionDataList.options[0].imageData != null)
			{
				if (m_OptionDataList.options[0].imageData.imageDataType == ImageDataType.Sprite)
				{
					DestroyImmediate(vectorIcon.gameObject);
				}
				else
				{
					DestroyImmediate(imageIcon.gameObject);
				}
			}
			else
			{
				DestroyImmediate(imageIcon.gameObject);
			}

            for (int i = 0; i < m_OptionDataList.options.Count; i++)
            {
                m_SelectionItems.Add(CreateSelectionItem(i));
            }

            float availableHeight = DialogManager.rectTransform.rect.height;

            LayoutGroup textAreaRectTransform = m_TitleSection.text.transform.parent.GetComponent<LayoutGroup>();

            if (textAreaRectTransform.gameObject.activeSelf)
            {
                textAreaRectTransform.CalculateLayoutInputVertical();
                availableHeight -= textAreaRectTransform.preferredHeight;
            }

            m_ListScrollLayoutElement.maxHeight = availableHeight - 48f;

            Destroy(m_OptionTemplate);

            m_OnItemClick = onItemClick;

            Initialize();
        }

        private DialogSimpleOption CreateSelectionItem(int i)
        {
            DialogSimpleOption option = Instantiate(m_OptionTemplate).GetComponent<DialogSimpleOption>();
            option.rectTransform.SetParent(m_OptionTemplate.transform.parent);
            option.rectTransform.localScale = Vector3.one;
            option.rectTransform.localEulerAngles = Vector3.zero;
            option.rectTransform.localPosition = new Vector3(option.rectTransform.localPosition.x, option.rectTransform.localPosition.y, 0f);

            OptionData data = m_OptionDataList.options[i];

            Text text = option.gameObject.GetChildByName<Text>("Text");
            Graphic graphic = option.gameObject.GetChildByName<Graphic>("Icon");

            text.text = data.text;

            if (data.imageData == null)
            {
                text.rectTransform.sizeDelta = new Vector2(-48f, 0f);
                text.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                Destroy(graphic.gameObject);
            }
            else
            {
                graphic.SetImage(data.imageData);
            }

            option.index = i;
            option.onClickAction += OnItemClick;

            return option;
        }

        public void OnItemClick(int index)
        {
            m_OnItemClick.InvokeIfNotNull(index);
			m_OptionDataList.options[index].onOptionSelected.InvokeIfNotNull();

            Hide();
        }
    }
}