//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated CircularProgressIndicators.
    /// </summary>
    /// <seealso cref="MaterialUI.InstantiationHelper" />
    public class CircleProgressIndicatorInstantiationHelper : InstantiationHelper
    {
        /// <summary>
        /// Should the indicator be raised?
        /// </summary>
        public const int optionRaised = 0;
        /// <summary>
        /// Should the indicator have a label?
        /// </summary>
        public const int optionHasLabel = 1;
        /// <summary>
        /// Should the indicator have a HorizontalLayoutGroup?
        /// </summary>
        public const int optionLayoutHorizontal = 2;
        /// <summary>
        /// Should the indicator have a VerticalLayoutGroup?
        /// </summary>
        public const int optionLayoutVertical = 3;

        /// <summary>
        /// The shadow.
        /// </summary>
        [SerializeField]
        private GameObject m_Shadow;

        /// <summary>
        /// The background image.
        /// </summary>
        [SerializeField]
        private GameObject m_BackgroundImage;

        /// <summary>
        /// The label.
        /// </summary>
        [SerializeField]
        private GameObject m_Label;

        /// <summary>
        /// The layout group.
        /// </summary>
        [SerializeField]
        private LayoutGroup m_LayoutGroup;

        /// <summary>
        /// The progress indicator.
        /// </summary>
        [SerializeField]
        private ProgressIndicator m_ProgressIndicator;

        /// <summary>
        /// Configures the object.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public override void HelpInstantiate(params int[] options)
        {
            if (!options.Contains(optionRaised))
            {
                DestroyImmediate(m_Shadow);
                DestroyImmediate(m_BackgroundImage);
            }

            if (!options.Contains(optionHasLabel))
            {
                DestroyImmediate(m_Label);
            }

            if (!options.Contains(optionLayoutHorizontal) && !options.Contains(optionLayoutVertical))
            {
                this.transform.SetParent(m_LayoutGroup.transform.parent, true);
                GameObject.DestroyImmediate(m_LayoutGroup.gameObject);
                m_ProgressIndicator.baseObjectOverride = null;
            }
            else
            {
                if (options.Contains(optionLayoutHorizontal))
                {
                    GameObject go = m_LayoutGroup.gameObject;
                    DestroyImmediate(m_LayoutGroup);
                    m_LayoutGroup = go.AddComponent<HorizontalLayoutGroup>();
                    m_LayoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    ((HorizontalLayoutGroup)m_LayoutGroup).childForceExpandWidth = false;
                    ((HorizontalLayoutGroup)m_LayoutGroup).childForceExpandHeight = false;
                }

                if (options.Contains(optionRaised))
                {
                    if (options.Contains(optionLayoutHorizontal))
                    {
                        ((HorizontalLayoutGroup)m_LayoutGroup).spacing = 16f;
                    }
                    else if (options.Contains(optionLayoutVertical))
                    {
                        ((VerticalLayoutGroup)m_LayoutGroup).spacing = 16f;
                    }
                }
            }

            base.HelpInstantiate(options);
        }
    }
}