//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MaterialUI
{
    [Serializable]
    [AddComponentMenu("MaterialUI/Dialogs/Title Section", 100)]
    public class DialogTitleSection
    {
        [SerializeField]
        private Text m_Text;
        public Text text
        {
            get { return m_Text; }
		}

		[SerializeField]
		private Graphic m_Sprite;
		public Graphic sprite
		{
			get { return m_Sprite; }
		}

		[SerializeField]
		private VectorImage m_VectorImageData;
		public VectorImage vectorImageData
		{
			get { return m_VectorImageData; }
		}

		public void SetTitle(string titleText, ImageData icon)
        {
            if (!string.IsNullOrEmpty(titleText) || icon != null)
            {
                if (!string.IsNullOrEmpty(titleText))
                {
                    m_Text.text = titleText;
                }
                else
                {
                    m_Text.gameObject.SetActive(false);
                }

				if (icon == null)
				{
					m_Sprite.gameObject.SetActive(false);
					m_VectorImageData.gameObject.SetActive(false);
				}
				else
				{
					if (icon.imageDataType == ImageDataType.VectorImage)
					{
						m_VectorImageData.vectorImageData = icon.vectorImageData;
						m_Sprite.gameObject.SetActive(false);
					}
					else
					{
						m_Sprite.GetComponent<Graphic>().SetImage(icon);
						m_VectorImageData.gameObject.SetActive(false);
					}
				}
            }
            else
            {
                m_Text.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    [Serializable]
    [AddComponentMenu("MaterialUI/Dialogs/Button Section", 100)]
    public class DialogButtonSection
    {
        private Action m_OnAffirmativeButtonClicked;
        public Action onAffirmativeButtonClicked
        {
            get { return m_OnAffirmativeButtonClicked; }
        }

        private Action m_OnDismissiveButtonClicked;
        public Action onDismissiveButtonClicked
        {
            get { return m_OnDismissiveButtonClicked; }
        }

        [SerializeField]
        private MaterialButton m_AffirmativeButton;
        public MaterialButton affirmativeButton
        {
            get { return m_AffirmativeButton; }
        }

        [SerializeField]
        private MaterialButton m_DismissiveButton;
        public MaterialButton dismissiveButton
        {
            get { return m_DismissiveButton; }
        }

		private bool m_ShowDismissiveButton;

		public void SetButtons(Action onAffirmativeButtonClick, string affirmativeButtonText, Action onDismissiveButtonClick, string dismissiveButtonText)
        {
			SetAffirmativeButton(onAffirmativeButtonClick, affirmativeButtonText);
			SetDismissiveButton(onDismissiveButtonClick, dismissiveButtonText);
        }

		public void SetAffirmativeButton(Action onButtonClick, string text)
        {
			m_OnAffirmativeButtonClicked = onButtonClick;
            m_AffirmativeButton.text.text = text;
        }

		public void SetDismissiveButton(Action onButtonClick, string text)
        {
            if (string.IsNullOrEmpty(text) && onButtonClick == null) return;

            m_ShowDismissiveButton = true;
			m_OnDismissiveButtonClicked = onButtonClick;
            m_DismissiveButton.text.text = text;
        }

        public void SetupButtonLayout(RectTransform dialogRectTransform)
        {
            m_DismissiveButton.gameObject.SetActive(m_ShowDismissiveButton);

            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogRectTransform);

            if (m_AffirmativeButton.minWidth + m_DismissiveButton.minWidth + 24f > DialogManager.rectTransform.rect.width - 48f)
            {
                Object.DestroyImmediate(m_AffirmativeButton.rectTransform.parent.GetComponent<HorizontalLayoutGroup>());

                VerticalLayoutGroup verticalLayoutGroup = m_AffirmativeButton.rectTransform.parent.gameObject.AddComponent<VerticalLayoutGroup>();
                verticalLayoutGroup.padding = new RectOffset(8, 8, 8, 8);
                verticalLayoutGroup.childAlignment = TextAnchor.UpperRight;
                verticalLayoutGroup.childForceExpandHeight = false;
                verticalLayoutGroup.childForceExpandWidth = false;
            }
        }

        public void OnAffirmativeButtonClicked()
        {
            m_OnAffirmativeButtonClicked.InvokeIfNotNull();
        }

        public void OnDismissiveButtonClicked()
        {
            m_OnDismissiveButtonClicked.InvokeIfNotNull();
        }
    }
}