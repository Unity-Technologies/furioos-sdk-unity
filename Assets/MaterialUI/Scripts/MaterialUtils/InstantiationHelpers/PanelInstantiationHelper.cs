//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated Panels.
    /// </summary>
    /// <seealso cref="MaterialUI.InstantiationHelper" />
    public class PanelInstantiationHelper : InstantiationHelper
    {
        /// <summary>
        /// Should the panel have a HorizontalLayoutGroup?
        /// </summary>
        public const int optionHasLayoutHorizontal = 0;
        /// <summary>
        /// Should the panel have a VerticalLayoutGroup?
        /// </summary>
        public const int optionHasLayoutVertical = 1;
        /// <summary>
        /// Should the panel be fitted to the content?
        /// </summary>
        public const int optionFitted = 2;
        /// <summary>
        /// Should the panel stretch to match its parent?
        /// </summary>
        public const int optionStretched = 3;

        /// <summary>
        /// The RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;

        /// <summary>
        /// The SizeFitter.
        /// </summary>
        [SerializeField]
        private ContentSizeFitter m_SizeFitter;

        /// <summary>
        /// The top LayoutGroup.
        /// </summary>
        [SerializeField]
        private LayoutGroup m_TopLayoutGroup;

        /// <summary>
        /// The bottom RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform m_BottomRectTransform;

        /// <summary>
        /// The bottom LayoutGroup.
        /// </summary>
        private LayoutGroup m_BottomLayoutGroup;

        /// <summary>
        /// Configures the object.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public override void HelpInstantiate(params int[] options)
        {
            if (!options.Contains(optionHasLayoutHorizontal) && !options.Contains(optionHasLayoutVertical))
            {
                DestroyImmediate(m_TopLayoutGroup);
                m_BottomRectTransform.anchorMin = Vector2.zero;
                m_BottomRectTransform.anchorMax = Vector2.one;
                m_BottomRectTransform.sizeDelta = Vector2.zero;
                m_BottomRectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                if (options.Contains(optionHasLayoutHorizontal))
                {
                    GameObject go = m_TopLayoutGroup.gameObject;
                    DestroyImmediate(m_TopLayoutGroup);
                    m_TopLayoutGroup = go.AddComponent<HorizontalLayoutGroup>();
                    m_TopLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

                    m_BottomLayoutGroup = m_BottomRectTransform.gameObject.AddComponent<HorizontalLayoutGroup>();
                    m_BottomLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    ((HorizontalLayoutGroup)m_BottomLayoutGroup).childForceExpandWidth = false;
                    ((HorizontalLayoutGroup)m_BottomLayoutGroup).childForceExpandHeight = false;
                }
                else
                {
                    m_BottomLayoutGroup = m_BottomRectTransform.gameObject.AddComponent<VerticalLayoutGroup>();
                    m_BottomLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    ((VerticalLayoutGroup)m_BottomLayoutGroup).childForceExpandWidth = false;
                    ((VerticalLayoutGroup)m_BottomLayoutGroup).childForceExpandHeight = false;
                }
            }

            if (!options.Contains(optionFitted))
            {
                DestroyImmediate(m_SizeFitter);
                m_RectTransform.sizeDelta = new Vector2(300f, 300f);
                m_RectTransform.anchoredPosition = Vector2.zero;
            }

            if (options.Contains(optionStretched))
            {
                m_RectTransform.anchorMin = new Vector2(0f, 0f);
                m_RectTransform.anchorMax = new Vector2(1f, 1f);
                m_RectTransform.sizeDelta = new Vector2(-48f, -48f);
                m_RectTransform.anchoredPosition = Vector2.zero;
            }

#if UNITY_EDITOR
            Selection.activeGameObject = m_BottomRectTransform.gameObject;
#endif

            base.HelpInstantiate(options);
        }
    }
}