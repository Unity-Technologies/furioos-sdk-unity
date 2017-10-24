//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated Empty UI objects.
    /// </summary>
    /// <seealso cref="MaterialUI.InstantiationHelper" />
    public class EmptyUIObjectInstantiationHelper : InstantiationHelper
    {
        /// <summary>
        /// Should the object have a HorizontalLayoutGroup?
        /// </summary>
        public const int optionHasLayoutHorizontal = 0;
        /// <summary>
        /// Should the object have a VerticalLayoutGroup?
        /// </summary>
        public const int optionHasLayoutVertical = 1;
        /// <summary>
        /// Should the object be fitted to its content?
        /// </summary>
        public const int optionFitted = 2;
        /// <summary>
        /// Should the object stretch to fill its parent?
        /// </summary>
        public const int optionStretched = 3;

        /// <summary>
        /// The RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;

        /// <summary>
        /// The ContentSizeFitter.
        /// </summary>
        [SerializeField]
        private ContentSizeFitter m_SizeFitter;

        /// <summary>
        /// The LayoutGroup.
        /// </summary>
        [SerializeField]
        private LayoutGroup m_LayoutGroup;

        /// <summary>
        /// Configures the object.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public override void HelpInstantiate(params int[] options)
        {
            if (!options.Contains(optionHasLayoutHorizontal) && !options.Contains(optionHasLayoutVertical))
            {
                DestroyImmediate(m_LayoutGroup);
            }
            else
            {
                if (options.Contains(optionHasLayoutHorizontal))
                {
                    GameObject go = m_LayoutGroup.gameObject;
                    DestroyImmediate(m_LayoutGroup);
                    m_LayoutGroup = go.AddComponent<HorizontalLayoutGroup>();
                    m_LayoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    ((HorizontalLayoutGroup)m_LayoutGroup).childForceExpandWidth = false;
                    ((HorizontalLayoutGroup)m_LayoutGroup).childForceExpandHeight = false;
                }
                else
                {
                    m_LayoutGroup = m_RectTransform.gameObject.GetAddComponent<VerticalLayoutGroup>();
                    m_LayoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    ((VerticalLayoutGroup)m_LayoutGroup).childForceExpandWidth = false;
                    ((VerticalLayoutGroup)m_LayoutGroup).childForceExpandHeight = false;
                }
            }

            if (!options.Contains(optionFitted))
            {
                DestroyImmediate(m_SizeFitter);
                m_RectTransform.sizeDelta = new Vector2(100f, 100f);
                m_RectTransform.anchoredPosition = Vector2.zero;
            }

            if (options.Contains(optionStretched))
            {
                m_RectTransform.anchorMin = new Vector2(0f, 0f);
                m_RectTransform.anchorMax = new Vector2(1f, 1f);
                m_RectTransform.sizeDelta = Vector2.zero;
                m_RectTransform.anchoredPosition = Vector2.zero;
            }

            base.HelpInstantiate(options);
        }
    }
}