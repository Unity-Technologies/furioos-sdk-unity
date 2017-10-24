//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated TabViews.
    /// </summary>
    /// <seealso cref="MaterialUI.InstantiationHelper" />
    public class TabViewInstantiationHelper : InstantiationHelper
    {
        /// <summary>
        /// Should the tabs have icons?
        /// </summary>
        public const int optionHasIcon = 0;
        /// <summary>
        /// Should the tabs have labels?
        /// </summary>
        public const int optionHasLabel = 1;

        /// <summary>
        /// The RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;

        /// <summary>
        /// The item template.
        /// </summary>
        [SerializeField]
        private TabItem m_ItemTemplate;

        /// <summary>
        /// The tab bar RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform m_TabBarRectTransform;

        /// <summary>
        /// The pages object RectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_PagesRectTransform;

        /// <summary>
        /// The icon RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform[] m_IconRectTransforms;

        /// <summary>
        /// The text RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform m_TextRectTransform;

        /// <summary>
        /// Configures the object.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public override void HelpInstantiate(params int[] options)
        {
            m_RectTransform.sizeDelta = Vector2.zero;
            m_RectTransform.anchoredPosition = Vector2.zero;

            if (!options.Contains(optionHasIcon) || !options.Contains(optionHasLabel))
            {
                m_TabBarRectTransform.sizeDelta = new Vector2(m_TabBarRectTransform.sizeDelta.x, 48);

                if (!options.Contains(optionHasIcon))
                {
                    m_ItemTemplate.itemIcon = null;

                    for (int i = 0; i < m_IconRectTransforms.Length; i++)
                    {
                        DestroyImmediate(m_IconRectTransforms[i].gameObject);
                    }

                    m_TextRectTransform.anchorMin = Vector2.zero;
                    m_TextRectTransform.anchorMax = Vector2.one;
                    m_TextRectTransform.anchoredPosition = Vector2.zero;
                    m_TextRectTransform.sizeDelta = Vector2.zero;
                    m_TextRectTransform.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                }
                else
                {
                    m_ItemTemplate.itemText = null;
                    DestroyImmediate(m_TextRectTransform.gameObject);

                    for (int i = 0; i < m_IconRectTransforms.Length; i++)
                    {
                        m_IconRectTransforms[i].anchorMin = new Vector2(0.5f, 0.5f);
                        m_IconRectTransforms[i].anchorMax = new Vector2(0.5f, 0.5f);
                        m_IconRectTransforms[i].pivot = new Vector2(0.5f, 0.5f);
                        m_IconRectTransforms[i].anchoredPosition = Vector2.zero;
                        m_IconRectTransforms[i].sizeDelta = new Vector2(24, 24);
                    }
                }

                m_PagesRectTransform.sizeDelta = new Vector2(m_PagesRectTransform.sizeDelta.x, -48);
                m_PagesRectTransform.anchoredPosition = new Vector2(m_PagesRectTransform.anchoredPosition.x, -24f);
            }

            base.HelpInstantiate(options);
        }
    }
}