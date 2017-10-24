//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated round MaterialButtons.
    /// </summary>
    /// <seealso cref="MaterialUI.InstantiationHelper" />
    public class ButtonRoundInstantiationHelper : InstantiationHelper
    {
        /// <summary>
        /// Should the button be mini?
        /// </summary>
        public const int optionMini = 0;
        /// <summary>
        /// Should the button be raised?
        /// </summary>
        public const int optionRaised = 1;
        /// <summary>
        /// Should the button contain a MaterialDropdown component?
        /// </summary>
        public const int optionHasDropdown = 2;

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
        /// The Image RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform m_ImageRectTransform;

        /// <summary>
        /// The shadows.
        /// </summary>
        [SerializeField]
        private RectTransform m_Shadows;

        /// <summary>
        /// Configures the object.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public override void HelpInstantiate(params int[] options)
        {
            m_Button.isCircularButton = true;

            if (options.Contains(optionMini))
            {
                m_Button.contentPadding = new Vector2(16, 16);
                m_Button.contentPadding = new Vector2(16, 16);
                m_Shadows.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 180);
                m_Shadows.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 180);
                m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
                m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
                m_ImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
                m_ImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
            }

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
            }

            base.HelpInstantiate(options);
        }
    }
}