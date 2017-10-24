//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated MaterialSliders.
    /// </summary>
    /// <seealso cref="MaterialUI.InstantiationHelper" />
    public class MaterialSliderInstantiationHelper : InstantiationHelper
    {
        /// <summary>
        /// Should the slider be discrete?
        /// </summary>
        public const int optionDiscrete = 0;
        /// <summary>
        /// Should the slider have an icon?
        /// </summary>
        public const int optionHasIcon = 1;
        /// <summary>
        /// Should the slider have a label?
        /// </summary>
        public const int optionHasLabel = 2;
        /// <summary>
        /// Should the slider have a text field?
        /// </summary>
        public const int optionHasTextField = 3;
        /// <summary>
        /// Should the slider have an InputField?
        /// </summary>
        public const int optionHasInputField = 4;

        /// <summary>
        /// The MaterialSlider.
        /// </summary>
        [SerializeField]
        private MaterialSlider m_MaterialSlider;

        /// <summary>
        /// The Slider.
        /// </summary>
        [SerializeField]
        private Slider m_Slider;

        /// <summary>
        /// The left label.
        /// </summary>
        [SerializeField]
        private GameObject m_LeftLabel;

        /// <summary>
        /// The left icon.
        /// </summary>
        [SerializeField]
        private GameObject m_LeftIcon;

        /// <summary>
        /// The right label.
        /// </summary>
        [SerializeField]
        private GameObject m_RightLabel;

        /// <summary>
        /// The right input field.
        /// </summary>
        [SerializeField]
        private GameObject m_RightInputField;

        /// <summary>
        /// The left content.
        /// </summary>
        [SerializeField]
        private GameObject m_LeftContent;

        /// <summary>
        /// The right content.
        /// </summary>
        [SerializeField]
        private GameObject m_RightContent;

        /// <summary>
        /// Configures the object.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public override void HelpInstantiate(params int[] options)
        {
            bool destroyLeft = true;
            bool destroyRight = true;

            if (options.Contains(optionDiscrete))
            {
                m_Slider.wholeNumbers = true;
            }

            if (!options.Contains(optionHasIcon))
            {
                DestroyImmediate(m_LeftIcon);
            }
            else
            {
                destroyLeft = false;
            }

            if (!options.Contains(optionHasLabel))
            {
                DestroyImmediate(m_LeftLabel);
            }
            else
            {
                destroyLeft = false;
            }

            if (!options.Contains(optionHasTextField))
            {
                DestroyImmediate(m_RightLabel);
                m_MaterialSlider.valueText = null;
            }
            else
            {
                destroyRight = false;
            }

            if (!options.Contains(optionHasInputField))
            {
                DestroyImmediate(m_RightInputField);
                m_MaterialSlider.inputField = null;
            }
            else
            {
                destroyRight = false;
                m_MaterialSlider.lowRightDisabledOpacity = false;
            }

            if (destroyLeft)
            {
                DestroyImmediate(m_LeftContent);
            }

            if (destroyRight)
            {
                DestroyImmediate(m_RightContent);
            }

            if (transform.parent.GetComponent<ILayoutController>() != null)
            {
                m_MaterialSlider.hasManualPreferredWidth = true;
                m_MaterialSlider.manualPreferredWidth = 200;
            }

            m_MaterialSlider.SetLayoutHorizontal();

            base.HelpInstantiate(options);
        }
    }
}