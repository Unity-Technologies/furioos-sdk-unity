//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated rectangular MaterialButtons.
    /// </summary>
    /// <seealso cref="MaterialUI.InstantiationHelper" />
    public class ButtonRectInstantiationHelper : InstantiationHelper
    {
        /// <summary>
        /// Should the button be raised?
        /// </summary>
        public const int optionRaised = 0;
        /// <summary>
        /// Should the button contain a MaterialDropdown component?
        /// </summary>
        public const int optionHasDropdown = 1;
        /// <summary>
        /// Should the button be multi-content?
        /// </summary>
        public const int optionHasContent = 2;

        /// <summary>
        /// The button.
        /// </summary>
        [SerializeField]
        private MaterialButton m_Button;

        /// <summary>
        /// The dropdown.
        /// </summary>
        [SerializeField]
        private MaterialDropdown m_Dropdown;

        /// <summary>
        /// The RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;

        /// <summary>
        /// The content.
        /// </summary>
        [SerializeField]
        private HorizontalLayoutGroup m_Content;

        /// <summary>
        /// The text.
        /// </summary>
        [SerializeField]
        private RectTransform m_Text;

        /// <summary>
        /// The icon.
        /// </summary>
        [SerializeField]
        private Graphic m_Icon;

        /// <summary>
        /// The icon data.
        /// </summary>
        [SerializeField]
        private VectorImageData m_IconData;

        /// <summary>
        /// Configures the object.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public override void HelpInstantiate(params int[] options)
        {
            m_Button.isCircularButton = false;

            if (!options.Contains(optionRaised))
            {
                m_Button.isRaisedButton = true;
                m_Button.Convert(true);
            }
            else
            {
                m_Button.isRaisedButton = true;
            }

            if (!options.Contains(optionHasDropdown))
            {
                DestroyImmediate(m_Dropdown);
                m_Button.buttonObject.onClick = null;
                m_Icon.rectTransform.SetAsFirstSibling();
                m_Icon.SetImage(m_IconData);
                RectOffset offset = m_Content.padding;
                offset.right = 0;
                m_Content.padding = offset;
                m_Button.text.text = "BUTTON";
            }
            else
            {
                m_Button.icon = null;

                m_Button.fitWidthToContent = false;
                m_Content.childAlignment = TextAnchor.MiddleLeft;
                m_Content.padding.top = 0;
                m_Content.padding.bottom = 0;
                RectTransform contentTransform = (RectTransform)m_Content.transform;
                contentTransform.sizeDelta = new Vector2(-30f, contentTransform.sizeDelta.y);
                contentTransform.anchoredPosition = new Vector2(0f, contentTransform.anchoredPosition.y);
                m_Button.rectTransform.sizeDelta = new Vector2(134, m_Button.rectTransform.sizeDelta.y);
                m_Icon.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
                m_Icon.rectTransform.anchorMin = new Vector2(1f, 0.5f);
                m_Icon.rectTransform.anchorMax = new Vector2(1f, 0.5f);
                m_Icon.rectTransform.anchoredPosition = new Vector2(-12f, 0f);
                m_Icon.rectTransform.sizeDelta = new Vector2(24f, 24f);
                gameObject.AddComponent<LayoutElement>().preferredWidth = 134;
            }

            if (!options.Contains(optionHasContent))
            {
                m_Button.contentRectTransform = m_Text;
                m_Text.SetParentAndScale(m_RectTransform, m_Text.localScale);
                m_Text.anchorMin = Vector2.zero;
                m_Text.anchorMax = Vector2.one;
                DestroyImmediate(m_Content.gameObject);
                m_Button.icon = null;
                m_Button.SetLayoutDirty();
            }

            base.HelpInstantiate(options);
        }
    }
}