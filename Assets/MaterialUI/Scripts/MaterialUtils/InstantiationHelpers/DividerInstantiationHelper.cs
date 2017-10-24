//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated Dividers.
    /// </summary>
    /// <seealso cref="MaterialUI.InstantiationHelper" />
    public class DividerInstantiationHelper : InstantiationHelper
    {
        /// <summary>
        /// Should the divider be light?
        /// </summary>
        public const int optionLight = 0;
        /// <summary>
        /// Should the divider be vertical?
        /// </summary>
        public const int optionVertical = 1;

        /// <summary>
        /// The LayoutElement.
        /// </summary>
        [SerializeField]
        private LayoutElement m_LayoutElement;

        /// <summary>
        /// The divider Image.
        /// </summary>
        [SerializeField]
        private Image m_Image;

        /// <summary>
        /// Configures the object.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public override void HelpInstantiate(params int[] options)
        {
            if (options.Contains(optionLight))
            {
                m_Image.color = MaterialColor.dividerLight;
            }

            if (options.Contains(optionVertical))
            {
                m_LayoutElement.minHeight = -1f;
                m_LayoutElement.minWidth = 1f;

                m_Image.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                m_Image.rectTransform.anchorMax = new Vector2(0.5f, 1f);
                m_Image.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                m_Image.rectTransform.sizeDelta = new Vector2(1f, 0f);
            }
            else
            {
                m_Image.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                m_Image.rectTransform.sizeDelta = new Vector2(0f, 1f);
            }

            base.HelpInstantiate(options);
        }
    }
}